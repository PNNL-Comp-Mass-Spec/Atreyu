namespace UimfDataExtractor
{
    partial class UimfDataExtractorGui
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
            this.components = new System.ComponentModel.Container();
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
            this.Getmsms = new System.Windows.Forms.CheckBox();
            this.ExtractDataDisabledLabel = new System.Windows.Forms.Label();
            this.SetInputDirectory = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.InputDirectoryLabel = new System.Windows.Forms.Label();
            this.OutputDirectoryLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SetOutputDirectory = new System.Windows.Forms.Button();
            this.FrameNumber = new System.Windows.Forms.NumericUpDown();
            this.FrameNumberLabel = new System.Windows.Forms.Label();
            this.xicTargetData = new System.Windows.Forms.DataGridView();
            this.xicTargetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.targetMzDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toleranceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XicSettingsGoupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FrameNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xicTargetData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xicTargetBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // Extract
            // 
            this.Extract.Enabled = false;
            this.Extract.Location = new System.Drawing.Point(360, 244);
            this.Extract.Name = "Extract";
            this.Extract.Size = new System.Drawing.Size(75, 23);
            this.Extract.TabIndex = 0;
            this.Extract.Text = "Extract Data";
            this.Extract.UseVisualStyleBackColor = true;
            this.Extract.Click += new System.EventHandler(this.ExtractClick);
            // 
            // AllFrames
            // 
            this.AllFrames.AutoSize = true;
            this.AllFrames.Location = new System.Drawing.Point(52, 40);
            this.AllFrames.Name = "AllFrames";
            this.AllFrames.Size = new System.Drawing.Size(107, 17);
            this.AllFrames.TabIndex = 1;
            this.AllFrames.Text = "Export All Frames";
            this.AllFrames.UseVisualStyleBackColor = true;
            this.AllFrames.CheckedChanged += new System.EventHandler(this.AllFramesCheckedChanged);
            // 
            // BulkPeakComparison
            // 
            this.BulkPeakComparison.AutoSize = true;
            this.BulkPeakComparison.Location = new System.Drawing.Point(12, 61);
            this.BulkPeakComparison.Name = "BulkPeakComparison";
            this.BulkPeakComparison.Size = new System.Drawing.Size(168, 17);
            this.BulkPeakComparison.TabIndex = 2;
            this.BulkPeakComparison.Text = "Output Bulk Peak Comparison";
            this.BulkPeakComparison.UseVisualStyleBackColor = true;
            // 
            // GetHeatMap
            // 
            this.GetHeatMap.AutoSize = true;
            this.GetHeatMap.Location = new System.Drawing.Point(12, 85);
            this.GetHeatMap.Name = "GetHeatMap";
            this.GetHeatMap.Size = new System.Drawing.Size(93, 17);
            this.GetHeatMap.TabIndex = 3;
            this.GetHeatMap.Text = "Get Heat Map";
            this.GetHeatMap.UseVisualStyleBackColor = true;
            this.GetHeatMap.CheckedChanged += new System.EventHandler(this.GetHeatMap_CheckedChanged);
            // 
            // GetMz
            // 
            this.GetMz.AutoSize = true;
            this.GetMz.Location = new System.Drawing.Point(12, 109);
            this.GetMz.Name = "GetMz";
            this.GetMz.Size = new System.Drawing.Size(64, 17);
            this.GetMz.TabIndex = 4;
            this.GetMz.Text = "Get m/z";
            this.GetMz.UseVisualStyleBackColor = true;
            this.GetMz.CheckedChanged += new System.EventHandler(this.GetMz_CheckedChanged);
            // 
            // GetTic
            // 
            this.GetTic.AutoSize = true;
            this.GetTic.Location = new System.Drawing.Point(11, 132);
            this.GetTic.Name = "GetTic";
            this.GetTic.Size = new System.Drawing.Size(62, 17);
            this.GetTic.TabIndex = 5;
            this.GetTic.Text = "Get TiC";
            this.GetTic.UseVisualStyleBackColor = true;
            this.GetTic.CheckedChanged += new System.EventHandler(this.GetTic_CheckedChanged);
            // 
            // PeakFind
            // 
            this.PeakFind.AutoSize = true;
            this.PeakFind.Location = new System.Drawing.Point(11, 178);
            this.PeakFind.Name = "PeakFind";
            this.PeakFind.Size = new System.Drawing.Size(158, 17);
            this.PeakFind.TabIndex = 6;
            this.PeakFind.Text = "Output Individual Peak Files";
            this.PeakFind.UseVisualStyleBackColor = true;
            // 
            // Recursive
            // 
            this.Recursive.AutoSize = true;
            this.Recursive.Location = new System.Drawing.Point(11, 201);
            this.Recursive.Name = "Recursive";
            this.Recursive.Size = new System.Drawing.Size(175, 17);
            this.Recursive.TabIndex = 7;
            this.Recursive.Text = "Process Directories Recursively";
            this.Recursive.UseVisualStyleBackColor = true;
            // 
            // GetXic
            // 
            this.GetXic.AutoSize = true;
            this.GetXic.Location = new System.Drawing.Point(11, 155);
            this.GetXic.Name = "GetXic";
            this.GetXic.Size = new System.Drawing.Size(62, 17);
            this.GetXic.TabIndex = 8;
            this.GetXic.Text = "Get XiC";
            this.GetXic.UseVisualStyleBackColor = true;
            this.GetXic.CheckedChanged += new System.EventHandler(this.GetXicCheckedChanged);
            // 
            // XicSettingsGoupBox
            // 
            this.XicSettingsGoupBox.Controls.Add(this.xicTargetData);
            this.XicSettingsGoupBox.Controls.Add(this.Getmsms);
            this.XicSettingsGoupBox.Enabled = false;
            this.XicSettingsGoupBox.Location = new System.Drawing.Point(12, 228);
            this.XicSettingsGoupBox.Name = "XicSettingsGoupBox";
            this.XicSettingsGoupBox.Size = new System.Drawing.Size(339, 318);
            this.XicSettingsGoupBox.TabIndex = 9;
            this.XicSettingsGoupBox.TabStop = false;
            this.XicSettingsGoupBox.Text = "XiC Settings";
            // 
            // Getmsms
            // 
            this.Getmsms.AutoSize = true;
            this.Getmsms.Location = new System.Drawing.Point(6, 295);
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
            this.ExtractDataDisabledLabel.Location = new System.Drawing.Point(357, 228);
            this.ExtractDataDisabledLabel.Name = "ExtractDataDisabledLabel";
            this.ExtractDataDisabledLabel.Size = new System.Drawing.Size(95, 13);
            this.ExtractDataDisabledLabel.TabIndex = 10;
            this.ExtractDataDisabledLabel.Text = "Set Input Directory";
            // 
            // SetInputDirectory
            // 
            this.SetInputDirectory.Location = new System.Drawing.Point(297, 12);
            this.SetInputDirectory.Name = "SetInputDirectory";
            this.SetInputDirectory.Size = new System.Drawing.Size(92, 41);
            this.SetInputDirectory.TabIndex = 11;
            this.SetInputDirectory.Text = "Set Input Directory";
            this.SetInputDirectory.UseVisualStyleBackColor = true;
            this.SetInputDirectory.Click += new System.EventHandler(this.SetInputDirectoryClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(254, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Selected Directory:";
            // 
            // InputDirectoryLabel
            // 
            this.InputDirectoryLabel.AutoSize = true;
            this.InputDirectoryLabel.Location = new System.Drawing.Point(349, 61);
            this.InputDirectoryLabel.Name = "InputDirectoryLabel";
            this.InputDirectoryLabel.Size = new System.Drawing.Size(78, 13);
            this.InputDirectoryLabel.TabIndex = 13;
            this.InputDirectoryLabel.Text = "None Selected";
            // 
            // OutputDirectoryLabel
            // 
            this.OutputDirectoryLabel.AutoSize = true;
            this.OutputDirectoryLabel.Location = new System.Drawing.Point(349, 180);
            this.OutputDirectoryLabel.Name = "OutputDirectoryLabel";
            this.OutputDirectoryLabel.Size = new System.Drawing.Size(78, 13);
            this.OutputDirectoryLabel.TabIndex = 16;
            this.OutputDirectoryLabel.Text = "None Selected";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(254, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Selected Directory:";
            // 
            // SetOutputDirectory
            // 
            this.SetOutputDirectory.Location = new System.Drawing.Point(297, 121);
            this.SetOutputDirectory.Name = "SetOutputDirectory";
            this.SetOutputDirectory.Size = new System.Drawing.Size(92, 51);
            this.SetOutputDirectory.TabIndex = 14;
            this.SetOutputDirectory.Text = "Set Output Directory (optional)";
            this.SetOutputDirectory.UseVisualStyleBackColor = true;
            this.SetOutputDirectory.Click += new System.EventHandler(this.SetOutputDirectoryClick);
            // 
            // FrameNumber
            // 
            this.FrameNumber.Location = new System.Drawing.Point(94, 14);
            this.FrameNumber.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.FrameNumber.Name = "FrameNumber";
            this.FrameNumber.Size = new System.Drawing.Size(65, 20);
            this.FrameNumber.TabIndex = 17;
            this.FrameNumber.ThousandsSeparator = true;
            this.FrameNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FrameNumberLabel
            // 
            this.FrameNumberLabel.AutoSize = true;
            this.FrameNumberLabel.Location = new System.Drawing.Point(12, 19);
            this.FrameNumberLabel.Name = "FrameNumberLabel";
            this.FrameNumberLabel.Size = new System.Drawing.Size(76, 13);
            this.FrameNumberLabel.TabIndex = 18;
            this.FrameNumberLabel.Text = "Frame Number";
            // 
            // xicTargetData
            // 
            this.xicTargetData.AutoGenerateColumns = false;
            this.xicTargetData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.xicTargetData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.targetMzDataGridViewTextBoxColumn,
            this.toleranceDataGridViewTextBoxColumn});
            this.xicTargetData.DataSource = this.xicTargetBindingSource;
            this.xicTargetData.Location = new System.Drawing.Point(6, 19);
            this.xicTargetData.Name = "xicTargetData";
            this.xicTargetData.Size = new System.Drawing.Size(327, 270);
            this.xicTargetData.TabIndex = 19;
            // 
            // xicTargetBindingSource
            // 
            this.xicTargetBindingSource.DataSource = typeof(UimfDataExtractor.Models.XicTarget);
            // 
            // targetMzDataGridViewTextBoxColumn
            // 
            this.targetMzDataGridViewTextBoxColumn.DataPropertyName = "TargetMz";
            this.targetMzDataGridViewTextBoxColumn.HeaderText = "TargetMz";
            this.targetMzDataGridViewTextBoxColumn.Name = "targetMzDataGridViewTextBoxColumn";
            // 
            // toleranceDataGridViewTextBoxColumn
            // 
            this.toleranceDataGridViewTextBoxColumn.DataPropertyName = "Tolerance";
            this.toleranceDataGridViewTextBoxColumn.HeaderText = "Tolerance";
            this.toleranceDataGridViewTextBoxColumn.Name = "toleranceDataGridViewTextBoxColumn";
            // 
            // UimfDataExtractorGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 560);
            this.Controls.Add(this.FrameNumberLabel);
            this.Controls.Add(this.FrameNumber);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UimfDataExtractorGui";
            this.Text = "Uimf Data Extractor";
            this.XicSettingsGoupBox.ResumeLayout(false);
            this.XicSettingsGoupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FrameNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xicTargetData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xicTargetBindingSource)).EndInit();
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
        private System.Windows.Forms.CheckBox Getmsms;
        private System.Windows.Forms.Label ExtractDataDisabledLabel;
        private System.Windows.Forms.Button SetInputDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label InputDirectoryLabel;
        private System.Windows.Forms.Label OutputDirectoryLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SetOutputDirectory;
        private System.Windows.Forms.NumericUpDown FrameNumber;
        private System.Windows.Forms.Label FrameNumberLabel;
        private System.Windows.Forms.DataGridView xicTargetData;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetMzDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn toleranceDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource xicTargetBindingSource;
    }
}