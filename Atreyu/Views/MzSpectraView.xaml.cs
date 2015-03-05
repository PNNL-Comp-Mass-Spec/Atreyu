// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MzSpectraView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MzSpectraView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Views
{
    using System.ComponentModel.Composition;

    using Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for MzSpectraView.xaml
    /// </summary>
    [Export]
    public partial class MzSpectraView
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