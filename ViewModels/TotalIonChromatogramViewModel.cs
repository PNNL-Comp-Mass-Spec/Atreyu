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

    using Falkor.Atreyu.Models;
    using Falkor.Events.Atreyu;

    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Practices.Prism.PubSubEvents;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    /// <summary>
    /// TODO The total ion chromatogram view model.
    /// </summary>
    [Export]
    public class TotalIonChromatogramViewModel : BindableBase
    {
        #region Fields

        /// <summary>
        /// TODO The _current frame number.
        /// </summary>
        private int _currentFrameNumber;

        /// <summary>
        /// TODO The _end scan.
        /// </summary>
        private int _endScan;

        /// <summary>
        /// TODO The _event aggregator.
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// TODO The _frame data.
        /// </summary>
        private double[,] _frameData;

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
        private UimfData _uimfData;

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
        public TotalIonChromatogramViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new NullReferenceException();
            }

            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<UimfFileChangedEvent>().Subscribe(this.UpdateReference, true);
            this._eventAggregator.GetEvent<XAxisChangedEvent>().Subscribe(this.UpdateAxes, true);
            this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Subscribe(this.UpdateFrameNumber, true);
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
                this.SetProperty(ref this._ticPlotModel, value);
            }
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
            this._frameData = this._uimfData.FrameData;
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

        /// <summary>
        /// TODO The update frame number.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        private void UpdateFrameNumber(int? frameNumber)
        {
            this._currentFrameNumber = frameNumber.Value;
            this._frameData = this._uimfData.FrameData;
            if (this._frameData != null)
            {
                if (this._endScan == 0)
                {
                    this._startScan = 0;
                    this._endScan = 359;
                }

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

        /// <summary>
        /// TODO The update reference.
        /// </summary>
        /// <param name="uimfData">
        /// TODO The uimf data.
        /// </param>
        private void UpdateReference(UimfData uimfData)
        {
            this._uimfData = uimfData;
            if (this.TicPlotModel != null)
            {
                return;
            }

            this.TicPlotModel = new PlotModel();
            var linearAxis = new LinearAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = 0 };
            linearAxis.IsPanEnabled = false;
            linearAxis.IsZoomEnabled = false;
            this.TicPlotModel.Axes.Add(linearAxis);

            var linearYAxis = new LinearAxis
                                  {
                                      IsZoomEnabled = false, 
                                      AbsoluteMinimum = 0, 
                                      MinimumPadding = 0.1, 
                                      IsPanEnabled = false
                                  };

            this.TicPlotModel.Axes.Add(linearYAxis);
            LineSeries series = new LineSeries { Color = OxyColors.Black, };

            this.TicPlotModel.Series.Add(series);
        }

        #endregion
    }
}