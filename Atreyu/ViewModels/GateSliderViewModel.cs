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
    using ReactiveUI;

    /// <summary>
    /// TODO The gate slider view model.
    /// </summary>
    public class GateSliderViewModel : ReactiveObject
    {
        #region Fields

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

            set
            {
                this.RaiseAndSetIfChanged(ref this.gate, value);
            }
        }

        #endregion
    }
}