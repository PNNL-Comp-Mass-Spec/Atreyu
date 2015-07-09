// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataExporter.cs" company="Pacific Northwest National Laboratory">
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
//   The data exporter class handles everything required to actually save the data to file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UimfDataExtractor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using UIMFLibrary;

    using Utilities.Models;

    /// <summary>
    /// The data exporter class.
    /// </summary>
    internal class DataExporter
    {
        #region Static Fields

        /// <summary>
        /// The input directory.
        /// </summary>
        private static DirectoryInfo inputDirectory;

        /// <summary>
        /// The output directory.
        /// </summary>
        private static DirectoryInfo outputDirectory;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the input directory.
        /// </summary>
        public static DirectoryInfo InputDirectory
        {
            get
            {
                return inputDirectory;
            }

            set
            {
                inputDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public static DirectoryInfo OutputDirectory
        {
            get
            {
                return outputDirectory;
            }

            set
            {
                outputDirectory = value;
            }
        }

        #endregion

        #region Public Methods and Operators

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
        public static FileInfo GetOutputLocation(
            FileInfo originFile, 
            string dataType, 
            int frameNumber, 
            string fileExtension = "csv")
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

        /// <summary>
        /// The output bulk mz peak data method.
        /// </summary>
        /// <param name="dateString">
        /// The date string.
        /// </param>
        /// <param name="inputFolder">
        /// The input folder.
        /// </param>
        /// <param name="mzPeaks">
        /// The mz peaks.
        /// </param>
        public static void OutputBulkMzPeakData(
            string dateString, 
            string inputFolder, 
            IEnumerable<BulkPeakData> mzPeaks)
        {
            var filename = dateString + "_" + inputFolder + "_" + "mz" + "_" + "BulkPeakComparison.csv";
            var fullLocation = Path.Combine(outputDirectory.FullName, filename);
            var file = new FileInfo(fullLocation);
            using (var writer = GetFileStream(file))
            {
                writer.WriteLine("File,Frame,Location,Full Width Half Max,Resolving Power");
                foreach (var bulkPeakData in mzPeaks)
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

        /// <summary>
        /// The output bulk tic peak data.
        /// </summary>
        /// <param name="dateString">
        /// The date string.
        /// </param>
        /// <param name="inputFolder">
        /// The input folder.
        /// </param>
        /// <param name="ticPeaks">
        /// The tic peaks.
        /// </param>
        public static void OutputBulkTicPeakData(
            string dateString, 
            string inputFolder, 
            IEnumerable<BulkPeakData> ticPeaks)
        {
            var filename = dateString + "_" + inputFolder + "_" + "tic" + "_" + "BulkPeakComparison.csv";
            var fullLocation = Path.Combine(outputDirectory.FullName, filename);
            var file = new FileInfo(fullLocation);
            using (var writer = GetFileStream(file))
            {
                writer.WriteLine("File,Frame,Location,Full Width Half Max,Resolving Power");
                foreach (var bulkPeakData in ticPeaks)
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

        /// <summary>
        /// The output bulk xic peak data.
        /// </summary>
        /// <param name="dateString">
        /// The date string.
        /// </param>
        /// <param name="inputFolder">
        /// The input folder.
        /// </param>
        /// <param name="xicPeaks">
        /// The xic peaks.
        /// </param>
        /// <param name="mz">
        /// The mz.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance.
        /// </param>
        public static void OutputBulkXicPeakData(
            string dateString, 
            string inputFolder, 
            IEnumerable<BulkPeakData> xicPeaks, 
            double mz, 
            double tolerance)
        {
            var filename = dateString + "_" + inputFolder + "_" + "XiC_mz_" + mz + "_tolerance_" + tolerance
                           + "BulkPeakComparison.csv";
            var fullLocation = Path.Combine(outputDirectory.FullName, filename);
            var file = new FileInfo(fullLocation);
            using (var writer = GetFileStream(file))
            {
                writer.WriteLine("File,Frame,Drift Time,Full Width Half Max,Resolving Power");
                foreach (var bulkPeakData in xicPeaks)
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

        /// <summary>
        /// Handles writing the heat map to file.
        /// </summary>
        /// <param name="data">
        /// The data to be written.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        /// <param name="verbose">
        /// The verbose.
        /// </param>
        public static void OutputHeatMap(double[,] data, FileInfo outputFile, bool verbose)
        {
            using (var stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    PrintFileCreationError(outputFile.FullName);
                    return;
                }

                for (var bin = 0; bin < data.GetLength(1); bin++)
                {
                    var content = string.Empty;
                    for (var scan = 0; scan < data.GetLength(0); scan++)
                    {
                        content += data[scan, bin].ToString("0.00") + ",";
                    }

                    stream.WriteLine(content);
                }

                if (verbose)
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
        /// <param name="verbose">
        /// The verbose.
        /// </param>
        public static void OutputMz(
            IEnumerable<KeyValuePair<double, int>> mzKeyedIntensities, 
            FileInfo outputFile, 
            bool verbose)
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

                if (verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
        }

        /// <summary>
        /// The output peaks.
        /// </summary>
        /// <param name="peakSet">
        /// The peak set.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        public static void OutputPeaks(PeakSet peakSet, FileInfo outputFile)
        {
            var serializer = new DataContractSerializer(typeof(PeakSet));
            using (var writer = GetXmlWriter(outputFile))
            {
                serializer.WriteObject(writer, peakSet);
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
        /// <param name="verbose">
        /// The verbose.
        /// </param>
        public static void OutputTiCbyTime(
            IEnumerable<ScanInfo> timeKeyedIntensities, 
            FileInfo outputFile, 
            bool verbose)
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

                if (verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
        }

        /// <summary>
        /// The output xi cby time.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        /// <param name="verbose">
        /// The verbose.
        /// </param>
        public static void OutputXiCbyTime(
            IEnumerable<KeyValuePair<double, double>> data, 
            FileInfo outputFile, 
            bool verbose)
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

                if (verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
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
                    "ERROR: We will expand upon this later, but we couldn't find the directory of the output file and it will not be output");
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
        /// The get xml writer.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="XmlWriter"/>.
        /// </returns>
        private static XmlWriter GetXmlWriter(FileInfo file)
        {
            return new XmlTextWriter(GetFileStream(file));
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

        #endregion
    }
}