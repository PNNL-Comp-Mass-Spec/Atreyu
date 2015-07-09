// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalIonChromatogramViewModel.cs" company="Pacific Northwest National Laboratory">
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
//   The total ion chromatogram view model.
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
    using System.Linq;

    using Atreyu.Models;

    using MagnitudeConcavityPeakFinder;

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
        /// The change end scan.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void ChangeEndScan(int value)
        {
            this.endScan = value;
        }

        /// <summary>
        /// The change start scan.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void ChangeStartScan(int value)
        {
            this.startScan = value;
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
        /// The update reference.
        /// </summary>
        /// <param name="uimfDataNew">
        /// The new <see cref="UimfData"/> that is coming in.
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
    }
}