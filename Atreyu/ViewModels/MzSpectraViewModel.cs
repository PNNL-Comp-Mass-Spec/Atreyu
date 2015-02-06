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
    /// TODO The mz spectra view model.
    /// </summary>
    [Export]
    public class MzSpectraViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// TODO The _current frame number.
        /// </summary>
        ////private int _currentFrameNumber;
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

        /// <summary>
        /// TODO The intercept.
        /// </summary>
        private double intercept;

        /// <summary>
        /// TODO The show mz.
        /// </summary>
        private bool showMz;

        /// <summary>
        /// TODO The slope.
        /// </summary>
        private double slope;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MzSpectraViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public MzSpectraViewModel()
        {
            this.WhenAnyValue(vm => vm.ShowMz).Subscribe(b => this.UpdateFrameData(this._frameData));

            ////this._eventAggregator = eventAggregator;
            ////this._eventAggregator.GetEvent<UimfFileChangedEvent>().Subscribe(this.UpdateReference, true);
            ////this._eventAggregator.GetEvent<YAxisChangedEvent>().Subscribe(this.UpdateXAxis, true);
            ////this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Subscribe(this.UpdateFrameNumber, true);
        }

        #endregion

        #region Public Properties

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
                this.RaiseAndSetIfChanged(ref this._mzPlotModel, value);
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The get m/z image.
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
        /// TODO The update frame number.
        /// </summary>
        /// <param name="framedata">
        /// The framedata.
        /// </param>
        public void UpdateFrameData(double[,] framedata)
        {
            if (this._uimfData == null)
            {
                return;
            }

            if (framedata == null)
            {
                return;
            }

            this._frameData = framedata;
            var frameData = new Dictionary<double, double>();

            for (int j = 0; j < this._frameData.GetLength(1); j++)
            {
                double index = j + this._startMzBin;
                if (this.showMz)
                {
                    // m/z=(K(t-t0))^2
                    // where K = slope
                    // t = bin
                    // and t0 = intercept
                    // but what units?
                    index = Math.Pow(this.Slope * (index - this.Intercept), 2) / 1000000;
                }

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
            series.MarkerType = MarkerType.None;

            if (series.YAxis != null)
            {
                series.YAxis.Title = this.ShowMz ? "m/z" : "Bin";
            }

            if (series != null)
            {
                series.Points.Clear();
                foreach (var d in frameData)
                {
                    series.Points.Add(new DataPoint(d.Value, d.Key));
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
            if (uimfData == null)
            {
                return;
            }

            this._uimfData = uimfData;
            this.MzPlotModel = new PlotModel();
            var linearAxis = new LinearAxis
                                 {
                                     Position = AxisPosition.Right, 
                                     AbsoluteMinimum = 0, 
                                     Key = "XAxisKey", 
                                     IsPanEnabled = false, 
                                     IsZoomEnabled = false, 
                                     MinimumPadding = 0.05, 
                                     Title = this.ShowMz ? "m/z" : "Bin"
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
        /// TODO The change end bin.
        /// </summary>
        /// <param name="bin">
        /// TODO The bin.
        /// </param>
        public void changeEndBin(int bin)
        {
            this._endMzBin = bin;
        }

        /// <summary>
        /// TODO The change start bin.
        /// </summary>
        /// <param name="bin">
        /// TODO The bin.
        /// </param>
        public void changeStartBin(int bin)
        {
            this._startMzBin = bin;
        }

        #endregion

        #region Methods

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