using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace Atreyu.Views
{
    using System.ComponentModel.Composition;

    using Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for TotalIonChromatogramView.xaml
    /// </summary>
    public partial class TotalIonChromatogramView : UserControl, IViewFor<TotalIonChromatogramViewModel>
    {
        #region Constructors and Destructors

        public TotalIonChromatogramView()
        {
            this.InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, view => view.DataContext);
            this.Bind(this.ViewModel, model => model.TicPlotModel, view => view.TicPlot.Model);
        }

        #endregion

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TotalIonChromatogramViewModel), typeof(TotalIonChromatogramView));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = value as TotalIonChromatogramViewModel; }
        }

        public TotalIonChromatogramViewModel ViewModel
        {
            get => (TotalIonChromatogramViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}