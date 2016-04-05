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

    public class MzExtraction : UimfExtraction
    {

        /// <summary>
        /// Gets full mass over charge info for a given frame.
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
        private List<KeyValuePair<double, int>> GetFullMzInfo(DataReader uimf, int frameNumber)
        {
            var frameParams = uimf.GetFrameParams(frameNumber);
            var maxScans = uimf.GetFrameParams(frameNumber).Scans;


            DataReader.FrameType frameType;
            var parseSuccess = Enum.TryParse(frameParams.GetValue(FrameParamKeyType.FrameType), out frameType);

            if (!parseSuccess)
            {
                Console.Error.WriteLine(
                    "ERROR: Had a problem getting the frame type which means we can't get the MZ data");
                return null;
            }

            double[] mzs;
            int[] intensities;
            uimf.GetSpectrum(frameNumber, frameNumber, frameType, 1, maxScans, out mzs, out intensities);
            var data = new List<KeyValuePair<double, int>>(mzs.Length);
            for (var i = 0; i < mzs.Length && i < intensities.Length; i++)
            {
                data.Add(new KeyValuePair<double, int>(mzs[i], intensities[i]));
            }

            return data;
        }

        public MzExtraction(CommandLineOptions options)
            : base(options)
        {
        }

        public override void OutputBulkPeaks(IEnumerable<BulkPeakData> peakData)
        {
            throw new NotImplementedException();
        }

        protected override void Extract(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var mzData = GetFullMzInfo(uimf, frameNumber);
            if (mzData == null)
            {
                Console.Error.WriteLine(
                    "ERROR: We had a problem getting the data for the MZ of" + Environment.NewLine + " frame "
                    + frameNumber + " in " + originFile.FullName);
            }

            var mzOutputFile = DataExporter.GetOutputLocation(originFile, "Mz", frameNumber);
            DataExporter.OutputMz(mzData, mzOutputFile, this.Options.Verbose);
        }

        protected override PeakSet BulkPeakFind(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var mzData = GetFullMzInfo(uimf, frameNumber);
            var doubleMzData =
                mzData.Select(
                    keyValuePair => new KeyValuePair<double, double>(keyValuePair.Key, keyValuePair.Value))
                    .ToList();
            var mzpeaks = PeakFinder.FindPeaks(doubleMzData);

            var mzPeakOutputLocation = DataExporter.GetOutputLocation(
                    originFile,
                    "Mz_Peaks",
                    frameNumber,
                    "xml");
            DataExporter.OutputPeaks(mzpeaks, mzPeakOutputLocation);
            return mzpeaks;
        }

        protected override IEnumerable<BulkPeakData> PeakCompare(PeakSet peakSet, FileInfo originFile, int frameNumber)
        {
            var bulkPeakList = new List<BulkPeakData>();
            foreach (var peak in peakSet.Peaks)
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

            return bulkPeakList;
        }
    }
}