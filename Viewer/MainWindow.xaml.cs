// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Viewer
{
    using System.Windows;

    using ReactiveUI;

    using Viewer.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel>
    {
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

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public MainWindowViewModel ViewModel { get; set; }

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
                this.ViewModel = value as MainWindowViewModel;
            }
        }

        #endregion
    }
}