// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameManipulationView.xaml.cs" company="Pacific Northwest National Laboratory">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Pacific Northwest National Laboratory
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// <summary>
//   Interaction logic for FrameManipulationView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;

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
        /// The view model.
        /// </summary>
        private readonly FrameManipulationViewModel viewModel;
        private int interimFrame;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameManipulationView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        [ImportingConstructor]
        public FrameManipulationView(FrameManipulationViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.DataContext = viewModel;
            this.PreviewMouseUp += new MouseButtonEventHandler(UIElement_OnMouseUp);
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The end frame text box text changed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
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
        /// The slider value changed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > 0)
            {
                this.interimFrame = ((int)e.NewValue);
            }
        }

        /// <summary>
        /// The start frame text box text changed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
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

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.viewModel.UpdateCurrentFrameNumber(interimFrame);
        }
    }
}