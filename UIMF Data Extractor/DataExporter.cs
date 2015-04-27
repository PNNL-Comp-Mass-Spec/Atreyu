namespace UimfDataExtractor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using UIMFLibrary;

    internal class DataExporter
    {
        /// <summary>
        /// The input directory.
        /// </summary>
        private static DirectoryInfo inputDirectory;

        /// <summary>
        /// The output directory.
        /// </summary>
        private static DirectoryInfo outputDirectory;

        public static DirectoryInfo InputDirectory
        {
            set
            {
                inputDirectory = value;
            }
            get
            {
                return inputDirectory;
            }
        }

        public static DirectoryInfo OutputDirectory
        {
            set
            {
                outputDirectory = value;
            }
            get
            {
                return outputDirectory;
            }
        }

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
        public static FileInfo GetOutputLocation(FileInfo originFile, string dataType, int frameNumber, string fileExtension = "csv")
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

        public static void OutputBulkXicPeakData(string dateString, string inputFolder, IEnumerable<BulkPeakData> xicPeaks, double mz, double tolerance)
        {
            var filename = dateString + "_" + inputFolder + "_" + "XiC_mz_" + mz + "_tolerance_"
                           + tolerance + "BulkPeakComparison.csv";
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

        public static void OutputBulkTicPeakData(string dateString, string inputFolder, IEnumerable<BulkPeakData> ticPeaks)
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

        public static void OutputBulkMzPeakData(string dateString, string inputFolder, IEnumerable<BulkPeakData> mzPeaks )
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
        /// Handles writing the heat map to file.
        /// </summary>
        /// <param name="data">
        /// The data tobe written.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
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

                for (var x = 0; x < data.GetLength(0); x++)
                {
                    var content = string.Empty;
                    for (var y = 0; y < data.GetLength(1); y++)
                    {
                        content += data[x, y].ToString("0.00") + ",";
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
        public static void OutputMz(IEnumerable<KeyValuePair<double, int>> mzKeyedIntensities, FileInfo outputFile, bool verbose)
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
        /// Handles Outputting the Total Ion Chromatogram data to file.
        /// </summary>
        /// <param name="timeKeyedIntensities">
        /// The time keyed intensities.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        public static void OutputTiCbyTime(IEnumerable<ScanInfo> timeKeyedIntensities, FileInfo outputFile, bool verbose)
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

        public static void OutputXiCbyTime(IEnumerable<KeyValuePair<double, double>> data, FileInfo outputFile, bool verbose)
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

        public static void OutputPeaks(PeakSet peakSet, FileInfo outputFile)
        {
            var serializer = new DataContractSerializer(typeof(PeakSet));
            using (var writer = getXmlWriter(outputFile))
            {
                serializer.WriteObject(writer, peakSet);
            }
        }
    }
}