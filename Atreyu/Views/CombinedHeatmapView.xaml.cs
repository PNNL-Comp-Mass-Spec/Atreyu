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

namespace Atreyu.Views
{
    using System.IO;

    using Atreyu.ViewModels;

    using Falkor.Views.Atreyu;

    using ReactiveUI;



 


    /// <summary>
    /// Interaction logic for CombinedHeatmapView.xaml
    /// </summary>
    public partial class CombinedHeatmapView : UserControl, IViewFor<CombinedHeatmapViewModel>
    {


        /// <summary>
        /// TODO The frame manipulation view.
        /// </summary>
        private FrameManipulationView frameManipulationView;

        /// <summary>
        /// TODO The heat map view.
        /// </summary>
        private HeatMapView heatMapView;

        /// <summary>
        /// TODO The mz spectra view.
        /// </summary>
        private MzSpectraView mzSpectraView;

        /// <summary>
        /// TODO The total ion chromatogram view.
        /// </summary>
        private TotalIonChromatogramView totalIonChromatogramView;


        public CombinedHeatmapView() : this(new CombinedHeatmapViewModel())
        {
        }


        public CombinedHeatmapView(CombinedHeatmapViewModel viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;

            this.heatMapView = new HeatMapView(this.ViewModel.HeatMapViewModel);
            Grid.SetColumn(this.heatMapView, 1);
            Grid.SetRow(this.heatMapView, 1);
            Grid.SetColumnSpan(this.heatMapView, 2);
            this.MainGrid.Children.Add(this.heatMapView);

            this.frameManipulationView = new FrameManipulationView(this.ViewModel.FrameManipulationViewModel);
            Grid.SetColumn(this.frameManipulationView, 1);
            Grid.SetRow(this.frameManipulationView, 0);
            this.MainGrid.Children.Add(this.frameManipulationView);

            this.mzSpectraView = new MzSpectraView(this.ViewModel.MzSpectraViewModel);
            Grid.SetColumn(this.mzSpectraView, 0);
            Grid.SetRow(this.mzSpectraView, 1);
            this.MainGrid.Children.Add(this.mzSpectraView);

            this.totalIonChromatogramView = new TotalIonChromatogramView(this.ViewModel.TotalIonChromatogramViewModel);
            Grid.SetColumn(this.totalIonChromatogramView, 1);
            Grid.SetRow(this.totalIonChromatogramView, 3);
            Grid.SetColumnSpan(this.totalIonChromatogramView, 2);
            this.MainGrid.Children.Add(this.totalIonChromatogramView);

            this.AllowDrop = true;
            this.PreviewDrop += this.MainTabControl_PreviewDragEnter;
        }

        object IViewFor.ViewModel
        {
            get
            {
                return this.ViewModel;
            }
            set
            {
                this.ViewModel = value as CombinedHeatmapViewModel;
            }
        }

        public CombinedHeatmapViewModel ViewModel { get; set; }

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
                this.LoadFile(filenames[0]);
            }

            e.Handled = true;
        }

        private void LoadFile(string fileName)
        {
            this.ViewModel.HeatMapViewModel.InitializeUimfData(fileName);
            this.ViewModel.FrameManipulationViewModel.CurrentFrame = 1;
            // this.totalIonChromatogramViewModel.UpdateReference(this.heatMapViewModel.HeatMapData);
        }
    }
}
