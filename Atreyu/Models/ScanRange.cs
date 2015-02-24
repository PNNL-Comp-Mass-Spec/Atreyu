// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScanRange.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The scan range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.Models
{
    using System;

    using ReactiveUI;

    /// <summary>
    /// TODO The scan range.
    /// </summary>
    public class ScanRange : Range, IEquatable<ScanRange>
    {
        #region Fields

        /// <summary>
        /// TODO The end scan.
        /// </summary>
        private int endScan;

        /// <summary>
        /// TODO The start scan.
        /// </summary>
        private int startScan;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanRange"/> class.
        /// </summary>
        public ScanRange()
            : base(RangeType.ScanRange)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanRange"/> class.
        /// </summary>
        /// <param name="start">
        /// TODO The start.
        /// </param>
        /// <param name="end">
        /// TODO The end.
        /// </param>
        public ScanRange(int start, int end)
            : base(RangeType.ScanRange)
        {
            this.StartScan = start;
            this.EndScan = end;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the end scan.
        /// </summary>
        public int EndScan
        {
            get
            {
                return this.endScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.endScan, value);
            }
        }

        /// <summary>
        /// Gets or sets the start scan.
        /// </summary>
        public int StartScan
        {
            get
            {
                return this.startScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.startScan, value);
            }
        }

        #endregion

        public bool Equals(ScanRange other)
        {
            return this.StartScan == other.StartScan && this.EndScan == other.EndScan;
        }
    }
}