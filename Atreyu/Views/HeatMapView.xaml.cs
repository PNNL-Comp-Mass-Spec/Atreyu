// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeatMapView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for HeatMapView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reactive.Linq;

namespace Falkor.Views.Atreyu
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using ReactiveUI;

    using global::Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for HeatMapView.xaml
    /// </summary>
    [Export]
    public partial class HeatMapView : UserControl, IViewFor<HeatMapViewModel>
    {


 		public HeatMapViewModel ViewModel { get; set; }
 
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = value as HeatMapViewModel; }
        }       
		       
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
            this.WhenAnyValue(x => x.ActualHeight, y => y.ActualWidth).Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(x => this.ViewModel.UpdatePlotSize(x.Item1, x.Item2));
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
            this.ViewModel.InitializeUimfData(files[0]);
        }

       
        #endregion

       
    }
}