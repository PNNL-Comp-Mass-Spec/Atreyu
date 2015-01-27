// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MzSpectraViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The mz spectra view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.ViewModels
{
    using Atreyu.Events;
    using Atreyu.Models;
    //using Falkor.Events.Atreyu;

    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Practices.Prism.PubSubEvents;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    /// <summary>
    /// TODO The mz spectra view model.
    /// </summary>
    [Export]
    public class MzSpectraViewModel : BindableBase
    {
        #region Fields

        /// <summary>
        /// TODO The _current frame number.
        /// </summary>
        private int _currentFrameNumber;

        /// <summary>
        /// TODO The _end mz bin.
        /// </summary>
        private int _endMzBin;

        /// <summary>
        /// TODO The _frame data.
        /// </summary>
        private double[,] _frameData;

        /// <summary>
        /// TODO The _mz plot model.
        /// </summary>
        private PlotModel _mzPlotModel;

        /// <summary>
        /// TODO The _start mz bin.
        /// </summary>
        private int _startMzBin;

        /// <summary>
        /// TODO The _uimf data.
        /// </summary>
        private UimfData _uimfData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MzSpectraViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        /// TODO The event aggregator.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [ImportingConstructor]
        public MzSpectraViewModel()
        {
            ////this._eventAggregator = eventAggregator;
            ////this._eventAggregator.GetEvent<UimfFileChangedEvent>().Subscribe(this.UpdateReference, true);
            ////this._eventAggregator.GetEvent<YAxisChangedEvent>().Subscribe(this.UpdateXAxis, true);
            ////this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Subscribe(this.UpdateFrameNumber, true);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the mz plot model.
        /// </summary>
        public PlotModel MzPlotModel
        {
            get
            {
                return this._mzPlotModel;
            }

            set
            {
                this.SetProperty(ref this._mzPlotModel, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The update frame number.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        public void UpdateFrameData(double[,] framedata)
        {
            if (this._uimfData == null) return;
            if (framedata == null)
            {
                return;
            }

            this._frameData = framedata;
            Dictionary<int, double> frameData = new Dictionary<int, double>();

            for (int j = 0; j < this._frameData.GetLength(1); j++)
            {
                var index = j + this._startMzBin;
                for (int i = 0; i < this._frameData.GetLength(0); i++)
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

            var series = this.MzPlotModel.Series[0] as LineSeries;
            if (series != null)
            {
                series.Points.Clear();
                foreach (var d in frameData)
                {
                    series.Points.Add(new DataPoint(d.Key, d.Value));
                }
            }

            this.MzPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// TODO The update reference.
        /// </summary>
        /// <param name="uimfData">
        /// TODO The uimf data.
        /// </param>
        public void UpdateReference(UimfData uimfData)
        {
            if (uimfData == null) return;

            this._uimfData = uimfData;
            this.MzPlotModel = new PlotModel();
            var linearAxis = new LinearAxis
                                 {
                                     Position = AxisPosition.Right, 
                                     AbsoluteMinimum = 0, 
                                     Key = "XAxisKey", 
                                     IsPanEnabled = false, 
                                     IsZoomEnabled = false, 
                                     MinimumPadding = 0.05
                                 };
            this.MzPlotModel.Axes.Add(linearAxis);

            var linearYAxis = new LinearAxis
                                  {
                                      AbsoluteMinimum = 0, 
                                      IsZoomEnabled = false, 
                                      Position = AxisPosition.Top, 
                                      Key = "YAxisKey", 
                                      IsPanEnabled = false, 
                                      MinimumPadding = 0
                                  };
            this.MzPlotModel.Axes.Add(linearYAxis);
            LineSeries series = new LineSeries
                                    {
                                        Color = OxyColors.Black, 
                                        YAxisKey = linearAxis.Key, 
                                        XAxisKey = linearYAxis.Key, 
                                    };
            this.MzPlotModel.Series.Add(series);
        }

        /// <summary>
        /// TODO The update x axis.
        /// </summary>
        /// <param name="linearAxis">
        /// TODO The linear axis.
        /// </param>
        private void UpdateXAxis(LinearAxis linearAxis)
        {
            var xAxis = this.MzPlotModel.Axes[0] as LinearAxis;
            this._startMzBin = (int)linearAxis.ActualMinimum;
            this._endMzBin = (int)linearAxis.ActualMaximum;

            xAxis.AbsoluteMaximum = this._endMzBin;
            this._frameData = this._uimfData.FrameData;
            if (this._frameData != null)
            {
                Dictionary<int, double> frameData = new Dictionary<int, double>();

                for (int j = 0; j < this._frameData.GetLength(1); j++)
                {
                    var index = j + this._startMzBin;
                    for (int i = 0; i < this._frameData.GetLength(0); i++)
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

                var series = this.MzPlotModel.Series[0] as LineSeries;
                if (series != null)
                {
                    series.Points.Clear();
                    foreach (var d in frameData)
                    {
                        series.Points.Add(new DataPoint(d.Key, d.Value));
                    }
                }

                this.MzPlotModel.InvalidatePlot(true);
            }
        }

        #endregion
    }
}