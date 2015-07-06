using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Atreyu.Controls
{
    /// <summary>
    /// Interaction logic for MzRange.xaml
    /// </summary>
    public partial class MzRange : UserControl
    {
        public MzRange()
        {
            InitializeComponent();
            RootPanel.DataContext = this;
        }


        public bool MzRangeEnabled
        {
            get { return (bool)GetValue(MzRangeEnabledProperty); }
            set { this.SetValue(MzRangeEnabledProperty, value); }
        }

        public static readonly DependencyProperty MzRangeEnabledProperty = DependencyProperty.Register(
            "MzRangeEnabled",
            typeof(bool),
            typeof(MzRange),
            new UIPropertyMetadata(false));

        public double MzCenter
        {
            get { return (double)GetValue(MzCenterProperty); }
            set { this.SetValue(MzCenterProperty, value); }
        }

        public static readonly DependencyProperty MzCenterProperty = DependencyProperty.Register(
            "MzCenter",
            typeof(double),
            typeof(MzRange),
            new UIPropertyMetadata(1000.00));

        public double PartsPerMillion
        {
            get { return (double)GetValue(PartsPerMillionProperty); }
            set { this.SetValue(PartsPerMillionProperty, value); }
        }

        public static readonly DependencyProperty PartsPerMillionProperty = DependencyProperty.Register(
            "PartsPerMillion",
            typeof(double),
            typeof(MzRange),
            new UIPropertyMetadata(150.0));

    }
}
