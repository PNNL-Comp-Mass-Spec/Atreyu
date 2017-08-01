using System.Reactive;
using System.Threading;
using System.Windows;
using Xceed.Wpf.DataGrid.Converters;

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
    /// The combined heatmap view model.
    /// </summary>
    [Export]
    public class CombinedHeatmapViewModel : ReactiveObject
    {
        #region Constants

        /// <summary>
        /// The return gated data bool.
        /// </summary>
        private const bool ReturnGatedData = true;

        #endregion

        #region Fields

        /// <summary>
        /// The center of the m/z window.
        /// </summary>
        private double centerMz = 1000.0D;

        /// <summary>
        /// A private backing field for a property that indicates whether The circular wait is visible.
        /// </summary>
        private bool circularWaitIsVisible;

        /// <summary>
        /// The current end frame.
        /// </summary>
        private int currentEndFrame;

        /// <summary>
        /// The name of the current file that is loaded, without path or extension.
        /// </summary>
        private string currentFile = "Heatmap";

        /// <summary>
        /// The current start frame.
        /// </summary>
        private int currentStartFrame;

        /// <summary>
        /// The height of the view.
        /// </summary>
        private int height;

       
        private Range<double> mzWindow;

        /// <summary>
        /// The parts per million tolerance for the m/z window.
        /// </summary>
        private double partsPerMillion = 150.0D;

        /// <summary>
        /// The uimf data.
        /// </summary>
        private UimfData uimfData;

        /// <summary>
        /// The width of the view.
        /// </summary>
        private int width;

        /// <summary>
        /// Whether or not to window the M/Z.
        /// </summary>
        private bool windowMz;
        private bool _bpiEnabled;
        private bool _ticEnabled;
        private Visibility _ticVisible;
        private Visibility _bpiVisible;
        private bool _rangeEnabled;
        private bool _calibEnabled;
        private Visibility _rangeVisible;
        private bool _showScanTime;
        private bool _showFrameCollapsed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapViewModel"/> class.
        /// </summary>
        public CombinedHeatmapViewModel()
        {
            // I wonder if I should break this up a little, as it is over 100 lines and breaking them up logically might make it more readable and maintainable
            this.FrameManipulationViewModel = new FrameManipulationViewModel();
            this.HeatMapViewModel = new HeatMapViewModel();
            this.MzSpectraViewModel = new MzSpectraViewModel();
            this.LowValueGateSliderViewModel = new GateSliderViewModel();
            this.TotalIonChromatogramViewModel = new TotalIonChromatogramViewModel();
            this.TofCalibratorViewModel = new ToFCalibratorViewModel();

            this.LowValueGateSliderViewModel.ControlLabel = "Low Gate";
            this.LowValueGateSliderViewModel.UpdateGate(0);

            this.TicEnabled = true;
            this.RangeEnabled = true;
            this.CalibEnabled = false;

            // Keep the M/Z mode settings updated
            this.WhenAnyValue(vm => vm.MzCenter, vm => vm.PartsPerMillion, vm => vm.MzRangeEnabled)
                .Where(tuple => this.UimfData != null && tuple.Item1 >= 1 && tuple.Item2 >= 1)
                .Subscribe(x =>
                {
                    this.UimfData.MzCenter = x.Item1;
                    this.UimfData.PartsPerMillion = x.Item2;
                    this.UimfData.WindowMz = x.Item3;
                    this.UpdateMzWindow();
                });

            this.ZoomOutFull = this.FrameManipulationViewModel.ZoomOutCommand;
            this.ZoomOutFull.Select(async _ => await Task.Run(() => this.ZoomOut())).Subscribe();

            // update the uimf data for the various components
            this.WhenAnyValue(vm => vm.UimfData).Where(x => x != null).Subscribe(data =>
            {
                this.FrameManipulationViewModel.UpdateUimf(data);
                this.HeatMapViewModel.UpdateReference(data);
                this.MzSpectraViewModel.UpdateReference(data);
                this.TotalIonChromatogramViewModel.UpdateReference(data);
                this.TofCalibratorViewModel.UpdateExistingCalib(data, this.currentFile);

               
            });


            // update the frame data of the TIC plot when needed; apparently the Throttler should always specify the schedule.
            this.WhenAnyValue(vm => vm.UimfData.GatedFrameData).Where(x => x != null).Throttle(TimeSpan.FromMilliseconds(50))
                .Subscribe(data =>
                {
                    this.HeatMapViewModel.UpdateData(data);
                    this.MzSpectraViewModel.UpdateFrameData(data);
                    this.TotalIonChromatogramViewModel.UpdateFrameData(data);
                });

            // update the frame whenever it is changed via the frame manipulation view
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.CurrentFrame)
                .Select(async x => await Task.Run(() => this.FetchSingleFrame(x)))
                .Subscribe();

            // hook up the frame summing feature
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.Range).Subscribe(this.SumFrames);
            
            // Update the Heatmap axes
            this.WhenAnyValue(vm => vm.UimfData.Ranges).Subscribe(ranges=>
            {
                lock (syncRoot)
                {
                    this.HeatMapViewModel.CurrentMinScan = ranges.StartScan;
                    this.TotalIonChromatogramViewModel.ChangeStartScan(ranges.StartScan);

                    this.HeatMapViewModel.CurrentMaxScan = ranges.EndScan;
                    this.TotalIonChromatogramViewModel.ChangeEndScan(ranges.EndScan);

                    this.HeatMapViewModel.CurrentMinMz = ranges.CurrentMinMz;
                    this.MzSpectraViewModel.ChangeStartMz(ranges.CurrentMinMz);

                    this.HeatMapViewModel.CurrentMaxMz = ranges.CurrentMaxMz;
                    this.MzSpectraViewModel.ChangeEndMz(ranges.CurrentMaxMz);

                    var data = this.UimfData.ReadData(ranges,
                        new Range<int>(this.uimfData.StartFrameNumber, this.uimfData.EndFrameNumber), this.Height, this.Width);
                    if (!ShowFrameCollapsed)
                    {
                        this.HeatMapViewModel.UpdateData(data);
                    }
                    else if (this.uimfData.FrameCollapsed != null)
                    {
                        this.HeatMapViewModel.UpdateData(this.uimfData.FrameCollapsed);
                    }
                    this.MzSpectraViewModel.UpdateFrameData(data);
                    this.TotalIonChromatogramViewModel.UpdateFrameData(data);

                    this.TotalIonChromatogramViewModel.StartScan = this.UimfData.Ranges.StartScan;
                    this.TotalIonChromatogramViewModel.EndScan = this.UimfData.Ranges.EndScan;
                }
                
            });

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
                .Throttle(TimeSpan.FromMilliseconds(50), RxApp.MainThreadScheduler)
                .Subscribe(this.UpdateLowGate);

            // Update the frame type on the Fram Manipulation view
            this.WhenAnyValue(vm => vm.UimfData.FrameType).Subscribe(s => this.FrameManipulationViewModel.FrameType = s);

            // update the values per pixel so that the m/z adjusts correctly
            this.WhenAnyValue(vm => vm.UimfData.ValuesPerPixelY)
                .Subscribe(d => this.MzSpectraViewModel.ValuesPerPixelY = d);

            this.WhenAnyValue(vm => vm.UimfData.BinToMzMap).Subscribe(d =>
            {
                this.HeatMapViewModel.BinToMzMap = d;
                this.MzSpectraViewModel.BinToMzMap = d;
            });
            this.WhenAnyValue(vm => vm.UimfData.BinToTofMap).Subscribe(d =>
            {
                this.MzSpectraViewModel.BinToTofMap = d;
            });
            this.WhenAnyValue(vm => vm.UimfData.MzArray).Subscribe(d => this.MzSpectraViewModel.MzArray = d);
            this.WhenAnyValue(vm => vm.UimfData.MzIntensities).Subscribe(i => this.MzSpectraViewModel.MzIntensities = i);

            //this.WhenAnyValue(vm => vm.UimfData.Uncompressed)
            //    .Subscribe(uncomp => this.HeatMapViewModel.uncompressed = uncomp);

            this.WhenAnyValue(vm => vm.UimfData.Calibrator).Subscribe(c => this.MzSpectraViewModel.Calibrator = c);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.Height).Subscribe(d => this.Height = d);
            this.WhenAnyValue(vm => vm.HeatMapViewModel.Width).Subscribe(d => this.Width = d);

            
            this.MzSpectraViewModel.WhenAnyValue(mzStart => mzStart.StartMZ, mzEnd => mzEnd.EndMZ)
                .Where(_ => this.UimfData != null)
                .Throttle(TimeSpan.FromMilliseconds(5), RxApp.MainThreadScheduler)
                .Subscribe(x => this.HeatMapViewModel.CurrentMzRange = new Range<double>(this.MzSpectraViewModel.StartMZ, this.MzSpectraViewModel.EndMZ));

            this.TofCalibratorViewModel.WhenAnyValue(x => x.ReloadUIMF)
                .Where(_ => this.TofCalibratorViewModel.ReloadUIMF)
                .Subscribe(y =>
                {
                    this.InitializeUimfData(this.TofCalibratorViewModel.NewFileName);
                    this.TofCalibratorViewModel.ReloadUIMF = false;
                });

            this.TotalIonChromatogramViewModel.WhenAnyValue(ticStart => ticStart.StartScan, ticEnd => ticEnd.EndScan)
                .Where(_ => this.UimfData != null)
                .Throttle(TimeSpan.FromMilliseconds(5), RxApp.MainThreadScheduler)
                .Subscribe(x => this.HeatMapViewModel.CurrentScanRange = new Range<int>(this.TotalIonChromatogramViewModel.StartScan, this.TotalIonChromatogramViewModel.EndScan));

            this.HeatMapViewModel.WhenAnyValue(palette => palette.SelectedPalette).Where(_ => this.UimfData != null)
                .Subscribe(x =>
                {
                    this.ZoomOut();
                    this.HeatMapViewModel.HeatMapPlotModel.ResetAllAxes();
                });

            this.HeatMapViewModel.WhenAnyValue(x => x.ShowLogData)
                .Where(_ => this.UimfData != null)
                .Subscribe(y =>
            {
                var value = this.HeatMapViewModel.ShowLogData;
                this.MzSpectraViewModel.ShowLogData = value;
            });
        }

        #endregion
        
        #region Public Properties


        private object syncRoot = new object();
        /// <summary>
        /// Gets the frame manipulation view model.
        /// </summary>
        public FrameManipulationViewModel FrameManipulationViewModel { get; }

        /// <summary>
        /// Gets the heat map view model.
        /// </summary>
        public HeatMapViewModel HeatMapViewModel { get; }

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
        public GateSliderViewModel LowValueGateSliderViewModel { get; }

        /// <summary>
        /// Gets or sets the mz center.
        /// </summary>
        public double MzCenter
        {
            get
            {
                return this.centerMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.centerMz, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mz range enabled.
        /// </summary>
        public bool MzRangeEnabled
        {
            get
            {
                return this.windowMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.windowMz, value);
            }
        }

        /// <summary>
        /// Gets the mz spectra view model.
        /// </summary>
        public MzSpectraViewModel MzSpectraViewModel { get; }

        /// <summary>
        /// Gets or sets the parts per million.
        /// </summary>
        public double PartsPerMillion
        {
            get
            {
                return this.partsPerMillion;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.partsPerMillion, value);
            }
        }

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
        public ReactiveCommand<Unit, Unit> ZoomOutFull { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The export heatmap data compressed.
        /// </summary>
        /// <returns>
        /// The 2d array of <see cref="double"/>s that represents that data in the view.
        /// </returns>
        public double[,] ExportHeatmapDataCompressed()
        {
            return new double[0,0];
        }

        /// <summary>
        /// The export mz data compressed method.
        /// </summary>
        /// <returns>
        /// The dictionary of data, keyed by mz.
        /// </returns>
        public IDictionary<double, double> ExportMzDataCompressed()
        {
            return this.MzSpectraViewModel.GetMzDataCompressed();
        }


        /// <summary>
        /// The set up plot method.
        /// </summary>
        /// <param name="frameNumber">
        /// The frame number to load.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public void FetchSingleFrame(int frameNumber)
        {
            this.currentStartFrame = frameNumber;
            this.currentEndFrame = frameNumber;
            if(frameNumber != 0)
                this.UimfData.UpdateTofTime(frameNumber);
        }

        /// <summary>
        /// The initialize uimf data.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public void InitializeUimfData(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return;
            }

            this.UimfData = new UimfData(file);
            this.FetchSingleFrame(1);
            this.TofCalibratorViewModel.FileName = Path.GetFullPath(file);
            this.HeatMapViewModel.CurrentFile = Path.GetFileNameWithoutExtension(file);
        }

        /// <summary>
        /// The sum frames.
        /// </summary>
        /// <param name="sumFrames">
        /// The frame range to sum.
        /// </param>
        public void SumFrames(Range<int> sumFrames)
        {
            if (sumFrames == null)
            {
                return;
            }

            if (this.UimfData == null)
            {
                return;
            }

            this.currentStartFrame = sumFrames.Start < 1 ? 1 : sumFrames.Start;

            this.currentEndFrame = sumFrames.End < 1 ? 1 : sumFrames.End;

            //this.UimfData.ReadData(
            //        this.UimfData.CurrentMinMz,
            //        this.UimfData.CurrentMaxMz,
            //        this.currentStartFrame,
            //        this.currentEndFrame,
            //        this.Height,
            //        this.Width,
            //        this.UimfData.StartScan,
            //        this.UimfData.EndScan,
            //        ReturnGatedData);
        }

        /// <summary>
        /// The update low gate.
        /// </summary>
        /// <param name="gate">
        /// The gate.
        /// </param>
        public void UpdateLowGate(double gate)
        {
            UimfData?.UpdateLowGate(gate);
        }

        /// <summary>
        /// The zoom out.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public void ZoomOut()
        {
            var scanRange = new Range<int>(0, this.UimfData.Scans);

            var mzRange = this.windowMz ? this.mzWindow : new Range<double>(this.UimfData.MinMz, this.UimfData.MaxMz);

            this.UimfData.Ranges = (mzRange.Start, mzRange.End, scanRange.Start, scanRange.End);
            this.HeatMapViewModel.HeatMapPlotModel.ResetAllAxes();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the mz window.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private void UpdateMzWindow()
        {
            this.mzWindow = this.uimfData.GetMzRangeForMzWindow(this.MzCenter, this.PartsPerMillion);
            this.HeatMapViewModel.MzWindow = this.mzWindow;
            var frameRange = new Range<int>(this.currentStartFrame, this.currentEndFrame);
            var scanRange = new Range<int>(this.TotalIonChromatogramViewModel.StartScan,
                this.TotalIonChromatogramViewModel.EndScan);
            this.UimfData.Ranges = (mzWindow.Start, mzWindow.End, scanRange.Start, scanRange.End);
        }

        #endregion

        public bool ShowScanTime
        {
            get { return this._showScanTime; }
            set
            {
                this.RaiseAndSetIfChanged(ref _showScanTime, value);
                this.TotalIonChromatogramViewModel.ShowScanTime = value;
            }
        }

        public bool TicEnabled
        {
            get { return this._ticEnabled; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._ticEnabled, value);
            }
        }

        public bool ShowFrameCollapsed { get; set; }

        public ToFCalibratorViewModel TofCalibratorViewModel { get; set; }

        public Visibility RangeVisible
        {
            get { return this._rangeVisible; }
            set { this.RaiseAndSetIfChanged(ref this._rangeVisible, value); }
        }

        public bool RangeEnabled
        {
            get { return this._rangeEnabled; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._rangeEnabled, value);
                RangeVisible = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility CalibVisible
        {
            get { return this.TofCalibratorViewModel.CalibVisible; }
            set { this.TofCalibratorViewModel.CalibVisible = value; }
        }

        public bool CalibEnabled
        {
            get { return this._calibEnabled; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._calibEnabled, value);
                CalibVisible = value ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}