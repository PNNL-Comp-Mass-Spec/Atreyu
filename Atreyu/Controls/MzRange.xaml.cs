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