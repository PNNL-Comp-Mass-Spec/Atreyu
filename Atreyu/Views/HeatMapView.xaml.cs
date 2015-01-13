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
    using global::Atreyu.ViewModels;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for HeatMapView.xaml
    /// </summary>
    [Export]
    public partial class HeatMapView : UserControl
    {
        #region Fields

        /// <summary>
        /// TODO The _view model.
        /// </summary>
        private readonly HeatMapViewModel _viewModel;

        #endregion

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
            this._viewModel = viewModel;
            this.DataContext = this._viewModel;
            this.InitializeComponent();
            this.HeatMapPlot.SizeChanged += this.HeatMapView_SizeChanged;
            this.HeatMapPlot.AllowDrop = true;
            this.HeatMapPlot.Drop += (sender, args) => this.OnDrop(args);

            // this.HandleFileOpen(new string[] { "test.uimf" });

            //this._viewModel.InitializeUimfData("test.uimf");

        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The on drop.
        /// </summary>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                this.HandleFileOpen(files);
            }

            e.Handled = true;
        }

        /// <summary>
        /// TODO The handle file open.
        /// </summary>
        /// <param name="files">
        /// TODO The files.
        /// </param>
        private void HandleFileOpen(string[] files)
        {
            this._viewModel.InitializeUimfData(files[0]);
        }

        /// <summary>
        /// TODO The heat map view_ size changed.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void HeatMapView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                var height = (int)e.NewSize.Height;
                this._viewModel.UpdatePlotNewHeight(height);
            }
        }

        #endregion
    }
}