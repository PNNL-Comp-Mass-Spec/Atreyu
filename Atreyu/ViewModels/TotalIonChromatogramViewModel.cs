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
    using OxyPlot.Wpf;

    using ReactiveUI;

    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LineSeries = OxyPlot.Series.LineSeries;

    // using Falkor.Events.Atreyu;

    /// <summary>
    /// The total ion chromatogram view model.
    /// </summary>
    [Export]
    public class TotalIonChromatogramViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The end scan.
        /// </summary>
        private int endScan;

        /// <summary>
        /// The frame data.
        /// </summary>
        private double[,] frameData;

        /// <summary>
        /// The frame data.
        /// </summary>
        private Dictionary<int, double> frameDictionary;

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

        /// <summary>
        /// To determine if a file was loaded yet
        /// </summary>
        private bool _uimfLoaded;

        private int maxScan;
        private Visibility _ticVisible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalIonChromatogramViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public TotalIonChromatogramViewModel()
        {
            this.frameDictionary = new Dictionary<int, double>();
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

        public bool UimfLoaded
        {
            get { return this._uimfLoaded; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._uimfLoaded, value);
                
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
        /// The get tic data.
        /// </summary>
        /// <returns>
        /// The dictionary of tic data, keyed by scan.
        /// </returns>
        public IDictionary<int, double> GetTicData()
        {
            return this.frameDictionary;
        }

        /// <summary>
        /// Gets the image of the tic plot.
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

            
                this.frameData = data;

            if (this.endScan == 0)
            {
                this.startScan = 0;
                this.endScan = frameData.GetLength(0);
            }

            this.frameDictionary.Clear();

            for (var i = 0; i < this.endScan - this.startScan; i++)
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

            //series.MarkerType = MarkerType.Circle;
            //series.MarkerSize = 2.5;

            // series.MarkerStrokeThickness = 2;
            series.MarkerFill = OxyColors.Black;
            series.BrokenLineColor = OxyColors.Automatic;
            series.BrokenLineStyle = LineStyle.Dot;
            series.BrokenLineThickness = 1;
            series.Points.Clear();
            foreach (var d in this.frameDictionary)
            {
                series.Points.Add(new DataPoint(d.Key, d.Value));
                //series.Points.Add(new DataPoint(double.NaN, double.NaN));
            }

            //this.FindPeaks();

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
            this.UimfLoaded = uimfDataNew != null;
            if (this.TicPlotModel != null)
            {
                return;
            }

            this.TicPlotModel = new PlotModel();
            var linearAxis = new LinearAxis
                                 {
                                     Position = AxisPosition.Bottom, 
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
                                      IsAxisVisible = true, 
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

            // Create a new dictionary so we don't modify the original one
            var tempFrameDict = new Dictionary<double, double>(this.uimfData.Scans);

            for (var i = 0; i < this.uimfData.Scans; i++)
            {
                // this is a hack to make the library work and return the proper location index
                double junk;
                tempFrameDict.Add(i, this.frameDictionary.TryGetValue(i, out junk) ? junk : 0);
            }

            var results = Utilities.PeakFinder.FindPeaks(tempFrameDict.ToList());

            foreach (var peakInformation in results.Peaks)
            {
                var resolutionString = peakInformation.ResolvingPower.ToString("F1", CultureInfo.InvariantCulture);

                var peakPoint = new OxyPlot.Annotations.PointAnnotation
                                    {
                                        Text = "R=" + resolutionString, 
                                        X = peakInformation.PeakCenter, 
                                        Y = peakInformation.Intensity / 2.5, 
                                        ToolTip = peakInformation.ToString()
                                    };
                this.ticPlotModel.Annotations.Add(peakPoint);
            }
        }

        #endregion

        public Visibility TicVisible
        {
            get { return _ticVisible; }
            set { this.RaiseAndSetIfChanged(ref this._ticVisible, value); }
        }

    }
}