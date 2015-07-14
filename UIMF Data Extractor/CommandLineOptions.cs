// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="Pacific Northwest National Laboratory">
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
//   The command line options for the UIMF Data Extractor
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UimfDataExtractor
{
    using System;
    using System.Linq;

    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// The command line options.
    /// </summary>
    public class CommandLineOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to get the data from all frames, ignoring the single frame option.
        /// </summary>
        [Option('a', "allframes", HelpText = "Outputs all frames to csv instead of just the first one.")]
        public bool AllFrames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output a bulk peak comparison file.
        /// </summary>
        [Option('b', "bulkpeakcomparison", 
            HelpText = "Outputs a file that lists all peak's location and Full Width Half Max")]
        public bool BulkPeakComparison { get; set; }

        /// <summary>
        /// Gets or sets the frame to output.
        /// </summary>
        [Option('f', "frame", HelpText = "Outputs a specific frame", DefaultValue = 1)]
        public int Frame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to get the heat map.
        /// </summary>
        [Option('h', "heatmap", 
            HelpText = "Specifies that you want the two-dimensional heatmap data")]
        public bool GetHeatmap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output mass over charge data.
        /// </summary>
        [Option('m', "mz", HelpText = "Specifies that you want the m/z data")]
        public bool GetMz { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output total ion chromatogram data.
        /// </summary>
        [Option('t', "tic", HelpText = "specifies that you want the TiC data")]
        public bool GetTiC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what, if any Extracted ion chromatogram data to target and output.
        /// </summary>
        [Option('x', "xic", DefaultValue = 0, HelpText = "Specifies that you want XiC data for a specific m/z")]
        public double GetXiC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to get ms ms data.
        /// </summary>
        [Option('s', "msms", HelpText = "Get msms data instead of ms data when fetching the XiC")]
        public bool Getmsms { get; set; }

        /// <summary>
        /// Gets or sets the input path.
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "Specify the directory containing the UIMFs to process")]
        public string InputPath { get; set; }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        [Option('o', "output", 
            HelpText =
                "Specify the output directory. If left empty results will be written into the same directory as the input directory"
            )]
        public string OutputPath { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to peak find and print out information.
        /// </summary>
        [Option('p', "peakfind", 
            HelpText = "Prints out a file listing the peaks for the m/z and/or TiC based on what output is selected")]
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

        /// <summary>
        /// Gets or sets the tolerance in Thompsons for the extracted ion chromatogram.
        /// </summary>
        [Option('e', "tolerance", DefaultValue = 0.5, 
            HelpText = "Specifies the tolerance from the m/z that you want for the XiC")]
        public double XicTolerance { get; set; }

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

            if (this.LastParserState.Errors.Any())
            {
                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces

                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                    help.AddPreOptionsLine(errors);
                }
            }

            help.AddOptions(this);

            return help;
        }

        #endregion
    }
}