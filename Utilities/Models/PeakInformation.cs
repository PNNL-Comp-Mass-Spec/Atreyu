namespace Utilities.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The peak information class.
    /// </summary>
    public class PeakInformation
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PeakInformation"/> class.
        /// </summary>
        public PeakInformation()
        {
            this.TotalDataPointSet = new List<PointInformation>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeakInformation"/> class.
        /// </summary>
        /// <param name="list">
        /// The list of points used to form the peak.
        /// </param>
        public PeakInformation(List<PointInformation> list)
        {
            this.TotalDataPointSet = list;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the area under the peak.
        /// </summary>
        [DataMember]
        public double AreaUnderThePeak { get; set; }

        /// <summary>
        /// Gets or sets the full width half max.
        /// </summary>
        [DataMember]
        public double FullWidthHalfMax { get; set; }

        /// <summary>
        /// Gets or sets the intensity.
        /// </summary>
        [DataMember]
        public double Intensity { get; set; }

        /// <summary>
        /// Gets or sets the left midpoint.
        /// </summary>
        [DataMember]
        public double LeftMidpoint { get; set; }

        /// <summary>
        /// Gets or sets the peak center.
        /// </summary>
        [DataMember]
        public double PeakCenter { get; set; }

        /// <summary>
        /// Gets or sets the resolving power.
        /// </summary>
        [DataMember]
        public double ResolvingPower { get; set; }

        /// <summary>
        /// Gets or sets the right midpoint.
        /// </summary>
        [DataMember]
        public double RightMidpoint { get; set; }

        /// <summary>
        /// Gets or sets the smoothed intensity.
        /// </summary>
        [DataMember]
        public double SmoothedIntensity { get; set; }

        /// <summary>
        /// Gets or sets the total data point set.
        /// </summary>
        [DataMember]
        public List<PointInformation> TotalDataPointSet { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            var temp = "Peak Center: " + this.PeakCenter.ToString("N") + Environment.NewLine + "Intensity: "
                       + this.Intensity.ToString("N") + Environment.NewLine + "Full Width Half Max: "
                       + this.FullWidthHalfMax.ToString("N") + Environment.NewLine + "Resolving Power: "
                       + this.ResolvingPower.ToString("N") + Environment.NewLine + "Area Under The Peak: "
                       + this.AreaUnderThePeak.ToString("N");
            return temp;
        }

        #endregion
    }
}