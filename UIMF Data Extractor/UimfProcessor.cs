// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UimfProcessor.cs" company="Pacific Northwest National Laboratory">
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
//   The uimf processor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UimfDataExtractor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MagnitudeConcavityPeakFinder;

    using UIMFLibrary;

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
        /// The extract data method is the entry funtion to this process, it assumes you have set all the options already.
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
        /// Find the peaks in the current data set and adds an annotation point with the resolution to the m/z.
        /// </summary>
        /// <param name="dataList">
        /// The data List.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/> of peaks.
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
                tempFrameList.Add(new KeyValuePair<int, double>((int)(dataList[i].Key * Precision), dataList[i].Value));
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
                    allPoints.Add(
                        new PointInformation
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
                    allPoints.Add(
                        new PointInformation
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
        /// Gets full heatmap data from the <see cref="DataReader"/> that the UIMF is open in.
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
        /// A List of Key, Value pairs, key is the m/z and the value is the intesity at that point.
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
            }
            else
            {
                var mzOutputFile = DataExporter.GetOutputLocation(originFile, "Mz", frameNumber);
                DataExporter.OutputMz(mzData, mzOutputFile, options.Verbose);
                if (options.PeakFind || options.BulkPeakComparison)
                {
                    var doubleMzData =
                        mzData.Select(
                            keyValuePair => new KeyValuePair<double, double>(keyValuePair.Key, keyValuePair.Value))
                            .ToList();
                    var mzpeaks = FindPeaks(doubleMzData);

                    if (options.PeakFind)
                    {
                        var mzPeakOutputLocation = DataExporter.GetOutputLocation(
                            originFile, 
                            "Mz_Peaks", 
                            frameNumber, 
                            "xml");
                        DataExporter.OutputPeaks(mzpeaks, mzPeakOutputLocation);
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
                            BulkMzPeaks.Add(temp);
                        }
                    }
                }
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

            if (options.PeakFind || options.BulkPeakComparison)
            {
                var doubleTicData =
                    ticData.Select(scanInfo => new KeyValuePair<double, double>(scanInfo.DriftTime, scanInfo.TIC))
                        .ToList();

                var ticPeaks = FindPeaks(doubleTicData);
                if (options.PeakFind)
                {
                    var mzPeakOutputLocation = DataExporter.GetOutputLocation(
                        originFile, 
                        "TiC_Peaks", 
                        frameNumber, 
                        "xml");
                    DataExporter.OutputPeaks(ticPeaks, mzPeakOutputLocation);
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
                        BulkTicPeaks.Add(temp);
                    }
                }
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
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static List<KeyValuePair<double, double>> GetXic(DataReader uimf, FileInfo originFile, int frameNumber)
        {
            var xicData = GetXicInfo(uimf, frameNumber, options.GetXiC, options.XicTolerance, options.Getmsms);
            var xicOutputFile = DataExporter.GetOutputLocation(
                originFile, 
                "XiC_mz_" + options.GetXiC + "_tolerance_" + options.XicTolerance + "_Frame", 
                frameNumber);

            if (xicData != null)
            {
                DataExporter.OutputXiCbyTime(xicData, xicOutputFile, options.Verbose);

                if (options.PeakFind || options.BulkPeakComparison)
                {
                    var xicPeaks = FindPeaks(xicData);
                    if (options.PeakFind)
                    {
                        var xicPeakOutputLocation = DataExporter.GetOutputLocation(
                            originFile, 
                            "XiC_Peaks_mz_" + options.GetXiC + "_tolerance_" + options.XicTolerance + "_Frame", 
                            frameNumber, 
                            "xml");
                        DataExporter.OutputPeaks(xicPeaks, xicPeakOutputLocation);
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
                            BulkXicPeaks.Add(temp);
                        }
                    }
                }
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
        /// The get msms.
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
                uimf.Dispose();

                var dataWriter = new DataWriter(fileName);
                dataWriter.CreateBinCentricTables();
                dataWriter.Dispose();

                uimf = new DataReader(fileName);
                Console.WriteLine("Finished Creating bin centric tables for " + uimf.UimfFilePath);
            }

            List<IntensityPoint> xic;
            try
            {
                xic = uimf.GetXic(xicMz, tolerance, frametype, Tolerance);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Unable to get XiC on first attempt for " + uimf.UimfFilePath);
                var tempreader = new DataReader(uimf.UimfFilePath);

                try
                {
                    xic = tempreader.GetXic(xicMz, tolerance, frametype, Tolerance);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine(
                        "Unable to get XiC on second attempt for " + uimf.UimfFilePath + ", we are not trying again.");
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
        /// Outputs the bulk peaks collected during the run.
        /// </summary>
        private static void OutputBulkPeaks()
        {
            var dateString = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var inputFolder = DataExporter.InputDirectory.Name;

            if (options.GetMz)
            {
                DataExporter.OutputBulkMzPeakData(dateString, inputFolder, BulkMzPeaks);
            }

            if (options.GetTiC)
            {
                DataExporter.OutputBulkTicPeakData(dateString, inputFolder, BulkTicPeaks);
            }

            if (options.GetXiC > 0)
            {
                DataExporter.OutputBulkXicPeakData(
                    dateString, 
                    inputFolder, 
                    BulkXicPeaks, 
                    options.GetXiC, 
                    options.XicTolerance);
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

            if (options.GetHeatmap)
            {
                var heatmapData = GetFullHeatmapData(uimf, frameNumber);
                var heatmapOutputFile = DataExporter.GetOutputLocation(originFile, "HeatMap", frameNumber);
                DataExporter.OutputHeatMap(heatmapData, heatmapOutputFile, options.Verbose);
            }

            if (options.GetMz)
            {
                GetMz(uimf, originFile, frameNumber);
            }

            if (options.GetTiC)
            {
                GetTiC(uimf, originFile, frameNumber);
            }

            if (options.GetXiC > 0)
            {
                GetXic(uimf, originFile, frameNumber);
            }

            if (options.Verbose)
            {
                Console.WriteLine("Finished processing Frame " + frameNumber + " of " + originFile.FullName);
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
                    for (var i = 0; i <= framecount; i++)
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