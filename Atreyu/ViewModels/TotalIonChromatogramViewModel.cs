using System;
using System.Reactive.Linq;
using System.Windows;

namespace Atreyu.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Atreyu.Models;

    using OxyPlot;
    using OxyPlot.Axes;

    using ReactiveUI;

    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LineSeries = OxyPlot.Series.LineSeries;

    // using Falkor.Events.Atreyu;

    /// <summary>
    /// The total ion chromatogram view model.
    /// </summary>
    public class TotalIonChromatogramViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The end scan.
        /// </summary>
        private int endScan;

        /// <summary>
        /// The start scan.
        /// </summary>
        private int startScan;

        /// <summary>
        /// The tic plot model.
        /// </summary>
        private PlotModel ticPlotModel;

        /// <summary>
        /// The uimf data.
        /// </summary>
        private UimfData uimfData;

        private int maxScan;
        private bool _showLogData;
        private double _maxValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalIonChromatogramViewModel"/> class.
        /// </summary>
        public TotalIonChromatogramViewModel(UimfData uimfData) : this()
        {
            this.uimfData = uimfData;
            this.UpdateReference(this.uimfData);

        }

        public TotalIonChromatogramViewModel()
        {
            this.WhenAnyValue(x => x.ShowScanTime).Where(x => this.uimfData != null).Subscribe(b =>
            {
               
            });
        }

        #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the tic plot model.
            /// </summary>
        public PlotModel TicPlotModel
        {
            get
            {
                return this.ticPlotModel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.ticPlotModel, value);
            }
        }

        public int StartScan
        {
            get { return startScan;}
            set { this.RaiseAndSetIfChanged(ref this.startScan, value); }
        }

        public int EndScan
        {
            get { return endScan; }
            set { this.RaiseAndSetIfChanged(ref this.endScan, value); }
        }

        public int MaxScan
        {
            get { return maxScan; }
            set { this.RaiseAndSetIfChanged(ref this.maxScan, value); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The change end scan.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void ChangeEndScan(int value)
        {
            if (value > this.MaxScan)
                value = this.MaxScan;
            this.EndScan = value;
        }

        public void ChangeMaxScan(int value)
        {
            this.MaxScan = value;
            ChangeEndScan(value);
        }

        /// <summary>
        /// The change start scan.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void ChangeStartScan(int value)
        {
            if (value < 0)
                value = 0;
            this.StartScan = value;
        }

        /// <summary>
        /// The update frame data.
        /// </summary>
        /// <param name="data">
        /// The Data.
        /// </param>
        public void UpdateFrameData(double[,] data)
        {
            if (data == null)
            {
                return;
            }

           var timeFactor = uimfData.TenthsOfNanoSecondsPerBin / 1000000.0;
            if (this.EndScan == 0)
            {
                this.StartScan = 0;
                this.EndScan = data.GetLength(0);
            }

            var frameDictionary = new Dictionary<int, double>();

            for (var i = 0; i < this.EndScan - this.StartScan; i++)
            {
                var index = i + this.StartScan;
                for (var j = 0; j < data.GetLength(1); j++)
                {
                    if (frameDictionary.ContainsKey(index))
                    {
                       frameDictionary[index] += data[i, j];
                    }
                    else
                    {
                        frameDictionary.Add(index, data[i, j]);
                    }
                }
            }
            var dataArray = frameDictionary.Select(x => new DataPoint(x.Key * timeFactor, x.Value));
            var logArray =  frameDictionary.Select(x => new DataPoint(x.Key, x.Value));
            UpdatePlotData(timeFactor, logArray);

            this.TicPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// The update reference.
        /// </summary>
        /// <param name="uimfDataNew">
        /// The new <see cref="UimfData"/> that is coming in.
        /// </param>
        public void UpdateReference(UimfData uimfDataNew)
        {
            this.uimfData = uimfDataNew;
           
            if (this.TicPlotModel != null)
            {
                return;
            }

            this.TicPlotModel = new PlotModel() { PlotType = PlotType.XY};
            var linearAxis = new LinearAxis
                                 {
                                     Position = AxisPosition.Bottom, 
                                     AbsoluteMinimum = 0, 
                                     Title = "Mobility Scan",
                                     Unit = "Scan Number",
                                     MinorTickSize = 0
                                 };
            this.TicPlotModel.Axes.Add(linearAxis);

            var linearYAxis = new LinearAxis
                                  {
                                      IsZoomEnabled = false, 
                                      AbsoluteMinimum = 0, 
                                      MinimumPadding = 0.1, 
                                      MaximumPadding = 0.1, 
                                      IsPanEnabled = false, 
                                      IsAxisVisible = false
                                      //Title = "Intensity"
                                  };
            var series = new LineSeries() { Title = "TIC", Color = OxyColors.Black, CanTrackerInterpolatePoints = false};
            this.TicPlotModel.Series.Add(series);
            this.TicPlotModel.Axes.Add(linearYAxis);
            this.MaxScan = uimfDataNew.Ranges.EndScan;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find the peaks in the current data set and adds an annotation point with the resolution to the TIC.
        /// </summary>
        private void FindPeaks()
        {
            //this.ticPlotModel.Annotations.Clear();

            //// Create a new dictionary so we don't modify the original one
            //var tempFrameDict = new Dictionary<double, double>(this.uimfData.Scans);

            //for (var i = 0; i < this.uimfData.Scans; i++)
            //{
            //    // this is a hack to make the library work and return the proper location index
            //    double junk;
            //    tempFrameDict.Add(i, this.frameDictionary.TryGetValue(i, out junk) ? junk : 0);
            //}

            //var results = Utilities.PeakFinder.FindPeaks(tempFrameDict.ToList());

            //foreach (var peakInformation in results.Peaks)
            //{
            //    var resolutionString = peakInformation.ResolvingPower.ToString("F1", CultureInfo.InvariantCulture);

            //    var peakPoint = new OxyPlot.Annotations.PointAnnotation
            //                        {
            //                            Text = "R=" + resolutionString, 
            //                            X = peakInformation.PeakCenter, 
            //                            Y = peakInformation.Intensity / 2.5, 
            //                            ToolTip = peakInformation.ToString()
            //                        };
            //    this.ticPlotModel.Annotations.Add(peakPoint);
            //}
        }

        #endregion

        public bool ShowScanTime
        {
            get { return _showLogData; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._showLogData, value);
            }
        }

        private void UpdatePlotData(double timeFactor, IEnumerable<DataPoint> points)
        {
            lock (TicPlotModel.SyncRoot)
            {
                var series = this.TicPlotModel.Series[0] as LineSeries;
                var axis = this.TicPlotModel.Axes[0] as LinearAxis;
                series.Points.Clear();
                series.Points.AddRange(points);

                if (this.ShowScanTime)
                {
                    axis.Title = "Arrival Time";
                    axis.Unit = "ms";
                    axis.AbsoluteMaximum = EndScan * timeFactor;
                    axis.AbsoluteMinimum = StartScan * timeFactor;
                }
                else
                {
                    axis.Title = "Mobility Scan";
                    axis.Unit = "Scan Number";
                    axis.AbsoluteMaximum = EndScan;
                    axis.AbsoluteMinimum = StartScan;
                    
                }
                TicPlotModel.ResetAllAxes();
                
            }
           
        }

        public double MaxValue
        {
            get => _maxValue;
            set => this.RaiseAndSetIfChanged(ref _maxValue, value);
        }
    }
}