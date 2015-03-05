// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameManipulationView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for FrameManipulationView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Views
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;

    using Atreyu.ViewModels;

    using Microsoft.Practices.Prism.Mvvm;

    /// <summary>
    /// Interaction logic for FrameManipulationView
    /// </summary>
    [Export]
    public partial class FrameManipulationView : IView
    {
        #region Fields

        /// <summary>
        /// TODO The _view model.
        /// </summary>
        private readonly FrameManipulationViewModel viewModel;

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
            this.viewModel = viewModel;
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
        private void EndFrameTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;

            if (box != null)
            {
                int value;

                if (int.TryParse(box.Text, out value))
                {
                    this.viewModel.EndFrame = value;
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
        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > 0)
            {
                this.viewModel.UpdateCurrentFrameNumber((int)e.NewValue);
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
        private void StartFrameTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;

            if (box != null)
            {
                int value;

                if (int.TryParse(box.Text, out value))
                {
                    this.viewModel.StartFrame = value;
                }
            }
        }

        #endregion
    }
}