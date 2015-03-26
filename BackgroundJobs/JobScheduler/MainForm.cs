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
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.PSfD.TeamFoundation.JobScheduler
{
    public partial class MainForm : Form
    {
        #region Fields

        const string TFS_APPTIER_PATH = @"SOFTWARE\Microsoft\TeamFoundationServer\12.0\InstalledComponents\ApplicationTier";
        const string TFS_INSTALL_PATH = @"InstallPath";
        const string TFS_WEB_CONFIG = @"Web Services\web.config";
        const string TFS_JOB_EXTENSIONS_PATH = @"TFSJobAgent\Plugins";

        TeamFoundationRequestContext deploymentContext = null;
        TeamFoundationRequestContext collectionContext = null;
        TeamFoundationRequestContext currentContext = null;

        string tfsInstallPath = null;
        string connectionString = null;

        int localTimeZoneIndex;
        Dictionary<string, Tuple<Assembly, Type>> jobs = new Dictionary<string, Tuple<Assembly, Type>>();

        TfsTeamProjectCollection tpc;
        TeamFoundationJobDefinition jobDefinition;

        bool warningDisabled;

        #endregion

        #region Ctors

        public MainForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Members

        private bool AddJobsFromAssembly(string file)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                var customJobs = from t in assembly.GetTypes()
                                 where t.GetInterface("ITeamFoundationJobExtension") != null && t.IsDefined(typeof(JobIdAttribute))
                                 select t;

                if (customJobs.Any())
                {
                    foreach (var customJob in customJobs)
                    {
                        var jobId = customJob.GetCustomAttribute<JobIdAttribute>().JobId.ToUpper();
                        if (!jobs.ContainsKey(jobId))
                        {
                            jobs.Add(jobId, new Tuple<Assembly, Type>(assembly, customJob));
                            ListViewItem item;
                            if (customJob.IsDefined(typeof(JobNameAttribute)))
                            {
                                item = new ListViewItem(string.Format("{0} ({1}) - Context: {2}", customJob.GetCustomAttribute<JobNameAttribute>().JobName, jobId,
                                    customJob.IsDefined(typeof(JobContextAttribute)) ? customJob.GetCustomAttribute<JobContextAttribute>().Context : JobContext.Any));
                            }
                            else
                            {
                                item = new ListViewItem(string.Format("{0} ({1}) - Context: {2}", customJob.Name, jobId,
                                    customJob.IsDefined(typeof(JobContextAttribute)) ? customJob.GetCustomAttribute<JobContextAttribute>().Context : JobContext.Any));
                            }
                            item.Tag = jobId;
                            lvJobs.Items.Add(item);
                        }
                    }
                    return true;
                }
            }
            catch (ReflectionTypeLoadException ex) { Trace.TraceError(ex.ToString()); }
            return false;
        }

        private bool PrepareConnection()
        {
            var appTierPath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(TFS_APPTIER_PATH);
            if (appTierPath != null)
            {
                tfsInstallPath = appTierPath.GetValue(TFS_INSTALL_PATH) as string;
                var webConfig = Path.Combine(tfsInstallPath, TFS_WEB_CONFIG);
                if (File.Exists(webConfig))
                {
                    using (var xr = XmlReader.Create(File.OpenText(webConfig)))
                    {
                        xr.ReadToFollowing("appSettings");
                        while (xr.Read())
                        {
                            if (string.Compare(xr.GetAttribute("key"), "applicationDatabase", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                connectionString = xr.GetAttribute("value");
                                break;
                            }
                        }
                    }
                }
                return !string.IsNullOrEmpty(connectionString);
            }

            return false;
        }

        private TeamFoundationJobService GetJobService()
        {
            TeamFoundationServiceHostProperties deploymentHostProperties = new TeamFoundationServiceHostProperties();

            deploymentHostProperties.HostType = TeamFoundationHostType.Deployment | TeamFoundationHostType.Application;
            deploymentHostProperties.Id = Guid.Empty;
            deploymentHostProperties.PlugInDirectory = Path.Combine(tfsInstallPath, TFS_JOB_EXTENSIONS_PATH);
            deploymentHostProperties.PhysicalDirectory = string.Empty;
            deploymentHostProperties.VirtualDirectory = string.Empty;

            DeploymentServiceHost host = null;
            ISqlConnectionInfo connInfo = SqlConnectionInfoFactory.Create(connectionString, null, null);
            deploymentHostProperties.ConnectionInfo = connInfo;
            host = new DeploymentServiceHost(deploymentHostProperties, true);

            if (collectionContext != null)
            {
                collectionContext.Dispose();
                collectionContext = null;
            }
            if (deploymentContext != null)
                deploymentContext.Dispose();

            deploymentContext = host.CreateSystemContext();

            if (rbServer.Checked)
            {
                currentContext = deploymentContext;
                return deploymentContext.GetService<TeamFoundationJobService>();
            }
            else
            {
                var hms = deploymentContext.GetService<TeamFoundationHostManagementService>();
                collectionContext = hms.BeginRequest(deploymentContext, tpc.InstanceId, RequestContextType.SystemContext);
                currentContext = collectionContext;
                return collectionContext.GetService<TeamFoundationJobService>();
            }
        }

        private void ReadJobSchedule(string jobId)
        {
            var job = jobs[jobId.ToUpper()];
            var jobType = job.Item2;
            if (jobType.IsDefined(typeof(JobContextAttribute)))
            {
                var context = jobType.GetCustomAttribute<JobContextAttribute>().Context;
                switch (context)
                {
                    case JobContext.Server:
                        if (!rbServer.Checked)
                        {
                            rbServer.Enabled = true;
                            rbServer.Checked = true;
                            rbCollection.Enabled = false;
                            return;
                        }
                        break;
                    case JobContext.Collection:
                        if (!rbCollection.Checked)
                        {
                            rbCollection.Enabled = true;
                            rbCollection.Checked = true;
                            rbServer.Enabled = false;
                            return;
                        }
                        break;
                    case JobContext.Any:
                        rbCollection.Enabled = true;
                        rbServer.Enabled = true;
                        break;
                    default:
                        break;
                }
            }
            TeamFoundationJobService jobService = GetJobService();
            jobDefinition = jobService.QueryJobDefinition(currentContext, new Guid(jobId));
            if (jobDefinition != null && jobDefinition.Schedule.Count > 0)
            {
                UpdateJobDataInUI();
            }
            else
            {
                ClearJobDataInUI(job, jobType);
            }
            txtJobData.Enabled = !string.IsNullOrEmpty(txtJobData.Text);
            UpdateRegistryInfo(job.Item1, jobType.GetCustomAttribute<RegistryInfoResourceAttribute>());
            tpSchedule.Enabled = true;
        }

        private void ClearJobDataInUI(Tuple<Assembly, Type> job, Type jobType)
        {
            ClearScheduleData();
            ClearRegistryData();
            if (jobType.IsDefined(typeof(JobDataResourceAttribute)))
            {
                var doc = new XmlDocument();
                doc.Load(job.Item1.GetManifestResourceStream(jobType.GetCustomAttribute<JobDataResourceAttribute>().JobDataResource));
                txtJobData.Text = doc.OuterXml;
            }
        }

        private void UpdateJobDataInUI()
        {
            var schedule = jobDefinition.Schedule[0];
            dtpScheduledTime.Value = schedule.ScheduledTime;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId);
            cbTimeZone.SelectedIndex = cbTimeZone.FindString(timeZone.DisplayName ?? TimeZoneInfo.Utc.Id);
            nudInterval.Value = schedule.Interval;
            if (jobDefinition.Data != null)
            {
                txtJobData.Text = jobDefinition.Data.OuterXml;
            }
            btnCreateSchedule.Text = "Update";
        }

        private void UpdateRegistryInfo(Assembly jobAssembly, RegistryInfoResourceAttribute regInfoAttrib)
        {
            if (tfsRegistryGrid.HasUnsavedData)
            {
                if (MessageBox.Show("There are unsaved registry changes. Do you want to save these changes?", "Unsaved Data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    tfsRegistryGrid.Save();
                }
            }
            if (regInfoAttrib != null)
            {
                var regInfoDoc = new XmlDocument();
                regInfoDoc.Load(jobAssembly.GetManifestResourceStream(regInfoAttrib.RegistryInfoResource));
                tfsRegistryGrid.RegistryInfoPath = null;
                tfsRegistryGrid.RegistryInfos.Clear();
                tfsRegistryGrid.RegistryInfos.Add(regInfoDoc);
                if (rbRegServer.Checked)
                    tfsRegistryGrid.Connection = tpc.ConfigurationServer;
                else
                    tfsRegistryGrid.Connection = tpc;
            }
            else
            {
                tfsRegistryGrid.Connection = null;
            }
            tpRegistry.Enabled = regInfoAttrib != null;
        }

        private void ClearScheduleData()
        {
            dtpScheduledTime.Value = DateTime.Today;
            cbTimeZone.SelectedIndex = localTimeZoneIndex;
            nudInterval.Value = 0;
            txtJobData.Clear();
            btnCreateSchedule.Text = "Create";
        }

        private void ClearRegistryData()
        {
            tfsRegistryGrid.Connection = null;
        }

        #endregion

        #region Eventhandlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            int index;
            foreach (var tzi in TimeZoneInfo.GetSystemTimeZones())
            {
                if (tzi.Id == TimeZoneInfo.Utc.Id)
                {
                    index = cbTimeZone.Items.Add(tzi.Id);
                }
                else
                {
                    index = cbTimeZone.Items.Add(tzi.DisplayName);
                }
                if (tzi.Id == TimeZoneInfo.Local.Id)
                    localTimeZoneIndex = index;
            }
            nudInterval.Maximum = int.MaxValue;
            tpSchedule.Enabled = false;
            tpRegistry.Enabled = false;
            ClearScheduleData();
        }

        private void rbServer_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtServerUri.Text))
            {
                if (rbServer.Checked)
                {
                    txtServerUri.Text = tpc.ConfigurationServer.Uri.ToString();
                }
                else
                {
                    txtServerUri.Text = tpc.Uri.ToString();
                }
                if (lvJobs.SelectedItems.Count > 0)
                {
                    var item = lvJobs.SelectedItems[0];
                    ReadJobSchedule((string)item.Tag);
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (collectionContext != null)
            {
                collectionContext.Dispose();
                collectionContext = null;
            }
            if (deploymentContext != null)
            {
                deploymentContext.Dispose();
                deploymentContext = null;
            }

            using (var pp = new TeamProjectPicker(TeamProjectPickerMode.NoProject, false))
            {
                if (pp.ShowDialog() == DialogResult.OK)
                {
                    if (PrepareConnection())
                    {
                        tpc = pp.SelectedTeamProjectCollection;
                    }
                    else
                    {
                        MessageBox.Show("Could not connect to local TFS App Tier. You must run this tool on the TFS App tier server, after you have copied your job extensions to the Job Agent's plugin folder!");
                        return;
                    }
                }
            }
            if (tpc != null)
            {
                if (rbServer.Checked)
                {
                    txtServerUri.Text = tpc.ConfigurationServer.Uri.ToString();
                }
                else
                {
                    txtServerUri.Text = tpc.Uri.ToString();
                }
                gpJobs.Enabled = true;
            }
            else
            {
                txtServerUri.Clear();
                gpJobs.Enabled = false;
            }
        }

        private void btnAddJob_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Path.Combine(tfsInstallPath, TFS_JOB_EXTENSIONS_PATH);
                ofd.Filter = "Background Job Plugins (*.dll)|*.dll";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    bool added = false;
                    foreach (var file in ofd.FileNames)
                    {
                        if (File.Exists(file))
                        {
                            added = AddJobsFromAssembly(file) ? true : added;
                        }
                    }
                    if (!added)
                    {
                        MessageBox.Show("The selected file(s) did not contain any custom background jobs!");
                    }
                }
            }
        }

        private void btnCreateSchedule_Click(object sender, EventArgs e)
        {
            TeamFoundationJobService jobService = GetJobService();

            if (jobDefinition == null)
            {
                // Create new job definition
                var jobId = (string)lvJobs.SelectedItems[0].Tag;
                var jobType = jobs[jobId.ToUpper()].Item2;
                var jobName = jobType.IsDefined(typeof(JobNameAttribute)) ? jobType.GetCustomAttribute<JobNameAttribute>().JobName : jobType.Name;
                XmlElement jobData = null;
                if (!string.IsNullOrEmpty(txtJobData.Text))
                {
                    var xElem = XElement.Parse(txtJobData.Text);
                    jobData = new XmlDocument().ReadNode(xElem.CreateReader()) as XmlElement;
                }
                var timeZone = (from tzi in TimeZoneInfo.GetSystemTimeZones()
                                where tzi.DisplayName == (string)cbTimeZone.SelectedItem
                                select tzi).FirstOrDefault() ?? TimeZoneInfo.Utc;
                var schedule = new TeamFoundationJobSchedule();
                schedule.TimeZoneId = timeZone.Id;
                schedule.Interval = (int)nudInterval.Value;
                schedule.ScheduledTime = dtpScheduledTime.Value;
                var definition = new TeamFoundationJobDefinition(new Guid(jobId), jobName, jobType.FullName, jobData, Microsoft.TeamFoundation.Framework.Common.TeamFoundationJobEnabledState.Enabled);
                definition.Schedule.Add(schedule);

                try
                {
                    jobService.UpdateJobDefinitions(currentContext, null, new[] { definition });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error creating job definition", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ReadJobSchedule(jobId);
                MessageBox.Show("Job definition created successfully. Please make sure to copy the necessary assemblies to the TFS job agent's plugin folder and restart the job agent!");
            }
            else
            {
                // Update existing job definition
                var timeZone = (from tzi in TimeZoneInfo.GetSystemTimeZones()
                                where tzi.DisplayName == (string)cbTimeZone.SelectedItem
                                select tzi).FirstOrDefault() ?? TimeZoneInfo.Utc;
                jobDefinition.Schedule[0].ScheduledTime = dtpScheduledTime.Value;
                jobDefinition.Schedule[0].TimeZoneId = timeZone.Id;
                jobDefinition.Schedule[0].Interval = (int)nudInterval.Value;
                XmlElement jobData = null;
                if (!string.IsNullOrEmpty(txtJobData.Text))
                {
                    var xElem = XElement.Parse(txtJobData.Text);
                    jobData = new XmlDocument().ReadNode(xElem.CreateReader()) as XmlElement;
                }
                jobDefinition.Data = jobData;

                try
                {
                    jobService.UpdateJobDefinitions(currentContext, null, new[] { jobDefinition });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error updating job definition", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ReadJobSchedule(jobDefinition.JobId.ToString());
                MessageBox.Show("Job definition updated successfully.");
            }
        }

        private void btnDeleteSchedule_Click(object sender, EventArgs e)
        {
            if (jobDefinition != null)
            {
                var jobId = jobDefinition.JobId.ToString();
                TeamFoundationJobService jobService = GetJobService();
                jobService.UpdateJobDefinitions(currentContext, new[] { jobDefinition.JobId }, null);
                jobDefinition = null;

                ReadJobSchedule(jobId);
                MessageBox.Show("Job definition deleted successfully.");
            }
        }

        private void rbRegServer_CheckedChanged(object sender, EventArgs e)
        {
            if (!warningDisabled)
            {
                if (tfsRegistryGrid.HasUnsavedData)
                {
                    if (MessageBox.Show("There are unsaved registry changes. If you continue, all unsaved changes will be lost!", "Unsaved Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        warningDisabled = true;
                        if (rbRegServer.Checked)
                            rbRegCollection.Checked = true;
                        else
                            rbRegServer.Checked = true;
                        warningDisabled = false;
                        return;
                    }
                }
                if (rbRegServer.Checked)
                    tfsRegistryGrid.Connection = tpc.ConfigurationServer;
                else
                    tfsRegistryGrid.Connection = tpc;
            }
        }

        private void btnSaveRegistry_Click(object sender, EventArgs e)
        {
            tfsRegistryGrid.Save();
        }

        private void lvJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvJobs.SelectedItems.Count > 0)
            {
                var item = lvJobs.SelectedItems[0];
                ReadJobSchedule((string)item.Tag);
            }
            else
            {
                ClearScheduleData();
                ClearRegistryData();
                tpSchedule.Enabled = false;
                tpRegistry.Enabled = false;
            }
        }

        #endregion
    }
}
