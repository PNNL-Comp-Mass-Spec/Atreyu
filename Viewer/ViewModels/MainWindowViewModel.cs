using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using ReactiveUI;
using Atreyu.ViewModels;

namespace Viewer.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public CombinedHeatmapViewModel CombinedHeatmapViewModel { get; set; }

        public ReactiveCommand<object> OpenFile { get; private set; }
        public ReactiveCommand<object> SaveHeatmap { get; private set; }

        public MainWindowViewModel()
        {
            CombinedHeatmapViewModel = new CombinedHeatmapViewModel();

            this.OpenFile = ReactiveCommand.Create();
            this.OpenFile.Subscribe(x => this.OpenHeatmapFile());

            this.SaveHeatmap = ReactiveCommand.Create();
            this.SaveHeatmap.Subscribe(x => this.SaveHeatmapImage());

        }

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
            if (String.IsNullOrEmpty(extension))
            {
                throw new ArgumentException(
                    String.Format("Unable to determine file extension for fileName: {0}", fileName));
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

            this.CombinedHeatmapViewModel.HeatMapViewModel.InitializeUimfData(filename);

        }

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
    }
}
