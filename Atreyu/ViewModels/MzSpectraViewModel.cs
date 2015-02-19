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
        /// TODO The mz frame data.
        /// </summary>
        private Dictionary<double, double> mzFrameData;

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
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the bin to mz map.
        /// </summary>
        public double[] BinToMzMap { get; set; }

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

        /// <summary>
        /// Gets or sets the values per pixel y.
        /// </summary>
        public double ValuesPerPixelY { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The get mz data compressed.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<double, double> GetMzDataCompressed()
        {
            return this.mzFrameData;
        }

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
        /// The frame data.
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

            if (this.BinToMzMap == null)
            {
                return;
            }

            this._frameData = framedata;
            var frameData = new Dictionary<double, double>();
            this.mzFrameData = new Dictionary<double, double>();

            for (var j = 0; j < this._frameData.GetLength(1); j++)
            {
                double index = j + this._startMzBin;
                var mzIndex = this.BinToMzMap[j];

                for (var i = 0; i < this._frameData.GetLength(0); i++)
                {
                    if (frameData.ContainsKey(index))
                    {
                        frameData[index] += this._frameData[i, j];
                    }
                    else
                    {
                        frameData.Add(index, this._frameData[i, j]);
                    }

                    if (this.mzFrameData.ContainsKey(mzIndex))
                    {
                        this.mzFrameData[mzIndex] += this._frameData[i, j];
                    }
                    else
                    {
                        this.mzFrameData.Add(mzIndex, this._frameData[i, j]);
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
                    foreach (var d in frameData)
                    {
                        series.Points.Add(new DataPoint(d.Value, d.Key));
                    }
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
            this.CreatePlotModel();
        }

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
    }
}