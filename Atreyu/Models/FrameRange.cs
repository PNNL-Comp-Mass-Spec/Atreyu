// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameRange.cs" company="Pacific Northwest National Laboratory">
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
//   The frame range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Models
{
    using System;

    using ReactiveUI;

    /// <summary>
    /// The frame range.
    /// </summary>
    public class FrameRange : Range, IEquatable<FrameRange>
    {
        #region Fields

        /// <summary>
        /// The end frame.
        /// </summary>
        private int endFrame;

        /// <summary>
        /// The start frame.
        /// </summary>
        private int startFrame;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameRange"/> class.
        /// </summary>
        public FrameRange()
            : base(RangeType.FrameRange)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameRange"/> class.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        public FrameRange(int start, int end)
            : base(RangeType.FrameRange)
        {
            this.StartFrame = start;
            this.EndFrame = end;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the end frame.
        /// </summary>
        public int EndFrame
        {
            get
            {
                return this.endFrame;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.endFrame, value);
            }
        }

        /// <summary>
        /// Gets or sets the start frame.
        /// </summary>
        public int StartFrame
        {
            get
            {
                return this.startFrame;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.startFrame, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(FrameRange other)
        {
            return this.StartFrame == other.StartFrame && this.EndFrame == other.EndFrame;
        }

        #endregion
    }
}