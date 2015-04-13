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
    [JobId("FB4801AD-14DA-49F7-AAFC-8D9430D732EA")]
    [JobName("Shelveset Cleanup Job")]
    [JobContext(JobContext.Collection)]
    [RegistryInfoResource("Microsoft.PSfD.TeamFoundation.BackgroundJobs.ShelvesetCleanupJob.reginfo")]
    public class ShelvesetCleanupJob : BackgroundJobBase
    {
        #region Fields

        private static readonly string emailEnabledSetting = "/Service/Integration/Settings/EmailEnabled";
        private static readonly string shelvesetCleanupRootPath = "/Configuration/ShelvesetCleanupJob";

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

            // First delete all outdated shelvesets so that they don't get picked up by the warning logic
            DeleteShelvesets(requestContext);
            SendEmail(requestContext);
        }

        protected override string GetFriendlyJobName()
        {
            return "shelveset cleanup";
        }

        protected override string GetLogLevelRegistryKey()
        {
            return shelvesetCleanupRootPath + "/LogLevel";
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

                var registryEntries = registry.ReadEntriesFallThru(requestContext, shelvesetCleanupRootPath + "/*");

                templateSearchPath = registryEntries["EmailTemplateSearchPath"].GetValue("Transforms");
                emailTemplate = registryEntries["EmailTemplate"].GetValue("ShelvesetCleanupTemplate.xsl");
                warningAgeInDays = registryEntries["WarningAgeInDays"].GetValue(300);
                deleteAgeInDays = registryEntries["DeleteAgeInDays"].GetValue(360);
                emailSubject = registryEntries["EmailSubject"].GetValue("ACTION REQUIRED - Shelveset Cleanup Warning");
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
            var shelvesetGroups = from s in vcs.QueryShelvesets(requestContext, null, null)
                                  where now.Subtract(s.CreationDate).TotalDays >= warningAgeInDays
                                  group s by new { Owner = s.Owner, OwnerDisplayName = s.OwnerDisplayName } into g
                                  orderby g.Key.Owner
                                  select g;

            string currentOwner = string.Empty;
            MemoryStream memStream = null;
            XmlWriter xmlWriter = null;

            foreach (var shelvesetGroup in shelvesetGroups)
            {
                if (currentOwner != shelvesetGroup.Key.Owner)
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
                    currentOwner = shelvesetGroup.Key.Owner;
                    xmlWriter.WriteStartElement("ShelvesetCleanup");
                    xmlWriter.WriteAttributeString("server", serverName);
                    xmlWriter.WriteAttributeString("collection", collectionName);
                    xmlWriter.WriteAttributeString("ownerDisplayName", shelvesetGroup.Key.OwnerDisplayName);
                    xmlWriter.WriteAttributeString("owner", shelvesetGroup.Key.Owner);
                    xmlWriter.WriteAttributeString("dateGenerated", now.ToString());
                    xmlWriter.WriteAttributeString("warnAge", warningAgeInDays.ToString());
                    xmlWriter.WriteAttributeString("delAge", deleteAgeInDays.ToString());
                }
                foreach (var shelveset in shelvesetGroup)
                {
                    xmlWriter.WriteStartElement("Shelveset");
                    xmlWriter.WriteAttributeString("name", shelveset.Name);
                    xmlWriter.WriteAttributeString("comment", shelveset.Comment);
                    xmlWriter.WriteAttributeString("created", shelveset.CreationDate.ToString());
                    xmlWriter.WriteAttributeString("age", now.Subtract(shelveset.CreationDate).TotalDays.ToString("#"));
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
                                messageBuilder.AppendFormat("The following shelvesets are older than {0} days:", warningAgeInDays);
                                messageBuilder.AppendLine();
                                foreach (XmlNode node in doc.SelectNodes("//Shelveset"))
                                {
                                    messageBuilder.AppendFormat("  {0}, created {1}", node.Attributes["name"].Value, node.Attributes["created"].Value);
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
                LogError("There was an error queuing a shelveset cleanup email!", exception);
            }
        }

        private void DeleteShelvesets(TeamFoundationRequestContext requestContext)
        {
            var now = DateTime.Now;

            var vcs = requestContext.GetService<TeamFoundationVersionControlService>();
            var shelvesets = from s in vcs.QueryShelvesets(requestContext, null, null)
                             where now.Subtract(s.CreationDate).TotalDays >= deleteAgeInDays
                             orderby s.Owner
                             select s;

            string currentOwner = null;
            string toAddress = null;
            foreach (var shelveset in shelvesets)
            {
                if (string.Compare(currentOwner, shelveset.Owner, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    currentOwner = shelveset.Owner;
                    toAddress = GetPreferredEmailAddress(requestContext, currentOwner);
                }
                if (!string.IsNullOrEmpty(toAddress) || deleteIfNoEmailAddress)
                {
                    vcs.DeleteShelveset(requestContext, shelveset.Name, shelveset.Owner);
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
