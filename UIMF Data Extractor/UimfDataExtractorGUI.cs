// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UimfDataExtractorGUI.cs" company="Pacific Northwest National Laboratory">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Pacific Northwest National Laboratory
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// <summary>
//   The uimf data extractor gui code behind.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UimfDataExtractor
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// The uimf data extractor gui codebehind.
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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UimfDataExtractorGui"/> class.
        /// </summary>
        public UimfDataExtractorGui()
        {
            this.InitializeComponent();
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
                                            GetHeatmap = this.GetHeatMap.Checked, 
                                            GetMz = this.GetMz.Checked, 
                                            GetTiC = this.GetTic.Checked, 
                                            GetXiC = (double)this.XicCenter.Value, 
                                            XicTolerance = (double)this.XicTolerance.Value, 
                                            Getmsms = this.Getmsms.Checked, 
                                            PeakFind = this.PeakFind.Checked, 
                                            Recursive = this.Recursive.Checked, 
                                            Verbose = true
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
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.inputDirectory = dialog.SelectedPath;
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
    }
}