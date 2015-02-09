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

        private double logarithmicGate;

        /// <summary>
        /// TODO The gate.
        /// </summary>
        private double gate;

        private bool logMode;

        private double maximumLogValue = 10000000.0;

        private double maximumValue = 100000.0;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the gate.
        /// </summary>
        public double Gate
        {
            get
            {
                return this.gate;
            }

            private set
            {
                this.gate = value;
                this.RaisePropertyChanged();
            }
        }

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

        #endregion

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
    }
}