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
    using Atreyu.Events;
    using Atreyu.Models;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Practices.Prism.PubSubEvents;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using System;
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    /// <summary>
    /// TODO The frame manipulation view model.
    /// </summary>
    [Export]
    public class FrameManipulationViewModel : BindableBase
    {
        #region Fields

        /// <summary>
        /// TODO The _event aggregator.
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// TODO The _current frame.
        /// </summary>
        private int _currentFrame;

        /// <summary>
        /// TODO The _min num frame.
        /// </summary>
        private int _minNumFrame;

        /// <summary>
        /// TODO The _num frames.
        /// </summary>
        private int _numFrames;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameManipulationViewModel"/> class. 
        /// </summary>
        /// <param name="eventAggregator">
        /// </param>
        [ImportingConstructor]
        public FrameManipulationViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException();
            }

            this.OpenFileCommand = new DelegateCommand(this.Open);
            this.SumFramesCommand = new DelegateCommand(this.SumFrames);
            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<NumberOfFramesChangedEvent>().Subscribe(this.NumFramesChanged);
            this._eventAggregator.GetEvent<MinimumNumberOfFrames>().Subscribe(this.MinimumNumberOfFramesChanged);
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
                return this._currentFrame;
            }

            set
            {
                this.SetProperty(ref this._currentFrame, value);
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
                return this._minNumFrame;
            }

            set
            {
                this.SetProperty(ref this._minNumFrame, value);
            }
        }

        /// <summary>
        /// Gets or sets the num frames.
        /// </summary>
        public int NumFrames
        {
            get
            {
                return this._numFrames;
            }

            set
            {
                this.SetProperty(ref this._numFrames, value);
            }
        }

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        public ICommand OpenFileCommand { get; private set; }

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
                this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Publish(frameNumber);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="frameNumber">
        /// </param>
        public void UpdateFrameNumber(int frameNumber)
        {
            this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Publish(frameNumber);
            this.CurrentFrame = frameNumber;
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
                this._eventAggregator.GetEvent<UimfFileLoadedEvent>().Publish(fileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SumFrames()
        {
            FrameRange range = new FrameRange();
            range.StartFrame = this.StartFrame;
            range.EndFrame = this.EndFrame;
            this._eventAggregator.GetEvent<SumFramesChangedEvent>().Publish(range);
        }

        #endregion
    }
}