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

        private static DirectoryInfo inputDirectory;

        private static DirectoryInfo outputDirectory;

        public static void Main(string[] args)
        {
            options = new CommandLineOptions();

            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                return;
            }
            // Domain logic here
            outputDirectory = string.IsNullOrWhiteSpace(options.OutputPath)
                                  ? new DirectoryInfo(Directory.GetCurrentDirectory())
                                  : new DirectoryInfo(options.OutputPath);

            inputDirectory = new DirectoryInfo(options.InputPath);




            var taskList = new List<Task>();

            if (options.Recursive)
            {
                Parallel.ForEach(
                    inputDirectory.EnumerateDirectories(),
                    async directory => await ProcessAllUimfInDirectoryRecursive(directory));
            }
            else
            {
                ProcessAllUimfInDirectory(inputDirectory).Wait();    
            }
        }

        private static async Task ProcessAllUimfInDirectoryRecursive(DirectoryInfo root)
        {
            if (root.Exists)
            {
                var task = ProcessAllUimfInDirectory(root);
                Parallel.ForEach(
                    root.EnumerateDirectories(),
                    async directory => await ProcessAllUimfInDirectoryRecursive(directory));
                await task;
            }
            else
            {
                PrintNotFoundError(root);
            }
        }

        private static async Task ProcessAllUimfInDirectory(DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                var uimfs = directory.EnumerateFiles("*.uimf");

                
            }
            else
            {
                PrintNotFoundError(directory);
            }
        }

        private static void PrintNotFoundError(string fileOrDirectory)
        {
            Console.WriteLine(fileOrDirectory + "does not exist");
        }

        private static void PrintNotFoundError(FileSystemInfo fileOrDirectory)
        {
            Console.WriteLine(fileOrDirectory.FullName + "does not exist");
        }
        
        private static async Task ProcessUimf(string file)
        {
            if (options.AllFrames)
            {
                Console.WriteLine("This feature is not yet implemented");
            }
            else
            {
                
            }
        }
    }
}
