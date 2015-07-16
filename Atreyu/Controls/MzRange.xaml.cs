// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MzRange.xaml.cs" company="Pacific Northwest National Laboratory">
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
//   Interaction logic for MzRange.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MzRange.xaml
    /// </summary>
    public partial class MzRange : UserControl
    {
        #region Static Fields

        /// <summary>
        /// The mz center property.
        /// </summary>
        public static readonly DependencyProperty MzCenterProperty = DependencyProperty.Register(
            "MzCenter", 
            typeof(double), 
            typeof(MzRange), 
            new UIPropertyMetadata(1000.00));

        /// <summary>
        /// The mz range enabled property.
        /// </summary>
        public static readonly DependencyProperty MzRangeEnabledProperty = DependencyProperty.Register(
            "MzRangeEnabled", 
            typeof(bool), 
            typeof(MzRange), 
            new UIPropertyMetadata(false));

        /// <summary>
        /// The parts per million property.
        /// </summary>
        public static readonly DependencyProperty PartsPerMillionProperty =
            DependencyProperty.Register(
                "PartsPerMillion", 
                typeof(double), 
                typeof(MzRange), 
                new UIPropertyMetadata(150.0));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MzRange"/> class.
        /// </summary>
        public MzRange()
        {
            this.InitializeComponent();
            this.RootPanel.DataContext = this;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the mz center.
        /// </summary>
        public double MzCenter
        {
            get
            {
                return (double)this.GetValue(MzCenterProperty);
            }

            set
            {
                this.SetValue(MzCenterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mz range enabled.
        /// </summary>
        public bool MzRangeEnabled
        {
            get
            {
                return (bool)this.GetValue(MzRangeEnabledProperty);
            }

            set
            {
                this.SetValue(MzRangeEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parts per million.
        /// </summary>
        public double PartsPerMillion
        {
            get
            {
                return (double)this.GetValue(PartsPerMillionProperty);
            }

            set
            {
                this.SetValue(PartsPerMillionProperty, value);
            }
        }

        #endregion
    }
}