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
            Console.WriteLine();
            Console.WriteLine("All done, Exiting");
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
                                Console.WriteLine("Starting to process UIMFs in " + directory.FullName);
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
            Console.WriteLine();
            Console.WriteLine(fileOrDirectory + "does not exist");
            Console.WriteLine();
        }

        private static void PrintNotFoundError(FileSystemInfo fileOrDirectory)
        {
            Console.WriteLine();
            Console.WriteLine(fileOrDirectory.FullName + "does not exist");
            Console.WriteLine();
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

                var framecount = uimfReader.GetGlobalParams().NumFrames;
                if (options.AllFrames)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    for (var i = 0; i < framecount; i++)
                    {
                        ProcessFrame(uimfReader, file, i);
                    }
                }
                else
                {
                    ProcessFrame(uimfReader, file);
                }
            }
            Console.WriteLine("Finished processing " + file.FullName);
        }


        private static void ProcessFrame(DataReader uimf, FileInfo originFile, int frameNumber = 1)
        {

            // eventually I will also have methods for mz and heatmap,
            // which is why we are sperating this into seperate funtions now

            var data = GetFullScanInfo(uimf, frameNumber);
            var outputFile = GetOutputLocation(originFile, "TiC", frameNumber);

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

        private static void OutputTiCbyTime(IEnumerable<ScanInfo> timeKeyedIntensities, FileInfo outputFile)
        {
            var output = outputFile;

            using (var stream = GetFileStream(output))
            {
                if (stream == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("we were unable to create" + outputFile.FullName + "so we aren't outputting data to it either, we are petty like that");
                    Console.WriteLine();
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
                Console.WriteLine();
                Console.WriteLine("ERROR: We will expand upong this later, but we couldn't find the directory of the output file and it will not be output");
                Console.WriteLine();
                return null;
            }

            var outDirectory = new DirectoryInfo(outstring);

            if (!outDirectory.Exists)
            {
                outDirectory.Create();
            }

            if (outputFile.Exists)
            {
                Console.WriteLine();
                Console.WriteLine("One of the output files (" + outputFile.FullName + ") already exists, so we are deleting it Mwahahaha!");
                Console.WriteLine();
                outputFile.Delete();
            }

            return outputFile.CreateText();
        }

        private static FileInfo GetOutputLocation(FileInfo originFile, string dataType, int frameNumber)
        {
            var locationRelativeToInput = originFile.FullName.Substring(inputDirectory.FullName.Length + 1);
            var nestedLocation = Path.Combine(outputDirectory.FullName, locationRelativeToInput);
            var csvFullPath = Path.ChangeExtension(nestedLocation, "csv");
            var oldName = new FileInfo(csvFullPath);

            var oldDir = oldName.DirectoryName;
            var newName = Path.GetFileNameWithoutExtension(csvFullPath);

            newName += "_" + dataType;

            newName += "_" + frameNumber.ToString("0000");


            newName += Path.GetExtension(oldName.FullName);

            return oldDir == null ? null : new FileInfo(Path.Combine(oldDir, newName));
        }
    }
}
