// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameManipulationView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for FrameManipulationView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Falkor.Views.Atreyu
{
    using System.Windows;
    using System.Windows.Controls;

    using global::Atreyu.ViewModels;

    using Microsoft.Practices.Prism.Mvvm;
    using System.ComponentModel.Composition;
    /// <summary>
    /// Interaction logic for FrameManipulationView.xaml
    /// </summary>
    [Export]
    public partial class FrameManipulationView : UserControl, IView
    {
        #region Fields

        /// <summary>
        /// TODO The _view model.
        /// </summary>
        private readonly FrameManipulationViewModel _viewModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameManipulationView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// TODO The view model.
        /// </param>
        [ImportingConstructor]
        public FrameManipulationView(FrameManipulationViewModel viewModel)
        {
            this._viewModel = viewModel;
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The end frame text box_ text changed.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void EndFrameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;

            if (box != null)
            {
                int value;

                if (int.TryParse(box.Text, out value))
                {
                    this._viewModel.EndFrame = value;
                }
            }
        }

        /// <summary>
        /// TODO The slider_ value changed.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > 0)
            {
                this._viewModel.UpdateFrameNumber((int)e.NewValue);
            }
        }

        /// <summary>
        /// TODO The start frame text box_ text changed.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void StartFrameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;

            if (box != null)
            {
                int value;

                if (int.TryParse(box.Text, out value))
                {
                    this._viewModel.StartFrame = value;
                }
            }
        }

        #endregion
    }
}