namespace UimfDataExtractor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using UimfDataExtractor.Data.Extractors;
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

            Console.WriteLine();
            Console.WriteLine("All done.");
        }

        #endregion

        #region Methods






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
            if (frameNumber < 1)
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
                if (Options.BulkPeakComparison || Options.PeakFind)
                {
                   var peaks = uimfExtraction.ComparePeaks(uimf, originFile, frameNumber);
                }
                else
                {
                    uimfExtraction.ExtractData(uimf, originFile, frameNumber);
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
                var uimfExtractions = new List<UimfExtraction>();

                foreach (var extractionType in options.ExtractionTypes)
                {
                    uimfExtractions.Add(UimfExtraction.UimfExtractionFactory(extractionType, options));
                }
                if (options.AllFrames)
                {
                    for (var i = 1; i <= framecount; i++)
                    {
                        ProcessFrame(uimfReader, file, uimfExtractions, i);
                        if (options.Verbose)
                        {
                            Console.WriteLine("Finished processing Frame " + i + " of " + file.FullName);
                        }
                    }
                }
                else
                {
                    ProcessFrame(uimfReader, file, uimfExtractions, options.Frame);
                }
            }

            Console.WriteLine("Finished processing " + file.FullName);
        }

        #endregion
    }
}