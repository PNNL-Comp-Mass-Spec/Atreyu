// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameManipulationViewModel.cs" company="Pacific Northwest National Laboratory">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Pacific Northwest National Laboratory
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// <summary>
//   The frame manipulation view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Atreyu.ViewModels
{
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Atreyu.Models;

    using Microsoft.Practices.Prism.Commands;

    using ReactiveUI;

    /// <summary>
    /// The frame manipulation view model.
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
        private FrameRange range;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameManipulationViewModel"/> class. 
        /// </summary>
        [ImportingConstructor]
        public FrameManipulationViewModel()
        {
            this.SumFramesCommand = new DelegateCommand(this.SumFrames);

            this.ZoomOutCommand = ReactiveCommand.Create();
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
                // always raise it currently I would like to go back and find another way using raise and set if changed,
                // but I have a meeting and I needed the slider bar to update on load and this was the easy way.
                this.currentFrame = value;
                this.RaisePropertyChanged();

                // this.RaiseAndSetIfChanged(ref this.currentFrame, value);
            }
        }

        /// <summary>
        /// Gets or sets the end frame.
        /// </summary>
        public int EndFrame { get; set; }

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

        /// <summary>
        /// Gets the zoom out command.
        /// </summary>
        public ReactiveCommand<object> ZoomOutCommand { get; private set; }

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
            var tempRange = new FrameRange { StartFrame = this.StartFrame, EndFrame = this.EndFrame };
            this.Range = tempRange;
        }

        #endregion
    }
}