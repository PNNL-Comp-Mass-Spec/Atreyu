namespace UimfDataExtractor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using UimfDataExtractor.Models;

    using UIMFLibrary;

    using Utilities;

    /// <summary>
    /// The uimf processor class handles finding and fetching data from the UIMFs.
    /// </summary>
    internal class UimfProcessor
    {
        #region Static Fields

        /// <summary>
        /// The options.
        /// </summary>
        private static CommandLineOptions options;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public static CommandLineOptions Options
        {
            get
            {
                return options;
            }

            set
            {
                options = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the bulk mz peaks.
        /// </summary>
        private static ConcurrentBag<BulkPeakData> BulkMzPeaks { get; set; }

        /// <summary>
        /// Gets or sets the bulk tic peaks.
        /// </summary>
        private static ConcurrentBag<BulkPeakData> BulkTicPeaks { get; set; }

        /// <summary>
        /// Gets or sets the bulk xic peaks.
        /// </summary>
        private static ConcurrentBag<BulkPeakData> BulkXicPeaks { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The extract data method is the entry function to this process, it assumes you have set all the options already.
        /// </summary>
        public static void ExtractData()
        {
            DataExporter.InputDirectory = new DirectoryInfo(Options.InputPath);

            DataExporter.OutputDirectory = string.IsNullOrWhiteSpace(Options.OutputPath)
                                               ? DataExporter.InputDirectory
                                               : new DirectoryInfo(Options.OutputPath);

            BulkMzPeaks = new ConcurrentBag<BulkPeakData>();
            BulkTicPeaks = new ConcurrentBag<BulkPeakData>();
            BulkXicPeaks = new ConcurrentBag<BulkPeakData>();

            if (Options.Verbose)
            {
                Console.WriteLine("Verbose Active");
                Console.WriteLine("Input Directory: " + DataExporter.InputDirectory.FullName);
                Console.WriteLine("Output Directory: " + DataExporter.OutputDirectory.FullName);
                Console.WriteLine("Run Recursively?: " + Options.Recursive);
                Console.WriteLine("Process All Frames in File?: " + Options.AllFrames);
            }

            if (Options.Recursive)
            {
                ProcessAllUimfInDirectoryRecursive(DataExporter.InputDirectory);
            }
            else
            {
                ProcessAllUimfInDirectory(DataExporter.InputDirectory);
            }

            if (Options.BulkPeakComparison)
            {
                OutputBulkPeaks();
            }

            Console.WriteLine();
            Console.WriteLine("All done.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets <see cref="DataReader.FrameType"/> that corresponds with the string value.
        /// </summary>
        /// <param name="frameType">
        /// The string value for the frame type.
        /// </param>
        /// <returns>
        /// The <see cref="DataReader.FrameType"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// This is thrown when something other than 1, 2, 3, or 4 is passed
        /// </exception>
        private static DataReader.FrameType GetFrameType(string frameType)
        {
            var temp = frameType.ToLower();
            switch (temp)
            {
                case "1":
                    return DataReader.FrameType.MS1;
                case "2":
                    return DataReader.FrameType.MS2;
                case "3":
                    return DataReader.FrameType.Calibration;
                case "4":
                    return DataReader.FrameType.Prescan;
                default:
                    throw new NotImplementedException(
                        "Only the MS1, MS2, Calibration, and Prescan frame types have been implemented in this version. Data Passed was "
                        + temp);
            }
        }

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
        private static double[,] GetFullHeatmapData(DataReader uimf, int frameNumber)
        {
            var global = uimf.GetGlobalParams();
            var endScan = uimf.GetFrameParams(frameNumber).Scans;
            var endBin = global.Bins;

            return uimf.AccumulateFrameData(frameNumber, frameNumber, false, 1, endScan, 1, endBin, 1, 1);
        }

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
        private static List<KeyValuePair<double, int>> GetFullMzInfo(DataReader uimf, int frameNumber)
        {
            var frameParams = uimf.GetFrameParams(frameNumber);
            var maxScans = uimf.GetFrameParams(frameNumber).Scans;

            var typeString = frameParams.GetValue(FrameParamKeyType.FrameType);

            if (string.IsNullOrWhiteSpace(typeString))
            {
                Console.Error.WriteLine(
                    "ERROR: Had a problem getting the frame type which means we can't get the MZ data");
                return null;
            }

            DataReader.FrameType frameType;
            var type = frameParams.GetValue(FrameParamKeyType.FrameType);

            try
            {
                frameType = GetFrameType(type);
            }
            catch (NotImplementedException)
            {
                Console.Error.WriteLine(
                    "ERROR: An Unknown Frame Type was read on Frame: {0}, we wil continue as best we can by defaulting to MS1.  The Type read was {1}.", 
                    frameNumber, 
                    type);
                frameType = DataReader.FrameType.MS1;
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
        private static List<ScanInfo> GetFullScanInfo(DataReader uimf, int frameNumber)
        {
            return uimf.GetFrameScans(frameNumber);
        }

        /// <summary>
        /// The get mz.
        /// </summary>
        /// <param name="uimf">
        /// The uimf.
        /// </param>
        /// <param name="originFile">
        /// The origin file.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        /// <returns>
        /// A List of Key, Value pairs, key is the m/z and the value is the intensity at that point.
        /// </returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static List<KeyValuePair<double, int>> GetMz(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var mzData = GetFullMzInfo(uimf, frameNumber);
            if (mzData == null)
            {
                Console.Error.WriteLine(
                    "ERROR: We had a problem getting the data for the MZ of" + Environment.NewLine + " frame "
                    + frameNumber + " in " + originFile.FullName);
                return null;
            }

            var mzOutputFile = DataExporter.GetOutputLocation(originFile, "Mz", frameNumber);
            DataExporter.OutputMz(mzData, mzOutputFile, options.Verbose);
            if (!options.PeakFind && !options.BulkPeakComparison)
            {
                return mzData;
            }

            var doubleMzData =
                mzData.Select(
                    keyValuePair => new KeyValuePair<double, double>(keyValuePair.Key, keyValuePair.Value))
                    .ToList();
            var mzpeaks = PeakFinder.FindPeaks(doubleMzData);

            if (options.PeakFind)
            {
                var mzPeakOutputLocation = DataExporter.GetOutputLocation(
                    originFile, 
                    "Mz_Peaks", 
                    frameNumber, 
                    "xml");
                DataExporter.OutputPeaks(mzpeaks, mzPeakOutputLocation);
            }

            if (!options.BulkPeakComparison)
            {
                return mzData;
            }

            foreach (var peak in mzpeaks.Peaks)
            {
                var temp = new BulkPeakData
                               {
                                   FileName = originFile.Name, 
                                   FrameNumber = frameNumber, 
                                   Location = peak.PeakCenter, 
                                   FullWidthHalfMax = peak.FullWidthHalfMax, 
                                   ResolvingPower = peak.ResolvingPower
                               };
                BulkMzPeaks.Add(temp);
            }

            return mzData;
        }

        /// <summary>
        /// The get tic function.
        /// </summary>
        /// <param name="uimf">
        /// The uimf.
        /// </param>
        /// <param name="originFile">
        /// The origin file.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        /// <returns>
        /// The List of ScanInfo.
        /// </returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static List<ScanInfo> GetTiC(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var ticData = GetFullScanInfo(uimf, frameNumber);
            var ticOutputFile = DataExporter.GetOutputLocation(originFile, "TiC", frameNumber);

            DataExporter.OutputTiCbyTime(ticData, ticOutputFile, options.Verbose);

            if (!options.PeakFind && !options.BulkPeakComparison)
            {
                return ticData;
            }

            var doubleTicData =
                ticData.Select(scanInfo => new KeyValuePair<double, double>(scanInfo.DriftTime, scanInfo.TIC))
                    .ToList();

            var ticPeaks = PeakFinder.FindPeaks(doubleTicData);
            if (options.PeakFind)
            {
                var mzPeakOutputLocation = DataExporter.GetOutputLocation(
                    originFile, 
                    "TiC_Peaks", 
                    frameNumber, 
                    "xml");
                DataExporter.OutputPeaks(ticPeaks, mzPeakOutputLocation);
            }

            if (!options.BulkPeakComparison)
            {
                return ticData;
            }

            foreach (var peak in ticPeaks.Peaks)
            {
                var temp = new BulkPeakData
                               {
                                   FileName = originFile.Name, 
                                   FrameNumber = frameNumber, 
                                   Location = peak.PeakCenter, 
                                   FullWidthHalfMax = peak.FullWidthHalfMax, 
                                   ResolvingPower = peak.ResolvingPower
                               };
                BulkTicPeaks.Add(temp);
            }

            return ticData;
        }

        /// <summary>
        /// The get xic.
        /// </summary>
        /// <param name="uimf">
        /// The uimf.
        /// </param>
        /// <param name="originFile">
        /// The origin file.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        /// <returns>
        /// The List of Key Value pairs where the Key is the scan, value is Intensity.
        /// </returns>
        private static List<KeyValuePair<double, double>> GetXic(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var xicData = GetXicInfo(uimf, frameNumber, options.XicMz, options.XicTolerance, options.Getmsms);
            var xicOutputFile = DataExporter.GetOutputLocation(
                originFile,
                "XiC_mz_" + options.XicMz + "_tolerance_" + options.XicTolerance + "_Frame", 
                frameNumber);

            if (xicData == null)
            {
                return null;
            }

            DataExporter.OutputXiCbyTime(xicData, xicOutputFile, options.Verbose);

            if (!options.PeakFind && !options.BulkPeakComparison)
            {
                return xicData;
            }

            var xicPeaks = PeakFinder.FindPeaks(xicData);
            if (options.PeakFind)
            {
                var xicPeakOutputLocation = DataExporter.GetOutputLocation(
                    originFile,
                    "XiC_Peaks_mz_" + options.XicMz + "_tolerance_" + options.XicTolerance + "_Frame", 
                    frameNumber, 
                    "xml");
                DataExporter.OutputPeaks(xicPeaks, xicPeakOutputLocation);
            }

            if (!options.BulkPeakComparison)
            {
                return xicData;
            }

            foreach (var peak in xicPeaks.Peaks)
            {
                var temp = new BulkPeakData
                               {
                                   FileName = originFile.Name, 
                                   FrameNumber = frameNumber, 
                                   Location = peak.PeakCenter, 
                                   FullWidthHalfMax = peak.FullWidthHalfMax, 
                                   ResolvingPower = peak.ResolvingPower
                               };
                BulkXicPeaks.Add(temp);
            }

            return xicData;
        }

        /// <summary>
        /// The get xic info.
        /// </summary>
        /// <param name="uimf">
        /// The uimf.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        /// <param name="xicMz">
        /// The xic mz.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance.
        /// </param>
        /// <param name="getMsms">
        /// Specifies whether or not to get ms 2 instead of ms 1 data.
        /// </param>
        /// <returns>
        /// The List of Key Value pairs where the Key is the scan, value is Intensity.
        /// </returns>
        private static List<KeyValuePair<double, double>> GetXicInfo(
            DataReader uimf, 
            int frameNumber, 
            double xicMz, 
            double tolerance, 
            bool getMsms)
        {
            const DataReader.ToleranceType Tolerance = DataReader.ToleranceType.Thomson;

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

            List<IntensityPoint> xic;
            var data = new List<KeyValuePair<double, double>>();
            try
            {
                xic = uimf.GetXic(xicMz, tolerance, frametype, Tolerance);
                var frameData = xic.Where(point => point.ScanLc == frameNumber - 1);



                // I think this is more readable with a regular loop than a very long linq query
                // ReSharper disable once LoopCanBeConvertedToQuery
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

        /// <summary>
        /// Outputs the bulk peaks collected during the run.
        /// </summary>
        private static void OutputBulkPeaks()
        {
            var dateString = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var inputFolder = DataExporter.InputDirectory.Name;

            foreach (var extractionType in options.ExtractionTypes)
            {
                switch (extractionType)
                {
                      case UimfExtraction.Mz:
                        DataExporter.OutputBulkMzPeakData(dateString, inputFolder, BulkMzPeaks);
                        break;
                        case UimfExtraction.Tic:
                        DataExporter.OutputBulkTicPeakData(dateString, inputFolder, BulkTicPeaks);
                        break;
                        case UimfExtraction.Xic:
                        DataExporter.OutputBulkXicPeakData(
                   dateString,
                   inputFolder,
                   BulkXicPeaks,
                   options.XicMz,
                   options.XicTolerance);
                        break;
                }
            }
        }

        /// <summary>
        /// Print a not found error.
        /// </summary>
        /// <param name="fileOrDirectory">
        /// The file or directory.
        /// </param>
        // ReSharper disable once UnusedMember.Local
        private static void PrintNotFoundError(string fileOrDirectory)
        {
            Console.WriteLine();
            Console.WriteLine(fileOrDirectory + "does not exist");
            Console.WriteLine();
        }

        /// <summary>
        /// Print a not found error.
        /// </summary>
        /// <param name="fileOrDirectory">
        /// The file or directory.
        /// </param>
        private static void PrintNotFoundError(FileSystemInfo fileOrDirectory)
        {
            Console.WriteLine();
            Console.WriteLine(fileOrDirectory.FullName + "does not exist");
            Console.WriteLine();
        }

        /// <summary>
        /// Process all uimf in a directory.
        /// </summary>
        /// <param name="directory">
        /// The directory to process.
        /// </param>
        private static void ProcessAllUimfInDirectory(DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                if (options.Verbose)
                {
                    Console.WriteLine("Starting to process UIMFs in " + directory.FullName);
                }

                var uimfs = directory.EnumerateFiles("*.uimf");

                foreach (var fileInfo in uimfs)
                {
                    ProcessUimf(fileInfo);
                }
            }
            else
            {
                PrintNotFoundError(directory);
            }
        }

        /// <summary>
        /// Processes all uimf in directory and all the sub directories in that directory.
        /// </summary>
        /// <param name="root">
        /// The root to start at.
        /// </param>
        private static void ProcessAllUimfInDirectoryRecursive(DirectoryInfo root)
        {
            if (root.Exists)
            {
                if (options.Verbose)
                {
                    Console.WriteLine("Started recursive work in " + root.FullName);
                }

                ProcessAllUimfInDirectory(root);

                Parallel.ForEach(root.EnumerateDirectories(), ProcessAllUimfInDirectoryRecursive);
            }
            else
            {
                PrintNotFoundError(root);
            }
        }

        /// <summary>
        /// Processes a single frame in a file.
        /// </summary>
        /// <param name="uimf">
        /// The uimf.
        /// </param>
        /// <param name="originFile">
        /// The origin file (we need to keep track o it for saving purposes) .
        /// </param>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", 
            Justification = "Reviewed. Suppression is OK here.")]
        private static void ProcessFrame(DataReader uimf, FileInfo originFile, IEnumerable<UimfExtraction> extractions , int frameNumber = 1)
        {
            if (frameNumber < 0)
            {
                throw new ArgumentOutOfRangeException("frameNumber", frameNumber, "Frame number must be greater than 0!");
            }
            var frameParams = uimf.GetFrameParams(frameNumber);

            if (frameParams == null)
            {
                // Frame number is out of range
                Console.WriteLine();
                Console.Error.WriteLine(
                    "ERROR: Somehow a frame number that doesn't exist was attempted to read" + Environment.NewLine
                    + "So we are not creating data for frame " + frameNumber + " Of " + Environment.NewLine
                    + originFile.FullName);
                Console.WriteLine();
                return;
            }

            foreach (var uimfExtraction in extractions)
            {
                switch (uimfExtraction)
                {
                    case UimfExtraction.Mz:
                        GetMz(uimf, originFile, frameNumber);
                        break;
                    case UimfExtraction.Tic:
                        GetTiC(uimf, originFile, frameNumber);
                        break;
                    case UimfExtraction.Xic:
                        GetXic(uimf, originFile, frameNumber);
                        break;
                    case UimfExtraction.Heatmap:
                        var heatmapData = GetFullHeatmapData(uimf, frameNumber);
                        var heatmapOutputFile = DataExporter.GetOutputLocation(originFile, "HeatMap", frameNumber);
                        DataExporter.OutputHeatMap(heatmapData, heatmapOutputFile, options.Verbose);
                        break;
                }
            }
           

            
        }

        /// <summary>
        /// Processes a uimf.
        /// </summary>
        /// <param name="file">
        /// The file to process.
        /// </param>
        private static void ProcessUimf(FileInfo file)
        {
            if (!file.Exists)
            {
                PrintNotFoundError(file);
                return;
            }

            using (var uimfReader = new DataReader(file.FullName))
            {
                if (options.Verbose)
                {
                    Console.WriteLine("Starting to process " + file.FullName);
                }

                var framecount = uimfReader.GetGlobalParams().NumFrames;
                if (options.AllFrames)
                {
                    for (var i = 0; i <= framecount; i++)
                    {
                        ProcessFrame(uimfReader, file, options.ExtractionTypes, i);
                        if (options.Verbose)
                        {
                            Console.WriteLine("Finished processing Frame " + i + " of " + file.FullName);
                        }
                    }
                }
                else
                {
                    ProcessFrame(uimfReader, file, options.ExtractionTypes, options.Frame);
                }
            }

            Console.WriteLine("Finished processing " + file.FullName);
        }

        #endregion
    }
}