using System.ComponentModel.Composition;

namespace Atreyu.Views
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Atreyu.ViewModels;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for CombinedHeatmapView
    /// </summary>
    public partial class CombinedHeatmapView : UserControl, IViewFor<CombinedHeatmapViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapView"/> class.
        /// </summary>
        public CombinedHeatmapView()
        {
            this.InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, view => view.DataContext);
            this.Bind(this.ViewModel, model => model.FrameManipulationViewModel,
                view => view.FrameManipulationViewHost.ViewModel);
            this.Bind(this.ViewModel, model => model.HeatMapViewModel,
                view => view.HeatMapViewHost.ViewModel);
            this.Bind(this.ViewModel, model => model.TotalIonChromatogramViewModel,
                view => view.TotalIonChromatogramViewHost.ViewModel);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public CombinedHeatmapViewModel ViewModel
        {
            get { return (CombinedHeatmapViewModel) GetValue(ViewModelProperty); }
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

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
                this.ViewModel = value as CombinedHeatmapViewModel;
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(CombinedHeatmapViewModel), typeof(CombinedHeatmapView));
    }
}