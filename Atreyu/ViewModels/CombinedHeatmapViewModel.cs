// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedHeatmapViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The combined heatmap view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.ViewModels
{
    using System;
    using System.ComponentModel.Composition;

    using ReactiveUI;

    /// <summary>
    /// TODO The combined heatmap view model.
    /// </summary>
    [Export]
    public class CombinedHeatmapViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapViewModel"/> class.
        /// </summary>
        public CombinedHeatmapViewModel()
        {
            this.FrameManipulationViewModel = new FrameManipulationViewModel(); 
            this.HeatMapViewModel = new HeatMapViewModel();
            this.MzSpectraViewModel = new MzSpectraViewModel();
            this.TotalIonChromatogramViewModel = new TotalIonChromatogramViewModel();

            // update the uimf data for the various components
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData)
                .Subscribe(this.TotalIonChromatogramViewModel.UpdateReference);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData)
                .Subscribe(this.FrameManipulationViewModel.UpdateUimf);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData).Subscribe(this.MzSpectraViewModel.UpdateReference);

            // update the frame data of the TIC plot when needed
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameData)
                .Subscribe(this.TotalIonChromatogramViewModel.UpdateFrameData);

            // Update the Framedata of the M/Z plot when needed
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameData)
                .Subscribe(this.MzSpectraViewModel.UpdateFrameData);

            // update the frame whenever it is changed via the frame manipulation view
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.CurrentFrame)
                .Subscribe(this.HeatMapViewModel.UpdateFrameNumber);

            // hook up the frame summing feature
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.Range).Subscribe(this.HeatMapViewModel.SumFrames);

            // These make the axis on the TIC update properly
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.StartScan)
                .Subscribe(this.TotalIonChromatogramViewModel.ChangeStartScan);
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.EndScan)
                .Subscribe(this.TotalIonChromatogramViewModel.ChangeEndScan);

            // These make the axis on the mz plot update properly
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.CurrentMinBin)
                .Subscribe(this.MzSpectraViewModel.changeStartBin);
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.CurrentMaxBin)
                .Subscribe(this.MzSpectraViewModel.changeEndBin);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the frame manipulation view model.
        /// </summary>
        public FrameManipulationViewModel FrameManipulationViewModel { get; private set; }

        /// <summary>
        /// Gets the heat map view model.
        /// </summary>
        public HeatMapViewModel HeatMapViewModel { get; private set; }

        /// <summary>
        /// Gets the mz spectra view model.
        /// </summary>
        public MzSpectraViewModel MzSpectraViewModel { get; private set; }

        /// <summary>
        /// Gets the total ion chromatogram view model.
        /// </summary>
        public TotalIonChromatogramViewModel TotalIonChromatogramViewModel { get; private set; }

        #endregion
    }
}