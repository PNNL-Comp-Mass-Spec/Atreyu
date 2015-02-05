// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedHeatmapView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for CombinedHeatmapView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.Views
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    using Atreyu.ViewModels;

    using Falkor.Views.Atreyu;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for CombinedHeatmapView.xaml
    /// </summary>
    public partial class CombinedHeatmapView : UserControl, IViewFor<CombinedHeatmapViewModel>
    {
        #region Fields

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

        private GateSlider sliderView;

        /// <summary>
        /// TODO The total ion chromatogram view.
        /// </summary>
        private TotalIonChromatogramView totalIonChromatogramView;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapView"/> class.
        /// </summary>
        public CombinedHeatmapView()
        {
            InitializeComponent();
            this.DataContextChanged += CombinedHeatmapView_DataContextChanged;
        }

        private void CombinedHeatmapView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ViewModel = e.NewValue as CombinedHeatmapViewModel;

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
            Grid.SetRow(this.totalIonChromatogramView, 2);
            Grid.SetColumnSpan(this.totalIonChromatogramView, 2);
            this.MainGrid.Children.Add(this.totalIonChromatogramView);

            this.sliderView = new GateSlider(this.ViewModel.GateSliderViewModel);
            Grid.SetRow(this.sliderView, 1);
            Grid.SetColumn(this.sliderView, 3);
            this.MainGrid.Children.Add(this.sliderView);

            this.AllowDrop = true;
            this.PreviewDrop += this.MainTabControl_PreviewDragEnter;        
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public CombinedHeatmapViewModel ViewModel { get; set; }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
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

        #endregion

        #region Methods

        /// <summary>
        /// TODO The load file.
        /// </summary>
        /// <param name="fileName">
        /// TODO The file name.
        /// </param>
        private void LoadFile(string fileName)
        {
            this.ViewModel.HeatMapViewModel.InitializeUimfData(fileName);
            this.ViewModel.FrameManipulationViewModel.CurrentFrame = 1;

            // this.totalIonChromatogramViewModel.UpdateReference(this.heatMapViewModel.HeatMapData);
        }

        /// <summary>
        /// TODO The main tab control_ preview drag enter.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
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

        #endregion
    }
}