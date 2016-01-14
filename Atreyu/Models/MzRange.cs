namespace Atreyu.Models
{
    using System;

    using ReactiveUI;

    /// <summary>
    /// The m/z range.
    /// </summary>
    public class MzRange : Range, IEquatable<MzRange>
    {
        #region Fields

        /// <summary>
        /// The end m/z.
        /// </summary>
        private double endMz;

        /// <summary>
        /// The start m/z.
        /// </summary>
        private double startMz;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MzRange"/> class.
        /// </summary>
        public MzRange()
            : base(RangeType.MzRange)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MzRange"/> class.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        public MzRange(double start, double end)
            : base(RangeType.MzRange)
        {
            this.StartMz = start;
            this.EndMz = end;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the end m/z.
        /// </summary>
        public double EndMz
        {
            get
            {
                return this.endMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.endMz, value);
            }
        }

        /// <summary>
        /// Gets or sets the start m/z.
        /// </summary>
        public double StartMz
        {
            get
            {
                return this.startMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.startMz, value);
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
        public static bool operator ==(MzRange left, MzRange right)
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
        public static bool operator !=(MzRange left, MzRange right)
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

            return this.Equals((MzRange)obj);
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
                return ((int)this.endMz * 397) ^ (int)this.startMz;

                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other MzRange to compare.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IEquatable<MzRange>.Equals(MzRange other)
        {
            return this.Equals(other);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other MzRange to compare.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool Equals(MzRange other)
        {
            return this.StartMz == other.StartMz && this.EndMz == other.EndMz;
        }

        #endregion
    }
}