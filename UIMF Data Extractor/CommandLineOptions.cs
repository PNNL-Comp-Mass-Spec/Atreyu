namespace UIMF_Data_Extractor
{
    using System.Collections.Generic;

    using CommandLine;
    using CommandLine.Text;

    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the input path.
        /// </summary>
        [Option('i', "input",
            Required = true,
            HelpText = "specify the directories containing the UIMFs to process")]
        public IEnumerable<string> InputPaths { get; set; }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        [Option('o', "output",
            DefaultValue = "",
            HelpText = "Specify the output directory. If left empty results will be written into the same directory as the input directory")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to dump everything to the command line.
        /// </summary>
        [Option('v', null, HelpText = "Print details during execution.")]
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