using System;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ReactiveUI;

namespace Atreyu.Views
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;

    using Atreyu.ViewModels;

    /// <summary>
    /// Interaction logic for FrameManipulationView
    /// </summary>
    public partial class FrameManipulationView : UserControl, IViewFor<FrameManipulationViewModel>
    {

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameManipulationView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        public FrameManipulationView()
        {
            this.InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, view => view.DataContext);
            this.FrameSlider.Events().MouseUp.Select(x => this.FrameSlider.Value).Where(x => x > 0).Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(args =>
            {
                this.ViewModel.CurrentFrame = (int)this.FrameSlider.Value;
            });
        }

        #endregion

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = value as FrameManipulationViewModel; }
        }

        public FrameManipulationViewModel ViewModel
        {
            get => (FrameManipulationViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(FrameManipulationViewModel), typeof(FrameManipulationView));
    }
}