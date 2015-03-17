using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIMF_Data_Extractor
{
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new CommandLineOptions();

            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                // Domain logic here
                var inputDirectories = options.InputPaths;
                var outputDirectories = !string.IsNullOrWhiteSpace(options.OutputPath)
                                            ? options.OutputPath
                                            : Directory.GetCurrentDirectory();
                var verbose = options.Verbose;

            }
        }
    }
}
