using System.Windows;
using System.Windows.Controls;

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
    public partial class HeatMapView : UserControl, IViewFor<HeatMapViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>

        public HeatMapView()
        {
            this.InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, view => view.DataContext);
            this.Bind(this.ViewModel, model => model.HeatMapPlotModel, view => view.HeatMapPlot.Model);
            this.HeatMapPlot.Events().SizeChanged.Throttle(TimeSpan.FromMilliseconds(100)).ObserveOn(RxApp.MainThreadScheduler).Subscribe(args =>
            {
                ViewModel.Size = (args.NewSize.Height, args.NewSize.Width);
            });

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public HeatMapViewModel ViewModel
        {
            get => (HeatMapViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

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

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(HeatMapViewModel), typeof(HeatMapView));

        #endregion
    }
}