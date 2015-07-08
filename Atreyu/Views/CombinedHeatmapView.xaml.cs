// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedHeatmapView.xaml.cs" company="Pacific Northwest National Laboratory">
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
//   Interaction logic for CombinedHeatmapView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Views
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Atreyu.ViewModels;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for CombinedHeatmapView
    /// </summary>
    public partial class CombinedHeatmapView : IViewFor<CombinedHeatmapViewModel>
    {
        #region Fields

        /// <summary>
        /// The frame manipulation view.
        /// </summary>
        private FrameManipulationView frameManipulationView;

        /// <summary>
        /// The heat map view.
        /// </summary>
        private HeatMapView heatMapView;

        /// <summary>
        /// The low slider view.
        /// </summary>
        private GateSlider lowSliderView;

        /// <summary>
        /// The mz spectra view.
        /// </summary>
        private MzSpectraView mzSpectraView;

        /// <summary>
        /// The total ion chromatogram view.
        /// </summary>
        private TotalIonChromatogramView totalIonChromatogramView;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapView"/> class.
        /// </summary>
        public CombinedHeatmapView()
        {
            this.InitializeComponent();
            this.DataContextChanged += this.CombinedHeatmapViewDataContextChanged;

            ////this.AllowDrop = true;
            ////this.PreviewDrop += this.CombinedHeatmapViewPreviewDrop;
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
        /// The combined heatmap view data context changed method.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void CombinedHeatmapViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ViewModel = e.NewValue as CombinedHeatmapViewModel;

            if (this.ViewModel == null)
            {
                throw new ArgumentException("arguement e is only allowed to be of type CombinedHeatmapViewModel", "e");
            }

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

            this.lowSliderView = new GateSlider(this.ViewModel.LowValueGateSliderViewModel);
            Grid.SetRow(this.lowSliderView, 1);
            Grid.SetColumn(this.lowSliderView, 3);
            this.MainGrid.Children.Add(this.lowSliderView);

            this.RangeControl.DataContext = this.ViewModel;

            this.PreviewDrop += this.MainTabControlPreviewDragEnter;
            this.AllowDrop = true;
        }

        /// <summary>
        /// The combined heatmap view preview drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private async void CombinedHeatmapViewPreviewDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                return;
            }

            var droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
            if (droppedFilePaths == null || droppedFilePaths.Length < 1)
            {
                return;
            }

            var file = droppedFilePaths[0];
            if (!File.Exists(file))
            {
                return;
            }

            await this.ViewModel.InitializeUimfData(file);
        }

        /// <summary>
        /// The load file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task LoadFile(string fileName)
        {
            await this.ViewModel.InitializeUimfData(fileName);
            this.ViewModel.FrameManipulationViewModel.CurrentFrame = 1;
        }

        /// <summary>
        /// The main tab control preview drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private async void MainTabControlPreviewDragEnter(object sender, DragEventArgs e)
        {
            var isCorrect = true;
            string[] filenames = { };
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
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
                try
                {
                    await this.LoadFile(filenames[0]);
                }
                catch (Exception ex)
                {
                    // This ignores a known race condition with an uknown cause and allows the user to continue as if all is well.
                    if (!ex.Message.Contains("already active"))
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            e.Handled = true;
        }

        #endregion
    }
}