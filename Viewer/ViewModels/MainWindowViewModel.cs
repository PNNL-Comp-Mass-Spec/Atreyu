// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The main window view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Viewer.ViewModels
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;

    using Atreyu.ViewModels;

    using Microsoft.Win32;

    using ReactiveUI;

    /// <summary>
    /// TODO The main window view model.
    /// </summary>
    public class MainWindowViewModel : ReactiveObject
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.CombinedHeatmapViewModel = new CombinedHeatmapViewModel();

            this.OpenFile = ReactiveCommand.Create();
            this.OpenFile.Subscribe(x => this.OpenHeatmapFile());

            this.SaveHeatmap = ReactiveCommand.Create();
            this.SaveHeatmap.Subscribe(x => this.SaveHeatmapImage());

            this.ExportCompressedHeatmapData = ReactiveCommand.Create();
            this.ExportCompressedHeatmapData.Subscribe(x => this.SaveExportedHeatmapCompressedData());

            this.ExportCompressedMzData = ReactiveCommand.Create();
            this.ExportCompressedMzData.Subscribe(x => this.SaveExportedMzCompressedData());

            this.ExportCompressedTicData = ReactiveCommand.Create();
            this.ExportCompressedTicData.Subscribe(x => this.SaveExportedTicCompressedData());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the combined heatmap view model.
        /// </summary>
        public CombinedHeatmapViewModel CombinedHeatmapViewModel { get; set; }

        /// <summary>
        /// Gets the export compressed heatmap data.
        /// </summary>
        public ReactiveCommand<object> ExportCompressedHeatmapData { get; private set; }

        /// <summary>
        /// Gets the export compressed mz data.
        /// </summary>
        public ReactiveCommand<object> ExportCompressedMzData { get; private set; }

        /// <summary>
        /// Gets the export compressed tic data.
        /// </summary>
        public ReactiveCommand<object> ExportCompressedTicData { get; private set; }

        /// <summary>
        /// Gets the open file.
        /// </summary>
        public ReactiveCommand<object> OpenFile { get; private set; }

        /// <summary>
        /// Gets the save heatmap.
        /// </summary>
        public ReactiveCommand<object> SaveHeatmap { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The get image format.
        /// </summary>
        /// <param name="fileName">
        /// TODO The file name.
        /// </param>
        /// <returns>
        /// The <see cref="ImageFormat"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private static ImageFormat GetImageFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));
            }

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// TODO The get data filename.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDataFilename()
        {
            const string Filter = "Comma Seperated Values (*.csv)|*.csv";
            var dialogue = new SaveFileDialog { DefaultExt = ".csv", AddExtension = true, Filter = Filter };

            var result = dialogue.ShowDialog();

            if (result != true)
            {
                return string.Empty;
            }

            return dialogue.FileName;
        }

        /// <summary>
        /// TODO The open heatmap file.
        /// </summary>
        private void OpenHeatmapFile()
        {
            var dialogue = new OpenFileDialog
                               {
                                   DefaultExt = ".uimf", 
                                   Filter = "Unified Ion Mobility File (*.uimf)|*.uimf"
                               };

            var result = dialogue.ShowDialog();

            if (result != true)
            {
                return;
            }

            var filename = dialogue.FileName;

            this.CombinedHeatmapViewModel.InitializeUimfData(filename);
        }

        /// <summary>
        /// TODO The save exported heatmap compressed data.
        /// </summary>
        private void SaveExportedHeatmapCompressedData()
        {
            var filename = this.GetDataFilename();

            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            var temp = this.CombinedHeatmapViewModel.ExportHeatmapDataCompressed();

            using (var outfile = new StreamWriter(filename))
            {
                for (var x = 0; x < temp.GetLength(0); x++)
                {
                    var content = string.Empty;
                    for (var y = 0; y < temp.GetLength(1); y++)
                    {
                        content += temp[x, y].ToString("0.00") + ",";
                    }

                    outfile.WriteLine(content);
                }
            }
        }

        /// <summary>
        /// TODO The save exported mz compressed data.
        /// </summary>
        private void SaveExportedMzCompressedData()
        {
            var filename = this.GetDataFilename();

            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            var temp = this.CombinedHeatmapViewModel.ExportMzDataCompressed();

            using (var outfile = new StreamWriter(filename))
            {
                var content = "mz, intensity" + Environment.NewLine;
                foreach (var kvp in temp)
                {
                    content += kvp.Key + "," + kvp.Value + Environment.NewLine;
                }

                outfile.WriteLine(content);
            }
        }

        /// <summary>
        /// TODO The save exported tic compressed data.
        /// </summary>
        private void SaveExportedTicCompressedData()
        {
            var filename = this.GetDataFilename();

            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            var temp = this.CombinedHeatmapViewModel.ExportTicDataCompressed();

            using (var outfile = new StreamWriter(filename))
            {
                var content = "scan, intensity" + Environment.NewLine;
                foreach (var kvp in temp)
                {
                    content += kvp.Key + "," + kvp.Value + Environment.NewLine;
                }

                outfile.WriteLine(content);
            }
        }

        /// <summary>
        /// TODO The save heatmap image.
        /// </summary>
        private void SaveHeatmapImage()
        {
            const string Filter =
                "PNG files (*.png)|*.png" + "|JPEG files (*.jpg)|*.jpg" + "|TIFF files (*.tif)|*.tif"
                + "|Bitmaps (*.bmp)|*.bmp";
            var dialogue = new SaveFileDialog { DefaultExt = ".png", AddExtension = true, Filter = Filter };

            var result = dialogue.ShowDialog();

            if (result != true)
            {
                return;
            }

            var image = this.CombinedHeatmapViewModel.GetImage();

            var filename = dialogue.FileName;

            var format = GetImageFormat(filename);
            image.Save(filename, format);
        }

        #endregion
    }
}