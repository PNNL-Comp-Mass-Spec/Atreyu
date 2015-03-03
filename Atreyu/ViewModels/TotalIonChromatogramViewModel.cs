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
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
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


        private void FindPeaks()
        {
            this.ticPlotModel.Annotations.Clear();
            var peakDetector = new PeakDetector();

            var finderOptions = PeakDetector.GetDefaultSICPeakFinderOptions();

            List<double> smoothedY;

            ////var highestPoint = this.frameDictionary.OrderByDescending(kvp => kvp.Value).First();
            ////var lastPoint = this.frameDictionary.OrderByDescending(kvp => kvp.Key).Last();
            ////var firstPoint = this.frameDictionary.OrderByDescending(kvp => kvp.Key).First();

            // I am not sure what this does and need to talk to Matt Monroe, but in the example exe file that came with the library
            // they used half of the length of the list in their previous examples and this seems to work on teh zoomed out version
            // but not when we zoom in, it seems like an offset problem.
            var originalpeakLocation = this.frameDictionary.Count / 2;

            var peaks = peakDetector.FindPeaks(
                finderOptions,
                this.frameDictionary.ToList(),
                originalpeakLocation,
                out smoothedY);

            foreach (var peak in peaks)
            {
                ////if (!peak.IsValid)
                ////{
                ////    continue;
                ////}

                double intensity;
                if (!this.frameDictionary.TryGetValue(peak.LocationIndex, out intensity))
                {
                    intensity = 50;
                }
                
                var annotation = new TextAnnotation { Text = "PEAK!", TextPosition = new DataPoint(peak.LocationIndex, intensity) };

                this.ticPlotModel.Annotations.Add(annotation);
            }

        }

    }
}