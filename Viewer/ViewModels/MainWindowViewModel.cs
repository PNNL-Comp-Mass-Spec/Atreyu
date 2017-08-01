using System.Linq;
using System.Reactive;
using System.Text;
using ReactiveUI.Legacy;
using Viewer.Views;

namespace Viewer.ViewModels
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using Atreyu.ViewModels;

    using Microsoft.Win32;

    using ReactiveUI;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : ReactiveObject
    {
        private string _currentFile;
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.CombinedHeatmapViewModel = new CombinedHeatmapViewModel();
            this.CombinedHeatmapViewModel.WindowTitle = "Please load data";

            this.OpenFile = ReactiveCommand.Create(() => this.OpenHeatmapFile());

            this.SaveHeatmap = ReactiveCommand.Create(() => this.SaveHeatmapImage());

            this.ExportCompressedHeatmapData = ReactiveCommand.Create(() => this.SaveExportedHeatmapCompressedData());

            this.ExportCompressedMzData = ReactiveCommand.Create(() => this.SaveExportedMzCompressedData());

            this.ExportCompressedTicData = ReactiveCommand.Create(() => this.SaveExportedTicCompressedData());

            this.ExportCompressedBpiData = ReactiveCommand.Create(() => this.SaveExportedBpiCompressedData());

            this.DisplayAboutWindow = ReactiveCommand.Create(() => this.SpawnAboutWindow());
        }

        private void SpawnAboutWindow()
        {
            var window = new AboutWindowView();
            window.ShowDialog();
        }

        #endregion

        #region Public Properties

        

        public string WindowTitle { get { return "Atreyu " + " - " + CombinedHeatmapViewModel.WindowTitle; } }

        /// <summary>
        /// Gets or sets the combined heatmap view model.
        /// </summary>
        public CombinedHeatmapViewModel CombinedHeatmapViewModel { get; set; }

        /// <summary>
        /// Gets the export compressed heatmap data.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExportCompressedHeatmapData { get; }

        /// <summary>
        /// Gets the export compressed mz data.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExportCompressedMzData { get; }

        /// <summary>
        /// Gets the export compressed tic data.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExportCompressedTicData { get; }


        public ReactiveCommand<Unit, Unit> ExportCompressedBpiData { get; }

        /// <summary>
        /// Gets the open file.
        /// </summary>
        public ReactiveCommand<Unit, Unit> OpenFile { get; }

        /// <summary>
        /// Gets the save heatmap.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveHeatmap { get; }

        public ReactiveCommand<Unit, Unit> DisplayAboutWindow { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The get data filename.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetDataFilename()
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
        /// The get image format.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="ImageFormat"/> from the given extension.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown is the filename passed doesn't have any extension.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Thrown if an unknown extension is passed, currently bmp, gif, ico, jpg, jpeg, png, tif, tiff, and wmf are recognized.
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
                    throw new NotImplementedException(
                        "The extension was not recognised, currently only  bmp, gif, ico, jpg, jpeg, png, tif, tiff, and wmf are recognized.");
            }
        }

        /// <summary>
        /// The open heatmap file.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

            this.CombinedHeatmapViewModel.WindowTitle = Path.GetFileNameWithoutExtension(filename);
        }

        /// <summary>
        /// The save exported heatmap compressed data.
        /// </summary>
        private void SaveExportedHeatmapCompressedData()
        {
            var filename = GetDataFilename();

            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            var temp = this.CombinedHeatmapViewModel.ExportHeatmapDataCompressed();

            using (var outfile = new StreamWriter(filename))
            {
                var sb = new StringBuilder();
                sb.Append("m/z,");
                var currentFrame = this.CombinedHeatmapViewModel.FrameManipulationViewModel.CurrentFrame;
                var thing = this.CombinedHeatmapViewModel.UimfData.GetFullScanInfo(currentFrame);
                var maxTime = thing.Last().DriftTime;

                for (var x = 0; x < temp.GetLength(0); x++)
                {
                    sb.Append(((x * maxTime) / temp.GetLength(0)).ToString("0.0000"));
                    sb.Append(',');
                }
                sb.Append(maxTime.ToString("0.0000"));
                outfile.WriteLine(sb);

                for (var y = 0; y < temp.GetLength(1); y++)
                {
                    sb = new StringBuilder();
                    sb.Append(
                        this.CombinedHeatmapViewModel.UimfData.Calibrator.TOFtoMZ(y).ToString("0.0000"));
                    sb.Append(',');
                    for (var x = 0; x < temp.GetLength(0); x++)
                    {
                        if (temp[x, y] < 0.0001)
                        {
                            sb.Append("0");
                        }
                        else
                        {
                            sb.Append(temp[x, y].ToString("0.0000"));
                        }
                        sb.Append(',');
                        //content += temp[x, y].ToString("0.00") + ",";
                    }
                    //sb.Append('\n');
                    
                    var content = sb.ToString(0, sb.ToString().Length - 1);
                    outfile.WriteLine(content);
                }
            }
        }

        /// <summary>
        /// The save exported mz compressed data.
        /// </summary>
        private void SaveExportedMzCompressedData()
        {
            var filename = GetDataFilename();

            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            var temp = this.CombinedHeatmapViewModel.ExportMzDataCompressed();

            var sb = new StringBuilder();
            using (var outfile = new StreamWriter(filename))
            {
                sb.AppendLine("mz, intensity");
                //var content = "mz, intensity" + Environment.NewLine;
                foreach (var kvp in temp)
                {
                    sb.AppendLine(kvp.Key + "," + kvp.Value);
                    //content += kvp.Key + "," + kvp.Value + Environment.NewLine;
                }
                outfile.WriteLine(sb.ToString());
                //outfile.WriteLine(content);
            }
        }

        /// <summary>
        /// The save exported tic compressed data.
        /// </summary>
        private void SaveExportedTicCompressedData()
        {
            var filename = GetDataFilename();

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
        /// The save exported tic compressed data.
        /// </summary>
        private void SaveExportedBpiCompressedData()
        {
            var filename = GetDataFilename();

            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            var temp = this.CombinedHeatmapViewModel.ExportBpiDataCompressed();

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
        /// The save heatmap image.
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