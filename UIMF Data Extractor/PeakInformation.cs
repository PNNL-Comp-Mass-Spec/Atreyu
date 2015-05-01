// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeakInformation.cs" company="Pacific Northwest National Laboratory">
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
//   Classes for handling the peak information and the tags to allow xml output.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UimfDataExtractor
{
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
    }

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