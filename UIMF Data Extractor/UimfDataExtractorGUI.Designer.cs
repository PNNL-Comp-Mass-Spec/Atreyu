﻿namespace UimfDataExtractor
{
    partial class UimfDataExtractorGUI
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
            this.Extract = new System.Windows.Forms.Button();
            this.AllFrames = new System.Windows.Forms.CheckBox();
            this.BulkPeakComparison = new System.Windows.Forms.CheckBox();
            this.GetHeatMap = new System.Windows.Forms.CheckBox();
            this.GetMz = new System.Windows.Forms.CheckBox();
            this.GetTic = new System.Windows.Forms.CheckBox();
            this.PeakFind = new System.Windows.Forms.CheckBox();
            this.Recursive = new System.Windows.Forms.CheckBox();
            this.GetXic = new System.Windows.Forms.CheckBox();
            this.XicSettingsGoupBox = new System.Windows.Forms.GroupBox();
            this.XicCenter = new System.Windows.Forms.NumericUpDown();
            this.XicCenterLabel = new System.Windows.Forms.Label();
            this.XicTolerance = new System.Windows.Forms.NumericUpDown();
            this.XicToleranceLabel = new System.Windows.Forms.Label();
            this.Getmsms = new System.Windows.Forms.CheckBox();
            this.ExtractDataDisabledLabel = new System.Windows.Forms.Label();
            this.SetInputDirectory = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.InputDirectoryLabel = new System.Windows.Forms.Label();
            this.OutputDirectoryLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SetOutputDirectory = new System.Windows.Forms.Button();
            this.XicSettingsGoupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XicCenter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XicTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // Extract
            // 
            this.Extract.Enabled = false;
            this.Extract.Location = new System.Drawing.Point(358, 283);
            this.Extract.Name = "Extract";
            this.Extract.Size = new System.Drawing.Size(75, 23);
            this.Extract.TabIndex = 0;
            this.Extract.Text = "Extract Data";
            this.Extract.UseVisualStyleBackColor = true;
            this.Extract.Click += new System.EventHandler(this.Extract_Click);
            // 
            // AllFrames
            // 
            this.AllFrames.AutoSize = true;
            this.AllFrames.Location = new System.Drawing.Point(13, 13);
            this.AllFrames.Name = "AllFrames";
            this.AllFrames.Size = new System.Drawing.Size(107, 17);
            this.AllFrames.TabIndex = 1;
            this.AllFrames.Text = "Export All Frames";
            this.AllFrames.UseVisualStyleBackColor = true;
            // 
            // BulkPeakComparison
            // 
            this.BulkPeakComparison.AutoSize = true;
            this.BulkPeakComparison.Location = new System.Drawing.Point(13, 37);
            this.BulkPeakComparison.Name = "BulkPeakComparison";
            this.BulkPeakComparison.Size = new System.Drawing.Size(168, 17);
            this.BulkPeakComparison.TabIndex = 2;
            this.BulkPeakComparison.Text = "Output Bulk Peak Comparison";
            this.BulkPeakComparison.UseVisualStyleBackColor = true;
            // 
            // GetHeatMap
            // 
            this.GetHeatMap.AutoSize = true;
            this.GetHeatMap.Location = new System.Drawing.Point(13, 61);
            this.GetHeatMap.Name = "GetHeatMap";
            this.GetHeatMap.Size = new System.Drawing.Size(93, 17);
            this.GetHeatMap.TabIndex = 3;
            this.GetHeatMap.Text = "Get Heat Map";
            this.GetHeatMap.UseVisualStyleBackColor = true;
            this.GetHeatMap.Visible = false;
            // 
            // GetMz
            // 
            this.GetMz.AutoSize = true;
            this.GetMz.Location = new System.Drawing.Point(13, 85);
            this.GetMz.Name = "GetMz";
            this.GetMz.Size = new System.Drawing.Size(64, 17);
            this.GetMz.TabIndex = 4;
            this.GetMz.Text = "Get m/z";
            this.GetMz.UseVisualStyleBackColor = true;
            // 
            // GetTic
            // 
            this.GetTic.AutoSize = true;
            this.GetTic.Location = new System.Drawing.Point(12, 108);
            this.GetTic.Name = "GetTic";
            this.GetTic.Size = new System.Drawing.Size(62, 17);
            this.GetTic.TabIndex = 5;
            this.GetTic.Text = "Get TiC";
            this.GetTic.UseVisualStyleBackColor = true;
            // 
            // PeakFind
            // 
            this.PeakFind.AutoSize = true;
            this.PeakFind.Location = new System.Drawing.Point(12, 132);
            this.PeakFind.Name = "PeakFind";
            this.PeakFind.Size = new System.Drawing.Size(158, 17);
            this.PeakFind.TabIndex = 6;
            this.PeakFind.Text = "Output Individual Peak Files";
            this.PeakFind.UseVisualStyleBackColor = true;
            // 
            // Recursive
            // 
            this.Recursive.AutoSize = true;
            this.Recursive.Location = new System.Drawing.Point(12, 156);
            this.Recursive.Name = "Recursive";
            this.Recursive.Size = new System.Drawing.Size(175, 17);
            this.Recursive.TabIndex = 7;
            this.Recursive.Text = "Process Directories Recursively";
            this.Recursive.UseVisualStyleBackColor = true;
            // 
            // GetXic
            // 
            this.GetXic.AutoSize = true;
            this.GetXic.Location = new System.Drawing.Point(12, 180);
            this.GetXic.Name = "GetXic";
            this.GetXic.Size = new System.Drawing.Size(62, 17);
            this.GetXic.TabIndex = 8;
            this.GetXic.Text = "Get XiC";
            this.GetXic.UseVisualStyleBackColor = true;
            this.GetXic.CheckedChanged += new System.EventHandler(this.GetXic_CheckedChanged);
            // 
            // XicSettingsGoupBox
            // 
            this.XicSettingsGoupBox.Controls.Add(this.Getmsms);
            this.XicSettingsGoupBox.Controls.Add(this.XicToleranceLabel);
            this.XicSettingsGoupBox.Controls.Add(this.XicTolerance);
            this.XicSettingsGoupBox.Controls.Add(this.XicCenterLabel);
            this.XicSettingsGoupBox.Controls.Add(this.XicCenter);
            this.XicSettingsGoupBox.Enabled = false;
            this.XicSettingsGoupBox.Location = new System.Drawing.Point(13, 204);
            this.XicSettingsGoupBox.Name = "XicSettingsGoupBox";
            this.XicSettingsGoupBox.Size = new System.Drawing.Size(214, 106);
            this.XicSettingsGoupBox.TabIndex = 9;
            this.XicSettingsGoupBox.TabStop = false;
            this.XicSettingsGoupBox.Text = "XiC Settings";
            // 
            // XicCenter
            // 
            this.XicCenter.DecimalPlaces = 5;
            this.XicCenter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.XicCenter.Location = new System.Drawing.Point(69, 19);
            this.XicCenter.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.XicCenter.Name = "XicCenter";
            this.XicCenter.Size = new System.Drawing.Size(120, 20);
            this.XicCenter.TabIndex = 0;
            // 
            // XicCenterLabel
            // 
            this.XicCenterLabel.AutoSize = true;
            this.XicCenterLabel.Location = new System.Drawing.Point(6, 21);
            this.XicCenterLabel.Name = "XicCenterLabel";
            this.XicCenterLabel.Size = new System.Drawing.Size(57, 13);
            this.XicCenterLabel.TabIndex = 1;
            this.XicCenterLabel.Text = "XiC Center";
            // 
            // XicTolerance
            // 
            this.XicTolerance.DecimalPlaces = 5;
            this.XicTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.XicTolerance.Location = new System.Drawing.Point(69, 59);
            this.XicTolerance.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.XicTolerance.Name = "XicTolerance";
            this.XicTolerance.Size = new System.Drawing.Size(120, 20);
            this.XicTolerance.TabIndex = 2;
            this.XicTolerance.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // XicToleranceLabel
            // 
            this.XicToleranceLabel.AutoSize = true;
            this.XicToleranceLabel.Location = new System.Drawing.Point(6, 61);
            this.XicToleranceLabel.Name = "XicToleranceLabel";
            this.XicToleranceLabel.Size = new System.Drawing.Size(55, 13);
            this.XicToleranceLabel.TabIndex = 3;
            this.XicToleranceLabel.Text = "Tolerance";
            // 
            // Getmsms
            // 
            this.Getmsms.AutoSize = true;
            this.Getmsms.Location = new System.Drawing.Point(6, 85);
            this.Getmsms.Name = "Getmsms";
            this.Getmsms.Size = new System.Drawing.Size(204, 17);
            this.Getmsms.TabIndex = 4;
            this.Getmsms.Text = "Look at msms data instead of ms data";
            this.Getmsms.UseVisualStyleBackColor = true;
            // 
            // ExtractDataDisabledLabel
            // 
            this.ExtractDataDisabledLabel.AutoSize = true;
            this.ExtractDataDisabledLabel.ForeColor = System.Drawing.Color.Red;
            this.ExtractDataDisabledLabel.Location = new System.Drawing.Point(338, 263);
            this.ExtractDataDisabledLabel.Name = "ExtractDataDisabledLabel";
            this.ExtractDataDisabledLabel.Size = new System.Drawing.Size(95, 13);
            this.ExtractDataDisabledLabel.TabIndex = 10;
            this.ExtractDataDisabledLabel.Text = "Set Input Directory";
            // 
            // SetInputDirectory
            // 
            this.SetInputDirectory.Location = new System.Drawing.Point(287, 13);
            this.SetInputDirectory.Name = "SetInputDirectory";
            this.SetInputDirectory.Size = new System.Drawing.Size(92, 41);
            this.SetInputDirectory.TabIndex = 11;
            this.SetInputDirectory.Text = "Set Input Directory";
            this.SetInputDirectory.UseVisualStyleBackColor = true;
            this.SetInputDirectory.Click += new System.EventHandler(this.SetInputDirectory_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(244, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Selected Directory:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // InputDirectoryLabel
            // 
            this.InputDirectoryLabel.AutoSize = true;
            this.InputDirectoryLabel.Location = new System.Drawing.Point(339, 62);
            this.InputDirectoryLabel.Name = "InputDirectoryLabel";
            this.InputDirectoryLabel.Size = new System.Drawing.Size(78, 13);
            this.InputDirectoryLabel.TabIndex = 13;
            this.InputDirectoryLabel.Text = "None Selected";
            // 
            // OutputDirectoryLabel
            // 
            this.OutputDirectoryLabel.AutoSize = true;
            this.OutputDirectoryLabel.Location = new System.Drawing.Point(339, 181);
            this.OutputDirectoryLabel.Name = "OutputDirectoryLabel";
            this.OutputDirectoryLabel.Size = new System.Drawing.Size(78, 13);
            this.OutputDirectoryLabel.TabIndex = 16;
            this.OutputDirectoryLabel.Text = "None Selected";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(244, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Selected Directory:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SetOutputDirectory
            // 
            this.SetOutputDirectory.Location = new System.Drawing.Point(287, 122);
            this.SetOutputDirectory.Name = "SetOutputDirectory";
            this.SetOutputDirectory.Size = new System.Drawing.Size(92, 51);
            this.SetOutputDirectory.TabIndex = 14;
            this.SetOutputDirectory.Text = "Set Output Directory (optional)";
            this.SetOutputDirectory.UseVisualStyleBackColor = true;
            this.SetOutputDirectory.Click += new System.EventHandler(this.SetOutputDirectory_Click);
            // 
            // UimfDataExtractorGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 325);
            this.Controls.Add(this.OutputDirectoryLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SetOutputDirectory);
            this.Controls.Add(this.InputDirectoryLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SetInputDirectory);
            this.Controls.Add(this.ExtractDataDisabledLabel);
            this.Controls.Add(this.XicSettingsGoupBox);
            this.Controls.Add(this.GetXic);
            this.Controls.Add(this.Recursive);
            this.Controls.Add(this.PeakFind);
            this.Controls.Add(this.GetTic);
            this.Controls.Add(this.GetMz);
            this.Controls.Add(this.GetHeatMap);
            this.Controls.Add(this.BulkPeakComparison);
            this.Controls.Add(this.AllFrames);
            this.Controls.Add(this.Extract);
            this.Name = "UimfDataExtractorGUI";
            this.Text = "Uimf Data Extractor";
            this.XicSettingsGoupBox.ResumeLayout(false);
            this.XicSettingsGoupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XicCenter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XicTolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Extract;
        private System.Windows.Forms.CheckBox AllFrames;
        private System.Windows.Forms.CheckBox BulkPeakComparison;
        private System.Windows.Forms.CheckBox GetHeatMap;
        private System.Windows.Forms.CheckBox GetMz;
        private System.Windows.Forms.CheckBox GetTic;
        private System.Windows.Forms.CheckBox PeakFind;
        private System.Windows.Forms.CheckBox Recursive;
        private System.Windows.Forms.CheckBox GetXic;
        private System.Windows.Forms.GroupBox XicSettingsGoupBox;
        private System.Windows.Forms.Label XicToleranceLabel;
        private System.Windows.Forms.NumericUpDown XicTolerance;
        private System.Windows.Forms.Label XicCenterLabel;
        private System.Windows.Forms.NumericUpDown XicCenter;
        private System.Windows.Forms.CheckBox Getmsms;
        private System.Windows.Forms.Label ExtractDataDisabledLabel;
        private System.Windows.Forms.Button SetInputDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label InputDirectoryLabel;
        private System.Windows.Forms.Label OutputDirectoryLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SetOutputDirectory;
    }
}