using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UimfDataExtractor
{
    public partial class UimfDataExtractorGUI : Form
    {
        private bool xicEnabled = false;

        private string inputDirectory;

        private string outputDirectory;

        public UimfDataExtractorGUI()
        {
            this.InitializeComponent();
        }

        private void Extract_Click(object sender, EventArgs e)
        {
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
                                            Recursive = this.Recursive.Checked
                                        };

            UimfProcessor.ExtractData();
        }

        private void GetXic_CheckedChanged(object sender, EventArgs e)
        {
            this.xicEnabled = this.GetXic.Checked;

            this.XicSettingsGoupBox.Enabled = this.xicEnabled;
        }

        private void SetInputDirectory_Click(object sender, EventArgs e)
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

        private void SetOutputDirectory_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.outputDirectory = dialog.SelectedPath;
            this.OutputDirectoryLabel.Text = this.outputDirectory;
        }
    }
}
