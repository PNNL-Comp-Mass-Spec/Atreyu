// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="PNNL">
//   Copyright PNNL 2015
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UimfDataExtractor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml;

    using MagnitudeConcavityPeakFinder;

    using UIMFLibrary;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        /// The input directory.
        /// </summary>
        private static DirectoryInfo inputDirectory;

        /// <summary>
        /// The options.
        /// </summary>
        private static CommandLineOptions options;

        /// <summary>
        /// The output directory.
        /// </summary>
        private static DirectoryInfo outputDirectory;

        /// <summary>
        /// All mz peaks found in all files.
        /// </summary>
        private static ConcurrentBag<BulkPeakData> bulkMzPeaks;
        
        /// <summary>
        /// All TiC peaks found in all files.
        /// </summary>
        private static ConcurrentBag<BulkPeakData> bulkTicPeaks;

        /// <summary>
        /// All XiC peaks found in all files.
        /// </summary>
        private static ConcurrentBag<BulkPeakData> bulkXicPeaks;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The main entry to the program.
        /// </summary>
        /// <param name="args">
        /// The arguments passed from the command line.
        /// </param>
        public static void Main(string[] args)
        {
            options = new CommandLineOptions();

            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                return;
            }

            // Domain logic here
            inputDirectory = new DirectoryInfo(options.InputPath);

            outputDirectory = string.IsNullOrWhiteSpace(options.OutputPath)
                                  ? inputDirectory
                                  : new DirectoryInfo(options.OutputPath);

            bulkMzPeaks = new ConcurrentBag<BulkPeakData>();
            bulkTicPeaks = new ConcurrentBag<BulkPeakData>();
            bulkXicPeaks = new ConcurrentBag<BulkPeakData>();

            if (options.Verbose)
            {
                Console.WriteLine("Verbose Active");
                Console.WriteLine("Input Directory: " + inputDirectory.FullName);
                Console.WriteLine("Output Directory: " + outputDirectory.FullName);
                Console.WriteLine("Run Recursively?: " + options.Recursive);
                Console.WriteLine("Process All Frames in File?: " + options.AllFrames);
            }

            if (options.Recursive)
            {
                ProcessAllUimfInDirectoryRecursive(inputDirectory);
            }
            else
            {
                ProcessAllUimfInDirectory(inputDirectory);
            }

            if (options.BulkPeakComparison)
            {
                OutputBulkPeaks();
            }

            Console.WriteLine();
            Console.WriteLine("All done, Exiting");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Makes sure that the output directory exists, checks to see if the file exists (and deletes it if it does)
        ///  then creates an output stream for writing text files.
        /// </summary>
        /// <param name="outputFile">
        /// The output file we want to create a text writer for.
        /// </param>
        /// <returns>
        /// The <see cref="StreamWriter"/>.
        /// </returns>
        private static StreamWriter GetFileStream(FileInfo outputFile)
        {
            var outstring = outputFile.DirectoryName;
            if (outstring == null)
            {
                Console.WriteLine();
                Console.Error.WriteLine(
                    "ERROR: We will expand upong this later, but we couldn't find the directory of the output file and it will not be output");
                Console.WriteLine();
                return null;
            }

            var outDirectory = new DirectoryInfo(outstring);

            if (!outDirectory.Exists)
            {
                outDirectory.Create();
            }

            if (outputFile.Exists)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "One of the output files (" + outputFile.FullName
                    + ") already exists, so we are deleting it Mwahahaha!");
                Console.WriteLine();
                outputFile.Delete();
            }

            return outputFile.CreateText();
        }

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
        /// Gets full heatmap data from the <see cref="DataReader"/> that the UIMF is open in. This is extremely slow to non-functioning
        /// </summary>
        /// <param name="uimf">
        /// The uimf that data is to be fetched from.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number that data is to be fetched.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
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
        /// The <see cref="List"/>.
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

            var frametype = GetFrameType(frameParams.GetValue(FrameParamKeyType.FrameType));

            double[] mzs;
            int[] intensities;
            uimf.GetSpectrum(frameNumber, frameNumber, frametype, 1, maxScans, out mzs, out intensities);
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
        /// The <see cref="List"/>.
        /// </returns>
        private static List<ScanInfo> GetFullScanInfo(DataReader uimf, int frameNumber)
        {
            return uimf.GetFrameScans(frameNumber);
        }

        private static List<KeyValuePair<double, double>> GetXicInfo(DataReader uimf, int frameNumber)
        {
            const DataReader.ToleranceType Tolerance = DataReader.ToleranceType.Thomson;

            var frametype = options.Getmsms ? DataReader.FrameType.MS2 : DataReader.FrameType.MS1;

            if (!uimf.DoesContainBinCentricData())
            {
                Console.WriteLine(uimf.UimfFilePath + " Does not have bin centric data which is required to get XiC");
                Console.WriteLine("starting to create it, this may take some time");
                var fileName = uimf.UimfFilePath;
                uimf.Dispose();

                var dataWriter = new DataWriter(fileName);
                dataWriter.CreateBinCentricTables();
                dataWriter.Dispose();

                uimf = new DataReader(fileName);
                Console.WriteLine("Finished Creating bin centric tables for " + uimf.UimfFilePath);
            }
            List<UIMFLibrary.IntensityPoint> xic;
            try
            {
                xic = uimf.GetXic(options.GetXiC, options.XicTolerance, frametype, Tolerance);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unable to get XiC on first attempt for " + uimf.UimfFilePath);
                var tempreader = new DataReader(uimf.UimfFilePath);
                try
                {
                    xic = tempreader.GetXic(options.GetXiC, options.XicTolerance, frametype, Tolerance);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Unable to get XiC on second attempt for " + uimf.UimfFilePath + ", we are not trying again.");
                    return null;
                }
            }

            var frameData = xic.Where(point => point.ScanLc == frameNumber - 1);
            
            var data = new List<KeyValuePair<double, double>>();

            foreach (var intensityPoint in frameData)
            {
                var driftTime = uimf.GetDriftTime(intensityPoint.ScanLc + 1, intensityPoint.ScanIms, true);
                data.Add(new KeyValuePair<double, double>(driftTime, intensityPoint.Intensity));
            }

            return data;
        }

        /// <summary>
        /// Gets output location, based on the name and location of the origin file, the type of data, and the current frame.
        /// </summary>
        /// <param name="originFile">
        /// The origin file, which we use for the first part of the name.
        /// </param>
        /// <param name="dataType">
        /// The data type which is put between underscores after the name, but before the frame number.
        /// </param>
        /// <param name="frameNumber">
        /// The frame number, if it is negative, no frame number will be printed
        /// </param>
        /// <param name="fileExtension">
        /// an optional parameter to specify the extension of the new filename, defaults to csv
        /// </param>
        /// <returns>
        /// The <see cref="FileInfo"/>.
        /// </returns>
        private static FileInfo GetOutputLocation(FileInfo originFile, string dataType, int frameNumber, string fileExtension = "csv")
        {
            var locationRelativeToInput = originFile.FullName.Substring(inputDirectory.FullName.Length + 1);
            var nestedLocation = Path.Combine(outputDirectory.FullName, locationRelativeToInput);
            var csvFullPath = Path.ChangeExtension(nestedLocation, fileExtension);
            var oldName = new FileInfo(csvFullPath);

            var oldDir = oldName.DirectoryName;
            var newName = Path.GetFileNameWithoutExtension(csvFullPath);

            newName += "_" + dataType;

            if (frameNumber >= 0)
            {
                newName += "_" + frameNumber.ToString("0000");
            }

            newName += Path.GetExtension(oldName.FullName);

            return oldDir == null ? null : new FileInfo(Path.Combine(oldDir, newName));
        }

        private static XmlWriter getXmlWriter(FileInfo file)
        {
            return new XmlTextWriter(GetFileStream(file));
        }

        /// <summary>
        /// Outputs the bulk peaks collected during the run.
        /// </summary>
        private static void OutputBulkPeaks()
        {
            var dateString = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var inputFolder = inputDirectory.Name;


            if (options.GetMz)
            {
                var filename = dateString + "_" + inputFolder + "_" + "mz" + "_" + "BulkPeakComparison.csv";
                var fullLocation = Path.Combine(outputDirectory.FullName, filename);
                var file = new FileInfo(fullLocation);
                using (var writer = GetFileStream(file))
                {
                    writer.WriteLine("File,Frame,Location,Full Width Half Max,Resolving Power");
                    foreach (var bulkPeakData in bulkMzPeaks)
                    {
                        var temp = bulkPeakData.FileName + ",";
                        temp += bulkPeakData.FrameNumber + ",";
                        temp += bulkPeakData.Location + ",";
                        temp += bulkPeakData.FullWidthHalfMax + ",";
                        temp += bulkPeakData.ResolvingPower;
                        writer.WriteLine(temp);
                    }
                }
            }


            if (options.GetTiC)
            {
                var filename = dateString + "_" + inputFolder + "_" + "tic" + "_" + "BulkPeakComparison.csv";
                var fullLocation = Path.Combine(outputDirectory.FullName, filename);
                var file = new FileInfo(fullLocation);
                using (var writer = GetFileStream(file))
                {
                    writer.WriteLine("File,Frame,Location,Full Width Half Max,Resolving Power");
                    foreach (var bulkPeakData in bulkTicPeaks)
                    {
                        var temp = bulkPeakData.FileName + ",";
                        temp += bulkPeakData.FrameNumber + ",";
                        temp += bulkPeakData.Location + ",";
                        temp += bulkPeakData.FullWidthHalfMax + ",";
                        temp += bulkPeakData.ResolvingPower;
                        writer.WriteLine(temp);
                    }
                }
            }

            if (options.GetXiC > 0)
            {
                var filename = dateString + "_" + inputFolder + "_" + "XiC_mz_" + options.GetXiC + "_tolerance_"
                               + options.XicTolerance + "BulkPeakComparison.csv";
                var fullLocation = Path.Combine(outputDirectory.FullName, filename);
                var file = new FileInfo(fullLocation);
                using (var writer = GetFileStream(file))
                {
                    writer.WriteLine("File,Frame,Drift Time,Full Width Half Max,Resolving Power");
                    foreach (var bulkPeakData in bulkXicPeaks)
                    {
                        var temp = bulkPeakData.FileName + ",";
                        temp += bulkPeakData.FrameNumber + ",";
                        temp += bulkPeakData.Location + ",";
                        temp += bulkPeakData.FullWidthHalfMax + ",";
                        temp += bulkPeakData.ResolvingPower;
                        writer.WriteLine(temp);
                    }
                }
            }
        }

        /// <summary>
        /// Handles writing the heat map to file.
        /// </summary>
        /// <param name="data">
        /// The data tobe written.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        private static void OutputHeatMap(double[,] data, FileInfo outputFile)
        {
            using (var stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    PrintFileCreationError(outputFile.FullName);
                    return;
                }

                for (var x = 0; x < data.GetLength(0); x++)
                {
                    var content = string.Empty;
                    for (var y = 0; y < data.GetLength(1); y++)
                    {
                        content += data[x, y].ToString("0.00") + ",";
                    }

                    stream.WriteLine(content);
                }

                if (options.Verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
        }

        /// <summary>
        /// Handles writing the mass over charge information to file.
        /// </summary>
        /// <param name="mzKeyedIntensities">
        /// The Mass over charge keyed intensities.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        private static void OutputMz(IEnumerable<KeyValuePair<double, int>> mzKeyedIntensities, FileInfo outputFile)
        {
            using (var stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    PrintFileCreationError(outputFile.FullName);
                    return;
                }

                foreach (var kvp in mzKeyedIntensities)
                {
                    stream.WriteLine(kvp.Key + ", " + kvp.Value);
                }

                if (options.Verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
        }

        /// <summary>
        /// Handles Outputting the Total Ion Chromatogram data to file.
        /// </summary>
        /// <param name="timeKeyedIntensities">
        /// The time keyed intensities.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        private static void OutputTiCbyTime(IEnumerable<ScanInfo> timeKeyedIntensities, FileInfo outputFile)
        {
            using (var stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    PrintFileCreationError(outputFile.FullName);
                    return;
                }

                foreach (var timeKeyedIntensity in timeKeyedIntensities)
                {
                    stream.WriteLine(timeKeyedIntensity.DriftTime + ", " + timeKeyedIntensity.TIC);
                }

                if (options.Verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
        }


        private static void OutputXiCbyTime(IEnumerable<KeyValuePair<double, double>> data, FileInfo outputFile)
        {
            using (var stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    PrintFileCreationError(outputFile.FullName);
                    return;
                }

                foreach (var kvp in data)
                {
                    stream.WriteLine(kvp.Key + ", " + kvp.Value);
                }

                if (options.Verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
        }

        /// <summary>
        /// Find the peaks in the current data set and adds an annotation point with the resolution to the m/z.
        /// </summary>
        /// <param name="dataList">
        /// The data List.
        /// </param>
        /// <returns>
        /// The <see cref="List"/> of peaks.
        /// </returns>
        private static PeakSet FindPeaks(IReadOnlyList<KeyValuePair<double, double>> dataList)
        {
            var datapointList = new PeakSet();
            const int Precision = 100000;

            var peakDetector = new PeakDetector();

            var finderOptions = PeakDetector.GetDefaultSICPeakFinderOptions();

            List<double> smoothedY;

            // Create a new dictionary so we don't modify the original one
            var tempFrameList = new List<KeyValuePair<int, double>>(dataList.Count);

            // We have to give it integers, but we need the mz, so we will multiply the mz by the precision
            // and later get the correct value back by dividing it out again
            for (var i = 0; i < dataList.Count; i++)
            {
                tempFrameList.Add(
                    new KeyValuePair<int, double>((int)(dataList[i].Key * Precision), dataList[i].Value));
            }

            // I am not sure what this does but in the example exe file that came with the library
            // they used half of the length of the list in their previous examples and this seems to work
            var originalpeakLocation = tempFrameList.Count / 2;

            var allPeaks = peakDetector.FindPeaks(
                finderOptions,
                tempFrameList.OrderBy(x => x.Key).ToList(),
                originalpeakLocation,
                out smoothedY);

            ////var topThreePeaks = allPeaks.OrderByDescending(peak => smoothedY[peak.LocationIndex]).Take(3);

            foreach (var peak in allPeaks)
            {
                const double Tolerance = 0.01;
                var centerPoint = tempFrameList.ElementAt(peak.LocationIndex);
                var offsetCenter = centerPoint.Key; // + firstpoint.Key; 
                var intensity = centerPoint.Value;
                var smoothedPeakIntensity = smoothedY[peak.LocationIndex];

                var realCenter = (double)offsetCenter / Precision;
                var halfmax = smoothedPeakIntensity / 2.0;

                var currPoint = new KeyValuePair<int, double>(0, 0);
                var currPointIndex = 0;
                double leftMidpoint = 0;
                double rightMidPoint = 0;

                var allPoints = new List<PointInformation>();
                var leftSidePeaks = new List<KeyValuePair<int, double>>();
                for (var l = peak.LeftEdge; l < peak.LocationIndex && l < tempFrameList.Count; l++)
                {
                    leftSidePeaks.Add(tempFrameList[l]);
                    allPoints.Add(new PointInformation
                                      {
                                          Location = (double)tempFrameList[l].Key / Precision,
                                          Intensity = tempFrameList[l].Value,
                                          SmoothedIntensity = smoothedY[l]
                                      });
                }

                var rightSidePeaks = new List<KeyValuePair<int, double>>();
                for (var r = peak.LocationIndex; r < peak.RightEdge && r < tempFrameList.Count; r++)
                {
                    rightSidePeaks.Add(tempFrameList[r]);
                    allPoints.Add(new PointInformation
                    {
                        Location = (double)tempFrameList[r].Key / Precision,
                        Intensity = tempFrameList[r].Value,
                        SmoothedIntensity = smoothedY[r]
                    });
                }

                // find the left side half max
                foreach (var leftSidePeak in leftSidePeaks)
                {
                    var prevPoint = currPoint;
                    currPoint = leftSidePeak;
                    var prevPointIndex = currPointIndex;

                    currPointIndex = tempFrameList.BinarySearch(
                        currPoint,
                        Comparer<KeyValuePair<int, double>>.Create((left, right) => left.Key - right.Key));

                    if (smoothedY[currPointIndex] < halfmax)
                    {
                        continue;
                    }

                    if (Math.Abs(smoothedY[currPointIndex] - halfmax) < Tolerance)
                    {
                        leftMidpoint = currPoint.Key;
                        continue;
                    }

                    ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                    double a1 = prevPoint.Key;
                    double a2 = currPoint.Key;
                    double c = halfmax;
                    double b1 = smoothedY[prevPointIndex];
                    double b2 = smoothedY[currPointIndex];

                    leftMidpoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                    break;
                }

                // find the right side of the half max
                foreach (var rightSidePeak in rightSidePeaks)
                {
                    var prevPoint = currPoint;
                    currPoint = rightSidePeak;
                    var prevPointIndex = currPointIndex;
                    currPointIndex = tempFrameList.BinarySearch(
                        currPoint,
                        Comparer<KeyValuePair<int, double>>.Create((left, right) => left.Key - right.Key));

                    if (smoothedY[currPointIndex] > halfmax || smoothedY[currPointIndex] < 0)
                    {
                        continue;
                    }

                    if (Math.Abs(smoothedY[currPointIndex] - halfmax) < Tolerance)
                    {
                        rightMidPoint = currPoint.Key;
                        continue;
                    }

                    ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                    double a1 = prevPoint.Key;
                    double a2 = currPoint.Key;
                    double c = halfmax;
                    double b1 = smoothedY[prevPointIndex];
                    double b2 = smoothedY[currPointIndex];

                    rightMidPoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                    break;
                }

                var correctedRightMidPoint = rightMidPoint / Precision;
                var correctedLeftMidPoint = leftMidpoint / Precision;
                var fullWidthHalfMax = correctedRightMidPoint - correctedLeftMidPoint;
                var resolution = realCenter / fullWidthHalfMax;

                var temp = new PeakInformation
                {
                    AreaUnderThePeak = peak.Area,
                    FullWidthHalfMax = fullWidthHalfMax,
                    Intensity = intensity,
                    LeftMidpoint = correctedLeftMidPoint,
                    PeakCenter = realCenter,
                    RightMidpoint = correctedRightMidPoint,
                    ResolvingPower = resolution,
                    SmoothedIntensity = smoothedPeakIntensity,
                    TotalDataPointSet = allPoints
                };

                if (temp.ResolvingPower > 0 && !double.IsInfinity(temp.ResolvingPower))
                {
                    datapointList.Peaks.Add(temp);
                }
            }

            return datapointList;

            ////var tempList =
            ////    datapointList.Where(x => !double.IsInfinity(x.ResolvingPower)).OrderByDescending(x => x.Intensity).Take(10);

            ////// Put the points on the graph
            ////foreach (var resolutionDatapoint in tempList)
            ////{
            ////    var resolutionString = resolutionDatapoint.ResolvingPower.ToString("F1", CultureInfo.InvariantCulture);
            ////    var annotationText = "Peak Location:" + resolutionDatapoint.PeakCenter + Environment.NewLine + "Intensity:"
            ////                         + resolutionDatapoint.Intensity + Environment.NewLine + "ResolvingPower:"
            ////                         + resolutionString;
            ////}
        }


        /// <summary>
        /// Print a file creation error.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        private static void PrintFileCreationError(string filename)
        {
            Console.WriteLine();
            Console.Error.WriteLine(
                "We were unable to create" + filename
                + "so we aren't outputting data to it either, we are petty like that");
            Console.WriteLine();
        }

        /// <summary>
        /// Print a not found error.
        /// </summary>
        /// <param name="fileOrDirectory">
        /// The file or directory.
        /// </param>
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

                Parallel.ForEach(uimfs, ProcessUimf);
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
        private static void ProcessFrame(DataReader uimf, FileInfo originFile, int frameNumber = 1)
        {
            // eventually I will also have methods for mz and heatmap,
            //// which is why we are sperating this into seperate funtions now
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

            ////if (options.GetHeatmap)
            ////{
            ////    var heatmapData = GetFullHeatmapData(uimf, frameNumber);
            ////    var heatmapOutputFile = GetOutputLocation(originFile, "HeatMap", frameNumber);
            ////    OutputHeatMap(heatmapData, heatmapOutputFile);
            ////}
            
            if (options.GetMz)
            {
                var mzData = GetMZ(uimf, originFile, frameNumber);
            }

            if (options.GetTiC)
            {
                var ticData = GetTiC(uimf, originFile, frameNumber);
            }

            if (options.GetXiC > 0)
            {
                var xicData = GetXic(uimf, originFile, frameNumber);
            }

            if (options.Verbose)
            {
                Console.WriteLine("Finished processing Frame " + frameNumber + " of " + originFile.FullName);
            }
        }

        private static List<KeyValuePair<double, double>> GetXic(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var xicData = GetXicInfo(uimf, frameNumber);
            var xicOutputFile = GetOutputLocation(
                originFile,
                "XiC_mz_" + options.GetXiC + "_tolerance_" + options.XicTolerance + "_Frame",
                frameNumber);

            if (xicData != null)
            {
                OutputXiCbyTime(xicData, xicOutputFile);

                if (options.PeakFind || options.BulkPeakComparison)
                {
                    var xicPeaks = FindPeaks(xicData);
                    if (options.PeakFind)
                    {
                        var xicPeakOutputLocation = GetOutputLocation(
                            originFile,
                            "XiC_Peaks_mz_" + options.GetXiC + "_tolerance_" + options.XicTolerance,
                            frameNumber,
                            "xml");
                        OutputPeaks(xicPeaks, xicPeakOutputLocation);
                    }

                    if (options.BulkPeakComparison)
                    {
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
                            bulkXicPeaks.Add(temp);
                        }
                    }
                }
            }
            return xicData;
        }

        private static List<KeyValuePair<double, int>> GetMZ(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var mzData = GetFullMzInfo(uimf, frameNumber);
            if (mzData == null)
            {
                Console.Error.WriteLine(
                    "ERROR: We had a problem getting the data for the MZ of" + Environment.NewLine + " frame " + frameNumber
                    + " in " + originFile.FullName);
            }
            else
            {
                var mzOutputFile = GetOutputLocation(originFile, "Mz", frameNumber);
                OutputMz(mzData, mzOutputFile);
                if (options.PeakFind || options.BulkPeakComparison)
                {
                    var doubleMzData =
                        mzData.Select(keyValuePair => new KeyValuePair<double, double>(keyValuePair.Key, keyValuePair.Value))
                            .ToList();
                    var mzpeaks = FindPeaks(doubleMzData);

                    if (options.PeakFind)
                    {
                        var mzPeakOutputLocation = GetOutputLocation(originFile, "Mz_Peaks", frameNumber, "xml");
                        OutputPeaks(mzpeaks, mzPeakOutputLocation);
                    }

                    if (options.BulkPeakComparison)
                    {
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
                            bulkMzPeaks.Add(temp);
                        }
                    }
                }
            }
            return mzData;
        }

        private static List<ScanInfo> GetTiC(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var ticData = GetFullScanInfo(uimf, frameNumber);
            var ticOutputFile = GetOutputLocation(originFile, "TiC", frameNumber);

            OutputTiCbyTime(ticData, ticOutputFile);

            if (options.PeakFind || options.BulkPeakComparison)
            {
                var doubleTicData =
                    ticData.Select(scanInfo => new KeyValuePair<double, double>(scanInfo.DriftTime, scanInfo.TIC)).ToList();

                var ticPeaks = FindPeaks(doubleTicData);
                if (options.PeakFind)
                {
                    var mzPeakOutputLocation = GetOutputLocation(originFile, "TiC_Peaks", frameNumber, "xml");
                    OutputPeaks(ticPeaks, mzPeakOutputLocation);
                }

                if (options.BulkPeakComparison)
                {
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
                        bulkTicPeaks.Add(temp);
                    }
                }
            }
            return ticData;
        }

        private static void OutputPeaks(PeakSet peakSet, FileInfo outputFile)
        {
            var serializer = new DataContractSerializer(typeof(PeakSet));
            using (var writer = getXmlWriter(outputFile))
            {
                serializer.WriteObject(writer, peakSet);
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
                    // ReSharper disable once AccessToDisposedClosure
                    for (var i = 0; i < framecount; i++)
                    {
                        ProcessFrame(uimfReader, file, i);
                    }
                }
                else
                {
                    ProcessFrame(uimfReader, file);
                }

            }

            Console.WriteLine("Finished processing " + file.FullName);
        }

        #endregion
    }
}