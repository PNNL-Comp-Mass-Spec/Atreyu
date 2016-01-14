namespace UimfDataExtractor
{
    using System;

    using UimfDataExtractor.Models;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Public Methods and Operators

        /// <summary>
        /// The main entry to the program.
        /// </summary>
        /// <param name="args">
        /// The arguments passed from the command line.
        /// </param>
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Starting windows application");
                var gui = new UimfDataExtractorGui();
                gui.ShowDialog();
            }
            else
            {
                UimfProcessor.Options = new CommandLineOptions();

                if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, UimfProcessor.Options))
                {
                    return;
                }

                // Domain logic here
                UimfProcessor.ExtractData();
            }

            Console.WriteLine("Exiting");
        }

        #endregion
    }
}