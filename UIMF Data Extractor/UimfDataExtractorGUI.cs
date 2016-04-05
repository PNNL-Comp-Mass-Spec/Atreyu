namespace UimfDataExtractor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Microsoft.WindowsAPICodePack.Dialogs;

    using UimfDataExtractor.Models;

    /// <summary>
    /// The uimf data extractor gui code behind.
    /// </summary>
    public partial class UimfDataExtractorGui : Form
    {
        #region Fields

        /// <summary>
        /// The input directory.
        /// </summary>
        private string inputDirectory;

        /// <summary>
        /// The output directory.
        /// </summary>
        private string outputDirectory;

        /// <summary>
        /// The xic enabled.
        /// </summary>
        private bool xicEnabled;

        private List<Extraction> extractionProcedures;

        private double selectedMz;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UimfDataExtractorGui"/> class.
        /// </summary>
        public UimfDataExtractorGui()
        {
            this.InitializeComponent();
            this.extractionProcedures = new List<Extraction>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The extract click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtractClick(object sender, EventArgs e)
        {
            this.Enabled = false;

            Task.Run(
                () =>
                    {
                        MessageBox.Show(
                            "Work started, controls will be re-enabled when complete.  See console for more details.", 
                            "Extraction Started", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                    });

            UimfProcessor.Options = new CommandLineOptions
                                        {
                                            InputPath = this.inputDirectory, 
                                            OutputPath = this.outputDirectory, 
                                            AllFrames = this.AllFrames.Checked, 
                                            BulkPeakComparison = this.BulkPeakComparison.Checked, 
                                            Frame = (int)this.FrameNumber.Value,
                                            XicTolerance = (double)this.XicTolerance.Value, 
                                            Getmsms = this.Getmsms.Checked, 
                                            PeakFind = this.PeakFind.Checked, 
                                            Recursive = this.Recursive.Checked, 
                                            Verbose = true,
                                            ExtractionTypes = this.extractionProcedures.ToArray(),
                                            XicMz = this.selectedMz
                                        };


            UimfProcessor.ExtractData();

            this.Enabled = true;
        }

        /// <summary>
        /// The get xic checkbox changed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GetXicCheckedChanged(object sender, EventArgs e)
        {
            this.xicEnabled = this.GetXic.Checked;
            this.XicSettingsGoupBox.Enabled = this.xicEnabled;
            if (this.xicEnabled)
            {
                this.extractionProcedures.Add(Extraction.Xic);
            }
            else
            {
                this.extractionProcedures.Remove(Extraction.Xic);
            }
        }

        /// <summary>
        /// The set input directory click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SetInputDirectoryClick(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            this.inputDirectory = dialog.FileName;
            this.ExtractDataDisabledLabel.Visible = false;
            this.Extract.Enabled = true;

            this.InputDirectoryLabel.Text = this.inputDirectory;
        }

        /// <summary>
        /// The set output directory click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SetOutputDirectoryClick(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.outputDirectory = dialog.SelectedPath;
            this.OutputDirectoryLabel.Text = this.outputDirectory;
        }

        #endregion

        /// <summary>
        /// The all frames checked changed event.  Enables and disables the frame number control.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void AllFramesCheckedChanged(object sender, EventArgs e)
        {
            this.FrameNumber.Enabled = !this.AllFrames.Checked;
            this.FrameNumberLabel.Enabled = !this.AllFrames.Checked;
        }

        private void GetHeatMap_CheckedChanged(object sender, EventArgs e)
        {
            if (this.GetHeatMap.Checked)
            {
                this.extractionProcedures.Add(Extraction.Heatmap);
            }
            else
            {
                this.extractionProcedures.Remove(Extraction.Heatmap);
            }
        }

        private void GetMz_CheckedChanged(object sender, EventArgs e)
        {
            if (this.GetMz.Checked)
            {
                this.extractionProcedures.Add(Extraction.Mz);
            }
            else
            {
                this.extractionProcedures.Remove(Extraction.Mz);
            }
        }

        private void GetTic_CheckedChanged(object sender, EventArgs e)
        {
            if (this.GetTic.Checked)
            {
                this.extractionProcedures.Add(Extraction.Tic);
            }
            else
            {
                this.extractionProcedures.Remove(Extraction.Tic);
            }
        }

        private void XicCenter_ValueChanged(object sender, EventArgs e)
        {
            this.selectedMz = (double)this.XicCenter.Value;
        }
    }
}