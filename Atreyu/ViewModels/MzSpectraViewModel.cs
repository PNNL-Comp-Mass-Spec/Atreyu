// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MzSpectraViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The mz spectra view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Atreyu.Models;

    using MagnitudeConcavityPeakFinder;

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
        private int endMzBin;

        /// <summary>
        /// The raw frame data that is compressed on the Y axis.
        /// </summary>
        private double[,] frameData;

        /// <summary>
        /// The bin centric frame data that has been compressed.
        /// </summary>
        private Dictionary<double, double> frameDictionary;

        /// <summary>
        /// The calibration intercept.
        /// </summary>
        private double intercept;

        /// <summary>
        /// The mz frame data that has been compressed.
        /// </summary>
        private Dictionary<double, double> mzFrameData;

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
        private int startMzBin;

        /// <summary>
        /// The uimf data reference.
        /// </summary>
        private UimfData uimfData;

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
        /// Changes the end bin for proper graph display.
        /// </summary>
        /// <param name="bin">
        /// The bin to change to.
        /// </param>
        public void ChangeEndBin(int bin)
        {
            this.endMzBin = bin;
        }

        /// <summary>
        /// Changes the start bin for proper graph display.
        /// </summary>
        /// <param name="bin">
        /// The bin to change to.
        /// </param>
        public void ChangeStartBin(int bin)
        {
            this.startMzBin = bin;
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
                                      EndPosition = 0, 
                                  };
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
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<double, double> GetMzDataCompressed()
        {
            return this.mzFrameData;
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

        /// <summary>
        /// Update frame data.
        /// </summary>
        /// <param name="framedata">
        /// The frame data.
        /// </param>
        public void UpdateFrameData(double[,] framedata)
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

            this.frameData = framedata;
            this.frameDictionary = new Dictionary<double, double>();
            this.mzFrameData = new Dictionary<double, double>();

            for (var j = 0; j < this.frameData.GetLength(1); j++)
            {
                double index = j + this.startMzBin;
                var mzIndex = this.BinToMzMap[j];

                for (var i = 0; i < this.frameData.GetLength(0); i++)
                {
                    if (this.frameDictionary.ContainsKey(index))
                    {
                        this.frameDictionary[index] += this.frameData[i, j];
                    }
                    else
                    {
                        this.frameDictionary.Add(index, this.frameData[i, j]);
                    }

                    if (this.mzFrameData.ContainsKey(mzIndex))
                    {
                        this.mzFrameData[mzIndex] += this.frameData[i, j];
                    }
                    else
                    {
                        this.mzFrameData.Add(mzIndex, this.frameData[i, j]);
                    }
                }
            }

            var series = this.MzPlotModel.Series[0] as LineSeries;

            if (series != null)
            {
                if (series.YAxis != null)
                {
                    series.YAxis.Title = this.ShowMz ? "m/z" : "Bin";
                }

                series.Points.Clear();
                if (this.ShowMz)
                {
                    foreach (var d in this.mzFrameData)
                    {
                        series.Points.Add(new DataPoint(d.Value, d.Key));
                    }
                }
                else
                {
                    foreach (var d in this.frameDictionary)
                    {
                        series.Points.Add(new DataPoint(d.Value, d.Key));
                    }
                }
            }

            this.FindPeaks();

            this.MzPlotModel.InvalidatePlot(true);
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
            var datapointList = new List<ResolutionDatapoint>();
            const int Precision = 10000;
            this.mzPlotModel.Annotations.Clear();

            if (!this.ShowMz)
            {
                // This currently does not support peakfinding in bin mode due to potential headaches,
                // But as it seems users never use that mode, it is a non-issue for now.
                return;
            }

            var peakDetector = new PeakDetector();

            var finderOptions = PeakDetector.GetDefaultSICPeakFinderOptions();

            List<double> smoothedY;

            // Create a new dictionary so we don't modify the original one
            var tempFrameList = new List<KeyValuePair<int, double>>(this.uimfData.MaxBins);

            // We have to give it integers, but we need the mz, so we will multiply the mz by the precision
            // and later get the correct value back by dividing it out again
            for (var i = 0; i < this.MzArray.Length && i < this.MzIntensities.Length; i++)
            {
                tempFrameList.Add(
                    new KeyValuePair<int, double>((int)(this.MzArray[i] * Precision), this.MzIntensities[i]));
            }

            // I am not sure what this does and need to talk to Matt Monroe, but in the example exe file that came with the library
            // they used half of the length of the list in their previous examples and this seems to work on teh zoomed out version
            // but not when we zoom in, it seems like an offset problem.
            var originalpeakLocation = tempFrameList.Count / 2;

            var allPeaks = peakDetector.FindPeaks(
                finderOptions, 
                tempFrameList.OrderBy(x => x.Key).ToList(), 
                originalpeakLocation, 
                out smoothedY);

            var topThreePeaks = allPeaks.OrderByDescending(peak => smoothedY[peak.LocationIndex]).Take(3);

            foreach (var peak in topThreePeaks)
            {
                const double Tolerance = 0.01;
                var centerPoint = tempFrameList.ElementAt(peak.LocationIndex);
                var offsetMz = centerPoint.Key; // + firstpoint.Key; 
                var intensity = centerPoint.Value;
                var smoothedPeakIntensity = smoothedY[peak.LocationIndex];

                var realMz = (double)offsetMz / Precision;
                var halfmax = smoothedPeakIntensity / 2.0;

                var currPoint = new KeyValuePair<int, double>(0, 0);
                var currPointIndex = 0;
                double leftMidpoint = 0;
                double rightMidPoint = 0;

                var leftSidePeaks = new List<KeyValuePair<int, double>>();
                for (var l = peak.LeftEdge; l < peak.LocationIndex && l < tempFrameList.Count; l++)
                {
                    leftSidePeaks.Add(tempFrameList[l]);
                }

                var rightSidePeaks = new List<KeyValuePair<int, double>>();
                for (var r = peak.LocationIndex; r < peak.RightEdge && r < tempFrameList.Count; r++)
                {
                    rightSidePeaks.Add(tempFrameList[r]);
                }

                // find the left side half max
                foreach (var leftSidePeak in leftSidePeaks)
                {
                    var prevPoint = currPoint;
                    currPoint = leftSidePeak;
                    var prevPointIndex = currPointIndex;

                    currPointIndex = tempFrameList.BinarySearch(currPoint, new KvpCompare<double>());

                    if (smoothedY[currPointIndex] < halfmax)
                    {
                        continue;
                    }

                    if (Math.Abs(smoothedY[currPointIndex] - halfmax) < Tolerance)
                    {
                        leftMidpoint = currPoint.Key;
                        continue;
                    }

                    ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                    double a1 = prevPoint.Key;
                    double a2 = currPoint.Key;
                    double c = halfmax;
                    double b1 = smoothedY[prevPointIndex];
                    double b2 = smoothedY[currPointIndex];

                    leftMidpoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                    break;
                }

                // find the right side of the half max
                foreach (var rightSidePeak in rightSidePeaks)
                {
                    var prevPoint = currPoint;
                    currPoint = rightSidePeak;
                    var prevPointIndex = currPointIndex;
                    currPointIndex = tempFrameList.BinarySearch(currPoint, new KvpCompare<double>());

                    if (smoothedY[currPointIndex] > halfmax || smoothedY[currPointIndex] < 0)
                    {
                        continue;
                    }

                    if (Math.Abs(smoothedY[currPointIndex] - halfmax) < Tolerance)
                    {
                        rightMidPoint = currPoint.Key;
                        continue;
                    }

                    ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                    double a1 = prevPoint.Key;
                    double a2 = currPoint.Key;
                    double c = halfmax;
                    double b1 = smoothedY[prevPointIndex];
                    double b2 = smoothedY[currPointIndex];

                    rightMidPoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                    break;
                }

                var correctedRightMidPoint = rightMidPoint / Precision;
                var correctedLeftMidPoint = leftMidpoint / Precision;

                var resolution = realMz / (correctedRightMidPoint - correctedLeftMidPoint);

                var temp = new ResolutionDatapoint
                               {
                                   Intensity = intensity, 
                                   Mz = realMz, 
                                   Resolution = resolution, 
                                   SmoothedIntensity = smoothedPeakIntensity
                               };
                datapointList.Add(temp);
            }

            var tempList =
                datapointList.Where(x => !double.IsInfinity(x.Resolution)).OrderByDescending(x => x.Intensity).Take(10);

            // Put the points on the graph
            foreach (var resolutionDatapoint in tempList)
            {
                var resolutionString = resolutionDatapoint.Resolution.ToString("F1", CultureInfo.InvariantCulture);
                var annotationText = "Peak Location:" + resolutionDatapoint.Mz + Environment.NewLine + "Intensity:"
                                     + resolutionDatapoint.Intensity + Environment.NewLine + "Resolution:"
                                     + resolutionString;
                var peakPoint = new OxyPlot.Annotations.PointAnnotation
                                    {
                                        Text = "R=" + resolutionString, 
                                        X = resolutionDatapoint.Intensity / 1.03125, 
                                        Y = resolutionDatapoint.Mz, 
                                        ToolTip = annotationText
                                    };
                this.mzPlotModel.Annotations.Add(peakPoint);
            }
        }

        #endregion

        /// <summary>
        /// The resolution data point.
        /// </summary>
        private struct ResolutionDatapoint
        {
            #region Fields

            /// <summary>
            /// The intensity.
            /// </summary>
            public double Intensity;

            /// <summary>
            /// The mz.
            /// </summary>
            public double Mz;

            /// <summary>
            /// The resolution.
            /// </summary>
            public double Resolution;

            /// <summary>
            /// The smoothed intensity (done by the peak finder).
            /// </summary>
            public double SmoothedIntensity;

            #endregion
        }

        /// <summary>
        /// A private class that specifies how to compare a Key Value Pair class where the key is an integer.
        /// </summary>
        /// <typeparam name="TValue">
        /// May be any value, is not examined
        /// </typeparam>
        private class KvpCompare<TValue> : IComparer<KeyValuePair<int, TValue>>
        {
            #region Public Methods and Operators

            /// <summary>
            /// The compare function.
            /// </summary>
            /// <param name="x">
            /// The x.
            /// </param>
            /// <param name="y">
            /// The y.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            public int Compare(KeyValuePair<int, TValue> x, KeyValuePair<int, TValue> y)
            {
                return x.Key - y.Key;
            }

            #endregion
        }
    }
}