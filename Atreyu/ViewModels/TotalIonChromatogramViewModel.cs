// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalIonChromatogramViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The total ion chromatogram view model.
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

    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LineSeries = OxyPlot.Series.LineSeries;
    using TextAnnotation = OxyPlot.Annotations.TextAnnotation;

    // using Falkor.Events.Atreyu;

    /// <summary>
    /// TODO The total ion chromatogram view model.
    /// </summary>
    [Export]
    public class TotalIonChromatogramViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// TODO The _end scan.
        /// </summary>
        private int endScan;

        /// <summary>
        /// TODO The _frame data.
        /// </summary>
        private double[,] frameData;

        /// <summary>
        /// TODO The frame data.
        /// </summary>
        private Dictionary<int, double> frameDictionary;

        /// <summary>
        /// TODO The _start scan.
        /// </summary>
        private int startScan;

        /// <summary>
        /// TODO The _tic plot model.
        /// </summary>
        private PlotModel ticPlotModel;

        /// <summary>
        /// TODO The _uimf data.
        /// </summary>
        private UimfData uimfData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalIonChromatogramViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public TotalIonChromatogramViewModel()
        {
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The change end scan.
        /// </summary>
        /// <param name="value">
        /// TODO The value.
        /// </param>
        public void ChangeEndScan(int value)
        {
            this.endScan = value;
        }

        /// <summary>
        /// TODO The change start scan.
        /// </summary>
        /// <param name="value">
        /// TODO The value.
        /// </param>
        public void ChangeStartScan(int value)
        {
            this.startScan = value;
        }

        /// <summary>
        /// TODO The get tic data.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<int, double> GetTicData()
        {
            return this.frameDictionary;
        }

        /// <summary>
        /// TODO The get tic image.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetTicImage()
        {
            var stream = new MemoryStream();
            PngExporter.Export(
                this.TicPlotModel, 
                stream, 
                (int)this.TicPlotModel.Width, 
                (int)this.TicPlotModel.Height, 
                OxyColors.White);

            Image image = new Bitmap(stream);
            return image;
        }

        /// <summary>
        /// TODO The update frame data.
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

            this.frameData = data;

            if (this.endScan == 0)
            {
                this.startScan = 0;
                this.endScan = 359;
            }

            this.frameDictionary = new Dictionary<int, double>();

            for (var i = 0; i < this.frameData.GetLength(0); i++)
            {
                var index = i + this.startScan;
                for (var j = 0; j < this.frameData.GetLength(1); j++)
                {
                    if (this.frameDictionary.ContainsKey(index))
                    {
                        this.frameDictionary[index] += this.frameData[i, j];
                    }
                    else
                    {
                        this.frameDictionary.Add(index, this.frameData[i, j]);
                    }
                }
            }

            var series = this.TicPlotModel.Series[0] as LineSeries;

            if (series == null)
            {
                return;
            }

            series.MarkerType = MarkerType.Circle;
            series.MarkerSize = 2.5;

            // series.MarkerStrokeThickness = 2;
            series.MarkerFill = OxyColors.Black;
            series.BrokenLineColor = OxyColors.Automatic;
            series.BrokenLineStyle = LineStyle.Dot;
            series.BrokenLineThickness = 1;
            series.Points.Clear();
            foreach (var d in this.frameDictionary)
            {
                series.Points.Add(new DataPoint(d.Key, d.Value));
                series.Points.Add(new DataPoint(double.NaN, double.NaN));
            }

            this.FindPeaks();

            this.TicPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// TODO The update reference.
        /// </summary>
        /// <param name="uimfDataNew">
        /// TODO The new <see cref="UimfData"/> that is coming in.
        /// </param>
        public void UpdateReference(UimfData uimfDataNew)
        {
            this.uimfData = uimfDataNew;
            if (this.TicPlotModel != null)
            {
                return;
            }

            this.TicPlotModel = new PlotModel();
            var linearAxis = new LinearAxis
                                 {
                                     Position = AxisPosition.Top, 
                                     AbsoluteMinimum = 0, 
                                     IsPanEnabled = false, 
                                     IsZoomEnabled = false, 
                                     Title = "Scan", 
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
                                      IsAxisVisible = false, 
                                      Title = "Intensity"
                                  };

            this.TicPlotModel.Axes.Add(linearYAxis);
            var series = new LineSeries { Color = OxyColors.Black, };

            this.TicPlotModel.Series.Add(series);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find the peaks in the current data set and adds an annotation point with the resolution to the TIC.
        /// </summary>
        private void FindPeaks()
        {
            this.ticPlotModel.Annotations.Clear();
            var peakDetector = new PeakDetector();

            var finderOptions = PeakDetector.GetDefaultSICPeakFinderOptions();

            List<double> smoothedY;

            // Create a new dictionary so we don't modify the original one
            var tempFrameDict = new Dictionary<int, double>(this.uimfData.Scans);

            // this is a hack to make the library work and return the proper location index
            double junk;
            for (var i = 0; i < this.uimfData.Scans; i++)
            {
                tempFrameDict.Add(i, this.frameDictionary.TryGetValue(i, out junk) ? junk : 0);
            }

            // I am not sure what this does and need to talk to Matt Monroe, but in the example exe file that came with the library
            // they used half of the length of the list in their previous examples and this seems to work on teh zoomed out version
            // but not when we zoom in, it seems like an offset problem.
            var originalpeakLocation = tempFrameDict.Count / 2;

            // The idea behind this is to always give the key of the mid point of the list, but this causes the finder to blow up.
            ////var originalpeakLocation = this.frameDictionary.First().Key + (this.frameDictionary.Count / 2);
            var peaks = peakDetector.FindPeaks(
                finderOptions, 
                tempFrameDict.OrderBy(x => x.Key).ToList(), 
                originalpeakLocation, 
                out smoothedY);

            foreach (var peak in peaks)
            {
                ////var firstpoint = this.frameDictionary.First();
                var index = peak.LocationIndex; // + firstpoint.Key; 

                double intensity;
                if (!tempFrameDict.TryGetValue(index, out intensity))
                {
                    intensity = 50;
                }

                var halfmax = intensity / 2.0;

                var peakDataset = tempFrameDict.Where(x => x.Key >= peak.LeftEdge && x.Key <= peak.RightEdge).ToList();

                // find the left mid point
                var currPoint = new KeyValuePair<int, double>(0, 0);
                double leftMidpoint = 0;
                double rightMidPoint = 0;
                for (var i = 0; i < peakDataset.Count; i++)
                {
                    const double Tolerance = 0.01;
                    var prevPoint = currPoint;
                    currPoint = peakDataset[i];

                    if (Math.Abs(leftMidpoint) < Tolerance)
                    {
                        if (smoothedY[currPoint.Key] < halfmax)
                        {
                            continue;
                        }

                        if (Math.Abs(smoothedY[currPoint.Key] - halfmax) < Tolerance)
                        {
                            leftMidpoint = currPoint.Key;
                            continue;
                        }

                        ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                        double a1 = prevPoint.Key;
                        double a2 = currPoint.Key;
                        double c = halfmax;
                        double b1 = smoothedY[prevPoint.Key];
                        double b2 = smoothedY[currPoint.Key];

                        leftMidpoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                        continue;
                    }

                    if (Math.Abs(rightMidPoint) < Tolerance)
                    {
                        if (smoothedY[currPoint.Key] > halfmax)
                        {
                            continue;
                        }

                        if (Math.Abs(smoothedY[currPoint.Key] - halfmax) < Tolerance)
                        {
                            rightMidPoint = currPoint.Key;
                            continue;
                        }

                        ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                        double a1 = prevPoint.Key;
                        double a2 = currPoint.Key;
                        double c = halfmax;
                        double b1 = smoothedY[prevPoint.Key];
                        double b2 = smoothedY[currPoint.Key];

                        rightMidPoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                        continue;
                    }
                }

                var resolution = peak.LocationIndex / (rightMidPoint - leftMidpoint);

                var pointAnnotation1 = new OxyPlot.Annotations.PointAnnotation
                                           {
                                               X = leftMidpoint, 
                                               Y = halfmax, 
                                               Text = "Left", 
                                               ToolTip =
                                                   "Left mid Point Found at "
                                                   + leftMidpoint
                                           };

                ////this.ticPlotModel.Annotations.Add(pointAnnotation1);
                var pointAnnotation2 = new OxyPlot.Annotations.PointAnnotation
                                           {
                                               X = rightMidPoint, 
                                               Y = halfmax, 
                                               Text = "right", 
                                               ToolTip =
                                                   "right mid Point Found at "
                                                   + rightMidPoint
                                           };

                ////this.ticPlotModel.Annotations.Add(pointAnnotation2);
                var resolutionString = resolution.ToString("F1", CultureInfo.InvariantCulture);

                var annotationText = "Peak Location:" + peak.LocationIndex + Environment.NewLine + "Intensity:"
                                     + intensity + Environment.NewLine + "Resolution:" + resolutionString;

                var annotation = new TextAnnotation
                                     {
                                         Text = annotationText, 
                                         TextPosition =
                                             new DataPoint(peak.LocationIndex, (int)(intensity / 3))
                                     };

                ////this.ticPlotModel.Annotations.Add(annotation);
                var peakPoint = new OxyPlot.Annotations.PointAnnotation
                                    {
                                        Text = "R=" + resolutionString, 
                                        X = peak.LocationIndex, 
                                        Y = intensity / 2.5, 
                                        ToolTip = annotationText
                                    };
                this.ticPlotModel.Annotations.Add(peakPoint);
            }
        }

        #endregion
    }
}