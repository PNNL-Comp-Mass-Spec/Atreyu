using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Atreyu.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using ReactiveUI;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace Atreyu.ViewModels
{
    [Export]
    public class BasePeakIntensityViewModel : ReactiveObject
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
        /// The bpi plot model.
        /// </summary>
        private PlotModel bpiPlotModel;

        /// <summary>
        /// The uimf data.
        /// </summary>
        private UimfData uimfData;

        /// <summary>
        /// To determine if a file was loaded yet
        /// </summary>
        private bool _uimfLoaded;

        private int maxScan;
        private Visibility _bpiVisible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalIonChromatogramViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public BasePeakIntensityViewModel()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the bpi plot model.
        /// </summary>
        public PlotModel BpiPlotModel
        {
            get
            {
                return this.bpiPlotModel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.bpiPlotModel, value);
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


        private List<DataPoint> dataArray = new List<DataPoint>();
        private List<DataPoint> logArray = new List<DataPoint>();
        private bool _showLogData;
        private double _maxValue;
        private double timeFactor;

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
        /// The get bpi data.
        /// </summary>
        /// <returns>
        /// The dictionary of bpi data, keyed by scan.
        /// </returns>
        public IDictionary<int, double> GetBpiData()
        {
            return this.frameDictionary;
        }

        /// <summary>
        /// Gets the image of the bpi plot.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetBpiImage()
        {
            var stream = new MemoryStream();
            PngExporter.Export(
                this.BpiPlotModel, 
                stream, 
                (int)this.BpiPlotModel.Width, 
                (int)this.BpiPlotModel.Height, 
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

            timeFactor = uimfData.TenthsOfNanoSecondsPerBin/1000000000.0;
            //if (frameData == null)
            //{
                this.frameData = data;
            //}


            if (this.endScan == 0)
            {
                this.startScan = 0;
                this.endScan = frameData.GetLength(0);
            }

            this.frameDictionary = new Dictionary<int, double>();

            for (var i = 0; i < this.endScan - this.startScan; i++)
            {
                var index = i + this.startScan;
                for (var j = 0; j < this.frameData.GetLength(1); j++)
                {
                    if (this.frameDictionary.ContainsKey(index))
                    {
                        if (this.frameDictionary[index] < this.frameData[i, j])
                        {
                            this.frameDictionary[index] = this.frameData[i, j];
                        }
                        //this.frameDictionary[index] += this.frameData[i, j];
                    }
                    else
                    {
                        this.frameDictionary.Add(index, this.frameData[i, j]);
                    }
                }
            }

            this.dataArray.Clear();
            this.logArray.Clear();
            foreach (var d in this.frameDictionary)
            {
                this.dataArray.Add(new DataPoint(d.Key * timeFactor, d.Value));
                this.logArray.Add(new DataPoint(d.Key * timeFactor, Math.Log10(d.Value)));
            }

            this.BpiPlotModel.InvalidatePlot(true);
            this.ShowLogData = false;
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
            if (this.BpiPlotModel != null)
            {
                return;
            }

            this.BpiPlotModel = new PlotModel();
            var linearAxis = new LinearAxis
                                 {
                                     Position = AxisPosition.Bottom, 
                                     AbsoluteMinimum = 0, 
                                     IsPanEnabled = false, 
                                     IsZoomEnabled = false, 
                                     Title = "Seconds", 
                                     MinorTickSize = 0
                                 };
            this.BpiPlotModel.Axes.Add(linearAxis);

            var linearYAxis = new LinearAxis
                                  {
                                      IsZoomEnabled = false, 
                                      AbsoluteMinimum = 0, 
                                      MinimumPadding = 0.1, 
                                      MaximumPadding = 0.1, 
                                      IsPanEnabled = false,
                                      IsAxisVisible = false
                                  };

            this.BpiPlotModel.Axes.Add(linearYAxis);
            var series = new LineSeries { Color = OxyColors.Black, };

            this.BpiPlotModel.Series.Add(series);
        }

        #endregion
        
        public Visibility BpiVisible
        {
            get { return _bpiVisible; }
            set { this.RaiseAndSetIfChanged(ref this._bpiVisible, value); }
        }

        public bool ShowLogData
        {
            get { return _showLogData; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._showLogData, value);
                UpdatePlotData();
            }
        }

        private void UpdatePlotData()
        {
            var series = this.BpiPlotModel.Series[0] as LineSeries;
            series.Points.RemoveRange(0, series.Points.Count);
            MaxValue = 0;
            var data = new List<DataPoint>();
            if (ShowLogData)
            {
                data = logArray;
            }
            else
            {
                data = dataArray;
            }
            foreach (var point in data)
            {
                series.Points.Add(point);
                if (MaxValue < point.Y)
                    MaxValue = point.Y;
            }
            this.BpiPlotModel.InvalidatePlot(true);
        }

        public double MaxValue { get { return _maxValue; } set { this.RaiseAndSetIfChanged(ref _maxValue, value); } }
    }
}
