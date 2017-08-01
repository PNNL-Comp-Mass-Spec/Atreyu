using System.Linq;
using Xceed.Wpf.DataGrid;

namespace Atreyu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Globalization;
    using System.IO;

    using Atreyu.Models;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Wpf;

    using ReactiveUI;

    using UIMFLibrary;

    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LineSeries = OxyPlot.Series.LineSeries;

    /// <summary>
    /// The view model for properly displaying the mz spectra graph.
    /// </summary>
    [Export]
    public class MzSpectraViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The end mz bin.
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private double endMz;

        /// <summary>
        /// The raw frame data that is compressed on the Y axis.
        /// </summary>
        private double[,] frameData;

        /// <summary>
        /// The calibration intercept.
        /// </summary>
        private double intercept;

        /// <summary>
        /// The plot model for the mz.
        /// </summary>
        private PlotModel mzPlotModel;

        /// <summary>
        /// Specifies whether or not to show the mz.
        /// </summary>
        private bool showMz;

        /// <summary>
        /// The calibration slope.
        /// </summary>
        private double slope;

        /// <summary>
        /// The start mz bin.
        /// </summary>
        private double startMz;

        /// <summary>
        /// The uimf data reference.
        /// </summary>
        private UimfData uimfData;
        private double maxMZ;
        private double minMZ;
        private Dictionary<double, double> tofFrameData;
        private bool _showLogData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MzSpectraViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public MzSpectraViewModel()
        {
            this.WhenAnyValue(vm => vm.ShowMz).Subscribe(b => this.UpdateFrameData(this.frameData));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the bin to mz map.
        /// </summary>
        public double[] BinToMzMap { get; set; }
        public double[] BinToTofMap { get; set; }
        private List<DataPoint> logArray = new List<DataPoint>();
  

        /// <summary>
        /// Gets or sets the mz calibrator.
        /// </summary>
        public MzCalibrator Calibrator { get; set; }

        /// <summary>
        /// Gets or sets the intercept.
        /// </summary>
        public double Intercept
        {
            get
            {
                return this.intercept;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.intercept, value);
            }
        }

        /// <summary>
        /// Gets or sets the mz array.
        /// </summary>
        public double[] MzArray { get; set; }

        /// <summary>
        /// Gets or sets the mz intensities.
        /// </summary>
        public int[] MzIntensities { get; set; }

        /// <summary>
        /// Gets or sets the mz plot model.
        /// </summary>
        public PlotModel MzPlotModel
        {
            get
            {
                return this.mzPlotModel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.mzPlotModel, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show mz.
        /// </summary>
        public bool ShowMz
        {
            get
            {
                return this.showMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.showMz, value);
            }
        }

        /// <summary>
        /// Gets or sets the slope.
        /// </summary>
        public double Slope
        {
            get
            {
                return this.slope;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.slope, value);
            }
        }

        /// <summary>
        /// Gets or sets the values per pixel y.
        /// </summary>
        public double ValuesPerPixelY { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Changes the end m/z for proper graph display.
        /// </summary>
        /// <param name="mz">
        /// The m/z to change to.
        /// </param>
        public void ChangeEndMz(double mz)
        {
            this.EndMZ = mz;
        }

        /// <summary>
        /// Changes the start m/z for proper graph display.
        /// </summary>
        /// <param name="mz">
        /// The m/z to change to.
        /// </param>
        public void ChangeStartMz(double mz)
        {
            this.StartMZ = mz;
        }

        /// <summary>
        /// Creates or recreates the create plot model.
        /// </summary>
        public void CreatePlotModel()
        {
            this.MzPlotModel = new PlotModel();
            var linearAxis = new LinearAxis
                                 {
                                     Position = AxisPosition.Right, 
                                     AbsoluteMinimum = 0, 
                                     Key = "XAxisKey", 
                                     IsPanEnabled = false, 
                                     IsZoomEnabled = false, 
                                     MinimumPadding = 0.0, 
                                     Title = this.ShowMz ? "m/z" : "Bin", 
                                     StringFormat = "f2"
                                 };
            this.MzPlotModel.Axes.Add(linearAxis);

            var linearYAxis = new LinearAxis
                                  {
                                      AbsoluteMinimum = 0, 
                                      IsZoomEnabled = false, 
                                      Position = AxisPosition.Top, 
                                      Key = "YAxisKey", 
                                      IsPanEnabled = false, 
                                      MinimumPadding = 0,
                                      StartPosition = 1,
                                      EndPosition = 0
                                  };
            linearYAxis.ToolTip = "Intensity";
            this.MzPlotModel.Axes.Add(linearYAxis);
            var series = new LineSeries
                             {
                                 Color = OxyColors.Black, 
                                 YAxisKey = linearAxis.Key, 
                                 XAxisKey = linearYAxis.Key, 
                                 StrokeThickness = 1
                             };
            this.MzPlotModel.Series.Add(series);
        }

        /// <summary>
        /// Gets a dictionary of the m/z data that has been compressed.
        /// </summary>
        /// <returns>
        /// The of the current mz data, keyed on mz.
        /// </returns>
        public IDictionary<double, double> GetMzDataCompressed()
        {
            //return this.mzFrameData;
            var returnDict = new Dictionary<double, double>();
            var delta = this.uimfData.UncompressedDeltaMz;
            //var mzKey = StartMZ;
            //for (int mz = 0; mz < this.uimfData.Uncompressed.GetLength(1); mz++)
            //{
            //    var summedMz = 0.0;
            //    for (int scan = 0; scan < this.uimfData.Uncompressed.GetLength(0); scan++)
            //    {
            //        summedMz += this.uimfData.Uncompressed[scan, mz];
            //    }
            //    returnDict.Add(mzKey, summedMz);

            //    mzKey += delta;
            //}
            return returnDict;
        }

        /// <summary>
        /// Get the current image of the m/z graph.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetMzImage()
        {
            var stream = new MemoryStream();
            PngExporter.Export(
                this.MzPlotModel, 
                stream, 
                (int)this.MzPlotModel.Width, 
                (int)this.MzPlotModel.Height, 
                OxyColors.White);

            Image image = new Bitmap(stream);
            return image;
        }

        private object syncRoot = new object();

        /// <summary>
        /// Update frame data.
        /// </summary>
        /// <param name="framedata">
        /// The frame data.
        /// </param>
        public void UpdateFrameData(double[,] framedata)
        {
            lock (syncRoot)
            {
                if (this.uimfData == null)
                {
                    return;
                }

                if (framedata == null)
                {
                    return;
                }

                if (this.BinToMzMap == null)
                {
                    return;
                }

                this.MaxMZ = this.uimfData.MaxMz;

                this.frameData = framedata;
                var frameDictionary = new Dictionary<double, double>();
                var mzFrameData = new Dictionary<double, double>();

                for (var j = 0; j < Math.Min(this.BinToMzMap.Length, this.frameData.GetLength(1)); j++)
                {
                    var mzIndex = this.BinToMzMap[j];

                    for (var i = 0; i < this.frameData.GetLength(0); i++)
                    {
                        if (mzFrameData.ContainsKey(mzIndex))
                        {
                            mzFrameData[mzIndex] += this.frameData[i, j];
                        }
                        else
                        {
                            mzFrameData.Add(mzIndex, this.frameData[i, j]);
                        }
                    }
                }

                var series = this.MzPlotModel.Series[0] as LineSeries;
                if (series != null)
                {
                    if (series.YAxis != null)
                    {
                        series.YAxis.Title = this.ShowMz ? "m/z" : "Tof";
                    }

                    series.Points.Clear();
                    this.logArray.Clear();
                    if (this.ShowMz)
                    {
                        var dataArray = new List<DataPoint>();
                        dataArray.AddRange(mzFrameData.Select(x => new DataPoint(x.Key, x.Value)));
                        UpdatePlotData(dataArray);
                    }


                }

            }
            

        }

        /// <summary>
        /// Updates the reference to UIMF data.
        /// </summary>
        /// <param name="uimfDataNew">
        /// The uimf data that has been changed.
        /// </param>
        public void UpdateReference(UimfData uimfDataNew)
        {
            if (uimfDataNew == null)
            {
                return;
            }

            this.uimfData = uimfDataNew;
            this.CreatePlotModel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find the peaks in the current data set and adds an annotation point with the resolution to the m/z.
        /// </summary>
        private void FindPeaks()
        {
            this.mzPlotModel.Annotations.Clear();

            if (!this.ShowMz)
            {
                // This currently does not support peakfinding in bin mode due to potential headaches,
                // But as it seems users never use that mode, it is a non-issue for now.
                return;
            }

            // Create a new dictionary to work with the peak finder
            var tempFrameList = new List<KeyValuePair<double, double>>(this.uimfData.MaxBins);

            for (var i = 0; i < this.MzArray.Length && i < this.MzIntensities.Length; i++)
            {
                tempFrameList.Add(new KeyValuePair<double, double>(this.MzArray[i], this.MzIntensities[i]));
            }

            var tempList = Utilities.PeakFinder.FindPeaks(tempFrameList, 3);

            // Put the points on the graph
            foreach (var resolutionDatapoint in tempList.Peaks)
            {
                var resolutionString = resolutionDatapoint.ResolvingPower.ToString("F1", CultureInfo.InvariantCulture);

                var peakPoint = new OxyPlot.Annotations.PointAnnotation
                                    {
                                        Text = "R=" + resolutionString, 
                                        X = resolutionDatapoint.Intensity,// / 1.03125, 
                                        Y = resolutionDatapoint.PeakCenter, 
                                        ToolTip = resolutionDatapoint.ToString()
                                    };
                this.mzPlotModel.Annotations.Add(peakPoint);
            }
        }

        #endregion

        public double MaxMZ
        {
            get { return this.maxMZ; } 
            set { this.RaiseAndSetIfChanged(ref this.maxMZ, value); }
        }

        public double StartMZ
        {
            get { return this.startMz; }
            set { this.RaiseAndSetIfChanged(ref this.startMz, value); }
        }

        public double EndMZ
        {
            get { return this.endMz; }
            set { this.RaiseAndSetIfChanged(ref this.endMz, value); }
        }

        public bool ShowLogData
        {
            get { return _showLogData; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._showLogData, value);
            }
        }

        private void UpdatePlotData(IEnumerable<DataPoint> points)
        {
            lock (this.MzPlotModel.SyncRoot)
            {
                var series = this.MzPlotModel.Series[0] as LineSeries;
                series.Points.Clear();

                series.Points.AddRange(points);
                this.MzPlotModel.InvalidatePlot(true);
            }
           
        }
    }
}