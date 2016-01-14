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
        /// The animation timer.
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

            this.animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, this.Dispatcher)
                                      {
                                          Interval =
                                              new TimeSpan
                                              (
                                              0, 
                                              0, 
                                              0, 
                                              0, 
                                              75)
                                      };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the animation tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleAnimationTick(object sender, EventArgs e)
        {
            this.SpinnerRotate.Angle = (this.SpinnerRotate.Angle + 36) % 360;
        }

        /// <summary>
        /// The handle loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double Offset = Math.PI;
            const double Step = Math.PI * 2 / 10.0;

            this.SetPosition(this.C0, Offset, 0.0, Step);
            this.SetPosition(this.C1, Offset, 1.0, Step);
            this.SetPosition(this.C2, Offset, 2.0, Step);
            this.SetPosition(this.C3, Offset, 3.0, Step);
            this.SetPosition(this.C4, Offset, 4.0, Step);
            this.SetPosition(this.C5, Offset, 5.0, Step);
            this.SetPosition(this.C6, Offset, 6.0, Step);
            this.SetPosition(this.C7, Offset, 7.0, Step);
            this.SetPosition(this.C8, Offset, 8.0, Step);
        }

        /// <summary>
        /// The handle unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        /// <summary>
        /// The handle visible changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;

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
        /// The set position.
        /// </summary>
        /// <param name="ellipse">
        /// The ellipse.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="posOffSet">
        /// The position offset.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        private void SetPosition(Ellipse ellipse, double offset, double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0 + Math.Sin(offset + posOffSet * step) * 50.0);

            ellipse.SetValue(Canvas.TopProperty, 50.0 + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        private void Start()
        {
            ////Mouse.OverrideCursor = Cursors.Wait;
            this.animationTimer.Tick += this.HandleAnimationTick;
            this.animationTimer.Start();
        }

        /// <summary>
        /// Stops the animation.
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