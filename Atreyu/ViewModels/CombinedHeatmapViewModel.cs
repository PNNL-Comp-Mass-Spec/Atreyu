// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedHeatmapViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The combined heatmap view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using Atreyu.Models;

    using ReactiveUI;

    /// <summary>
    /// TODO The combined heatmap view model.
    /// </summary>
    [Export]
    public class CombinedHeatmapViewModel : ReactiveObject
    {
        /// <summary>
        /// TODO The current end frame.
        /// </summary>
        private int currentEndFrame;

        /// <summary>
        /// The name of the current file that is loaded, without path or extension.
        /// </summary>
        private string currentFile = "Heatmap";

        /// <summary>
        /// TODO The current start frame.
        /// </summary>
        private int currentStartFrame;

        /// <summary>
        ///  The data relevant to the UIMF for viewing.
        /// </summary>
        private UimfData uimfData;

        private int width;

        private int height;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapViewModel"/> class.
        /// </summary>
        public CombinedHeatmapViewModel()
        {
            this.FrameManipulationViewModel = new FrameManipulationViewModel();
            this.HeatMapViewModel = new HeatMapViewModel();
            this.MzSpectraViewModel = new MzSpectraViewModel();
            this.LowValueGateSliderViewModel = new GateSliderViewModel();
            this.HighValueGateSliderViewModel = new GateSliderViewModel();
            this.TotalIonChromatogramViewModel = new TotalIonChromatogramViewModel();

            this.LowValueGateSliderViewModel.ControlLabel = "Low Gate";
            this.LowValueGateSliderViewModel.UpdateGate(0);
            this.HighValueGateSliderViewModel.ControlLabel = "High Cutoff";
            this.HighValueGateSliderViewModel.UpdateGate(this.HighValueGateSliderViewModel.MaximumValue);

            this.ZoomOutFull = this.FrameManipulationViewModel.ZoomOutCommand;
            this.ZoomOutFull.Subscribe(x => this.HeatMapViewModel.ZoomOutFull());

            // update the uimf data for the various components
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData)
                .Subscribe(this.TotalIonChromatogramViewModel.UpdateReference);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData)
                .Subscribe(this.FrameManipulationViewModel.UpdateUimf);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData).Subscribe(this.MzSpectraViewModel.UpdateReference);

            // update the frame data of the TIC plot when needed; apparently the Throttler should always specify the schedule.
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.GatedFrameData)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(this.TotalIonChromatogramViewModel.UpdateFrameData);

            // Update the Framedata of the M/Z plot when needed
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.GatedFrameData)
                .Subscribe(this.MzSpectraViewModel.UpdateFrameData);

            // update the frame whenever it is changed via the frame manipulation view
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.CurrentFrame)
                .Subscribe(this.HeatMapViewModel.UpdateFrameNumber);

            // hook up the frame summing feature
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.Range).Subscribe(this.HeatMapViewModel.SumFrames);

            // These make the axis on the TIC update properly
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.StartScan)
                .Subscribe(this.TotalIonChromatogramViewModel.ChangeStartScan);
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.EndScan)
                .Subscribe(this.TotalIonChromatogramViewModel.ChangeEndScan);

            // These make the axis on the mz plot update properly
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.CurrentMinBin)
                .Subscribe(this.MzSpectraViewModel.changeStartBin);
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.CurrentMaxBin)
                .Subscribe(this.MzSpectraViewModel.changeEndBin);

            // This makes the axis of the mz plot be in mz mode properly
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.MzModeEnabled)
                .Subscribe(b => this.MzSpectraViewModel.ShowMz = b);

            ////this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.MzData).Where(i => this.FrameManipulationViewModel.MzModeEnabled)
            ////    .Subscribe(this.MzSpectraViewModel.UpdateFrameData);

            // These two allow the mz calculation to be done on the fly so it is fast, instead of calling the hard drive again 
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameSlope)
                .Where(b => this.HeatMapViewModel.HeatMapData != null)
                .Subscribe(d => this.MzSpectraViewModel.Slope = d);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameIntercept)
                .Where(b => this.HeatMapViewModel.HeatMapData != null)
                .Subscribe(d => this.MzSpectraViewModel.Intercept = d);

            // Attach the heatmap threshold to the slider's gate, using Throttle so it doesn't seem jerky.
            this.WhenAnyValue(vm => vm.LowValueGateSliderViewModel.LogarithmicGate)
                .Throttle(TimeSpan.FromMilliseconds(200), RxApp.MainThreadScheduler)
                .Subscribe(this.HeatMapViewModel.UpdateLowThreshold);

            // Update the frame type on the Fram Manipulation view
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameType)
                .Subscribe(s => this.FrameManipulationViewModel.FrameType = s);

            // update the values per pixel so that the m/z adjusts correctly
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.ValuesPerPixelY)
                .Subscribe(d => this.MzSpectraViewModel.ValuesPerPixelY = d);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.BinToMzMap)
                .Subscribe(d => this.MzSpectraViewModel.BinToMzMap = d);
        }

        #endregion

        #region Public Properties


        /// <summary>
        /// Gets or sets the current file.
        /// </summary>
        public string CurrentFile
        {
            get
            {
                return this.currentFile;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentFile, value);
            }
        }


        /// <summary>
        /// Gets the height of the Heat map plot.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.height, value);
            }
        }

        /// <summary>
        /// Gets the width of the Heat map plot.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.width, value);
            }
        }

        /// <summary>
        /// Gets the frame manipulation view model.
        /// </summary>
        public FrameManipulationViewModel FrameManipulationViewModel { get; private set; }

        /// <summary>
        /// Gets the heat map view model.
        /// </summary>
        public HeatMapViewModel HeatMapViewModel { get; private set; }

        /// <summary>
        /// Gets the high value gate slider view model.
        /// </summary>
        public GateSliderViewModel HighValueGateSliderViewModel { get; private set; }

        /// <summary>
        /// Gets the low value gate slider view model.
        /// </summary>
        public GateSliderViewModel LowValueGateSliderViewModel { get; private set; }

        /// <summary>
        /// Gets the mz spectra view model.
        /// </summary>
        public MzSpectraViewModel MzSpectraViewModel { get; private set; }

        /// <summary>
        /// Gets the total ion chromatogram view model.
        /// </summary>
        public TotalIonChromatogramViewModel TotalIonChromatogramViewModel { get; private set; }

        /// <summary>
        /// Gets the zoom out full.
        /// </summary>
        public ReactiveCommand<object> ZoomOutFull { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The export heatmap data compressed.
        /// </summary>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public double[,] ExportHeatmapDataCompressed()
        {
            return this.HeatMapViewModel.GetCompressedDataInView();
        }

        /// <summary>
        /// TODO The export mz data compressed.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<double, double> ExportMzDataCompressed()
        {
            return this.MzSpectraViewModel.GetMzDataCompressed();
        }

        /// <summary>
        /// TODO The export tic data compressed.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<int, double> ExportTicDataCompressed()
        {
            return this.TotalIonChromatogramViewModel.GetTicData();
        }

        /// <summary>
        /// TODO The get image.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetImage()
        {
            var tic = this.TotalIonChromatogramViewModel.GetTicImage();
            var mz = this.MzSpectraViewModel.GetMzImage();
            var heatmap = this.HeatMapViewModel.GetHeatmapImage();
            var alignment = 25;

            var bitmap = new Bitmap(mz.Width + heatmap.Width, heatmap.Height + tic.Height + alignment);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(mz, 0, 0);
                g.DrawImage(heatmap, mz.Width, alignment);
                g.DrawImage(tic, mz.Width, heatmap.Height + alignment);
            }

            return bitmap;
        }



        /// <summary>
        /// TODO The initialize uimf data.
        /// </summary>
        /// <param name="file">
        /// TODO The file.
        /// </param>
        public void InitializeUimfData(string file)
        {
            this.uimfData = new UimfData(file) { CurrentMinBin = 0 };
            this.uimfData.CurrentMaxBin = this.uimfData.TotalBins;
            this.FetchSingleFrame(1);
            this.CurrentFile = Path.GetFileNameWithoutExtension(file);
        }


        /// <summary>
        /// TODO The set up plot.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        public void FetchSingleFrame(int frameNumber)
        {
            this.currentStartFrame = frameNumber;
            this.currentEndFrame = frameNumber;

            this.uimfData.ReadData(
                1,
                this.uimfData.MaxBins,
                frameNumber,
                frameNumber,
                this.Height,
                this.Width,
                0,
                this.uimfData.Scans,
                true);
        }


        public async void SumFrames(FrameRange sumFrames)
        {
            if (sumFrames == null)
            {
                return;
            }
            
            if (this.uimfData == null)
            {
                return;
            }

            this.currentStartFrame = sumFrames.StartFrame < 1 ? 1 : sumFrames.StartFrame;

            this.currentEndFrame = sumFrames.EndFrame < 1 ? 1 : sumFrames.EndFrame;
                await Task.Run(
                    () =>
                    {
                        this.uimfData.ReadData(
                            this.uimfData.CurrentMinBin,
                            this.uimfData.CurrentMaxBin,
                            this.currentStartFrame,
                            this.currentEndFrame,
                            this.Height,
                            this.Width,
                            this.uimfData.StartScan,
                            this.uimfData.EndScan,
                            true);
                    });
        }


        public void ZoomOut()
        {
            this.uimfData.UpdateScanRange(0, this.uimfData.EndScan);

            this.uimfData.CurrentMinBin = 0;
            this.uimfData.CurrentMaxBin = this.uimfData.MaxBins;

            this.uimfData.ReadData(true);
        }

        #endregion
    }
}