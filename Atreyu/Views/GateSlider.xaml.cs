using System.ComponentModel.Composition;
using System.Windows.Controls;
using ReactiveUI;

namespace Atreyu.Views
{
    using Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for GateSlider.xaml
    /// </summary>
    public partial class GateSlider : UserControl, IViewFor<GateSliderViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GateSlider"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        [ImportingConstructor]
        public GateSlider()
        {
            this.InitializeComponent();

           

            this.GateSliderControl.ValueChanged += (sender, args) => this.ViewModel.UpdateGate(args.NewValue);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public GateSliderViewModel ViewModel { get; set; }

        #endregion

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = value as GateSliderViewModel; }
        }
    }
}