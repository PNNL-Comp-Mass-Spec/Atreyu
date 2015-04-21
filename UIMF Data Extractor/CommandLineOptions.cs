// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="PNNL">
//   Copyright PNNL 2015
// </copyright>
// <summary>
//   The command line options for the UIMF Data Extractor
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UimfDataExtractor
{
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// The command line options.
    /// </summary>
    public class CommandLineOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to get the data from all frames or just the first one.
        /// </summary>
        [Option('a', "allframes", HelpText = "Outputs all frames to csv instead of just the first one.")]
        public bool AllFrames { get; set; }

        [Option('b', "bulkpeakcomparison", HelpText = "Outputs a file that lists all peak's location and Full Width Half Max")]
        public bool BulkPeakComparison { get; set; }

        ////[Option('h', "heatmap", 
        ////    HelpText = "specifies that you want the two-dimensional heatmap data *NotImplementedYet*")]
        ////public bool GetHeatmap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output mass over charge data.
        /// </summary>
        [Option('m', "mz", HelpText = "specifies that you want the m/z data")]
        public bool GetMz { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output total ion chromatogram data.
        /// </summary>
        [Option('t', "tic", HelpText = "specifies that you want the TiC data")]
        public bool GetTiC { get; set; }

        /// <summary>
        /// Gets or sets the input path.
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "specify the directory containing the UIMFs to process")]
        public string InputPath { get; set; }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        [Option('o', "output", 
            HelpText =
                "Specify the output directory. If left empty results will be written into the same directory as the input directory")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to peak find and print out information.
        /// </summary>
        [Option('p', "peakfind", HelpText = "Prints out a file listing the peaks for the m/z and/or TiC based on what output is selected")]
        public bool PeakFind { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to recursively process through subdirectories or not.
        /// </summary>
        [Option('r', "recursive", HelpText = "Recurse through files in sub directories.")]
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to flood the command line with relatively useless information.
        /// </summary>
        [Option('v', "verbose", HelpText = "Print details during execution. *NotImplementedWellYet*")]
        public bool Verbose { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Explains how to use the program.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
                           {
                               Heading =
                                   new HeadingInfo(
                                   "UIMF Data Extractor", 
                                   typeof(Program).Assembly.GetName().Version.ToString()), 
                               Copyright = new CopyrightInfo("PNNL", 2015), 
                               AdditionalNewLineAfterOption = true, 
                               AddDashesToOption = true
                           };

            help.AddPreOptionsLine(string.Empty);
            help.AddPreOptionsLine("    This application batch processes Unified Ion Mobility Files (UIMF)");
            help.AddPreOptionsLine("    to output the raw data into Comma Seperated Value (csv) format");
            help.AddPreOptionsLine(string.Empty);
            help.AddPreOptionsLine("    Usage:");
            help.AddPreOptionsLine("      UIMFDataExtractor.exe -i SOURCEFOLDER is the minimum requirement to run");
            help.AddPreOptionsLine("      If you do not specifiy an output format (m/z or tic) than the ");
            help.AddPreOptionsLine("      program will simply print what files it found.");
            help.AddPreOptionsLine("      If no output directory is specified, then it will default to the same");
            help.AddPreOptionsLine("      folder as the UIMF");

            help.AddOptions(this);

            return help;
        }

        #endregion
    }
}