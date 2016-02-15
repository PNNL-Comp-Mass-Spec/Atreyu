using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Shell;

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
    [Export]
    public class HeatMapViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The current m/z range.
        /// </summary>
        private MzRange currentMzRange;

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
        private ScanRange currentScanRange;

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
        private List<SolidColorBrush> _heatmapColors;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public HeatMapViewModel()
        {
            HeatmapBackgroundColor = new SolidColorBrush(Colors.Black);;
            HeatmapPeakColor = new SolidColorBrush(Colors.Purple);
            //var ddlColor = new List<OxyColor>();
            var ddlColor = new List<SolidColorBrush>();
            Type colors = typeof (Color);
            PropertyInfo[] colorInfo = colors.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var info in colorInfo)
            {
                var temp = Color.FromName(info.Name);
                var color = System.Windows.Media.Color.FromArgb(temp.A, temp.R, temp.G, temp.B);
                //ddlColor.Add(OxyColor.FromRgb(color.R, color.G, color.B));
                ddlColor.Add(new SolidColorBrush(color));
            }
            AvailableColors = ddlColor;
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
        public MzRange CurrentMzRange
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
        public ScanRange CurrentScanRange
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

            set { this.RaiseAndSetIfChanged(ref this.heatMapPlotModel, value); }
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
        public MzRange MzWindow { get; set; }

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
        public void SetUpPlot()
        {
            this.HeatMapPlotModel = new PlotModel();

            var dis = System.Windows.Application.Current.Dispatcher;

            dis.Invoke(() =>
            {
                var subDis = System.Windows.Application.Current.Dispatcher;
                subDis.Invoke(() =>
                {
                    this.HeatMapPlotModel.Axes.Clear();
                    var linearColorAxis1 = new LinearColorAxis
                    {
                        HighColor =
                            OxyColor.FromRgb(HeatmapPeakColor.Color.R, HeatmapPeakColor.Color.G,
                                HeatmapPeakColor.Color.B),
                        LowColor =
                            OxyColor.FromRgb(HeatmapBackgroundColor.Color.R, HeatmapBackgroundColor.Color.G,
                                HeatmapBackgroundColor.Color.B),
                        Position = AxisPosition.Right,
                        Minimum = 1,
                        Title = "Intensity",
                        IsAxisVisible = this.AxisVisible
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
                        Title = "m/z",
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
                        Interpolate = false,
                        Background =
                            OxyColor.FromRgb(HeatmapBackgroundColor.Color.R, HeatmapBackgroundColor.Color.G,
                                HeatmapBackgroundColor.Color.B),
                    };

                    this.HeatMapPlotModel.Series.Add(heatMapSeries1);
                    //this.HeatMapPlotModel.InvalidatePlot(true);
                });
            });
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

            var series = this.HeatMapPlotModel.Series[0] as HeatMapSeries;
            if (series == null)
            {
                return;
            }

            if (this.ForceMinMaxMz)
            {
                this.heatMapPlotModel.Axes[2].AbsoluteMaximum = this.MzWindow.EndMz;
                this.heatMapPlotModel.Axes[2].AbsoluteMinimum = this.MzWindow.StartMz;
            }
            else
            {
                this.heatMapPlotModel.Axes[2].AbsoluteMaximum = this.HeatMapData.MaxMz;
                this.heatMapPlotModel.Axes[2].AbsoluteMinimum = this.HeatMapData.MinMz;
            }

            this.dataArray = framedata;

            series.Data = this.dataArray;

            // scans
            series.X0 = this.CurrentMinScan;
            series.X1 = this.CurrentMaxScan;

            // bins
            series.Y0 = this.CurrentMinMz;
            series.Y1 = this.CurrentMaxMz;

            //if ((this.CurrentMinScan == 0 && this.CurrentMinMz.Equals(0))
            //    || (this.ForceMinMaxMz && this.CurrentMinMz.Equals(this.MzWindow.StartMz)))
            //{
            this.heatMapPlotModel.ResetAllAxes();
            //}

            this.HeatMapPlotModel.InvalidatePlot(true);
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
                this.CurrentScanRange = new ScanRange(this.currentMinScan, this.currentMaxScan);
            }
            else
            {
                this.CurrentScanRange = new ScanRange((int) axis.ActualMinimum, (int) axis.ActualMaximum);
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
                this.CurrentMzRange = new MzRange(this.currentMinMz, this.currentMaxMz);
            }
            else
            {
                this.CurrentMzRange = new MzRange(axis.ActualMinimum, axis.ActualMaximum);
                this.currentMinMz = axis.ActualMinimum;
                this.currentMaxMz = axis.ActualMaximum;
            }
        }

        #endregion

        internal double[,] ExportData()
        {
            return uncompressed;
        }

        public double[,] uncompressed { get; set; }

        public SolidColorBrush HeatmapBackgroundColor
        {
            get { return this._heatmapBackgroundColor; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._heatmapBackgroundColor, value);
                var dis = System.Windows.Application.Current.Dispatcher;
                if (HeatmapPeakColor != null && HeatMapData != null)
                {
                    dis.Invoke(() =>
                    {
                        SetUpPlot();
                        UpdateData(this.FrameData);
                    });
                }
            }
        }

        public SolidColorBrush HeatmapPeakColor
        {
            get { return this._heatmapPeakColor; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._heatmapPeakColor, value);
                var dis = System.Windows.Application.Current.Dispatcher;
                if (HeatmapBackgroundColor != null && HeatMapData != null)
                {
                    dis.Invoke(() =>
                    {
                        SetUpPlot();
                        UpdateData(this.FrameData);
                    });
                }
            }
        }

        public List<SolidColorBrush> AvailableColors
        {
            get { return this._heatmapColors; }
            set { this.RaiseAndSetIfChanged(ref this._heatmapColors, value); }
        }
    }
}