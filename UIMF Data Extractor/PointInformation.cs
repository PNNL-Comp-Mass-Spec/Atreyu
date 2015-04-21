namespace UimfDataExtractor
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class PointInformation
    {
        [DataMember]
        public double Location { get; set; }

        [DataMember]
        public double Intensity { get; set; }

        [DataMember]
        public double SmoothedIntensity { get; set; }
    }
}