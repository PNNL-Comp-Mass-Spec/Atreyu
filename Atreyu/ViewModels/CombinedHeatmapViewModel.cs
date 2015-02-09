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
    using System.Drawing;
    using System.Reactive.Linq;

    using ReactiveUI;

    /// <summary>
    /// TODO The combined heatmap view model.
    /// </summary>
    [Export]
    public class CombinedHeatmapViewModel : ReactiveObject
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
            this.LowValueGateSliderViewModel = new GateSliderViewModel();
            this.HighValueGateSliderViewModel = new GateSliderViewModel();
            this.TotalIonChromatogramViewModel = new TotalIonChromatogramViewModel();

            this.LowValueGateSliderViewModel.ControlLabel = "Low Gate";
            this.LowValueGateSliderViewModel.UpdateGate(0);
            this.HighValueGateSliderViewModel.ControlLabel = "High Cutoff";
            this.HighValueGateSliderViewModel.UpdateGate(this.HighValueGateSliderViewModel.MaximumValue);

            this.ZoomOutFull = this.FrameManipulationViewModel.ZoomOutCommand;
            this.ZoomOutFull.Subscribe(x => this.HeatMapViewModel.ZoomOutFull());

            // update the uimf data for the various components
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData)
                .Subscribe(this.TotalIonChromatogramViewModel.UpdateReference);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData)
                .Subscribe(this.FrameManipulationViewModel.UpdateUimf);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData).Subscribe(this.MzSpectraViewModel.UpdateReference);

            // update the frame data of the TIC plot when needed
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.GatedFrameData)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(this.TotalIonChromatogramViewModel.UpdateFrameData);

            // Update the Framedata of the M/Z plot when needed
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.GatedFrameData)
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

            // This makes the axis of the mz plot be in mz mode properly
            this.WhenAnyValue(vm => vm.FrameManipulationViewModel.MzModeEnabled)
                .Subscribe(b => this.MzSpectraViewModel.ShowMz = b);

            ////this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.MzData).Where(i => this.FrameManipulationViewModel.MzModeEnabled)
            ////    .Subscribe(this.MzSpectraViewModel.UpdateFrameData);

            // These two allow the mz calculation to be done on the fly so it is fast, instead of calling the hard drive again 
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameSlope)
                .Where(b => this.HeatMapViewModel.HeatMapData != null)
                .Subscribe(d => this.MzSpectraViewModel.Slope = d);

            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameIntercept)
                .Where(b => this.HeatMapViewModel.HeatMapData != null)
                .Subscribe(d => this.MzSpectraViewModel.Intercept = d);

            // Attach the heatmap threshold to the slider's gate, using Throttle so it doesn't seem jerky.
            this.WhenAnyValue(vm => vm.LowValueGateSliderViewModel.LogarithmicGate)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(this.HeatMapViewModel.UpdateLowThreshold);

            this.WhenAnyValue(vm => vm.HighValueGateSliderViewModel.LogarithmicGate)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(this.HeatMapViewModel.UpdateHighThreshold);


            // Update the frame type on the Fram Manipulation view
            this.WhenAnyValue(vm => vm.HeatMapViewModel.HeatMapData.FrameType)
                .Subscribe(s => this.FrameManipulationViewModel.FrameType = s);
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

        public GateSliderViewModel LowValueGateSliderViewModel { get; private set; }

        public GateSliderViewModel HighValueGateSliderViewModel { get; private set; }
        /// <summary>
        /// Gets the total ion chromatogram view model.
        /// </summary>
        public TotalIonChromatogramViewModel TotalIonChromatogramViewModel { get; private set; }

        public ReactiveCommand<object> ZoomOutFull { get; private set; }

        #endregion

        public Image GetImage()
        {
            var tic = this.TotalIonChromatogramViewModel.GetTicImage();
            var mz = this.MzSpectraViewModel.GetMzImage();
            var heatmap = this.HeatMapViewModel.GetHeatmapImage();
            var alignment = 25;

            var bitmap = new Bitmap(mz.Width + heatmap.Width, heatmap.Height + tic.Height + alignment);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(mz, 0, 0);
                g.DrawImage(heatmap, mz.Width, alignment);
                g.DrawImage(tic, mz.Width, heatmap.Height + alignment);
            }

            return bitmap;
        }
    }
}