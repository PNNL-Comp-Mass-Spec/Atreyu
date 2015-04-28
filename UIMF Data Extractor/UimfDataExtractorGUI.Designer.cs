namespace UimfDataExtractor
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
            this.XicSettingsGoupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XicCenter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XicTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // Extract
            // 
            this.Extract.Location = new System.Drawing.Point(484, 376);
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
            this.PeakFind.Size = new System.Drawing.Size(74, 17);
            this.PeakFind.TabIndex = 6;
            this.PeakFind.Text = "Peak Find";
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
            this.XicSettingsGoupBox.Controls.Add(this.XicToleranceLabel);
            this.XicSettingsGoupBox.Controls.Add(this.XicTolerance);
            this.XicSettingsGoupBox.Controls.Add(this.XicCenterLabel);
            this.XicSettingsGoupBox.Controls.Add(this.XicCenter);
            this.XicSettingsGoupBox.Enabled = false;
            this.XicSettingsGoupBox.Location = new System.Drawing.Point(13, 204);
            this.XicSettingsGoupBox.Name = "XicSettingsGoupBox";
            this.XicSettingsGoupBox.Size = new System.Drawing.Size(200, 100);
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
            // UimfDataExtractorGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 411);
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
            this.Text = "UimfDataExtractorGUI";
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
    }
}