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

namespace Microsoft.PSfD.TeamFoundation.JobScheduler
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gpJobs = new System.Windows.Forms.GroupBox();
            this.lvJobs = new System.Windows.Forms.ListView();
            this.btnAddJob = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbCollection = new System.Windows.Forms.RadioButton();
            this.rbServer = new System.Windows.Forms.RadioButton();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtServerUri = new System.Windows.Forms.TextBox();
            this.tcJobDetails = new System.Windows.Forms.TabControl();
            this.tpSchedule = new System.Windows.Forms.TabPage();
            this.txtJobData = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDeleteSchedule = new System.Windows.Forms.Button();
            this.cbTimeZone = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpScheduledTime = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCreateSchedule = new System.Windows.Forms.Button();
            this.tpRegistry = new System.Windows.Forms.TabPage();
            this.btnSaveRegistry = new System.Windows.Forms.Button();
            this.rbRegCollection = new System.Windows.Forms.RadioButton();
            this.rbRegServer = new System.Windows.Forms.RadioButton();
            this.tfsRegistryGrid = new Microsoft.PSfD.TeamFoundation.Controls.TfsRegistryGridView();
            this.gpJobs.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tcJobDetails.SuspendLayout();
            this.tpSchedule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).BeginInit();
            this.tpRegistry.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpJobs
            // 
            this.gpJobs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpJobs.Controls.Add(this.lvJobs);
            this.gpJobs.Controls.Add(this.btnAddJob);
            this.gpJobs.Enabled = false;
            this.gpJobs.Location = new System.Drawing.Point(12, 86);
            this.gpJobs.Name = "gpJobs";
            this.gpJobs.Size = new System.Drawing.Size(872, 207);
            this.gpJobs.TabIndex = 1;
            this.gpJobs.TabStop = false;
            this.gpJobs.Text = "Available Jobs";
            // 
            // lvJobs
            // 
            this.lvJobs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvJobs.FullRowSelect = true;
            this.lvJobs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvJobs.HideSelection = false;
            this.lvJobs.Location = new System.Drawing.Point(6, 19);
            this.lvJobs.MultiSelect = false;
            this.lvJobs.Name = "lvJobs";
            this.lvJobs.ShowGroups = false;
            this.lvJobs.Size = new System.Drawing.Size(779, 182);
            this.lvJobs.TabIndex = 2;
            this.lvJobs.UseCompatibleStateImageBehavior = false;
            this.lvJobs.View = System.Windows.Forms.View.List;
            this.lvJobs.SelectedIndexChanged += new System.EventHandler(this.lvJobs_SelectedIndexChanged);
            // 
            // btnAddJob
            // 
            this.btnAddJob.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAddJob.Location = new System.Drawing.Point(791, 98);
            this.btnAddJob.Name = "btnAddJob";
            this.btnAddJob.Size = new System.Drawing.Size(75, 23);
            this.btnAddJob.TabIndex = 1;
            this.btnAddJob.Text = "Add...";
            this.btnAddJob.UseVisualStyleBackColor = true;
            this.btnAddJob.Click += new System.EventHandler(this.btnAddJob_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.rbCollection);
            this.groupBox3.Controls.Add(this.rbServer);
            this.groupBox3.Controls.Add(this.btnConnect);
            this.groupBox3.Controls.Add(this.txtServerUri);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(872, 69);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Team Foundation Server";
            // 
            // rbCollection
            // 
            this.rbCollection.AutoSize = true;
            this.rbCollection.Location = new System.Drawing.Point(97, 47);
            this.rbCollection.Name = "rbCollection";
            this.rbCollection.Size = new System.Drawing.Size(100, 17);
            this.rbCollection.TabIndex = 3;
            this.rbCollection.Text = "Collection Level";
            this.rbCollection.UseVisualStyleBackColor = true;
            // 
            // rbServer
            // 
            this.rbServer.AutoSize = true;
            this.rbServer.Checked = true;
            this.rbServer.Location = new System.Drawing.Point(6, 47);
            this.rbServer.Name = "rbServer";
            this.rbServer.Size = new System.Drawing.Size(85, 17);
            this.rbServer.TabIndex = 2;
            this.rbServer.TabStop = true;
            this.rbServer.Text = "Server Level";
            this.rbServer.UseVisualStyleBackColor = true;
            this.rbServer.CheckedChanged += new System.EventHandler(this.rbServer_CheckedChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(791, 19);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect...";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtServerUri
            // 
            this.txtServerUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerUri.Location = new System.Drawing.Point(6, 21);
            this.txtServerUri.Name = "txtServerUri";
            this.txtServerUri.ReadOnly = true;
            this.txtServerUri.Size = new System.Drawing.Size(779, 20);
            this.txtServerUri.TabIndex = 0;
            // 
            // tcJobDetails
            // 
            this.tcJobDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcJobDetails.Controls.Add(this.tpSchedule);
            this.tcJobDetails.Controls.Add(this.tpRegistry);
            this.tcJobDetails.Location = new System.Drawing.Point(12, 299);
            this.tcJobDetails.Name = "tcJobDetails";
            this.tcJobDetails.SelectedIndex = 0;
            this.tcJobDetails.Size = new System.Drawing.Size(872, 310);
            this.tcJobDetails.TabIndex = 4;
            // 
            // tpSchedule
            // 
            this.tpSchedule.Controls.Add(this.txtJobData);
            this.tpSchedule.Controls.Add(this.label4);
            this.tpSchedule.Controls.Add(this.btnDeleteSchedule);
            this.tpSchedule.Controls.Add(this.cbTimeZone);
            this.tpSchedule.Controls.Add(this.label3);
            this.tpSchedule.Controls.Add(this.nudInterval);
            this.tpSchedule.Controls.Add(this.label2);
            this.tpSchedule.Controls.Add(this.dtpScheduledTime);
            this.tpSchedule.Controls.Add(this.label1);
            this.tpSchedule.Controls.Add(this.btnCreateSchedule);
            this.tpSchedule.Location = new System.Drawing.Point(4, 22);
            this.tpSchedule.Name = "tpSchedule";
            this.tpSchedule.Padding = new System.Windows.Forms.Padding(3);
            this.tpSchedule.Size = new System.Drawing.Size(864, 284);
            this.tpSchedule.TabIndex = 0;
            this.tpSchedule.Text = "Schedule";
            this.tpSchedule.UseVisualStyleBackColor = true;
            // 
            // txtJobData
            // 
            this.txtJobData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJobData.Location = new System.Drawing.Point(9, 57);
            this.txtJobData.Multiline = true;
            this.txtJobData.Name = "txtJobData";
            this.txtJobData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtJobData.Size = new System.Drawing.Size(849, 221);
            this.txtJobData.TabIndex = 19;
            this.txtJobData.WordWrap = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Job Data";
            // 
            // btnDeleteSchedule
            // 
            this.btnDeleteSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSchedule.Location = new System.Drawing.Point(783, 8);
            this.btnDeleteSchedule.Name = "btnDeleteSchedule";
            this.btnDeleteSchedule.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteSchedule.TabIndex = 17;
            this.btnDeleteSchedule.Text = "Delete";
            this.btnDeleteSchedule.UseVisualStyleBackColor = true;
            this.btnDeleteSchedule.Click += new System.EventHandler(this.btnDeleteSchedule_Click);
            // 
            // cbTimeZone
            // 
            this.cbTimeZone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTimeZone.FormattingEnabled = true;
            this.cbTimeZone.Location = new System.Drawing.Point(177, 10);
            this.cbTimeZone.Name = "cbTimeZone";
            this.cbTimeZone.Size = new System.Drawing.Size(384, 21);
            this.cbTimeZone.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Time Zone";
            // 
            // nudInterval
            // 
            this.nudInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudInterval.Location = new System.Drawing.Point(615, 11);
            this.nudInterval.Name = "nudInterval";
            this.nudInterval.Size = new System.Drawing.Size(81, 20);
            this.nudInterval.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(567, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Interval";
            // 
            // dtpScheduledTime
            // 
            this.dtpScheduledTime.CustomFormat = "HH:mm";
            this.dtpScheduledTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpScheduledTime.Location = new System.Drawing.Point(42, 11);
            this.dtpScheduledTime.Name = "dtpScheduledTime";
            this.dtpScheduledTime.ShowUpDown = true;
            this.dtpScheduledTime.Size = new System.Drawing.Size(65, 20);
            this.dtpScheduledTime.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Time";
            // 
            // btnCreateSchedule
            // 
            this.btnCreateSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateSchedule.Location = new System.Drawing.Point(702, 8);
            this.btnCreateSchedule.Name = "btnCreateSchedule";
            this.btnCreateSchedule.Size = new System.Drawing.Size(75, 23);
            this.btnCreateSchedule.TabIndex = 10;
            this.btnCreateSchedule.Text = "Create";
            this.btnCreateSchedule.UseVisualStyleBackColor = true;
            this.btnCreateSchedule.Click += new System.EventHandler(this.btnCreateSchedule_Click);
            // 
            // tpRegistry
            // 
            this.tpRegistry.Controls.Add(this.btnSaveRegistry);
            this.tpRegistry.Controls.Add(this.rbRegCollection);
            this.tpRegistry.Controls.Add(this.rbRegServer);
            this.tpRegistry.Controls.Add(this.tfsRegistryGrid);
            this.tpRegistry.Location = new System.Drawing.Point(4, 22);
            this.tpRegistry.Name = "tpRegistry";
            this.tpRegistry.Padding = new System.Windows.Forms.Padding(3);
            this.tpRegistry.Size = new System.Drawing.Size(864, 284);
            this.tpRegistry.TabIndex = 1;
            this.tpRegistry.Text = "Registry";
            this.tpRegistry.UseVisualStyleBackColor = true;
            // 
            // btnSaveRegistry
            // 
            this.btnSaveRegistry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveRegistry.Location = new System.Drawing.Point(783, 6);
            this.btnSaveRegistry.Name = "btnSaveRegistry";
            this.btnSaveRegistry.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRegistry.TabIndex = 6;
            this.btnSaveRegistry.Text = "Save";
            this.btnSaveRegistry.UseVisualStyleBackColor = true;
            this.btnSaveRegistry.Click += new System.EventHandler(this.btnSaveRegistry_Click);
            // 
            // rbRegCollection
            // 
            this.rbRegCollection.AutoSize = true;
            this.rbRegCollection.Location = new System.Drawing.Point(97, 6);
            this.rbRegCollection.Name = "rbRegCollection";
            this.rbRegCollection.Size = new System.Drawing.Size(100, 17);
            this.rbRegCollection.TabIndex = 5;
            this.rbRegCollection.Text = "Collection Level";
            this.rbRegCollection.UseVisualStyleBackColor = true;
            // 
            // rbRegServer
            // 
            this.rbRegServer.AutoSize = true;
            this.rbRegServer.Checked = true;
            this.rbRegServer.Location = new System.Drawing.Point(6, 6);
            this.rbRegServer.Name = "rbRegServer";
            this.rbRegServer.Size = new System.Drawing.Size(85, 17);
            this.rbRegServer.TabIndex = 4;
            this.rbRegServer.TabStop = true;
            this.rbRegServer.Text = "Server Level";
            this.rbRegServer.UseVisualStyleBackColor = true;
            this.rbRegServer.CheckedChanged += new System.EventHandler(this.rbRegServer_CheckedChanged);
            // 
            // tfsRegistryGrid
            // 
            this.tfsRegistryGrid.AllowAddingCustomEntries = false;
            this.tfsRegistryGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tfsRegistryGrid.IncludePaths = new string[0];
            this.tfsRegistryGrid.Location = new System.Drawing.Point(3, 35);
            this.tfsRegistryGrid.Name = "tfsRegistryGrid";
            this.tfsRegistryGrid.OnlyReadEntriesWithRegistryInfo = true;
            this.tfsRegistryGrid.Size = new System.Drawing.Size(858, 246);
            this.tfsRegistryGrid.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 621);
            this.Controls.Add(this.tcJobDetails);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gpJobs);
            this.Name = "MainForm";
            this.Text = "Team Foundation Custom Job Scheduler";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gpJobs.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tcJobDetails.ResumeLayout(false);
            this.tpSchedule.ResumeLayout(false);
            this.tpSchedule.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).EndInit();
            this.tpRegistry.ResumeLayout(false);
            this.tpRegistry.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpJobs;
        private System.Windows.Forms.Button btnAddJob;
        private System.Windows.Forms.ListView lvJobs;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtServerUri;
        private System.Windows.Forms.RadioButton rbCollection;
        private System.Windows.Forms.RadioButton rbServer;
        private System.Windows.Forms.TabControl tcJobDetails;
        private System.Windows.Forms.TabPage tpSchedule;
        private System.Windows.Forms.TextBox txtJobData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDeleteSchedule;
        private System.Windows.Forms.ComboBox cbTimeZone;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpScheduledTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCreateSchedule;
        private System.Windows.Forms.TabPage tpRegistry;
        private System.Windows.Forms.RadioButton rbRegCollection;
        private System.Windows.Forms.RadioButton rbRegServer;
        private Microsoft.PSfD.TeamFoundation.Controls.TfsRegistryGridView tfsRegistryGrid;
        private System.Windows.Forms.Button btnSaveRegistry;
    }
}

