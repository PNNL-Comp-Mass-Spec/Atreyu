// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GateSliderViewModel.cs" company="Pacific Northwest National Laboratory">
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
//   The gate slider view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Media;

namespace Atreyu.ViewModels
{
    using System;

    using ReactiveUI;

    /// <summary>
    /// The gate slider view model.
    /// </summary>
    public class GateSliderViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The control label.
        /// </summary>
        private string controlLabel = "Low Gate";

        /// <summary>
        /// The gate.
        /// </summary>
        private double gate;

        /// <summary>
        /// The log mode.
        /// </summary>
        private bool logMode;

        /// <summary>
        /// The logarithmic gate.
        /// </summary>
        private double logarithmicGate;

        /// <summary>
        /// The maximum log value.
        /// </summary>
        private double maximumLogValue = 10000000.0;

        /// <summary>
        /// The maximum value.
        /// </summary>
        private double maximumValue = 7;

        private DoubleCollection _logScaleList; 

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the control label.
        /// </summary>
        public string ControlLabel
        {
            get
            {
                return this.controlLabel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.controlLabel, value);
            }
        }

        /// <summary>
        /// Gets or sets the gate.
        /// </summary>
        public double Gate
        {
            get
            {
                return this.gate;
            }

            set
            {
                this.gate = value;
                this.RaisePropertyChanged();
            }
        }

        public DoubleCollection LogScaleList
        {
            get { return _logScaleList; }
            set { this.RaiseAndSetIfChanged(ref this._logScaleList, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether log mode.
        /// </summary>
        public bool LogMode
        {
            get
            {
                return this.logMode;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.logMode, value);
            }
        }

        /// <summary>
        /// Gets the logarithmic gate.
        /// </summary>
        public double LogarithmicGate
        {
            get
            {
                return Math.Round(this.logarithmicGate*1000)/1000.0;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.logarithmicGate, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum log value.
        /// </summary>
        public double MaximumLogValue
        {
            get
            {
                return this.maximumLogValue;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.maximumLogValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public double MaximumValue
        {
            get
            {
                return this.maximumValue;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.maximumValue, value);
            }
        }

        #endregion

        public GateSliderViewModel()
        {
            LogScaleList = new DoubleCollection();
            for (int i = 1; Math.Pow(10, i) <= maximumLogValue; i++)
            {
                LogScaleList.Add(i);
                for (int j = 2; j < 10; j++)
                {
                    LogScaleList.Add(Math.Log10(j) + i);
                }
            }
        }

        #region Public Methods and Operators

        /// <summary>
        /// The update gate.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void UpdateGate(double value)
        {
            this.Gate = value;

            // position will be between 0 and whatever the Maximum is
            const int Minp = 0;
            var maxp = this.MaximumValue;

            // The result should be between 0 an whatever the maximum log value is
            const int Minv = 0;
            var maxv = Math.Log(this.MaximumLogValue);

            // calculate adjustment factor
            var scale = (maxv - Minv) / (maxp - Minp);

            // scale it all.
            var x = Math.Exp(Minv + (scale * (value - Minp)));

            this.LogarithmicGate = x;
        }

        #endregion
    }
}