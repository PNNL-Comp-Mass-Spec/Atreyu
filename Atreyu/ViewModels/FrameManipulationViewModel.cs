using System;
using System.Reactive;
using ReactiveUI.Legacy;

namespace Atreyu.ViewModels
{
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Atreyu.Models;


    using ReactiveUI;

    /// <summary>
    /// The frame manipulation view model.
    /// </summary>
    public class FrameManipulationViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The current frame.
        /// </summary>
        private int currentFrame;

        /// <summary>
        /// The frame type.
        /// </summary>
        private string frameType;

        /// <summary>
        /// The lowest frame number.
        /// </summary>
        private int minNumFrame;

        /// <summary>
        /// The mz mode enabled.
        /// </summary>
        private bool mzModeEnabled = true;

        /// <summary>
        /// The total number of frames.
        /// </summary>
        private int numFrames;

        private int _tickSize;

        /// <summary>
        /// The range.
        /// </summary>
        private Range<int> range;

        private int startFrame;

        private int endFrame;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameManipulationViewModel"/> class. 
        /// </summary>
        public FrameManipulationViewModel()
        {
            this.SumFramesCommand = ReactiveCommand.Create(SumFrames);

            this.ZoomOutCommand = ReactiveCommand.Create(() => {});
        }

        #endregion

        #region Public Properties

        public int TickSize
        {
            get { return (int)Math.Ceiling(NumFrames/20.0); }
        }

        public bool IsTickSizeOne
        {
            get { return (int) Math.Ceiling(NumFrames/20.0) == 1; }
        }

        /// <summary>
        /// Gets or sets the current frame.
        /// </summary>
        public int CurrentFrame
        {
            get
            {
                return this.currentFrame;
            }

            set
            {
                 this.RaiseAndSetIfChanged(ref this.currentFrame, value);
            }
        }

        /// <summary>
        /// Gets or sets the end frame.
        /// </summary>
        public int EndFrame
        {
            get => this.endFrame;
            set => this.RaiseAndSetIfChanged(ref this.endFrame, value);
        }

        /// <summary>
        /// Gets or sets the frame type.
        /// </summary>
        public string FrameType
        {
            get
            {
                return this.frameType;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.frameType, value);
            }
        }

        /// <summary>
        /// Gets or sets the min num frame.
        /// </summary>
        public int MinNumFrame
        {
            get
            {
                return this.minNumFrame;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.minNumFrame, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mz mode enabled.
        /// </summary>
        public bool MzModeEnabled
        {
            get
            {
                return this.mzModeEnabled;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.mzModeEnabled, value);
            }
        }

        /// <summary>
        /// Gets or sets the num frames.
        /// </summary>
        public int NumFrames
        {
            get
            {
                return this.numFrames;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.numFrames, value);
                this.RaisePropertyChanged("TickSize");
                this.RaisePropertyChanged("IsTickSizeOne");
            }
        }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        public Range<int> Range
        {
            get
            {
                return this.range;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.range, value);
            }
        }

        /// <summary>
        /// Gets or sets the start frame.
        /// </summary>
        public int StartFrame
        {
            get => this.startFrame;
            set => this.RaiseAndSetIfChanged(ref this.startFrame, value);
        }

        /// <summary>
        /// Gets the sum frames command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SumFramesCommand { get; }

        /// <summary>
        /// Gets the zoom out command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ZoomOutCommand { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The update current frame number.
        /// </summary>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        public void UpdateCurrentFrameNumber(int frameNumber)
        {
            this.CurrentFrame = frameNumber;
        }

        /// <summary>
        /// The update uimf method.
        /// </summary>
        /// <param name="uimfData">
        /// The uimf data.
        /// </param>
        public void UpdateUimf(UimfData uimfData)
        {
            if (uimfData == null)
            {
                return;
            }

            this.MinNumFrame = 1;
            this.NumFrames = uimfData.Frames;
            this.currentFrame = 1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// publishes the range of frames to be summed
        /// </summary>
        private void SumFrames()
        {
            var tempRange = new Range<int>(this.StartFrame, this.EndFrame);
            this.Range = tempRange;
        }

        #endregion
    }
}