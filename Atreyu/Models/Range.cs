// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Range.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Models
{
    using ReactiveUI;

    /// <summary>
    /// TODO The range.
    /// </summary>
    public abstract class Range : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// TODO The range type.
        /// </summary>
        protected readonly RangeType rangeType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="type">
        /// TODO The type.
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