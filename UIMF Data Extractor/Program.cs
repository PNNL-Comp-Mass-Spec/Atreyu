using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UimfDataExtractor
{
    using System.IO;
    
    using UIMFLibrary;

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
            inputDirectory = new DirectoryInfo(options.InputPath);

            outputDirectory = string.IsNullOrWhiteSpace(options.OutputPath)
                                  ? inputDirectory
                                  : new DirectoryInfo(options.OutputPath);

            if (options.Verbose)
            {
                Console.WriteLine("Verbose Active");
                Console.WriteLine("Input Directory: " + inputDirectory.FullName);
                Console.WriteLine("Output Directory: " + outputDirectory.FullName);
                Console.WriteLine("Run Recursively?: " + options.Recursive);
                Console.WriteLine("Process All Frames in File?: " + options.AllFrames);
            }

            if (options.Recursive)
            {
                ProcessAllUimfInDirectoryRecursive(inputDirectory);
            }
            else
            {
                ProcessAllUimfInDirectory(inputDirectory);    
            }

            if (options.Verbose)
            {
                Console.WriteLine("All done, Exiting");
            }
        }

        private static void ProcessAllUimfInDirectoryRecursive(DirectoryInfo root)
        {
            if (root.Exists)
            {
                if (options.Verbose)
                {
                    Console.WriteLine("Started recursive work in " + root.FullName);
                }

                ProcessAllUimfInDirectory(root);

                Parallel.ForEach(
                    root.EnumerateDirectories(),
                    ProcessAllUimfInDirectoryRecursive);
            }
            else
            {
                PrintNotFoundError(root);
            }
        }

        private static void ProcessAllUimfInDirectory(DirectoryInfo directory)
        {
            
                        if (directory.Exists)
                        {
                            if (options.Verbose)
                            {
                                ////Console.WriteLine("Starting to process UIMFs in " + directory.FullName);
                            }

                            var uimfs = directory.EnumerateFiles("*.uimf");

                            Parallel.ForEach(uimfs, ProcessUimf);
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

                if (options.AllFrames)
                {
                    Console.WriteLine("The All Frames feature is not yet implemented, so we are going to process just one");
                    ProcessFrame(uimfReader, file);
                }
                else
                {
                    ProcessFrame(uimfReader, file);
                }
            }

            if (options.Verbose)
            {
                Console.WriteLine("Finished processing " + file.FullName);
            }
        }


        private static void ProcessFrame(DataReader uimf, FileInfo originFile, int frameNumber = 1)
        {

            // eventually I will also have methods for mz and heatmap,
            // which is why we are sperating this into seperate funtions now

            var data = GetFullScanInfo(uimf, frameNumber);
            var outputFile = GetOutputLocation(originFile);

            OutputTiCbyTime(data, outputFile);
            if (options.Verbose)
            {
                Console.WriteLine("Finished processing Frame " + frameNumber + " of " + originFile.FullName);
            }
        }

        private static List<ScanInfo> GetFullScanInfo(DataReader uimf, int frameNumber)
        {
            return uimf.GetFrameScans(frameNumber);
        }

        private static void OutputTiCbyTime(List<ScanInfo> timeKeyedIntensities, FileInfo outputFile)
        {
            StreamWriter stream;
            using (stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    Console.WriteLine("we were unable to create" + outputFile.FullName + "so we aren't outputting data to it either, we are petty like that");
                    return;
                }

                foreach (var timeKeyedIntensity in timeKeyedIntensities)
                {
                    stream.WriteLine(timeKeyedIntensity.DriftTime + ", " + timeKeyedIntensity.TIC);
                }
                if (options.Verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
                ////await stream.FlushAsync();
                ////stream.Close();
            }
        }

        private static StreamWriter GetFileStream(FileInfo outputFile)
        {
            var outstring = outputFile.DirectoryName;
            if (outstring == null)
            {
                Console.WriteLine("ERROR: We will expand upong this later, but we couldn't find the directory of the output file and it will not be output");
                return null;
            }

            var outDirectory = new DirectoryInfo(outstring);

            if (!outDirectory.Exists)
            {
                outDirectory.Create();
            }

            if (outputFile.Exists)
            {
                outputFile.Delete();
            }

            return outputFile.CreateText();
        }

        private static FileInfo GetOutputLocation(FileInfo originFile)
        {
            var locationRelativeToInput = originFile.FullName.Substring(inputDirectory.FullName.Length + 1);
            ////Console.WriteLine("Relative: " + locationRelativeToInput);
            ////Console.WriteLine("output: " + outputDirectory.FullName);
            var nestedLocation = Path.Combine(outputDirectory.FullName, locationRelativeToInput);
            ////Console.WriteLine("Combined: " + nestedLocation);
            
            var csvFullPath = Path.ChangeExtension(nestedLocation, "csv");
            return new FileInfo(csvFullPath);
        }

    }
}
