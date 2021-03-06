﻿namespace UimfDataExtractor.Data.Extractors
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using UimfDataExtractor.Models;

    using UIMFLibrary;

    using Utilities;
    using Utilities.Models;

    public class TicExtraction : UimfExtraction
    {


        /// <summary>
        /// Gets the Total Ion Chromatogram info for a given frame.
        /// </summary>
        /// <param name="uimf">
        /// The <see cref="DataReader"/> that has the UIMF to be read.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        private List<ScanInfo> GetFullScanInfo(DataReader uimf, int frameNumber)
        {
            return uimf.GetFrameScans(frameNumber);
        }

        public TicExtraction(CommandLineOptions options)
            : base(options)
        {
        }

        public override void OutputBulkPeaks(IEnumerable<BulkPeakData> peakData)
        {
            throw new System.NotImplementedException();
        }

        protected override void Extract(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var ticData = GetFullScanInfo(uimf, frameNumber);
            var ticOutputFile = DataExporter.GetOutputLocation(originFile, "TiC", frameNumber);

            DataExporter.OutputTiCbyTime(ticData, ticOutputFile, this.Options.Verbose);
        }

        protected override IEnumerable<PeakSet> BulkPeakFind(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var ticData = GetFullScanInfo(uimf, frameNumber);
            var doubleTicData =
               ticData.Select(scanInfo => new KeyValuePair<double, double>(scanInfo.DriftTime, scanInfo.TIC))
                   .ToList();

            var ticPeaks = PeakFinder.FindPeaks(doubleTicData);
            var mzPeakOutputLocation = DataExporter.GetOutputLocation(
                   originFile,
                   "TiC_Peaks",
                   frameNumber,
                   "xml");
            DataExporter.OutputPeaks(ticPeaks, mzPeakOutputLocation);
            var peakSets = new List<PeakSet>();
            peakSets.Add(ticPeaks);
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