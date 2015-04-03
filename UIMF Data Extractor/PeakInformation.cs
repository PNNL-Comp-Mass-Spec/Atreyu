namespace UimfDataExtractor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;

    public class PeakInformation
    {

        public PeakInformation()
        {
            this.TotalDataPointSet = new List<PointInformation>();
        }

        public PeakInformation(List<PointInformation> list)
        {
            this.TotalDataPointSet = list;
        }

        [DataMember]
        public double FullWidthHalfMax { get; set; }

        [DataMember]
        public double Intensity { get; set; }

        [DataMember]
        public double LeftMidpoint { get; set; }

        [DataMember]
        public double SmoothedIntensity { get; set; }

        [DataMember]
        public double PeakCenter { get; set; }

        [DataMember]
        public double RightMidpoint { get; set; }

        [DataMember]
        public double ResolvingPower { get; set; }

        [DataMember]
        public List<PointInformation> TotalDataPointSet { get; set; } 
    }

    public class PeakSet
    {
        public PeakSet()
        {
            this.Peaks = new List<PeakInformation>();
        }

        [DataMember]
        public List<PeakInformation> Peaks { get; set; }
    }
}