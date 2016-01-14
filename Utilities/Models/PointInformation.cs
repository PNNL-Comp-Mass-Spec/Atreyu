namespace Utilities.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The point information.
    /// </summary>
    [DataContract]
    public class PointInformation
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the intensity.
        /// </summary>
        [DataMember]
        public double Intensity { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [DataMember]
        public double Location { get; set; }

        /// <summary>
        /// Gets or sets the smoothed intensity.
        /// </summary>
        [DataMember]
        public double SmoothedIntensity { get; set; }

        #endregion
    }
}