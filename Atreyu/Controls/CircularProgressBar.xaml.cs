// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularProgressBar.xaml.cs" company="Pacific Northwest National Laboratory">
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
//   Interaction logic for CircularProgressBar.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for CircularProgressBar.xaml
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        #region Fields

        /// <summary>
        /// TODO The animation timer.
        /// </summary>
        private readonly DispatcherTimer animationTimer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularProgressBar"/> class.
        /// </summary>
        public CircularProgressBar()
        {
            this.InitializeComponent();

            this.animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, this.Dispatcher);
            this.animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 75);
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The handle animation tick.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void HandleAnimationTick(object sender, EventArgs e)
        {
            this.SpinnerRotate.Angle = (this.SpinnerRotate.Angle + 36) % 360;
        }

        /// <summary>
        /// TODO The handle loaded.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            this.SetPosition(this.C0, offset, 0.0, step);
            this.SetPosition(this.C1, offset, 1.0, step);
            this.SetPosition(this.C2, offset, 2.0, step);
            this.SetPosition(this.C3, offset, 3.0, step);
            this.SetPosition(this.C4, offset, 4.0, step);
            this.SetPosition(this.C5, offset, 5.0, step);
            this.SetPosition(this.C6, offset, 6.0, step);
            this.SetPosition(this.C7, offset, 7.0, step);
            this.SetPosition(this.C8, offset, 8.0, step);
        }

        /// <summary>
        /// TODO The handle unloaded.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        /// <summary>
        /// TODO The handle visible changed.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void HandleVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            if (isVisible)
            {
                this.Start();
            }
            else
            {
                this.Stop();
            }
        }

        /// <summary>
        /// TODO The set position.
        /// </summary>
        /// <param name="ellipse">
        /// TODO The ellipse.
        /// </param>
        /// <param name="offset">
        /// TODO The offset.
        /// </param>
        /// <param name="posOffSet">
        /// TODO The pos off set.
        /// </param>
        /// <param name="step">
        /// TODO The step.
        /// </param>
        private void SetPosition(Ellipse ellipse, double offset, double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0 + Math.Sin(offset + posOffSet * step) * 50.0);

            ellipse.SetValue(Canvas.TopProperty, 50 + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        /// <summary>
        /// TODO The start.
        /// </summary>
        private void Start()
        {
            ////Mouse.OverrideCursor = Cursors.Wait;
            this.animationTimer.Tick += this.HandleAnimationTick;
            this.animationTimer.Start();
        }

        /// <summary>
        /// TODO The stop.
        /// </summary>
        private void Stop()
        {
            this.animationTimer.Stop();
            Mouse.OverrideCursor = Cursors.Arrow;
            this.animationTimer.Tick -= this.HandleAnimationTick;
        }

        #endregion
    }
}