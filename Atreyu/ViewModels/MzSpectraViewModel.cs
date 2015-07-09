// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MzSpectraViewModel.cs" company="Pacific Northwest National Laboratory">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Pacific Northwest National Laboratory
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// <summary>
//   The mz spectra view model.
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

    using Atreyu.Models;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Wpf;

    using ReactiveUI;

    using UIMFLibrary;

    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LineSeries = OxyPlot.Series.LineSeries;

    /// <summary>
    /// The view model for properly displaying the mz spectra graph.
    /// </summary>
    [Export]
    public class MzSpectraViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The end mz bin.
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private int endMzBin;

        /// <summary>
        /// The raw frame data that is compressed on the Y axis.
        /// </summary>
        private double[,] frameData;

        /// <summary>
        /// The bin centric frame data that has been compressed.
        /// </summary>
        private Dictionary<double, double> frameDictionary;

        /// <summary>
        /// The calibration intercept.
        /// </summary>
        private double intercept;

        /// <summary>
        /// The mz frame data that has been compressed.
        /// </summary>
        private Dictionary<double, double> mzFrameData;

        /// <summary>
        /// The plot model for the mz.
        /// </summary>
        private PlotModel mzPlotModel;

        /// <summary>
        /// Specifies whether or not to show the mz.
        /// </summary>
        private bool showMz;

        /// <summary>
        /// The calibration slope.
        /// </summary>
        private double slope;

        /// <summary>
        /// The start mz bin.
        /// </summary>
        private int startMzBin;

        /// <summary>
        /// The uimf data reference.
        /// </summary>
        private UimfData uimfData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MzSpectraViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public MzSpectraViewModel()
        {
            this.WhenAnyValue(vm => vm.ShowMz).Subscribe(b => this.UpdateFrameData(this.frameData));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the bin to mz map.
        /// </summary>
        public double[] BinToMzMap { get; set; }

        /// <summary>
        /// Gets or sets the mz calibrator.
        /// </summary>
        public MzCalibrator Calibrator { get; set; }

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
        /// Gets or sets the mz array.
        /// </summary>
        public double[] MzArray { get; set; }

        /// <summary>
        /// Gets or sets the mz intensities.
        /// </summary>
        public int[] MzIntensities { get; set; }

        /// <summary>
        /// Gets or sets the mz plot model.
        /// </summary>
        public PlotModel MzPlotModel
        {
            get
            {
                return this.mzPlotModel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.mzPlotModel, value);
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
        /// Changes the end bin for proper graph display.
        /// </summary>
        /// <param name="bin">
        /// The bin to change to.
        /// </param>
        public void ChangeEndBin(int bin)
        {
            this.endMzBin = bin;
        }

        /// <summary>
        /// Changes the start bin for proper graph display.
        /// </summary>
        /// <param name="bin">
        /// The bin to change to.
        /// </param>
        public void ChangeStartBin(int bin)
        {
            this.startMzBin = bin;
        }

        /// <summary>
        /// Creates or recreates the create plot model.
        /// </summary>
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
        /// Gets a dictionary of the m/z data that has been compressed.
        /// </summary>
        /// <returns>
        /// The of the current mz data, keyed on mz.
        /// </returns>
        public IDictionary<double, double> GetMzDataCompressed()
        {
            return this.mzFrameData;
        }

        /// <summary>
        /// Get the current image of the m/z graph.
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
        /// Update frame data.
        /// </summary>
        /// <param name="framedata">
        /// The frame data.
        /// </param>
        public void UpdateFrameData(double[,] framedata)
        {
            if (this.uimfData == null)
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

            this.frameData = framedata;
            this.frameDictionary = new Dictionary<double, double>();
            this.mzFrameData = new Dictionary<double, double>();

            for (var j = 0; j < this.frameData.GetLength(1); j++)
            {
                double index = j + this.startMzBin;
                var mzIndex = this.BinToMzMap[j];

                for (var i = 0; i < this.frameData.GetLength(0); i++)
                {
                    if (this.frameDictionary.ContainsKey(index))
                    {
                        this.frameDictionary[index] += this.frameData[i, j];
                    }
                    else
                    {
                        this.frameDictionary.Add(index, this.frameData[i, j]);
                    }

                    if (this.mzFrameData.ContainsKey(mzIndex))
                    {
                        this.mzFrameData[mzIndex] += this.frameData[i, j];
                    }
                    else
                    {
                        this.mzFrameData.Add(mzIndex, this.frameData[i, j]);
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
                    foreach (var d in this.frameDictionary)
                    {
                        series.Points.Add(new DataPoint(d.Value, d.Key));
                    }
                }
            }

            this.FindPeaks();

            this.MzPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Updates the reference to UIMF data.
        /// </summary>
        /// <param name="uimfDataNew">
        /// The uimf data that has been changed.
        /// </param>
        public void UpdateReference(UimfData uimfDataNew)
        {
            if (uimfDataNew == null)
            {
                return;
            }

            this.uimfData = uimfDataNew;
            this.CreatePlotModel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find the peaks in the current data set and adds an annotation point with the resolution to the m/z.
        /// </summary>
        private void FindPeaks()
        {
            this.mzPlotModel.Annotations.Clear();

            if (!this.ShowMz)
            {
                // This currently does not support peakfinding in bin mode due to potential headaches,
                // But as it seems users never use that mode, it is a non-issue for now.
                return;
            }

            // Create a new dictionary to work with the peak finder
            var tempFrameList = new List<KeyValuePair<double, double>>(this.uimfData.MaxBins);

            for (var i = 0; i < this.MzArray.Length && i < this.MzIntensities.Length; i++)
            {
                tempFrameList.Add(new KeyValuePair<double, double>(this.MzArray[i], this.MzIntensities[i]));
            }

            var tempList = Utilities.PeakFinder.FindPeaks(tempFrameList, 3);

            // Put the points on the graph
            foreach (var resolutionDatapoint in tempList.Peaks)
            {
                var resolutionString = resolutionDatapoint.ResolvingPower.ToString("F1", CultureInfo.InvariantCulture);

                var peakPoint = new OxyPlot.Annotations.PointAnnotation
                                    {
                                        Text = "R=" + resolutionString, 
                                        X = resolutionDatapoint.Intensity / 1.03125, 
                                        Y = resolutionDatapoint.PeakCenter, 
                                        ToolTip = resolutionDatapoint.ToString()
                                    };
                this.mzPlotModel.Annotations.Add(peakPoint);
            }
        }

        #endregion
    }
}