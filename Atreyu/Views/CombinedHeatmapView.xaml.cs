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

namespace Atreyu.Views
{
    using Atreyu.ViewModels;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for CombinedHeatmapView.xaml
    /// </summary>
    public partial class CombinedHeatmapView : UserControl, IViewFor<CombinedHeatmapViewModel>
    {
        public CombinedHeatmapView() : this(new CombinedHeatmapViewModel())
        {
        }


        public CombinedHeatmapView(CombinedHeatmapViewModel viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;
        }

        object IViewFor.ViewModel
        {
            get
            {
                return this.ViewModel;
            }
            set
            {
                this.ViewModel = value as CombinedHeatmapViewModel;
            }
        }

        public CombinedHeatmapViewModel ViewModel { get; set; }
    }
}
