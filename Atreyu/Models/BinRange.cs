// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinRange.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The bin range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atreyu.Models
{
    using System;

    using ReactiveUI;

    /// <summary>
    /// TODO The bin range.
    /// </summary>
    public class BinRange : Range, IEquatable<BinRange>
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((BinRange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.endBin * 397) ^ this.startBin;
            }
        }

        #region Fields

        /// <summary>
        /// TODO The end bin.
        /// </summary>
        private int endBin;

        /// <summary>
        /// TODO The start bin.
        /// </summary>
        private int startBin;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinRange"/> class.
        /// </summary>
        public BinRange()
            : base(RangeType.BinRange)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinRange"/> class.
        /// </summary>
        /// <param name="start">
        /// TODO The start.
        /// </param>
        /// <param name="end">
        /// TODO The end.
        /// </param>
        public BinRange(int start, int end)
            : base(RangeType.BinRange)
        {
            this.StartBin = start;
            this.EndBin = end;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the end bin.
        /// </summary>
        public int EndBin
        {
            get
            {
                return this.endBin;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.endBin, value);
            }
        }

        /// <summary>
        /// Gets or sets the start bin.
        /// </summary>
        public int StartBin
        {
            get
            {
                return this.startBin;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.startBin, value);
            }
        }

        #endregion

        bool IEquatable<BinRange>.Equals(BinRange other)
        {
            return this.Equals(other);
        }

        bool Equals(BinRange other)
        {
            return this.StartBin == other.StartBin && this.EndBin == other.EndBin;
        }


        public static bool operator ==(BinRange left, BinRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BinRange left, BinRange right)
        {
            return !Equals(left, right);
        }
    }
}