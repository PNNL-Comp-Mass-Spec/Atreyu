using Atreyu.ViewModels;
using Atreyu.Views;
using ReactiveUI;
using Splat;

namespace Viewer
{
    public class AppBootstrapper
    {
        public AppBootstrapper()
        {
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
            Locator.CurrentMutable.Register(() => new CombinedHeatmapView(), typeof(IViewFor<CombinedHeatmapViewModel>));
            Locator.CurrentMutable.Register(() => new FrameManipulationView(), typeof(IViewFor<FrameManipulationViewModel>));
            Locator.CurrentMutable.Register(() => new HeatMapView(), typeof(IViewFor<HeatMapViewModel>));
        }
    }
}