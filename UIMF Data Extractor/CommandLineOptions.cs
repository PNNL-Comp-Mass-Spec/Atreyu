namespace UimfDataExtractor
{
    using System.Collections.Generic;

    using CommandLine;
    using CommandLine.Text;

    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to get the data from all frames or just the first one.
        /// </summary>
        [Option('a', "allframes", HelpText = "Outputs all frames to csv instead of just the first one.")]
        public bool AllFrames { get; set; }

        [Option('h', "heatmap", 
            HelpText = "specifies that you want the two-dimensional heatmap data *NotImplementedYet*")]
        public bool GetHeatmap { get; set; }

        [Option('m', "mz",
            HelpText = "specifies that you want the m/z data *NotImplementedYet*")]
        public bool GetMz { get; set; }

        [Option('t', "tic",
            HelpText = "specifies that you want the TiC data")]
        public bool GetTiC { get; set; }

        /// <summary>
        /// Gets or sets the input path.
        /// </summary>
        [Option('i', "input",
            Required = true,
            HelpText = "specify the directory containing the UIMFs to process")]
        public string InputPath { get; set; }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        [Option('o', "output",
            HelpText = "Specify the output directory. If left empty results will be written into the same directory as the input directory")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to recurse through subdirectories or not.
        /// </summary>
        [Option('r', "recursive", HelpText = "Recurse through files in sub directories.")]
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to dump everything to the command line.
        /// </summary>
        [Option('v', "verbose", HelpText = "Print details during execution. *NotImplementedWellYet*")]
        public bool Verbose { get; set; }

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
            help.AddPreOptionsLine("      If you do not specifiy an output format (heatmap, m/z, or tic) than the ");
            help.AddPreOptionsLine("      program will simply print what files is found.");
            help.AddPreOptionsLine("      If no output directory is specified, then it will default to the same folder");
            help.AddPreOptionsLine("      as the UIMF");

            help.AddOptions(this);

            return help;
        } 
    }
}