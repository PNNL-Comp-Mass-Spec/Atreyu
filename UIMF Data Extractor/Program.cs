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
    using System.IO;
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
            UimfProcessor.Options = new CommandLineOptions();

            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, UimfProcessor.Options))
            {
                return;
            }

            // Domain logic here
            DataExporter.InputDirectory = new DirectoryInfo(UimfProcessor.Options.InputPath);

            DataExporter.OutputDirectory = string.IsNullOrWhiteSpace(UimfProcessor.Options.OutputPath)
                                  ? DataExporter.InputDirectory
                                  : new DirectoryInfo(UimfProcessor.Options.OutputPath);

            UimfProcessor.BulkMzPeaks = new ConcurrentBag<BulkPeakData>();
            UimfProcessor.BulkTicPeaks = new ConcurrentBag<BulkPeakData>();
            UimfProcessor.BulkXicPeaks = new ConcurrentBag<BulkPeakData>();

            if (UimfProcessor.Options.Verbose)
            {
                Console.WriteLine("Verbose Active");
                Console.WriteLine("Input Directory: " + DataExporter.InputDirectory.FullName);
                Console.WriteLine("Output Directory: " + DataExporter.OutputDirectory.FullName);
                Console.WriteLine("Run Recursively?: " + UimfProcessor.Options.Recursive);
                Console.WriteLine("Process All Frames in File?: " + UimfProcessor.Options.AllFrames);
            }

            if (UimfProcessor.Options.Recursive)
            {
                UimfProcessor.ProcessAllUimfInDirectoryRecursive(DataExporter.InputDirectory);
            }
            else
            {
                UimfProcessor.ProcessAllUimfInDirectory(DataExporter.InputDirectory);
            }

            if (UimfProcessor.Options.BulkPeakComparison)
            {
                UimfProcessor.OutputBulkPeaks();
            }

            Console.WriteLine();
            Console.WriteLine("All done, Exiting");
        }

        #endregion

        #region Methods

        #endregion
    }
}