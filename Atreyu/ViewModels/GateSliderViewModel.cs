// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GateSliderViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The gate slider view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.ViewModels
{
    using System;

    using ReactiveUI;

    /// <summary>
    /// TODO The gate slider view model.
    /// </summary>
    public class GateSliderViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// TODO The control label.
        /// </summary>
        private string controlLabel = "Low Gate";

        /// <summary>
        /// TODO The gate.
        /// </summary>
        private double gate;

        /// <summary>
        /// TODO The log mode.
        /// </summary>
        private bool logMode;

        /// <summary>
        /// TODO The logarithmic gate.
        /// </summary>
        private double logarithmicGate;

        /// <summary>
        /// TODO The maximum log value.
        /// </summary>
        private double maximumLogValue = 10000000.0;

        /// <summary>
        /// TODO The maximum value.
        /// </summary>
        private double maximumValue = 100000.0;

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
                return this.logarithmicGate;
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

        #region Public Methods and Operators

        /// <summary>
        /// TODO The update gate.
        /// </summary>
        /// <param name="value">
        /// TODO The value.
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