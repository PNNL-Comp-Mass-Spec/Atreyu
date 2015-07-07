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
    /// Interaction logic for FieldUserControl.xaml
    /// </summary>
    public partial class FieldUserControl : UserControl
    {
        public FieldUserControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label",
            typeof(string),
            typeof(FieldUserControl),
            new PropertyMetadata(""));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(FieldUserControl),
            new PropertyMetadata(null));
    }
}
