namespace Atreyu.Models
{
    using ReactiveUI;

    /// <summary>
    /// The range.
    /// </summary>
    public abstract class Range : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// The range type.
        /// </summary>
        private readonly RangeType rangeType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        protected Range(RangeType type)
        {
            this.rangeType = type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the range type.
        /// </summary>
        public RangeType RangeType
        {
            get
            {
                return this.rangeType;
            }
        }

        #endregion
    }
}