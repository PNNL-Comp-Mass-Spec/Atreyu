// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MzSpectraView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MzSpectraView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Falkor.Views.Atreyu
{
    using System.ComponentModel.Composition;
    using System.Windows.Controls;

    using global::Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for MzSpectraView.xaml
    /// </summary>
    [Export]
    public partial class MzSpectraView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MzSpectraView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// TODO The view model.
        /// </param>
        [ImportingConstructor]
        public MzSpectraView(MzSpectraViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        #endregion
    }
}