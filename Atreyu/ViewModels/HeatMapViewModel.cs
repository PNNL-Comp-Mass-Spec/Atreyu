// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeatMapViewModel.cs" company="Pacific Northwest National Laboratory">
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
//   The heat map view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

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
        /// The current bin range.
        /// </summary>
        private MzRange currentMzRange;

        /// <summary>
        /// The current file.
        /// </summary>
        private string currentFile = "Heatmap";

        /// <summary>
        /// The current max bin.
        /// </summary>
        private double currentMaxMz;

        /// <summary>
        /// The current max scan.
        /// </summary>
        private int currentMaxScan;

        /// <summary>
        /// The current min bin.
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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public HeatMapViewModel()
        {
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
        /// Gets or sets the current bin range.
        /// </summary>
        public MzRange CurrentMzRange
        {
            get
            {
                return this.currentMzRange;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMzRange, value);
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
            }
        }

        /// <summary>
        /// Gets or sets the current max bin.
        /// </summary>
        public double CurrentMaxMz
        {
            get
            {
                return this.currentMaxMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMaxMz, value);
            }
        }

        /// <summary>
        /// Gets or sets the current max scan.
        /// </summary>
        public int CurrentMaxScan
        {
            get
            {
                return this.currentMaxScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMaxScan, value);
            }
        }

        /// <summary>
        /// Gets or sets the current min bin.
        /// </summary>
        public double CurrentMinMz
        {
            get
            {
                return this.currentMinMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMinMz, value);
            }
        }

        /// <summary>
        /// Gets or sets the current min scan.
        /// </summary>
        public int CurrentMinScan
        {
            get
            {
                return this.currentMinScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMinScan, value);
            }
        }

        /// <summary>
        /// Gets or sets the current scan range.
        /// </summary>
        public ScanRange CurrentScanRange
        {
            get
            {
                return this.currentScanRange;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentScanRange, value);
            }
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
            get
            {
                return this.heatMapData;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.heatMapData, value);
            }
        }

        /// <summary>
        /// Gets or sets the heat map plot model.
        /// </summary>
        public PlotModel HeatMapPlotModel
        {
            get
            {
                return this.heatMapPlotModel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.heatMapPlotModel, value);
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.height, value);
            }
        }

        /// <summary>
        /// Gets or sets the high threshold.
        /// </summary>
        public double HighThreshold
        {
            get
            {
                return this.highThreshold;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.highThreshold, value);
            }
        }

        /// <summary>
        /// Gets or sets the threshold, the value at which intensities will not be added to the map (inclusive).
        /// </summary>
        public double LowThreshold
        {
            get
            {
                return this.lowThreshold;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.lowThreshold, value);
            }
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
            get
            {
                return this.width;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.width, value);
            }
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
                (int)this.HeatMapPlotModel.Width, 
                (int)this.HeatMapPlotModel.Height, 
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

            var linearColorAxis1 = new LinearColorAxis
                                       {
                                           HighColor = OxyColors.Purple, 
                                           LowColor = OxyColors.Black, 
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
                                       //AbsoluteMinimum = 0,
                                       //AbsoluteMaximum = this.HeatMapData.MaxBins,
                                       AbsoluteMinimum = this.HeatMapData.MinMz,
                                       AbsoluteMaximum = this.HeatMapData.MaxMz, 
                                       MinimumRange = 10, 
                                       MaximumPadding = 0,
                                       //Title = "TOF Bins",
                                       Title = "m/z", 
                                       TickStyle = TickStyle.Inside, 
                                       AxisDistance = -2, 
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
                                         //Y0 = 0,
                                         //Y1 = this.HeatMapData.MaxBins,
                                         Y0 = this.HeatMapData.MinMz,
                                         Y1 = this.HeatMapData.MaxMz,
                                         Interpolate = false,
                                         Background = OxyColors.Black,
                                     };

            this.HeatMapPlotModel.Series.Add(heatMapSeries1);
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
                //this.heatMapPlotModel.Axes[2].AbsoluteMaximum = this.MzWindow.EndBin;
                //this.heatMapPlotModel.Axes[2].AbsoluteMinimum = this.MzWindow.StartBin;
                this.heatMapPlotModel.Axes[2].AbsoluteMaximum = this.MzWindow.EndMz;
                this.heatMapPlotModel.Axes[2].AbsoluteMinimum = this.MzWindow.StartMz;
            }
            else
            {
                //this.heatMapPlotModel.Axes[2].AbsoluteMaximum = this.HeatMapData.MaxBins;
                //this.heatMapPlotModel.Axes[2].AbsoluteMinimum = 0;
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

            if ((this.CurrentMinScan == 0 && this.CurrentMinMz.Equals(0))
                || (this.ForceMinMaxMz && this.CurrentMinMz.Equals(this.MzWindow.StartMz)))
            {
                this.heatMapPlotModel.ResetAllAxes();
            }

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
            //this.CurrentMinMz = 1;
            //this.CurrentMaxMz = this.HeatMapData.MaxBins;
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
                this.CurrentScanRange = new ScanRange((int)axis.AbsoluteMinimum, (int)axis.AbsoluteMaximum);
            }
            else
            {
                this.CurrentScanRange = new ScanRange((int)axis.ActualMinimum, (int)axis.ActualMaximum);
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
                axis.Maximum = this.HeatMapData.MaxMz;
                axis.Minimum = this.HeatMapData.MinMz;
                this.CurrentMzRange = new MzRange(this.HeatMapData.MinMz, this.HeatMapData.MaxMz);
            }
            else
            {
                this.CurrentMzRange = new MzRange(axis.ActualMinimum, axis.ActualMaximum);
            }
        }

        #endregion

        internal double[,] ExportData()
        {
            return uncompressed;
        }

        public double[,] uncompressed { get; set; }
    }
}