//--------------------------------------------------------------------------------
// This file is part of a Microsoft sample.
//
// (c) 2013 Microsoft Corporation. All rights reserved. 
// 
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//--------------------------------------------------------------------------------

using Microsoft.PSfD.TeamFoundation.BackgroundJobs.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Microsoft.PSfD.TeamFoundation.BackgroundJobs
{
    [JobId("C76725FB-B54C-48D9-8BBA-8F69B6317463")]
    [JobName("Workspace Cleanup Job")]
    [JobContext(JobContext.Collection)]
    [RegistryInfoResource("Microsoft.PSfD.TeamFoundation.BackgroundJobs.WorkspaceCleanupJob.reginfo")]
    public class WorkspaceCleanupJob : BackgroundJobBase
    {
        #region Fields

        private static readonly string emailEnabledSetting = "/Service/Integration/Settings/EmailEnabled";
        private static readonly string workspaceCleanupRootPath = "/Configuration/WorkspaceCleanupJob";

        private string serverName;
        private string collectionName;

        private bool emailEnabled;
        private string templateSearchPath;
        private string emailTemplate;
        private int warningAgeInDays;
        private int deleteAgeInDays;
        private string emailSubject;
        private bool warnIfNoEmailAddress;
        private bool deleteIfNoEmailAddress;
        private MailPriority emailPriority;

        #endregion

        #region Overrides

        protected override void DoWork(TeamFoundationRequestContext requestContext, TeamFoundationJobDefinition jobDefinition, DateTime queueTime, ref TeamFoundationJobExecutionResult result, ref string resultMessage, ref bool skipFurtherResultAnalysis)
        {
            ReadJobSettings(requestContext);

            if (!emailEnabled)
            {
                resultMessage = string.Format("Email settings have not been configured or are disabled for host {0}", requestContext.ServiceHost.InstanceId);
                skipFurtherResultAnalysis = true;
                return;
            }

            if (!File.Exists(emailTemplate))
            {
                LogError(string.Format("The template file '{0}' does not exist!", emailTemplate), null);
                return;
            }

            // First delete all outdated workspaces so that they don't get picked up by the warning logic
            DeleteWorkspaces(requestContext);
            SendEmail(requestContext);
        }

        protected override string GetFriendlyJobName()
        {
            return "workspace cleanup";
        }

        protected override string GetLogLevelRegistryKey()
        {
            return workspaceCleanupRootPath + "/LogLevel";
        }

        #endregion

        #region Members

        private void ReadJobSettings(TeamFoundationRequestContext requestContext)
        {
            var registry = requestContext.GetService<TeamFoundationRegistryService>();
            emailEnabled = registry.GetValue(requestContext, emailEnabledSetting, true, false);

            // Since we're tied to the TFS email service, we only run if the email system has been enabled
            if (emailEnabled)
            {
                collectionName = requestContext.ServiceHost.CollectionServiceHost.Name;

                var locationService = requestContext.GetService<TeamFoundationLocationService>();
                serverName = locationService.GetSelfReferenceUri(requestContext, locationService.GetPublicAccessMapping(requestContext)).DnsSafeHost;

                var registryEntries = registry.ReadEntriesFallThru(requestContext, workspaceCleanupRootPath + "/*");

                templateSearchPath = registryEntries["EmailTemplateSearchPath"].GetValue("Transforms");
                emailTemplate = registryEntries["EmailTemplate"].GetValue("WorkspaceCleanupTemplate.xsl");
                warningAgeInDays = registryEntries["WarningAgeInDays"].GetValue(300);
                deleteAgeInDays = registryEntries["DeleteAgeInDays"].GetValue(360);
                emailSubject = registryEntries["EmailSubject"].GetValue("ACTION REQUIRED - Workspace Cleanup Warning");
                emailPriority = registryEntries["EmailPriority"].GetValue(MailPriority.High);
                warnIfNoEmailAddress = registryEntries["WarnIfNoEmailAddress"].GetValue(true);
                deleteIfNoEmailAddress = registryEntries["DeleteIfNoEmailAddress"].GetValue(false);

                // Expand paths to the email template
                if (!Path.IsPathRooted(templateSearchPath))
                {
                    emailTemplate = Path.Combine(requestContext.ServiceHost.PhysicalDirectory, templateSearchPath, emailTemplate);
                }
            }
        }

        private void SendEmail(TeamFoundationRequestContext requestContext)
        {
            var now = DateTime.Now;

            var vcs = requestContext.GetService<TeamFoundationVersionControlService>();
            var workspaceGroups = from ws in vcs.QueryWorkspaces(requestContext, null, null, 0)
                                  where now.Subtract(ws.LastAccessDate).TotalDays >= warningAgeInDays
                                  group ws by new { Owner = ws.OwnerName, OwnerDisplayName = ws.OwnerDisplayName, Computer = ws.Computer } into g
                                  orderby g.Key.Owner, g.Key.Computer
                                  select g;

            string currentOwner = string.Empty;
            MemoryStream memStream = null;
            XmlWriter xmlWriter = null;

            foreach (var workspaceGroup in workspaceGroups)
            {
                if (currentOwner != workspaceGroup.Key.Owner)
                {
                    if (memStream != null)
                    {
                        // Create XML document
                        xmlWriter.WriteEndElement();
                        xmlWriter.Flush();
                        var doc = new XmlDocument();
                        memStream.Seek(0, SeekOrigin.Begin);
                        doc.Load(memStream);

                        // Send Email
                        QueueEmail(requestContext, currentOwner, doc);

                        // Clean up
                        xmlWriter.Close();
                        xmlWriter.Dispose();
                        memStream.Dispose();
                    }
                    memStream = new MemoryStream();
                    xmlWriter = XmlWriter.Create(memStream);
                    currentOwner = workspaceGroup.Key.Owner;
                    xmlWriter.WriteStartElement("WorkspaceCleanup");
                    xmlWriter.WriteAttributeString("server", serverName);
                    xmlWriter.WriteAttributeString("collection", collectionName);
                    xmlWriter.WriteAttributeString("ownerDisplayName", workspaceGroup.Key.OwnerDisplayName);
                    xmlWriter.WriteAttributeString("owner", workspaceGroup.Key.Owner);
                    xmlWriter.WriteAttributeString("dateGenerated", now.ToString());
                    xmlWriter.WriteAttributeString("warnAge", warningAgeInDays.ToString());
                    xmlWriter.WriteAttributeString("delAge", deleteAgeInDays.ToString());
                }
                foreach (var workspace in workspaceGroup)
                {
                    xmlWriter.WriteStartElement("Workspace");
                    xmlWriter.WriteAttributeString("computer", workspace.Computer);
                    xmlWriter.WriteAttributeString("name", workspace.Name);
                    xmlWriter.WriteAttributeString("comment", workspace.Comment);
                    xmlWriter.WriteAttributeString("type", workspace.IsLocal ? "Local" : "Server");
                    xmlWriter.WriteAttributeString("lastAccessed", workspace.LastAccessDate.ToString());
                    xmlWriter.WriteAttributeString("age", now.Subtract(workspace.LastAccessDate).TotalDays.ToString("#"));
                    xmlWriter.WriteEndElement();
                }
            }

            if (memStream != null)
            {
                // Create XML document
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
                var doc = new XmlDocument();
                memStream.Seek(0, SeekOrigin.Begin);
                doc.Load(memStream);

                // Send Email
                QueueEmail(requestContext, currentOwner, doc);

                // Clean up
                xmlWriter.Close();
                xmlWriter.Dispose();
                memStream.Dispose();
            }
        }

        private void QueueEmail(TeamFoundationRequestContext requestContext, string owner, XmlDocument doc)
        {
            var mailService = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationMailService>();


            try
            {
                var transform = new XslCompiledTransform();
                transform.Load(emailTemplate);

                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    using (XmlWriter xmlWriter = XmlTextWriter.Create(sw, transform.OutputSettings))
                    {
                        transform.Transform(doc, null, xmlWriter);
                        xmlWriter.Flush();

                        var message = new MailMessage();
                        message.Body = sw.ToString();
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.IsBodyHtml = true;
                        message.Subject = emailSubject;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.Priority = emailPriority;

                        var toAddress = GetPreferredEmailAddress(requestContext, owner);
                        if (string.IsNullOrEmpty(toAddress))
                        {
                            if (warnIfNoEmailAddress)
                            {
                                var messageBuilder = new StringBuilder();
                                messageBuilder.AppendFormat("The email address for user {0} could not be found!", owner);
                                messageBuilder.AppendLine();
                                messageBuilder.AppendFormat("The following workspaces are older than {0} days:", warningAgeInDays);
                                messageBuilder.AppendLine();
                                foreach (XmlNode node in doc.SelectNodes("//Workspace"))
                                {
                                    messageBuilder.AppendFormat("  {0} on {1}, last accessed {2}", node.Attributes["name"].Value, node.Attributes["computer"].Value, node.Attributes["lastAccessed"].Value);
                                    messageBuilder.AppendLine();
                                }
                                LogWarning(messageBuilder.ToString());
                            }
                            return;
                        }

                        message.To.Add(toAddress);

                        mailService.QueueMailJob(requestContext, message);
                    }
                }
            }
            catch (Exception exception)
            {
                LogError("There was an error queuing a workspace cleanup email!", exception);
            }
        }

        private void DeleteWorkspaces(TeamFoundationRequestContext requestContext)
        {
            var now = DateTime.Now;

            var vcs = requestContext.GetService<TeamFoundationVersionControlService>();
            var workspaces = from ws in vcs.QueryWorkspaces(requestContext, null, null, 0)
                             where now.Subtract(ws.LastAccessDate).TotalDays >= deleteAgeInDays
                             orderby ws.OwnerName
                             select ws;

            string currentOwner = null;
            string toAddress = null;
            foreach (var workspace in workspaces)
            {
                if (string.Compare(currentOwner, workspace.OwnerName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    currentOwner = workspace.OwnerName;
                    toAddress = GetPreferredEmailAddress(requestContext, currentOwner);
                }
                if (!string.IsNullOrEmpty(toAddress) || deleteIfNoEmailAddress)
                {
                    vcs.DeleteWorkspace(requestContext, workspace.Name, workspace.OwnerName);
                }
            }
        }

        private string GetPreferredEmailAddress(TeamFoundationRequestContext requestContext, string owner)
        {
            var identityService = requestContext.GetService<TeamFoundationIdentityService>();

            var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.AccountName, owner);
            if (identity == null)
            {
                LogError(string.Format("Team Foundation Identity for user {0} could not be found!", owner), null);
                return null;
            }
            return identityService.GetPreferredEmailAddress(requestContext, identity.TeamFoundationId, true);
        }

        #endregion
    }
}
