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