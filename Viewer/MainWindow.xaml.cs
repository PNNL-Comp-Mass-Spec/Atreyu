using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Viewer
{
    using System.IO;

    using Atreyu.ViewModels;

    using Falkor.Views.Atreyu;

    using Microsoft.Practices.Prism.PubSubEvents;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EventAggregator eventAggregator = new EventAggregator();

        private HeatMapViewModel heatMapViewModel;
        private HeatMapView heatMapView;

        private FrameManipulationViewModel frameManipulationViewModel;

        private FrameManipulationView frameManipulationView;

        private MzSpectraViewModel mzSpectraViewModel;

        private MzSpectraView mzSpectraView;

        private TotalIonChromatogramViewModel totalIonChromatogramViewModel;

        private TotalIonChromatogramView totalIonChromatogramView;


        public MainWindow()
        {
            InitializeComponent();

            heatMapViewModel = new HeatMapViewModel(eventAggregator);
            heatMapView = new HeatMapView(heatMapViewModel);
            MainTabControl.Items.Add(heatMapView);

            this.frameManipulationViewModel = new FrameManipulationViewModel(this.eventAggregator);
            this.frameManipulationView = new FrameManipulationView(this.frameManipulationViewModel);
            MainTabControl.Items.Add(this.frameManipulationView);

            this.mzSpectraViewModel = new MzSpectraViewModel(this.eventAggregator);
            this.mzSpectraView = new MzSpectraView(this.mzSpectraViewModel);
            MainTabControl.Items.Add(this.mzSpectraView);

            this.totalIonChromatogramViewModel = new TotalIonChromatogramViewModel(this.eventAggregator);
            this.totalIonChromatogramView = new TotalIonChromatogramView(this.totalIonChromatogramViewModel);
            MainTabControl.Items.Add(this.totalIonChromatogramView);

            this.AllowDrop = true;
            this.PreviewDrop += this.MainTabControl_PreviewDragEnter;
        }

        private void MainTabControl_PreviewDragEnter(object sender, DragEventArgs e)
        {
            var isCorrect = true;
            string[] filenames = { };
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }

                    var info = new FileInfo(filename);

                    if (info.Extension.ToLower() != ".uimf")
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }

            e.Effects = isCorrect ? DragDropEffects.All : DragDropEffects.None;

            if (isCorrect)
            {
                this.heatMapViewModel.InitializeUimfData(filenames[0]);
            }

            e.Handled = true;
        }
    }
}
