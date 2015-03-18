using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UimfDataExtractor
{
    using System.IO;

    class Program
    {
        private static CommandLineOptions options;

        public static void Main(string[] args)
        {
            options = new CommandLineOptions();

            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                // Domain logic here
                var inputDirectories = options.InputPaths;
                var outputDirectory = !string.IsNullOrWhiteSpace(options.OutputPath)
                                            ? options.OutputPath
                                            : Directory.GetCurrentDirectory();
                var verbose = options.Verbose;

                var taskList = new List<Task>();

                if (options.Recursive)
                {
                    Parallel.ForEach(
                        inputDirectories,
                        async directory => await ProcessAllUimfInDirectoryRecursive(directory));
                }
                else
                {
                    Parallel.ForEach(inputDirectories, async directory => await ProcessAllUimfInDirectory(directory));    
                }
            }
        }

        private static async Task ProcessAllUimfInDirectoryRecursive(string root)
        {
            if (Directory.Exists(root))
            {
                var task = ProcessAllUimfInDirectory(root);
                Parallel.ForEach(
                    Directory.GetDirectories(root),
                    async directory => await ProcessAllUimfInDirectoryRecursive(directory));
                await task;
            }
        }

        private static async Task ProcessAllUimfInDirectory(string directory)
        {
            
        }
    }
}
