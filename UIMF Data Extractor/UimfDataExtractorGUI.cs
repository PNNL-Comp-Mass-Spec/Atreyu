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

        public UimfDataExtractorGUI()
        {
            this.InitializeComponent();
        }

        private void Extract_Click(object sender, EventArgs e)
        {

        }

        private void GetXic_CheckedChanged(object sender, EventArgs e)
        {
            this.xicEnabled = this.GetXic.Checked;

            this.XicSettingsGoupBox.Enabled = this.xicEnabled;
        }
    }
}
