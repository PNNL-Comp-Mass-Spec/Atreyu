// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeatMapView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for HeatMapView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Falkor.Views.Atreyu
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using global::Atreyu.ViewModels;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for HeatMapView.xaml
    /// </summary>
    [Export]
    public partial class HeatMapView : UserControl, IViewFor<HeatMapViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// TODO The view model.
        /// </param>
        [ImportingConstructor]
        public HeatMapView(HeatMapViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.DataContext = this.ViewModel;
            this.InitializeComponent();

            // x and y are magically is assigned "this" via extension methods
            this.WhenAnyValue(x => x.ActualHeight, y => y.ActualWidth)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(
                    z =>
                        {
                            viewModel.Height = (int)z.Item1;
                            viewModel.Width = (int)z.Item2;
                        });
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public HeatMapViewModel ViewModel { get; set; }

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

        #endregion

        #region Methods

        /// <summary>
        /// TODO The on drop.
        /// </summary>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        protected override async void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                await this.HandleFileOpen(files);
            }

            e.Handled = true;
        }

        /// <summary>
        /// TODO The handle file open.
        /// </summary>
        /// <param name="files">
        /// TODO The files.
        /// </param>
        private async Task HandleFileOpen(string[] files)
        {
            await this.ViewModel.InitializeUimfData(files[0]);
        }

        #endregion
    }
}