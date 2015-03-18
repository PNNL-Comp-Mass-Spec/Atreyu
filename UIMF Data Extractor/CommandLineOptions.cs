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
        [Option('a', "allframes", HelpText = "Outputs all frames to csv instead of just the first one. *NotImplementedYet)")]
        public bool AllFrames { get; set; }

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
            DefaultValue = "",
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
        [Option('v', "verbose", HelpText = "Print details during execution. *NotImplementedYet*")]
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

            help.AddOptions(this);

            return help;
        } 
    }
}