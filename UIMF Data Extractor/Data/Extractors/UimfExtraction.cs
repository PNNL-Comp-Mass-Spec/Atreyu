namespace UimfDataExtractor.Data.Extractors
{
    using System.Collections.Generic;
    using System.IO;

    using UimfDataExtractor.Models;

    using UIMFLibrary;

    using Utilities.Models;

    public abstract class UimfExtraction
    {
        protected CommandLineOptions Options { get; private set; }

        protected UimfExtraction(CommandLineOptions options)
        {
            this.Options = options;
        }

        public static UimfExtraction UimfExtractionFactory(UimfDataExtractor.Extraction extraction, CommandLineOptions options)
        {
            switch (extraction)
            {
                    case UimfDataExtractor.Extraction.Heatmap:
                    return new HeatmapExtraction(options);

                    case UimfDataExtractor.Extraction.Mz:
                    return new MzExtraction(options);

                    case UimfDataExtractor.Extraction.Tic:
                    return new TicExtraction(options);

                    case UimfDataExtractor.Extraction.Xic:
                    return new XicExtraction(options);

                default:
                    return null;

            }
        }

        public void ExtractData(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            Extract(uimf, originFile, frameNumber);
        
        }

        public IEnumerable<BulkPeakData> ComparePeaks(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            Extract(uimf, originFile, frameNumber);
            var peakSet = BulkPeakFind(uimf, originFile, frameNumber);
           return PeakCompare(peakSet, originFile, frameNumber);
        }

        protected abstract void Extract(DataReader uimf, FileInfo originFile, int frameNumber);

        protected abstract PeakSet BulkPeakFind(DataReader uimf, FileInfo originFile, int frameNumber);

        protected abstract IEnumerable<BulkPeakData> PeakCompare(PeakSet peakSet, FileInfo originFile, int frameNumber);
    }
}