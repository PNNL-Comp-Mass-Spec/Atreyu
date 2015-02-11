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
    using System.IO;

    using Atreyu.Models;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Wpf;

    using ReactiveUI;

    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LineSeries = OxyPlot.Series.LineSeries;

    // using Falkor.Events.Atreyu;

    /// <summary>
    /// TODO The total ion chromatogram view model.
    /// </summary>
    [Export]
    public class TotalIonChromatogramViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// TODO The _current frame number.
        /// </summary>
        ////private int _currentFrameNumber;
        /// <summary>
        /// TODO The _end scan.
        /// </summary>
        private int _endScan;

        /// <summary>
        /// TODO The _frame data.
        /// </summary>
        private double[,] _frameData;

        private Dictionary<int, double> frameData;

        /// <summary>
        /// TODO The _start scan.
        /// </summary>
        private int _startScan;

        /// <summary>
        /// TODO The _tic plot model.
        /// </summary>
        private PlotModel _ticPlotModel;

        /// <summary>
        /// TODO The _uimf data.
        /// </summary>
        private UimfData uimfData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalIonChromatogramViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        /// TODO The event aggregator.
        /// </param>
        /// <exception cref="NullReferenceException">
        /// </exception>
        [ImportingConstructor]
        public TotalIonChromatogramViewModel()
        {
            // this._eventAggregator = eventAggregator;
            // this._eventAggregator.GetEvent<UimfFileChangedEvent>().Subscribe(this.UpdateReference, true);
            // this._eventAggregator.GetEvent<XAxisChangedEvent>().Subscribe(this.UpdateAxes, true);
            // this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Subscribe(this.UpdateFrameNumber, true);
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
                return this._ticPlotModel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this._ticPlotModel, value);
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
            this._endScan = value;
        }

        /// <summary>
        /// TODO The change start scan.
        /// </summary>
        /// <param name="value">
        /// TODO The value.
        /// </param>
        public void ChangeStartScan(int value)
        {
            this._startScan = value;
        }

        /// <summary>
        /// Returns a dictionary of <scan, intensity> data that is currently being displayed.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<int, double> GetTicData()
        {
            return this.frameData;
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
        /// <param name="Data">
        /// The Data.
        /// </param>
        public void UpdateFrameData(double[,] Data)
        {
            this._frameData = Data;
            if (this._frameData == null)
            {
                return;
            }

            if (this._endScan == 0)
            {
                this._startScan = 0;
                this._endScan = 359;
            }

            this.frameData = new Dictionary<int, double>();

            for (var i = 0; i < this._frameData.GetLength(0); i++)
            {
                var index = i + this._startScan;
                for (var j = 0; j < this._frameData.GetLength(1); j++)
                {
                    if (this.frameData.ContainsKey(index))
                    {
                        this.frameData[index] += this._frameData[i, j];
                    }
                    else
                    {
                        this.frameData.Add(index, this._frameData[i, j]);
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
            foreach (var d in frameData)
            {
                series.Points.Add(new DataPoint(d.Key, d.Value));
                series.Points.Add(new DataPoint(double.NaN, double.NaN));
            }

            this.TicPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// TODO The update reference.
        /// </summary>
        /// <param name="uimfData">
        /// TODO The uimf data.
        /// </param>
        public void UpdateReference(UimfData uimfData)
        {
            this.uimfData = uimfData;
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
        /// TODO The on x axis changed.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        protected void OnXAxisChanged(object sender, AxisChangedEventArgs e)
        {
        }

        /// <summary>
        /// TODO The on y axis change.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        protected void OnYAxisChange(object sender, AxisChangedEventArgs e)
        {
        }

        /// <summary>
        /// TODO The update axes.
        /// </summary>
        /// <param name="linearAxis">
        /// TODO The linear axis.
        /// </param>
        private void UpdateAxes(LinearAxis linearAxis)
        {
            var xAxis = this.TicPlotModel.Axes[0] as LinearAxis;
            this._startScan = (int)linearAxis.ActualMinimum;
            this._endScan = (int)linearAxis.ActualMaximum;

            xAxis.AbsoluteMaximum = this._endScan;
            xAxis.Minimum = this._startScan;
            xAxis.Maximum = this._endScan;
            this._frameData = this.uimfData.FrameData;
            if (this._frameData != null)
            {
                Dictionary<int, double> frameData = new Dictionary<int, double>();

                for (int i = 0; i < this._frameData.GetLength(0); i++)
                {
                    var index = i + this._startScan;
                    for (int j = 0; j < this._frameData.GetLength(1); j++)
                    {
                        if (frameData.ContainsKey(index))
                        {
                            frameData[index] += this._frameData[i, j];
                        }
                        else
                        {
                            frameData.Add(index, this._frameData[i, j]);
                        }
                    }
                }

                var series = this.TicPlotModel.Series[0] as LineSeries;
                series.Points.Clear();
                foreach (var d in frameData)
                {
                    series.Points.Add(new DataPoint(d.Key, d.Value));
                }

                this.TicPlotModel.InvalidatePlot(true);
            }
        }

        #endregion
    }
}