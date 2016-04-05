namespace UimfDataExtractor.Data.Extractors
{
    using System.Collections.Generic;
    using System.IO;

    using UimfDataExtractor.Models;

    using UIMFLibrary;

    using Utilities.Models;

    public class HeatmapExtraction : UimfExtraction
    {


        /// <summary>
        /// Gets full heat map data from the <see cref="DataReader"/> that the UIMF is open in.
        /// </summary>
        /// <param name="uimf">
        /// The uimf that data is to be fetched from.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number that data is to be fetched.
        /// </param>
        /// <returns>
        /// The 2d array of doubles with the heat map data.
        /// </returns>
        private double[,] GetFullHeatmapData(DataReader uimf, int frameNumber)
        {
            var global = uimf.GetGlobalParams();
            var endScan = uimf.GetFrameParams(frameNumber).Scans;
            var endBin = global.Bins;

            return uimf.AccumulateFrameData(frameNumber, frameNumber, false, 1, endScan, 1, endBin, 1, 1);
        }

        public HeatmapExtraction(CommandLineOptions options)
            : base(options)
        {
        }

        protected override void Extract(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var heatmapData = GetFullHeatmapData(uimf, frameNumber);
            var heatmapOutputFile = DataExporter.GetOutputLocation(originFile, "HeatMap", frameNumber);
            DataExporter.OutputHeatMap(heatmapData, heatmapOutputFile, this.Options.Verbose);
        }

        protected override PeakSet BulkPeakFind(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            return new PeakSet();
        }

        protected override IEnumerable<BulkPeakData> PeakCompare(PeakSet peakSet, FileInfo originFile, int frameNumber)
        {
            return null;
        }
    }
}