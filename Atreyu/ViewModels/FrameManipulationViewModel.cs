// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameManipulationViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The frame manipulation view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.ViewModels
{
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Atreyu.Models;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.WindowsAPICodePack.Dialogs;

    using ReactiveUI;

    /// <summary>
    /// TODO The frame manipulation view model.
    /// </summary>
    [Export]
    public class FrameManipulationViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The current frame.
        /// </summary>
        private int currentFrame;

        /// <summary>
        /// TODO The lowest frame number.
        /// </summary>
        private int minNumFrame;

        /// <summary>
        /// TODO The mz mode enabled.
        /// </summary>
        private bool mzModeEnabled;

        /// <summary>
        /// TODO The total number of frames.
        /// </summary>
        private int numFrames;

        /// <summary>
        /// TODO The range.
        /// </summary>
        private FrameRange range;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameManipulationViewModel"/> class. 
        /// </summary>
        /// <param name="eventAggregator">
        /// </param>
        [ImportingConstructor]
        public FrameManipulationViewModel()
        {
            this.OpenFileCommand = new DelegateCommand(this.Open);
            this.SumFramesCommand = new DelegateCommand(this.SumFrames);

            ////this._eventAggregator.GetEvent<NumberOfFramesChangedEvent>().Subscribe(this.NumFramesChanged);
            ////this._eventAggregator.GetEvent<MinimumNumberOfFrames>().Subscribe(this.MinimumNumberOfFramesChanged);
        }

        #endregion

        #region Public Properties

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
                // always raise it currently I would like to go back and find another way using rais and set if changed,
                // but I have a meeting and I needed the slider bar to update on load and this was the easy way.
                this.currentFrame = value;
                this.raisePropertyChanged("CurrentFrame");

                // this.RaiseAndSetIfChanged(ref this.currentFrame, value);
            }
        }

        /// <summary>
        /// Gets or sets the end frame.
        /// </summary>
        public int EndFrame { get; set; }

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
            }
        }

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        public ICommand OpenFileCommand { get; private set; }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        public FrameRange Range
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
        public int StartFrame { get; set; }

        /// <summary>
        /// Gets the sum frames command.
        /// </summary>
        public ICommand SumFramesCommand { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The update current frame number.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        public void UpdateCurrentFrameNumber(int frameNumber)
        {
            if (this.CurrentFrame != frameNumber)
            {
                this.CurrentFrame = frameNumber;

                ////this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Publish(frameNumber);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="frameNumber">
        /// </param>
        public void UpdateFrameNumber(int frameNumber)
        {
            ////this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Publish(frameNumber);
            this.CurrentFrame = frameNumber;
        }

        /// <summary>
        /// TODO The update uimf.
        /// </summary>
        /// <param name="uimfData">
        /// TODO The uimf data.
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
        /// </summary>
        /// <param name="obj">
        /// </param>
        private void MinimumNumberOfFramesChanged(int obj)
        {
            this.MinNumFrame = obj;
        }

        /// <summary>
        /// </summary>
        /// <param name="numberOfFrames">
        /// </param>
        private void NumFramesChanged(int numberOfFrames)
        {
            this.NumFrames = numberOfFrames;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Open()
        {
            var openFileDialog = new CommonOpenFileDialog { DefaultExtension = ".uimf" };

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string fileName = openFileDialog.FileName;

                ////this._eventAggregator.GetEvent<UimfFileLoadedEvent>().Publish(fileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SumFrames()
        {
            FrameRange tempRange = new FrameRange { StartFrame = this.StartFrame, EndFrame = this.EndFrame };
            this.Range = tempRange;

            ////this._eventAggregator.GetEvent<SumFramesChangedEvent>().Publish(range);
        }

        #endregion
    }
}