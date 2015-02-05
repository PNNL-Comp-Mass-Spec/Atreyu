// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Viewer.ViewModels;

namespace Viewer
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows;

    using Microsoft.Win32;

    using ReactiveUI;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel>
    {
        #region Fields
     
        public MainWindowViewModel ViewModel { get; set; }

        #endregion

        #region Properies

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = value as MainWindowViewModel; }
        }

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

            // Explicitly binding the content of CombinedHeatMapViewControl to the to CombinedHeatmapViewModel in the MainWindow model 
            this.Bind(this.ViewModel, vm => vm.CombinedHeatmapViewModel, v => v.CombinedHeatMapViewControl.Content);
          
            ////this.AllowDrop = true;
            ////this.PreviewDrop += this.MainTabControl_PreviewDragEnter;
        }

        #endregion

    }
}