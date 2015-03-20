using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UimfDataExtractor
{
    using System.Diagnostics.CodeAnalysis;
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

        private static DataReader.FrameType GetFrameType(string frameType)
        {
            var temp = frameType.ToLower();
            switch (temp)
            {
                case "1":
                    return DataReader.FrameType.MS1;
                case "2":
                    return DataReader.FrameType.MS2;
                case "3":
                    return DataReader.FrameType.Calibration;
                case "4":
                    return DataReader.FrameType.Prescan;
                default:
                    throw new NotImplementedException(
                        "Only the MS1, MS2, Calibration, and Prescan frame types have been implemented in this version. Data Passed was " + temp);
            }
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private static void ProcessFrame(DataReader uimf, FileInfo originFile, int frameNumber = 1)
        {

            // eventually I will also have methods for mz and heatmap,
            // which is why we are sperating this into seperate funtions now

            var frameParams = uimf.GetFrameParams(frameNumber);
            
            if (frameParams == null)
            {
                // Frame number is out of range
                Console.WriteLine();
                Console.Error.WriteLine("ERROR: Somehow a frame number that doesn't exist was attempted to read" + Environment.NewLine
                + "So we are not creating data for frame " + frameNumber + " Of " + Environment.NewLine 
                + originFile.FullName);
                Console.WriteLine();
                return;
            }
            
            if (options.GetHeatmap)
            {
                
            }

            if (options.GetMz)
            {
                var mzData = GetFullMzInfo(uimf, frameNumber);
                if (mzData == null)
                {
                    Console.Error.WriteLine("ERROR: We had a problem getting the data for the MZ of" + Environment.NewLine
                        + " frame " + frameNumber + " in " + originFile.FullName);
                }
                else
                {
                    var mzOutputFile = GetOutputLocation(originFile, "Mz", frameNumber);
                    OutputMz(mzData, mzOutputFile);   
                }
            }

            if (options.GetTiC)
            {
                var ticData = GetFullScanInfo(uimf, frameNumber);
                var ticOutputFile = GetOutputLocation(originFile, "TiC", frameNumber);

                OutputTiCbyTime(ticData, ticOutputFile);
            }

            if (options.Verbose)
            {
                Console.WriteLine("Finished processing Frame " + frameNumber + " of " + originFile.FullName);
            }
        }

        private static List<KeyValuePair<double, int>> GetFullMzInfo(DataReader uimf, int frameNumber)
        {
            var frameParams = uimf.GetFrameParams(frameNumber);
            var maxScans = uimf.GetFrameParams(frameNumber).Scans;

            var typeString = frameParams.GetValue(FrameParamKeyType.FrameType);

            if (String.IsNullOrWhiteSpace(typeString))
            {
                Console.Error.WriteLine("ERROR: Had a problem getting the frame type which means we can't get the MZ data");
                return null;
            }

            var frametype = GetFrameType(frameParams.GetValue(FrameParamKeyType.FrameType));
            
            double[] mzs;
            int[] intensities;
            uimf.GetSpectrum(
                frameNumber,
                frameNumber,
                frametype,
                1,
                maxScans,
                out mzs,
                out intensities);
            var data = new List<KeyValuePair<double, int>>(mzs.Length);
            for (var i = 0; i < mzs.Length && i < intensities.Length; i++)
            {
                data.Add(new KeyValuePair<double, int>(mzs[i], intensities[i]));
            }

            return data;
        }

        private static List<ScanInfo> GetFullScanInfo(DataReader uimf, int frameNumber)
        {
            return uimf.GetFrameScans(frameNumber);
        }

        private static void OutputMz(IEnumerable<KeyValuePair<double, int>> mzKeyedIntensities, FileInfo outputFile)
        {
            using (var stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    PrintFileCreationError(outputFile.FullName);
                    return;
                }

                foreach (var kvp in mzKeyedIntensities)
                {
                    stream.WriteLine(kvp.Key + ", " + kvp.Value);
                }

                if (options.Verbose)
                {
                    Console.WriteLine("flushing data to file " + outputFile.FullName);
                }
            }
        }

        private static void OutputTiCbyTime(IEnumerable<ScanInfo> timeKeyedIntensities, FileInfo outputFile)
        {
            using (var stream = GetFileStream(outputFile))
            {
                if (stream == null)
                {
                    PrintFileCreationError(outputFile.FullName);
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
            }
        }

        private static void PrintFileCreationError(string filename)
        {
            Console.WriteLine();
            Console.Error.WriteLine("We were unable to create" + filename + "so we aren't outputting data to it either, we are petty like that");
            Console.WriteLine();
        }

        private static StreamWriter GetFileStream(FileInfo outputFile)
        {
            var outstring = outputFile.DirectoryName;
            if (outstring == null)
            {
                Console.WriteLine();
                Console.Error.WriteLine("ERROR: We will expand upong this later, but we couldn't find the directory of the output file and it will not be output");
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
