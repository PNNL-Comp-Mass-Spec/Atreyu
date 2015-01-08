// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalIonChromatogramView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for TotalIonChromatogramView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Falkor.Views.Atreyu
{
    using System.ComponentModel.Composition;
    using System.Windows.Controls;

    using global::Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for TotalIonChromatogramView.xaml
    /// </summary>
    [Export]
    public partial class TotalIonChromatogramView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalIonChromatogramView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// TODO The view model.
        /// </param>
        [ImportingConstructor]
        public TotalIonChromatogramView(TotalIonChromatogramViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        #endregion
    }
}