// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="PNNL">
//   Copyright PNNL 2015
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UimfDataExtractor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml;

    using MagnitudeConcavityPeakFinder;

    using UIMFLibrary;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        #endregion

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
                var gui = new UimfDataExtractorGUI();
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
            
            Console.WriteLine();
            Console.WriteLine("All done, Exiting");
        }

        #endregion

        #region Methods

        #endregion
    }
}