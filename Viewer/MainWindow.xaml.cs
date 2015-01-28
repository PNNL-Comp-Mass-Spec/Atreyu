// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Viewer
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Atreyu.ViewModels;

    using Falkor.Views.Atreyu;

    using Microsoft.Practices.Prism.PubSubEvents;
    using Microsoft.Win32;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// TODO The event aggregator.
        /// </summary>
        private EventAggregator eventAggregator = new EventAggregator();

        /// <summary>
        /// TODO The frame manipulation view.
        /// </summary>
        private FrameManipulationView frameManipulationView;

        /// <summary>
        /// TODO The frame manipulation view model.
        /// </summary>
        private FrameManipulationViewModel frameManipulationViewModel;

        /// <summary>
        /// TODO The heat map view.
        /// </summary>
        private HeatMapView heatMapView;

        /// <summary>
        /// TODO The heat map view model.
        /// </summary>
        private HeatMapViewModel heatMapViewModel;

        /// <summary>
        /// TODO The load button.
        /// </summary>
        private Button loadButton;

        /// <summary>
        /// TODO The mz spectra view.
        /// </summary>
        private MzSpectraView mzSpectraView;

        /// <summary>
        /// TODO The mz spectra view model.
        /// </summary>
        private MzSpectraViewModel mzSpectraViewModel;

        /// <summary>
        /// TODO The total ion chromatogram view.
        /// </summary>
        private TotalIonChromatogramView totalIonChromatogramView;

        /// <summary>
        /// TODO The total ion chromatogram view model.
        /// </summary>
        private TotalIonChromatogramViewModel totalIonChromatogramViewModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.heatMapViewModel = new HeatMapViewModel();
            this.heatMapView = new HeatMapView(this.heatMapViewModel);
            Grid.SetColumn(this.heatMapView, 1);
            Grid.SetRow(this.heatMapView, 1);
            this.MainGrid.Children.Add(this.heatMapView);

            this.frameManipulationViewModel = new FrameManipulationViewModel();
            this.frameManipulationView = new FrameManipulationView(this.frameManipulationViewModel);
            Grid.SetColumn(this.frameManipulationView, 1);
            Grid.SetRow(this.frameManipulationView, 0);
            this.MainGrid.Children.Add(this.frameManipulationView);

            this.mzSpectraViewModel = new MzSpectraViewModel();
            this.mzSpectraView = new MzSpectraView(this.mzSpectraViewModel);
            var transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(90));

            // transform.Children.Add(new ScaleTransform(-1, 1));
            // transform.Children.Add(new );
            // this.mzSpectraView.RenderTransform = transform;
            Grid.SetColumn(this.mzSpectraView, 0);
            Grid.SetRow(this.mzSpectraView, 1);

            // this.mzSpectraView.VerticalAlignment = VerticalAlignment.Stretch;
            // this.mzSpectraView.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.MainGrid.Children.Add(this.mzSpectraView);

            this.totalIonChromatogramViewModel = new TotalIonChromatogramViewModel();
            this.totalIonChromatogramView = new TotalIonChromatogramView(this.totalIonChromatogramViewModel);
            Grid.SetColumn(this.totalIonChromatogramView, 1);
            Grid.SetRow(this.totalIonChromatogramView, 3);
            this.MainGrid.Children.Add(this.totalIonChromatogramView);

            // update the uimf data for the various components
            this.WhenAnyValue(vm => vm.heatMapViewModel.HeatMapData)
                .Subscribe(this.totalIonChromatogramViewModel.UpdateReference);

            this.WhenAnyValue(vm => vm.heatMapViewModel.HeatMapData)
                .Subscribe(this.frameManipulationViewModel.UpdateUimf);

            this.WhenAnyValue(vm => vm.heatMapViewModel.HeatMapData).Subscribe(this.mzSpectraViewModel.UpdateReference);

            // update the frame data of the TIC plot when needed
            this.WhenAnyValue(vm => vm.heatMapViewModel.HeatMapData.FrameData)
                .Subscribe(this.totalIonChromatogramViewModel.UpdateFrameData);

            // Update the Framedata of the M/Z plot when needed
            this.WhenAnyValue(vm => vm.heatMapViewModel.HeatMapData.FrameData)
                .Subscribe(this.mzSpectraViewModel.UpdateFrameData);

            // update the frame whenever it is changed via the frame manipulation view
            this.WhenAnyValue(vm => vm.frameManipulationViewModel.CurrentFrame)
                .Subscribe(this.heatMapViewModel.UpdateFrameNumber);

            // hook up the frame summing feature
            this.WhenAnyValue(vm => vm.frameManipulationViewModel.Range).Subscribe(this.heatMapViewModel.SumFrames);

            this.loadButton = new Button { Content = "Load" };
            this.loadButton.Click += this.LoadButtonClick;
            Grid.SetColumn(this.loadButton, 0);
            Grid.SetRow(this.loadButton, 0);
            this.MainGrid.Children.Add(this.loadButton);

            this.AllowDrop = true;
            this.PreviewDrop += this.MainTabControl_PreviewDragEnter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The load button click.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted", 
            Justification = "Reviewed. Suppression is OK here.")]
        private void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            var dialogue = new OpenFileDialog
                               {
                                   DefaultExt = ".uimf",
                                   Filter = "Unified Ion Mobility File (*.uimf)|*.uimf"
                               };

            var result = dialogue.ShowDialog();

            if (result != true)
            {
                return;
            }

            var filename = dialogue.FileName;
            this.LoadFile(filename);
        }

        /// <summary>
        /// TODO The load file.
        /// </summary>
        /// <param name="fileName">
        /// TODO The file name.
        /// </param>
        private void LoadFile(string fileName)
        {
            this.heatMapViewModel.InitializeUimfData(fileName);

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