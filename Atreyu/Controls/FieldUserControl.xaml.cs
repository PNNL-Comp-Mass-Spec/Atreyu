// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldUserControl.xaml.cs" company="Pacific Northwest National Laboratory">
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
//   Interaction logic for FieldUserControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FieldUserControl.xaml
    /// </summary>
    public partial class FieldUserControl : UserControl
    {
        #region Static Fields

        /// <summary>
        /// TODO The label property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", 
            typeof(string), 
            typeof(FieldUserControl), 
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// TODO The value property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", 
            typeof(object), 
            typeof(FieldUserControl), 
            new PropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldUserControl"/> class.
        /// </summary>
        public FieldUserControl()
        {
            this.InitializeComponent();
            this.LayoutRoot.DataContext = this;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label
        {
            get
            {
                return (String)this.GetValue(LabelProperty);
            }

            set
            {
                this.SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value
        {
            get
            {
                return (object)this.GetValue(ValueProperty);
            }

            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        #endregion
    }
}