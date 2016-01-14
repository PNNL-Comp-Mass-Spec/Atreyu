namespace UimfDataExtractor
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

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
                                            Frame = (int)this.FrameNumber.Value,
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
    }
}