using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Atreyu.ViewModels
{
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;

    using Atreyu.Models;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Wpf;

    using ReactiveUI;

    using HeatMapSeries = OxyPlot.Series.HeatMapSeries;
    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LinearColorAxis = OxyPlot.Axes.LinearColorAxis;

    /// <summary>
    /// The heat map view model.
    /// </summary>
    public class HeatMapViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The current m/z range.
        /// </summary>
        private Range<double> currentMzRange;

        /// <summary>
        /// The current file.
        /// </summary>
        private string currentFile = "Heatmap";

        /// <summary>
        /// The current max m/z.
        /// </summary>
        private double currentMaxMz;

        /// <summary>
        /// The current max scan.
        /// </summary>
        private int currentMaxScan;

        /// <summary>
        /// The current min m/z.
        /// </summary>
        private double currentMinMz;

        /// <summary>
        /// The current min scan.
        /// </summary>
        private int currentMinScan;

        /// <summary>
        /// The current scan range.
        /// </summary>
        private Range<int> currentScanRange;

        /// <summary>
        /// The data array.
        /// </summary>
        private double[,] dataArray;

        /// <summary>
        /// The heat map data.
        /// </summary>
        private UimfData heatMapData;

        /// <summary>
        /// The heat map plot model.
        /// </summary>
        private PlotModel heatMapPlotModel;

        /// <summary>
        /// The height.
        /// </summary>
        private int height;

        /// <summary>
        /// The high threshold.
        /// </summary>
        private double highThreshold;

        /// <summary>
        /// The backing field for <see cref="LowThreshold"/>.
        /// </summary>
        private double lowThreshold;

        /// <summary>
        /// The width.
        /// </summary>
        private int width;

        private SolidColorBrush _heatmapBackgroundColor;
        private SolidColorBrush _heatmapPeakColor;
        private string _tofDisplay;
        private string _mzDisplay;
        private List<OxyPaletteMap> _heatmapPalettes;
        private OxyPaletteMap _selectedPalette;
        private bool _showLogData;
private  bool _heatmapWhite;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapViewModel"/> class.
        /// </summary>
        public HeatMapViewModel()
        {
            this.HeatMapPlotModel = new PlotModel() { Title = "IMS Data"};
            var ddlColor = new List<OxyPaletteMap>();
            Type colors = typeof (OxyPalettes);
            MethodInfo[] colorInfo = colors.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var info in colorInfo)
            {
                if (!info.Name.StartsWith("get_"))
                {
                    var name = info.Name;
                    var mappedPallet = OxyPaletteMap.CreateFromName(name);
                    if (mappedPallet != null)
                    {
                        ddlColor.Add(mappedPallet);
                        if (name == "Jet")
                        {
                            SelectedPalette = mappedPallet;
                        }
                    }

                }
            }
            AvailableColors = ddlColor;

            this.WhenAnyValue(x => x.SelectedPalette).Where(x => this.HeatMapData != null).Subscribe(map =>
            {
                lock (this.HeatMapPlotModel.SyncRoot)
                {
                    var axis = HeatMapPlotModel.Axes[0] as LinearColorAxis;
                    axis.Palette = SelectedPalette.Palette;
                    UpdateData(this.FrameData);
                    HeatMapPlotModel.ResetAllAxes();
                }
            });

            this.WhenAnyValue(x => x.ShowLogData).Where(x => this.HeatMapData != null && this.HeatMapPlotModel != null).Subscribe(b =>
            {
                lock (this.HeatMapPlotModel.SyncRoot)
                {
                    
                }
            });

            this.WhenAnyValue(hm => hm.CurrentMzRange, model => model.CurrentScanRange)
                .Where(_ => this.HeatMapData != null && this.CurrentMzRange != null && this.CurrentScanRange != null)
                .Subscribe(
                    tuple =>
                    {
                        this.HeatMapData.Ranges =
                            (tuple.Item1.Start, tuple.Item1.End, tuple.Item2.Start, tuple.Item2.End);
                    });

            this.WhenAnyValue(x => x.MakeHeatmapWhite).Where(x => this.HeatMapData != null).Subscribe(b =>
            {
                lock (this.HeatMapPlotModel.SyncRoot)
                {
                    var colorAxis = this.heatMapPlotModel.Axes[0] as LinearColorAxis;
                    colorAxis.LowColor = MakeHeatmapWhite ? OxyColors.White : OxyColors.Black;
                    this.HeatMapPlotModel.InvalidatePlot(true);
                }
                
            });

            //this.WhenAnyValue(x => x.HeatMapData.Ranges).Where(x => this.HeatMapData != null).Subscribe(tuple =>
            //{
            //    this.CurrentMzRange = new Range<double>(tuple.CurrentMinMz, tuple.CurrentMaxMz);
            //});
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether axis visible.
        /// </summary>
        public bool AxisVisible { get; set; }

        /// <summary>
        /// Gets or sets the bin to mz map.
        /// </summary>
        public double[] BinToMzMap { get; set; }

        public double[,] FrameData { get; set; }

        /// <summary>
        /// Gets or sets the current m/z range.
        /// </summary>
        public Range<double> CurrentMzRange
        {
            get { return this.currentMzRange; }

            set { this.RaiseAndSetIfChanged(ref this.currentMzRange, value); }
        }

        /// <summary>
        /// Gets or sets the current file.
        /// </summary>
        public string CurrentFile
        {
            get { return this.currentFile; }

            set { this.RaiseAndSetIfChanged(ref this.currentFile, value); }
        }

        /// <summary>
        /// Gets or sets the current max bin.
        /// </summary>
        public double CurrentMaxMz
        {
            get { return this.currentMaxMz; }

            set { this.RaiseAndSetIfChanged(ref this.currentMaxMz, value); }
        }

        /// <summary>
        /// Gets or sets the current max scan.
        /// </summary>
        public int CurrentMaxScan
        {
            get { return this.currentMaxScan; }

            set { this.RaiseAndSetIfChanged(ref this.currentMaxScan, value); }
        }

        /// <summary>
        /// Gets or sets the current min bin.
        /// </summary>
        public double CurrentMinMz
        {
            get { return this.currentMinMz; }

            set { this.RaiseAndSetIfChanged(ref this.currentMinMz, value); }
        }

        /// <summary>
        /// Gets or sets the current min scan.
        /// </summary>
        public int CurrentMinScan
        {
            get { return this.currentMinScan; }

            set { this.RaiseAndSetIfChanged(ref this.currentMinScan, value); }
        }

        /// <summary>
        /// Gets or sets the current scan range.
        /// </summary>
        public Range<int> CurrentScanRange
        {
            get { return this.currentScanRange; }

            set { this.RaiseAndSetIfChanged(ref this.currentScanRange, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether force min max mz.
        /// </summary>
        public bool ForceMinMaxMz { get; set; }

        /// <summary>
        /// Gets The heat map data (<seealso cref="UimfData"/>).
        /// </summary>
        public UimfData HeatMapData
        {
            get { return this.heatMapData; }

            private set { this.RaiseAndSetIfChanged(ref this.heatMapData, value); }
        }

        /// <summary>
        /// Gets or sets the heat map plot model.
        /// </summary>
        public PlotModel HeatMapPlotModel
        {
            get { return this.heatMapPlotModel; }

            private set { this.RaiseAndSetIfChanged(ref this.heatMapPlotModel, value); }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height
        {
            get { return this.height; }

            set { this.RaiseAndSetIfChanged(ref this.height, value); }
        }

        /// <summary>
        /// Gets or sets the high threshold.
        /// </summary>
        public double HighThreshold
        {
            get { return this.highThreshold; }

            set { this.RaiseAndSetIfChanged(ref this.highThreshold, value); }
        }

        /// <summary>
        /// Gets or sets the threshold, the value at which intensities will not be added to the map (inclusive).
        /// </summary>
        public double LowThreshold
        {
            get { return this.lowThreshold; }

            set { this.RaiseAndSetIfChanged(ref this.lowThreshold, value); }
        }

        /// <summary>
        /// Gets or sets the mz window which will be enforced if <see cref="ForceMinMaxMz"/> is true.
        /// </summary>
        public Range<double> MzWindow { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width
        {
            get { return this.width; }

            set { this.RaiseAndSetIfChanged(ref this.width, value); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get compressed data in view.
        /// </summary>
        /// <returns>
        /// The 2d array of doubles that holds the data in view.
        /// </returns>
        public double[,] GetCompressedDataInView()
        {
            var minScan = this.CurrentMinScan;

            var exportData = new double[this.dataArray.GetLength(0) + 1, this.dataArray.GetLength(1) + 1];

            // populate the scan numbers along one axis (the vertical)
            for (var x = 1; x < exportData.GetLength(0); x++)
            {
                var scan = x - 1 + minScan;
                exportData[x, 0] = scan;
            }

            // populate the m/zs on the other axis
            for (var y = 1; y < exportData.GetLength(1); y++)
            {
                var bin = y - 1;
                var mz = this.BinToMzMap[bin];
                exportData[0, y] = mz;
            }

            // fill the rest of the array with the intensity values (0,0 of the array never assigned, but defaults to "0.0")
            for (var mz = 1; mz < exportData.GetLength(1); mz++)
            {
                for (var scan = 1; scan < exportData.GetLength(0); scan++)
                {
                    exportData[scan, mz] = this.dataArray[scan - 1, mz - 1];
                }
            }

            return exportData;
        }

        /// <summary>
        /// The save heat map image.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetHeatmapImage()
        {
            var stream = new MemoryStream();
            PngExporter.Export(
                this.HeatMapPlotModel,
                stream,
                (int) this.HeatMapPlotModel.Width,
                (int) this.HeatMapPlotModel.Height,
                OxyColors.White);

            Image image = new Bitmap(stream);
            return image;
        }

        /// <summary>
        /// The set up plot.
        /// </summary>
        private void SetUpPlot()
        {
            lock (this.HeatMapPlotModel.SyncRoot)
            {
                this.HeatMapPlotModel.Axes.Clear();

                var linearColorAxis1 = new LinearColorAxis
                {
                    Position = AxisPosition.Right,
                    Minimum = 1,
                    Title = "abundance",
                    IsAxisVisible = this.AxisVisible,
                    Palette = SelectedPalette.Palette,
                    LowColor = OxyColors.Black
                };

                this.HeatMapPlotModel.Axes.Add(linearColorAxis1);

                var horizontalAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    AbsoluteMinimum = 0,
                    AbsoluteMaximum = this.HeatMapData.Scans,
                    MinimumRange = 10,
                    MaximumPadding = 0,
                    Title = "Mobility Scans",
                    IsAxisVisible = this.AxisVisible
                };

                horizontalAxis.AxisChanged += this.PublishXAxisChange;

                this.HeatMapPlotModel.Axes.Add(horizontalAxis);

                var verticalAxis = new LinearAxis
                {
                    AbsoluteMinimum = this.HeatMapData.MinMz,
                    AbsoluteMaximum = this.HeatMapData.MaxMz,
                    MaximumPadding = 0,
                    Title = "M/Z",
                    TickStyle = TickStyle.Inside,
                    AxisDistance = -2,
                    IsZoomEnabled = true,
                    TextColor = OxyColors.Red,
                    TicklineColor = OxyColors.Red,
                    Layer = AxisLayer.AboveSeries,
                    IsAxisVisible = this.AxisVisible
                };

                verticalAxis.AxisChanged += this.PublishYAxisChange;

                this.HeatMapPlotModel.Axes.Add(verticalAxis);

                var heatMapSeries1 = new HeatMapSeries
                {
                    X0 = 0,
                    X1 = this.HeatMapData.Scans,
                    Y0 = this.HeatMapData.MinMz,
                    Y1 = this.HeatMapData.MaxMz,
                    Interpolate = false
                };

                this.HeatMapPlotModel.Series.Add(heatMapSeries1);
            }
        }

        public string TofDisplay
        {
            get { return _tofDisplay; }
            set { this.RaiseAndSetIfChanged(ref this._tofDisplay, value); }
        }
        public string MzDisplay
        {
            get { return _mzDisplay; }
            set { this.RaiseAndSetIfChanged(ref this._mzDisplay, value); }
        }

        /// <summary>
        /// The update data.
        /// </summary>
        /// <param name="framedata">
        /// The frame data.
        /// </param>
        public void UpdateData(double[,] framedata)
        {
            if (framedata == null)
            {
                return;
            }
            lock (this.HeatMapPlotModel.SyncRoot)
            {
                var series = this.HeatMapPlotModel.Series[0] as HeatMapSeries;
                if (series == null)
                {
                    return;
                }

                if (this.ForceMinMaxMz)
                {
                    this.heatMapPlotModel.Axes[2].AbsoluteMaximum = this.MzWindow.End;
                    this.heatMapPlotModel.Axes[2].AbsoluteMinimum = this.MzWindow.Start;
                }
                else
                {
                    this.heatMapPlotModel.Axes[2].AbsoluteMaximum = this.HeatMapData.MaxMz;
                    this.heatMapPlotModel.Axes[2].AbsoluteMinimum = this.HeatMapData.MinMz;
                }

                this.dataArray = new double[framedata.GetLength(0), framedata.GetLength(1)];
                Array.Copy(framedata, dataArray, framedata.GetLength(0) * framedata.GetLength(1));

                series.Data = dataArray;

                // scans
                series.X0 = this.CurrentMinScan;
                series.X1 = this.CurrentMaxScan;

                // bins
                series.Y0 = this.CurrentMinMz;
                series.Y1 = this.CurrentMaxMz;

                this.HeatMapPlotModel.ResetAllAxes();

                this.HeatMapPlotModel.InvalidatePlot(true);
            }
            
        }

        /// <summary>
        /// The update reference.
        /// </summary>
        /// <param name="uimfData">
        /// The uimf data.
        /// </param>
        public void UpdateReference(UimfData uimfData)
        {
            if (uimfData == null)
            {
                return;
            }

            this.HeatMapData = uimfData;
            this.CurrentMinMz = this.HeatMapData.MinMz;
            this.CurrentMaxMz = this.HeatMapData.MaxMz;
            this.CurrentMinScan = 0;
            this.CurrentMaxScan = this.HeatMapData.Scans;

            this.SetUpPlot();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The publish x axis change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PublishXAxisChange(object sender, AxisChangedEventArgs e)
        {
            var axis = sender as LinearAxis;
            if (axis == null)
            {
                return;
            }

            if (e.ChangeType == AxisChangeTypes.Reset)
            {
                this.CurrentScanRange = new Range<int>(this.currentMinScan, this.currentMaxScan);
            }
            else
            {
                this.CurrentScanRange = new Range<int>((int) axis.ActualMinimum, (int) axis.ActualMaximum);
                this.currentMaxScan = (int) axis.ActualMaximum;
                this.currentMinScan = (int) axis.ActualMinimum;
            }
        }

        /// <summary>
        /// The publish y axis change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PublishYAxisChange(object sender, AxisChangedEventArgs e)
        {
            var axis = sender as LinearAxis;
            if (axis == null)
            {
                return;
            }

            if (e.ChangeType == AxisChangeTypes.Reset)
            {
                //axis.Maximum = this.HeatMapData.MaxMz;
                //axis.Minimum = this.HeatMapData.MinMz;
                //this.CurrentMzRange = new MzRange(this.HeatMapData.MinMz, this.HeatMapData.MaxMz);
                this.CurrentMzRange = new Range<double>(this.currentMinMz, this.currentMaxMz);
            }
            else
            {
                this.CurrentMzRange = new Range<double>(axis.ActualMinimum, axis.ActualMaximum);
                this.currentMinMz = axis.ActualMinimum;
                this.currentMaxMz = axis.ActualMaximum;
            }
        }


        #endregion

        public List<OxyPaletteMap> AvailableColors
        {
            get { return this._heatmapPalettes; }
            set { this.RaiseAndSetIfChanged(ref this._heatmapPalettes, value); }
        }

        public OxyPaletteMap SelectedPalette
        {
            get { return _selectedPalette; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPalette, value);
            }
        }

        public bool ShowLogData
        {
            get { return _showLogData; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._showLogData, value);
               
            }
        }

        public bool MakeHeatmapWhite
        {
            get { return _heatmapWhite; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._heatmapWhite, value);
            }
        }
    }
}