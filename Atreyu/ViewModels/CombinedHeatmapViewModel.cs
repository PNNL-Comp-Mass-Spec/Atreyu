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

        /// <summary>
        /// The m/z window represented as a <see cref="MzRange"/>.
        /// </summary>
        private MzRange mzWindow = new MzRange();

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapViewModel"/> class.
        /// </summary>
        public CombinedHeatmapViewModel()
        {
            this.CurrentFile = "Please load data";

            // I wonder if I should break this up a little, as it is over 100 lines and breaking them up logically might make it more readable and maintainable
            this.FrameManipulationViewModel = new FrameManipulationViewModel();
            this.HeatMapViewModel = new HeatMapViewModel();
            this.MzSpectraViewModel = new MzSpectraViewModel();
            this.LowValueGateSliderViewModel = new GateSliderViewModel();
            this.TotalIonChromatogramViewModel = new TotalIonChromatogramViewModel();
            this.BasePeakIntensityViewModel = new BasePeakIntensityViewModel();

            this.LowValueGateSliderViewModel.ControlLabel = "Low Gate";
            this.LowValueGateSliderViewModel.UpdateGate(0);

            this.TicEnabled = true;
            this.BpiEnabled = false;

            // Shows loading image
            this.WhenAnyValue(vm => vm.UimfData.LoadingData).Subscribe(x => this.CircularWaitIsVisible = x);

            // Keep the M/Z mode settings updated
            this.WhenAnyValue(vm => vm.MzCenter)
                .Where(b => this.UimfData != null)
                .Subscribe(x => this.UimfData.MzCenter = x);
            this.WhenAnyValue(vm => vm.PartsPerMillion)
                .Where(b => this.UimfData != null)
                .Subscribe(x => this.UimfData.PartsPerMillion = x);

            this.WhenAnyValue(vm => vm.MzRangeEnabled).Subscribe(x =>
            {
                this.HeatMapViewModel.ForceMinMaxMz = x;
                if (this.UimfData != null)
                {
                    this.UimfData.WindowMz = x;
                    this.UpdateMzWindow();
                }
            });

            this.ZoomOutFull = this.FrameManipulationViewModel.ZoomOutCommand;
            this.ZoomOutFull.Select(async _ => await Task.Run(() => this.ZoomOut())).Subscribe();

            // update the uimf data for the various components
            this.WhenAnyValue(vm => vm.UimfData).Subscribe(data =>
            {
                this.FrameManipulationViewModel.UpdateUimf(data);
                this.HeatMapViewModel.UpdateReference(data);
                this.MzSpectraViewModel.UpdateReference(data);
                this.TotalIonChromatogramViewModel.UpdateReference(data);
                this.BasePeakIntensityViewModel.UpdateReference(data);
            });

            // update the frame data of the TIC plot when needed; apparently the Throttler should always specify the schedule.
            this.WhenAnyValue(vm => vm.UimfData.GatedFrameData)
                .Subscribe(data =>
                {
                    this.HeatMapViewModel.UpdateData(data);
                    this.MzSpectraViewModel.UpdateFrameData(data);
                    this.TotalIonChromatogramViewModel.UpdateFrameData(data);
                    this.BasePeakIntensityViewModel.UpdateFrameData(data);
                });

            // update the frame whenever it is changed via the frame manipulation view
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.CurrentFrame)
                .Select(async x => await Task.Run(() => this.FetchSingleFrame(x)))
                .Subscribe();

            // hook up the frame summing feature
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.Range).Subscribe(this.SumFrames);

            // Update the Heatmap axes
            this.WhenAnyValue(vm => vm.UimfData.StartScan).Subscribe(i =>
            {
                this.HeatMapViewModel.CurrentMinScan = i;
                this.TotalIonChromatogramViewModel.ChangeStartScan(i);
                this.BasePeakIntensityViewModel.ChangeStartScan(i);
            });
            this.WhenAnyValue(vm => vm.UimfData.EndScan).Subscribe(i =>
            {
                this.HeatMapViewModel.CurrentMaxScan = i;
                this.TotalIonChromatogramViewModel.ChangeEndScan(i);
                this.BasePeakIntensityViewModel.ChangeEndScan(i);
            });
            this.WhenAnyValue(vm => vm.UimfData.CurrentMinMz).Subscribe(i =>
            {
                this.HeatMapViewModel.CurrentMinMz = i;
                this.MzSpectraViewModel.ChangeStartMz(i);
            });
            this.WhenAnyValue(vm => vm.UimfData.CurrentMaxMz).Subscribe(i =>
            {
                this.HeatMapViewModel.CurrentMaxMz = i;
                this.MzSpectraViewModel.ChangeEndMz(i);
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
            this.WhenAnyValue(vm => vm.UimfData.MzArray).Subscribe(d => this.MzSpectraViewModel.MzArray = d);
            this.WhenAnyValue(vm => vm.UimfData.MzIntensities).Subscribe(i => this.MzSpectraViewModel.MzIntensities = i);

            this.WhenAnyValue(vm => vm.UimfData.Uncompressed)
                .Subscribe(uncomp => this.HeatMapViewModel.uncompressed = uncomp);

            this.WhenAnyValue(vm => vm.UimfData.Calibrator).Subscribe(c => this.MzSpectraViewModel.Calibrator = c);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.Height).Subscribe(d => this.Height = d);
            this.WhenAnyValue(vm => vm.HeatMapViewModel.Width).Subscribe(d => this.Width = d);

            this.HeatMapViewModel.WhenAnyValue(hm => hm.CurrentMzRange)
                .Where(_ => this.UimfData != null && this.HeatMapViewModel.CurrentMzRange != null)
                .Select(
                    async x =>
                    {
                        await Task.Run(() =>
                        {
                            this.UimfData.RangeUpdateList.Enqueue(x);
                            this.uimfData.CheckQueue();
                        });
                    }).Subscribe();

            this.HeatMapViewModel.WhenAnyValue(hm => hm.CurrentScanRange)
                .Where(_ => this.UimfData != null && this.HeatMapViewModel.CurrentScanRange != null)
                .Select(
                    async x =>
                    {
                        await Task.Run(() =>
                        {
                            this.UimfData.RangeUpdateList.Enqueue(x);
                            this.uimfData.CheckQueue();
                            this.TotalIonChromatogramViewModel.StartScan = this.uimfData.StartScan;
                            this.TotalIonChromatogramViewModel.EndScan = this.uimfData.EndScan;
                            this.BasePeakIntensityViewModel.StartScan = this.uimfData.StartScan;
                            this.BasePeakIntensityViewModel.EndScan = this.uimfData.EndScan;
                        });
                    }).Subscribe();

            this.BasePeakIntensityViewModel.WhenAnyValue(ticStart => ticStart.StartScan, ticEnd => ticEnd.EndScan)
                .Where(_ => this.UimfData != null)
                .Throttle(TimeSpan.FromMilliseconds(5), RxApp.MainThreadScheduler)
                .Subscribe(x => this.HeatMapViewModel.CurrentScanRange = new ScanRange(this.BasePeakIntensityViewModel.StartScan, this.BasePeakIntensityViewModel.EndScan));
        

            this.TotalIonChromatogramViewModel.WhenAnyValue(ticStart => ticStart.StartScan, ticEnd => ticEnd.EndScan)
                .Where(_ => this.UimfData != null)
                .Throttle(TimeSpan.FromMilliseconds(5), RxApp.MainThreadScheduler)
                .Subscribe(x => this.HeatMapViewModel.CurrentScanRange = new ScanRange(this.TotalIonChromatogramViewModel.StartScan, this.TotalIonChromatogramViewModel.EndScan));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the circular wait is visible.
        /// </summary>
        public bool CircularWaitIsVisible
        {
            get
            {
                return this.circularWaitIsVisible;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.circularWaitIsVisible, value);
            }
        }

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
                this.RaisePropertyChanged("WindowTitle");
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
        public MzSpectraViewModel MzSpectraViewModel { get; private set; }

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

        public BasePeakIntensityViewModel BasePeakIntensityViewModel { get; private set; }

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
        /// The export heatmap data compressed.
        /// </summary>
        /// <returns>
        /// The 2d array of <see cref="double"/>s that represents that data in the view.
        /// </returns>
        public double[,] ExportHeatmapDataCompressed()
        {
            //return this.HeatMapViewModel.GetCompressedDataInView();
            return this.HeatMapViewModel.ExportData();
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
        /// The export tic data compressed method.
        /// </summary>
        /// <returns>
        /// The dictionary of data, keyed by scan.
        /// </returns>
        public IDictionary<int, double> ExportTicDataCompressed()
        {
            return this.TotalIonChromatogramViewModel.GetTicData();
        }

        public IDictionary<int, double> ExportBpiDataCompressed()
        {
            return this.BasePeakIntensityViewModel.GetBpiData();
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
            if (this.UimfData != null)
            {
                this.UimfData.ReadData(
                    //1,
                    //this.UimfData.MaxBins,
                    this.UimfData.MinMz,
                    this.UimfData.MaxMz,
                    frameNumber,
                    frameNumber,
                    this.Height,
                    this.Width,
                    0,
                    this.UimfData.Scans,
                    ReturnGatedData);
            }
        }

        /// <summary>
        /// The get image method.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/> of the current three plots in view.
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

            this.UimfData = new UimfData(file);// { CurrentMinMz = 0 };
            //this.UimfData.CurrentMaxMz = this.UimfData.TotalMzRange;
            this.UimfData.CurrentMinMz = this.UimfData.MinMz;
            this.UimfData.CurrentMaxMz = this.UimfData.MaxMz;
            this.FetchSingleFrame(1);
            this.CurrentFile = Path.GetFileNameWithoutExtension(file);
            this.HeatMapViewModel.CurrentFile = Path.GetFileNameWithoutExtension(file);
            this.TotalIonChromatogramViewModel.MaxScan = this.UimfData.EndScan;
            this.BasePeakIntensityViewModel.MaxScan = this.UimfData.EndScan;
        }

        /// <summary>
        /// The sum frames.
        /// </summary>
        /// <param name="sumFrames">
        /// The frame range to sum.
        /// </param>
        public void SumFrames(FrameRange sumFrames)
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

            this.UimfData.ReadData(
                    this.UimfData.CurrentMinMz,
                    this.UimfData.CurrentMaxMz,
                    this.currentStartFrame,
                    this.currentEndFrame,
                    this.Height,
                    this.Width,
                    this.UimfData.StartScan,
                    this.UimfData.EndScan,
                    ReturnGatedData);
        }

        /// <summary>
        /// The update low gate.
        /// </summary>
        /// <param name="gate">
        /// The gate.
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
        /// The zoom out.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public void ZoomOut()
        {
            var scanRange = new ScanRange(0, this.UimfData.Scans);

            var mzRange = this.windowMz ? this.mzWindow : new MzRange(this.UimfData.MinMz, this.UimfData.MaxMz);

            this.UimfData.RangeUpdateList.Enqueue(scanRange);
            this.UimfData.RangeUpdateList.Enqueue(mzRange);
            this.UimfData.CheckQueue();
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
            this.UimfData.RangeUpdateList.Enqueue(this.mzWindow);
            this.UimfData.CheckQueue();
        }

        #endregion

        public Visibility BpiVisible
        {
            get { return this.BasePeakIntensityViewModel.BpiVisible; }
            set { this.BasePeakIntensityViewModel.BpiVisible = value; }
        }

        public bool BpiEnabled
        {
            get { return this._bpiEnabled; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._bpiEnabled, value);
                BpiVisible = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility TicVisible
        {
            get { return this.TotalIonChromatogramViewModel.TicVisible; }
            set { this.TotalIonChromatogramViewModel.TicVisible =  value; }
        }

        public bool TicEnabled
        {
            get { return this._ticEnabled; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._ticEnabled, value);
                TicVisible = value ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}