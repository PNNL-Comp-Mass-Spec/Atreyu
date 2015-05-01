// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeatMapView.xaml.cs" company="Pacific Northwest National Laboratory">
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
//   Interaction logic for HeatMapView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Views
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reactive.Linq;

    using Atreyu.ViewModels;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for HeatMapView.xaml
    /// </summary>
    [Export]
    public partial class HeatMapView : IViewFor<HeatMapViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        [ImportingConstructor]
        public HeatMapView(HeatMapViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.DataContext = this.ViewModel;
            this.InitializeComponent();

            // x and y are magically is assigned "this" via extension methods
            this.WhenAnyValue(x => x.ActualHeight, y => y.ActualWidth)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(
                    z =>
                        {
                            viewModel.Height = (int)z.Item1;
                            viewModel.Width = (int)z.Item2;
                        });
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public HeatMapViewModel ViewModel { get; set; }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        object IViewFor.ViewModel
        {
            get
            {
                return this.ViewModel;
            }

            set
            {
                this.ViewModel = value as HeatMapViewModel;
            }
        }

        #endregion
    }
}