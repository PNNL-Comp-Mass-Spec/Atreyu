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

        #endregion

        public void UpdateGate(double value)
        {
            this.Gate = value;
            // position will be between 0 and 100000
            var minp = 0;
            var maxp = 100000;

            // The result should be between 0 an 10000000
            var minv = 0;
            var maxv = Math.Log(10000000);

            // calculate adjustment factor
            var scale = (maxv - minv) / (maxp - minp);

            var x = Math.Exp(minv + (scale * (value - minp)));

            this.LogarithmicGate = x;
        }
    }
}