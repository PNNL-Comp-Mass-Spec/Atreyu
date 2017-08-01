using System.Windows.Controls;
using ReactiveUI;

namespace Atreyu.Views
{
    using System.ComponentModel.Composition;

    using Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for MzSpectraView.xaml
    /// </summary>
    public partial class MzSpectraView : UserControl, IViewFor<MzSpectraViewModel>
    {
        #region Constructors and Destructors

        public MzSpectraView()
        {
            this.InitializeComponent();
        }

        #endregion

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = value as MzSpectraViewModel; }
        }

        public MzSpectraViewModel ViewModel { get; set; }
    }
}