namespace UimfDataExtractor.Data.Extractors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using UimfDataExtractor.Models;

    using UIMFLibrary;

    using Utilities;
    using Utilities.Models;

    public class XicExtraction : UimfExtraction
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uimf"></param>
        /// <param name="frameNumber"></param>
        /// <param name="xicMz"></param>
        /// <param name="tolerance"></param>
        /// <param name="getMsms"></param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<double, double>> GetXicInfo(
            DataReader uimf,
            int frameNumber,
            double xicMz,
            double tolerance,
            bool getMsms)
        {
            const DataReader.ToleranceType Tolerance = DataReader.ToleranceType.PPM;

            var frametype = getMsms ? DataReader.FrameType.MS2 : DataReader.FrameType.MS1;

            if (!uimf.DoesContainBinCentricData())
            {
                Console.WriteLine(uimf.UimfFilePath + " Does not have bin centric data which is required to get XiC");
                Console.WriteLine("starting to create it, this may take some time");
                var fileName = uimf.UimfFilePath;
                uimf.Dispose(); // why is this being disposed -- this is INSIDE a using statement! SAP 4/4/2016

                using (var dataWriter = new DataWriter(fileName))
                {
                    dataWriter.CreateBinCentricTables();
                }

                uimf = new DataReader(fileName); // WHY IS THIS BEING SET?! This is inside a using statement! 
                Console.WriteLine("Finished Creating bin centric tables for " + uimf.UimfFilePath);
            }

            var data = new List<KeyValuePair<double, double>>();
            try
            {
                var xic = uimf.GetXic(xicMz, tolerance, frametype, Tolerance);
                var frameData = xic.Where(point => point.ScanLc == frameNumber - 1);



                // I think this is more readable with a regular loop than a very long linq query
                foreach (var intensityPoint in frameData)
                {
                    var driftTime = uimf.GetDriftTime(intensityPoint.ScanLc + 1, intensityPoint.ScanIms, true);
                    data.Add(new KeyValuePair<double, double>(driftTime, intensityPoint.Intensity));
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Unable to get XiC on first attempt for " + uimf.UimfFilePath);
            }



            return data;
        }

        public XicExtraction(CommandLineOptions options)
            : base(options)
        {
        }

        public override void OutputBulkPeaks(IEnumerable<BulkPeakData> peakData)
        {
            
        }

        protected override void Extract(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            foreach (var xicTarget in Options.XicTargetList)
            {
                var xicData = GetXicInfo(uimf, frameNumber, xicTarget.TargetMz, xicTarget.Tolerance, this.Options.Getmsms);
                var xicOutputFile = DataExporter.GetOutputLocation(
                    originFile,
                    "XiC_mz_" + xicTarget.TargetMz + "_tolerance_" + xicTarget.Tolerance + "_Frame",
                    frameNumber);

                if (xicData == null)
                {
                    return;
                }

                DataExporter.OutputXiCbyTime(xicData, xicOutputFile, this.Options.Verbose);
            }
          

        }

        protected override IEnumerable<PeakSet> BulkPeakFind(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var peakSets = new List<PeakSet>();
            foreach (var xicTarget in Options.XicTargetList)
            {
                var xicData = GetXicInfo(uimf, frameNumber, xicTarget.TargetMz, xicTarget.Tolerance, this.Options.Getmsms);
                var xicPeaks = PeakFinder.FindPeaks(xicData);
                var xicPeakOutputLocation = DataExporter.GetOutputLocation(
                       originFile,
                       "XiC_Peaks_mz_" + xicTarget.TargetMz + "_tolerance_" + xicTarget.Tolerance + "_Frame",
                       frameNumber,
                       "xml");
                DataExporter.OutputPeaks(xicPeaks, xicPeakOutputLocation);

               peakSets.Add(xicPeaks);
            }

            return peakSets;
        }

        protected override IEnumerable<BulkPeakData> PeakCompare(IEnumerable<PeakSet> peakSet, FileInfo originFile, int frameNumber)
        {
            var bulkPeakList = new List<BulkPeakData>();

            foreach (var set in peakSet)
            {
                foreach (var peak in set.Peaks)
                {
                    var temp = new BulkPeakData
                    {
                        FileName = originFile.Name,
                        FrameNumber = frameNumber,
                        Location = peak.PeakCenter,
                        FullWidthHalfMax = peak.FullWidthHalfMax,
                        ResolvingPower = peak.ResolvingPower
                    };
                    bulkPeakList.Add(temp);
                }
            }
          

            return bulkPeakList;
        }
    }
}