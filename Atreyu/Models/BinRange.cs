// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinRange.cs" company="Pacific Northwest National Laboratory">
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
//   The bin range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Models
{
    using System;

    using ReactiveUI;

    /// <summary>
    /// The bin range.
    /// </summary>
    public class BinRange : Range, IEquatable<BinRange>
    {
        #region Fields

        /// <summary>
        /// The end bin.
        /// </summary>
        private int endBin;

        /// <summary>
        /// The start bin.
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
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
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

        #region Public Methods and Operators

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator ==(BinRange left, BinRange right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator !=(BinRange left, BinRange right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

            return this.Equals((BinRange)obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyFieldInGetHashCode
                return (this.endBin * 397) ^ this.startBin;
                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other BinRange to compare.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IEquatable<BinRange>.Equals(BinRange other)
        {
            return this.Equals(other);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other BinRange to compare.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool Equals(BinRange other)
        {
            return this.StartBin == other.StartBin && this.EndBin == other.EndBin;
        }

        #endregion
    }
}