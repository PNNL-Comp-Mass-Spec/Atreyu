namespace UimfDataExtractor.Models
{
    /// <summary>
    /// The bulk peak data.
    /// </summary>
    public struct BulkPeakData
    {
        #region Fields

        /// <summary>
        /// The file name the peak was found in.
        /// </summary>
        public string FileName;

        /// <summary>
        /// The frame the peak was found in.
        /// </summary>
        public int FrameNumber;

        /// <summary>
        /// The full width half max of the peak.
        /// </summary>
        public double FullWidthHalfMax;

        /// <summary>
        /// The location of the peak. Depending on data, this is either the m/z or the scan number.
        /// </summary>
        public double Location;

        /// <summary>
        /// The resolving power of the peak, this is the location divided by the full width half max.
        /// </summary>
        public double ResolvingPower;

        #endregion
    }
}