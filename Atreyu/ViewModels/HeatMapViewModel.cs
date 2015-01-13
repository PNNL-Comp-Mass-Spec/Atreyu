// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeatMapViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The heat map view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.ViewModels
{
    using Atreyu.Events;
    using Atreyu.Models;
    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Practices.Prism.PubSubEvents;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO The heat map view model.
    /// </summary>
    [Export]
    public class HeatMapViewModel : BindableBase
    {
        #region Fields

        /// <summary>
        /// TODO The _event aggregator.
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// TODO The _current frame.
        /// </summary>
        private int _currentFrame;

        /// <summary>
        /// TODO The _heat map data.
        /// </summary>
        private UimfData _heatMapData;

        /// <summary>
        /// TODO The _heat map plot model.
        /// </summary>
        private PlotModel _heatMapPlotModel;

        /// <summary>
        /// TODO The _num frames.
        /// </summary>
        private int _numFrames;

        /// <summary>
        /// TODO The _sum frames.
        /// </summary>
        private FrameRange _sumFrames;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        /// TODO The event aggregator.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [ImportingConstructor]
        public HeatMapViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException();
            }

            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Subscribe(this.UpdateFrameNumber);
            this._eventAggregator.GetEvent<UimfFileLoadedEvent>().Subscribe(this.InitializeUimfData);
            this._eventAggregator.GetEvent<SumFramesChangedEvent>().Subscribe(this.SumFrames);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the heat map plot model.
        /// </summary>
        public PlotModel HeatMapPlotModel
        {
            get
            {
                return this._heatMapPlotModel;
            }

            set
            {
                this.SetProperty(ref this._heatMapPlotModel, value);
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The initialize uimf data.
        /// </summary>
        /// <param name="file">
        /// TODO The file.
        /// </param>
        public void InitializeUimfData(string file)
        {
            this._heatMapData = new UimfData(file);
            this._heatMapData.currentMinBin = 0;
            this._heatMapData.currentMaxBin = this._heatMapData.TotalBins;
            this.SetUpPlot(1);
            this._eventAggregator.GetEvent<UimfFileChangedEvent>().Publish(this._heatMapData);

            this._numFrames = this._heatMapData.Frames;
            this._eventAggregator.GetEvent<NumberOfFramesChangedEvent>().Publish(this._numFrames);
            this._eventAggregator.GetEvent<MinimumNumberOfFrames>().Publish(1);

            int? frameNumber = 1;
            this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Publish(frameNumber);
        }

        /// <summary>
        /// TODO The set up plot.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        public void SetUpPlot(int frameNumber)
        {
            this._currentFrame = frameNumber;
            this.HeatMapPlotModel = new PlotModel();

            var linearColorAxis1 = new LinearColorAxis
                                       {
                                           HighColor = OxyColors.Purple, 
                                           LowColor = OxyColors.Black, 
                                           Position = AxisPosition.Right, 
                                           Minimum = 1, 
                                           Title = "Intensity"
                                       };

            this.HeatMapPlotModel.Axes.Add(linearColorAxis1);

            var horizontalAxis = new LinearAxis
                                     {
                                         Position = AxisPosition.Bottom, 
                                         AbsoluteMinimum = 0, 
                                         AbsoluteMaximum = this._heatMapData.Scans, 
                                         MinimumRange = 10, 
                                         MaximumPadding = 0, 
                                         Title = "Mobility Scans"
                                     };

            // 	horizontalAxis.AxisChanged += OnXAxisChange;
            this.HeatMapPlotModel.Axes.Add(horizontalAxis);

            var verticalAxis = new LinearAxis
                                   {
                                       AbsoluteMinimum = 0, 
                                       AbsoluteMaximum = this._heatMapData.MaxBins, 
                                       MinimumRange = 10, 
                                       MaximumPadding = 0, 
                                       Title = "TOF Bins"
                                   };

            verticalAxis.AxisChanged += this.OnYAxisChange;

            this.HeatMapPlotModel.Axes.Add(verticalAxis);

            var heatMapSeries1 = new HeatMapSeries
                                     {
                                         X0 = 0, 
                                         X1 = 359, 
                                         Y0 = 0, 
                                         Y1 = this._heatMapData.MaxBins, 
                                         Data =
                                             this._heatMapData.ReadData(
                                                 1, 
                                                 this._heatMapData.MaxBins, 
                                                 this._currentFrame, 
                                                 this._currentFrame, 
                                                 this.Height, 
                                                 0, 
                                                 359), 
                                         Interpolate = false
                                     };

            this.HeatMapPlotModel.Series.Add(heatMapSeries1);

            this.HeatMapPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// TODO The update frame number.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        public void UpdateFrameNumber(int? frameNumber)
        {
            this._currentFrame = frameNumber.Value;

            var series = this.HeatMapPlotModel.Series[0] as HeatMapSeries;
            if (series != null)
            {
                series.Data = this._heatMapData.ReadData(
                    this._heatMapData.currentMinBin, 
                    this._heatMapData.currentMaxBin, 
                    this._currentFrame, 
                    this._currentFrame, 
                    this.Height, 
                    (int)this._heatMapPlotModel.Axes[1].ActualMinimum, 
                    (int)this._heatMapPlotModel.Axes[1].ActualMaximum);
                this.HeatMapPlotModel.InvalidatePlot(true);
                this._eventAggregator.GetEvent<XAxisChangedEvent>()
                    .Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
                this._eventAggregator.GetEvent<YAxisChangedEvent>()
                    .Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
            }
        }

        /// <summary>
        /// TODO The update plot new height.
        /// </summary>
        /// <param name="height">
        /// TODO The height.
        /// </param>
        public void UpdatePlotNewHeight(int height)
        {
            this.Height = height;
            if (this.HeatMapPlotModel != null)
            {
                var series = this.HeatMapPlotModel.Series[0] as HeatMapSeries;
                if (series != null)
                {
                    series.Data = this._heatMapData.ReadData(
                        this._heatMapData.currentMinBin, 
                        this._heatMapData.currentMaxBin, 
                        this._currentFrame, 
                        this._currentFrame, 
                        height, 
                        (int)this._heatMapPlotModel.Axes[1].ActualMinimum, 
                        (int)this._heatMapPlotModel.Axes[1].ActualMaximum);
                    this.HeatMapPlotModel.InvalidatePlot(true);
                    this._eventAggregator.GetEvent<XAxisChangedEvent>()
                        .Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
                    this._eventAggregator.GetEvent<YAxisChangedEvent>()
                        .Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
                }
            }
        }

        #endregion

        #region Methods

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
            var series = this._heatMapPlotModel.Series[0] as HeatMapSeries;
            if (e.ChangeType == AxisChangeTypes.Reset)
            {
                this._heatMapData.currentMinBin = 0;
                this._heatMapData.currentMaxBin = this._heatMapData.MaxBins;
                var startScan = 0;
                var endScan = this._heatMapData.Scans - 1;
                if (series != null)
                {
                    series.Data = this._heatMapData.ReadData(
                        this._heatMapData.currentMinBin, 
                        this._heatMapData.currentMaxBin, 
                        this._currentFrame, 
                        this._currentFrame, 
                        this.Height, 
                        startScan, 
                        endScan);
                    series.X0 = startScan;
                    series.X1 = endScan;
                    series.Y0 = this._heatMapData.currentMinBin;
                    series.Y1 = this._heatMapData.currentMaxBin;
                }
            }
            else
            {
                LinearAxis yAxis = sender as LinearAxis;

                var xAxis = this._heatMapPlotModel.Axes[1];
                this._heatMapData.currentMinBin = (int)yAxis.ActualMinimum;
                this._heatMapData.currentMaxBin = (int)yAxis.ActualMaximum;

                var startScan = (int)xAxis.ActualMinimum;
                var endScan = (int)xAxis.ActualMaximum;

                if (series != null)
                {
                    series.Data = this._heatMapData.ReadData(
                        this._heatMapData.currentMinBin, 
                        this._heatMapData.currentMaxBin, 
                        this._currentFrame, 
                        this._currentFrame, 
                        this.Height, 
                        startScan, 
                        endScan);
                    series.X0 = startScan;
                    series.X1 = endScan;
                    series.Y0 = this._heatMapData.currentMinBin;
                    series.Y1 = this._heatMapData.currentMaxBin;
                }
            }

            this._heatMapPlotModel.InvalidatePlot(true);
            this._eventAggregator.GetEvent<XAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
            this._eventAggregator.GetEvent<YAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
        }

        /// <summary>
        /// TODO The sum frames.
        /// </summary>
        /// <param name="sumFrames">
        /// TODO The sum frames.
        /// </param>
        private async void SumFrames(FrameRange sumFrames)
        {
            LinearAxis yAxis = this._heatMapPlotModel.Axes[2] as LinearAxis;
            var series = this._heatMapPlotModel.Series[0] as HeatMapSeries;
            var xAxis = this._heatMapPlotModel.Axes[1];
            this._heatMapData.currentMinBin = (int)yAxis.ActualMinimum;
            this._heatMapData.currentMaxBin = (int)yAxis.ActualMaximum;

            var startScan = (int)xAxis.ActualMinimum;
            var endScan = (int)xAxis.ActualMaximum;

            if (series != null)
            {
                await
                    Task.Run(
                        () =>
                        series.Data =
                        this._heatMapData.ReadData(
                            this._heatMapData.currentMinBin, 
                            this._heatMapData.currentMaxBin, 
                            sumFrames.StartFrame, 
                            sumFrames.EndFrame, 
                            this.Height, 
                            startScan, 
                            endScan));
                series.X0 = startScan;
                series.X1 = endScan;
                series.Y0 = this._heatMapData.currentMinBin;
                series.Y1 = this._heatMapData.currentMaxBin;
            }

            this._heatMapPlotModel.InvalidatePlot(true);
            this._eventAggregator.GetEvent<XAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
            this._eventAggregator.GetEvent<YAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
        }

        #endregion
    }
}