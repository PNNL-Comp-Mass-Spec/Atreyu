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
            // UimfDataExtractorGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 411);
            this.Controls.Add(this.Extract);
            this.Name = "UimfDataExtractorGUI";
            this.Text = "UimfDataExtractorGUI";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Extract;
    }
}