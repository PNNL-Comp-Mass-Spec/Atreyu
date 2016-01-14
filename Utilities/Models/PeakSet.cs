namespace Utilities.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The peak set.
    /// </summary>
    public class PeakSet
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PeakSet"/> class.
        /// </summary>
        public PeakSet()
        {
            this.Peaks = new List<PeakInformation>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeakSet"/> class.
        /// </summary>
        /// <param name="peakInformationEnumerable">
        /// The peak information to initialize with.
        /// </param>
        public PeakSet(IEnumerable<PeakInformation> peakInformationEnumerable)
        {
            this.Peaks = new List<PeakInformation>(peakInformationEnumerable);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the peaks.
        /// </summary>
        [DataMember]
        public List<PeakInformation> Peaks { get; set; }

        #endregion
    }
}