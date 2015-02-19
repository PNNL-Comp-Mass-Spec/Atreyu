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
        #region Constants

        /// <summary>
        /// TODO The return gated data.
        /// </summary>
        private const bool ReturnGatedData = true;

        #endregion

        #region Fields

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
        /// TODO The height.
        /// </summary>
        private int height;

        /// <summary>
        /// TODO The uimf data.
        /// </summary>
        private UimfData uimfData;

        /// <summary>
        /// TODO The width.
        /// </summary>
        private int width;

        #endregion

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
            this.TotalIonChromatogramViewModel = new TotalIonChromatogramViewModel();

            this.LowValueGateSliderViewModel.ControlLabel = "Low Gate";
            this.LowValueGateSliderViewModel.UpdateGate(0);

            this.ZoomOutFull = this.FrameManipulationViewModel.ZoomOutCommand;
            this.ZoomOutFull.Select(async _ => await this.ZoomOut()).Subscribe();

            // update the uimf data for the various components
            this.WhenAnyValue(vm => vm.UimfData).Subscribe(this.TotalIonChromatogramViewModel.UpdateReference);

            this.WhenAnyValue(vm => vm.UimfData).Subscribe(this.HeatMapViewModel.UpdateReference);

            this.WhenAnyValue(vm => vm.UimfData).Subscribe(this.FrameManipulationViewModel.UpdateUimf);

            this.WhenAnyValue(vm => vm.UimfData).Subscribe(this.MzSpectraViewModel.UpdateReference);

            // update the frame data of the TIC plot when needed; apparently the Throttler should always specify the schedule.
            this.WhenAnyValue(vm => vm.UimfData.GatedFrameData)
                .Subscribe(this.TotalIonChromatogramViewModel.UpdateFrameData);

            // Update the Framedata of the M/Z plot when needed
            this.WhenAnyValue(vm => vm.UimfData.GatedFrameData).Subscribe(this.MzSpectraViewModel.UpdateFrameData);

            this.WhenAnyValue(vm => vm.UimfData.GatedFrameData).Subscribe(this.HeatMapViewModel.UpdateData);

            // update the frame whenever it is changed via the frame manipulation view
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.CurrentFrame)
                .Select(async x => await this.FetchSingleFrame(x))
                .Subscribe();

            // hook up the frame summing feature
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.Range).Subscribe(this.SumFrames);

            // These make the axis on the TIC update properly
            this.WhenAnyValue(vm => vm.UimfData.StartScan).Subscribe(this.TotalIonChromatogramViewModel.ChangeStartScan);
            this.WhenAnyValue(vm => vm.UimfData.EndScan).Subscribe(this.TotalIonChromatogramViewModel.ChangeEndScan);

            // These make the axis on the mz plot update properly
            this.WhenAnyValue(vm => vm.UimfData.CurrentMinBin).Subscribe(this.MzSpectraViewModel.changeStartBin);

            // This makes the axis of the mz plot be in mz mode properly
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.MzModeEnabled)
                .Subscribe(b => this.MzSpectraViewModel.ShowMz = b);

            // These two allow the mz calculation to be done on the fly so it is fast, instead of calling the hard drive again 
            this.WhenAnyValue(vm => vm.UimfData.FrameSlope)
                .Where(b => this.UimfData != null)
                .Subscribe(d => this.MzSpectraViewModel.Slope = d);

            this.WhenAnyValue(vm => vm.UimfData.FrameIntercept)
                .Where(b => this.UimfData != null)
                .Subscribe(d => this.MzSpectraViewModel.Intercept = d);

            // Attach the heatmap threshold to the slider's gate, using Throttle so it doesn't seem jerky.
            this.WhenAnyValue(vm => vm.LowValueGateSliderViewModel.LogarithmicGate)
                .Where(b => this.UimfData != null)
                .Throttle(TimeSpan.FromMilliseconds(200), RxApp.MainThreadScheduler)
                .Subscribe(this.UpdateLowGate);

            // Update the frame type on the Fram Manipulation view
            this.WhenAnyValue(vm => vm.UimfData.FrameType).Subscribe(s => this.FrameManipulationViewModel.FrameType = s);

            // update the values per pixel so that the m/z adjusts correctly
            this.WhenAnyValue(vm => vm.UimfData.ValuesPerPixelY)
                .Subscribe(d => this.MzSpectraViewModel.ValuesPerPixelY = d);

            this.WhenAnyValue(vm => vm.UimfData.BinToMzMap).Subscribe(d => this.MzSpectraViewModel.BinToMzMap = d);

            this.WhenAnyValue(vm => vm.UimfData.BinToMzMap).Subscribe(d => this.HeatMapViewModel.BinToMzMap = d);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.Height).Subscribe(d => this.Height = d);
            this.WhenAnyValue(vm => vm.HeatMapViewModel.Width).Subscribe(d => this.Width = d);

            var minBin = this.HeatMapViewModel.WhenAnyValue(vm => vm.CurrentMinBin).Where(b => this.UimfData != null);

            var maxBin = this.HeatMapViewModel.WhenAnyValue(vm => vm.CurrentMaxBin).Where(b => this.UimfData != null);

            var zipBin = minBin.Zip(
                maxBin, 
                delegate(int i, int i1)
                    {
                        this.UimfData.CurrentMinBin = i;
                        this.UimfData.CurrentMaxBin = i1;
                        return 0;
                    });

            var startScan = this.HeatMapViewModel.WhenAnyValue(vm => vm.CurrentMinScan)
                .Where(b => this.UimfData != null);

            var endScan = this.HeatMapViewModel.WhenAnyValue(vm => vm.CurrentMaxScan).Where(b => this.UimfData != null);

            var zipScan = startScan.Zip(
                endScan, 
                delegate(int i, int i1)
                    {
                        this.UimfData.StartScan = i;
                        this.uimfData.EndScan = i1;
                        return 0;
                    });

            ////zipBin.Throttle(TimeSpan.FromMilliseconds(2), RxApp.MainThreadScheduler).Select(async _ => await this.uimfData.ReadData(ReturnGatedData)).Subscribe();
            ////zipScan.Throttle(TimeSpan.FromMilliseconds(2), RxApp.MainThreadScheduler).Select(async _ => await this.uimfData.ReadData(ReturnGatedData)).Subscribe();
            zipBin.CombineLatest(zipScan, (x, z) => x + z)
                .Synchronize(true)
                .Select(async _ => await this.uimfData.ReadData(ReturnGatedData))
                .Subscribe();

            ////var pattern = minBin.And(maxBin).And(startScan).And(endScan);
            ////var plan = pattern.Then(
            ////        (int minB, int maxB, int startS, int endS) =>
            ////        {
            ////            this.uimfData.CurrentMinBin = minB;
            ////            this.uimfData.CurrentMaxBin = maxB;
            ////            this.uimfData.StartScan = startS;
            ////            this.uimfData.EndScan = endS;
            ////            return 0;
            ////        });

            ////var zippedSequence = Observable.When(plan);
            ////zippedSequence.Select(async _ => await this.uimfData.ReadData(ReturnGatedData)).Subscribe();
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
        /// Gets the frame manipulation view model.
        /// </summary>
        public FrameManipulationViewModel FrameManipulationViewModel { get; private set; }

        /// <summary>
        /// Gets the heat map view model.
        /// </summary>
        public HeatMapViewModel HeatMapViewModel { get; private set; }

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
        ///  Gets or sets the data relevant to the UIMF that is loaded.
        /// </summary>
        public UimfData UimfData
        {
            get
            {
                return this.uimfData;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.uimfData, value);
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
        /// TODO The set up plot.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task FetchSingleFrame(int frameNumber)
        {
            if (this.UimfData == null)
            {
                return;
            }

            this.currentStartFrame = frameNumber;
            this.currentEndFrame = frameNumber;

            await
                this.UimfData.ReadData(
                    1, 
                    this.UimfData.MaxBins, 
                    frameNumber, 
                    frameNumber, 
                    this.Height, 
                    this.Width, 
                    0, 
                    this.UimfData.Scans, 
                    ReturnGatedData);
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
            const int Alignment = 25;

            var bitmap = new Bitmap(mz.Width + heatmap.Width, heatmap.Height + tic.Height + Alignment);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(mz, 0, 0);
                g.DrawImage(heatmap, mz.Width, Alignment);
                g.DrawImage(tic, mz.Width, heatmap.Height + Alignment);
            }

            return bitmap;
        }

        /// <summary>
        /// TODO The initialize uimf data.
        /// </summary>
        /// <param name="file">
        /// TODO The file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeUimfData(string file)
        {
            this.UimfData = new UimfData(file) { CurrentMinBin = 0 };
            this.UimfData.CurrentMaxBin = this.UimfData.TotalBins;
            await this.FetchSingleFrame(1);
            this.CurrentFile = Path.GetFileNameWithoutExtension(file);
        }

        /// <summary>
        /// TODO The sum frames.
        /// </summary>
        /// <param name="sumFrames">
        /// TODO The sum frames.
        /// </param>
        public async void SumFrames(FrameRange sumFrames)
        {
            if (sumFrames == null)
            {
                return;
            }

            if (this.UimfData == null)
            {
                return;
            }

            this.currentStartFrame = sumFrames.StartFrame < 1 ? 1 : sumFrames.StartFrame;

            this.currentEndFrame = sumFrames.EndFrame < 1 ? 1 : sumFrames.EndFrame;
            await
                this.UimfData.ReadData(
                    this.UimfData.CurrentMinBin, 
                    this.UimfData.CurrentMaxBin, 
                    this.currentStartFrame, 
                    this.currentEndFrame, 
                    this.Height, 
                    this.Width, 
                    this.UimfData.StartScan, 
                    this.UimfData.EndScan, 
                    ReturnGatedData);
        }

        /// <summary>
        /// TODO The update low gate.
        /// </summary>
        /// <param name="gate">
        /// TODO The gate.
        /// </param>
        public void UpdateLowGate(double gate)
        {
            if (this.UimfData == null)
            {
                return;
            }

            this.UimfData.UpdateLowGate(gate);
        }

        /// <summary>
        /// TODO The zoom out.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ZoomOut()
        {
            this.UimfData.UpdateScanRange(0, this.UimfData.Scans);

            this.UimfData.CurrentMinBin = 0;
            this.UimfData.CurrentMaxBin = this.UimfData.MaxBins;

            await this.UimfData.ReadData(ReturnGatedData);
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The update view region.
        /// </summary>
        /// <param name="minBin">
        /// TODO The min bin.
        /// </param>
        /// <param name="maxBin">
        /// TODO The max bin.
        /// </param>
        /// <param name="minScan">
        /// TODO The min scan.
        /// </param>
        /// <param name="maxScan">
        /// TODO The max scan.
        /// </param>
        private void UpdateViewRegion(int minBin, int maxBin, int minScan, int maxScan)
        {
            this.uimfData.CurrentMinBin = minBin;
            this.uimfData.CurrentMaxBin = maxBin;
            this.uimfData.StartScan = minScan;
            this.uimfData.EndScan = maxScan;
        }

        #endregion
    }
}