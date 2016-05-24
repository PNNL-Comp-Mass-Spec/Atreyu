namespace UimfDataExtractor.Models
{
    /// <summary>
    /// The bulk peak data.
    /// </summary>
    public class BulkPeakData
    {
        #region Fields

        /// <summary>
        /// The file name the peak was found in.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The frame the peak was found in.
        /// </summary>
        public int FrameNumber { get; set; }

        /// <summary>
        /// The full width half max of the peak.
        /// </summary>
        public double FullWidthHalfMax { get; set; }

        /// <summary>
        /// The location of the peak. Depending on data, this is either the m/z or the scan number.
        /// </summary>
        public double Location { get; set; }

        /// <summary>
        /// The resolving power of the peak, this is the location divided by the full width half max.
        /// </summary>
        public double ResolvingPower { get; set; }

        #endregion
    }
}