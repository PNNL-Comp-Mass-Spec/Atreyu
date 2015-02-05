// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GateSlider.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for GateSlider.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.Views
{
    using System.Windows.Controls;

    using Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for GateSlider.xaml
    /// </summary>
    public partial class GateSlider : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GateSlider"/> class.
        /// </summary>
        public GateSlider()
            : this(new GateSliderViewModel())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GateSlider"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// TODO The view model.
        /// </param>
        public GateSlider(GateSliderViewModel viewModel)
        {
            this.InitializeComponent();
            this.ViewModel = viewModel;
            this.DataContext = this.ViewModel;

            this.GateSliderControl.ValueChanged += (sender, args) => ViewModel.UpdateGate(args.NewValue);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public GateSliderViewModel ViewModel { get; set; }

        #endregion
    }
}