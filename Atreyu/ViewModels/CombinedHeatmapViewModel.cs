namespace Atreyu.ViewModels
{
    using System.ComponentModel.Composition;

    [Export]
    public class CombinedHeatmapViewModel
    {

        public CombinedHeatmapViewModel()
        {
            
        }


        /// <summary>
        /// TODO The frame manipulation view model.
        /// </summary>
        public FrameManipulationViewModel frameManipulationViewModel { get; private set; }

        /// <summary>
        /// TODO The heat map view model.
        /// </summary>
        public HeatMapViewModel heatMapViewModel { get; private set; }

        /// <summary>
        /// TODO The mz spectra view model.
        /// </summary>
        public MzSpectraViewModel mzSpectraViewModel { get; private set; }

        /// <summary>
        /// TODO The total ion chromatogram view model.
        /// </summary>
        public TotalIonChromatogramViewModel totalIonChromatogramViewModel { get; private set; }

    }
}