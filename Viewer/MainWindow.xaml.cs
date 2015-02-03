// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Viewer
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Atreyu.ViewModels;

    using Falkor.Views.Atreyu;

    using Microsoft.Practices.Prism.PubSubEvents;
    using Microsoft.Win32;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.OpenButton.Click += this.OpenButtonClick;

            this.SaveButton.Click += this.SaveButtonClick;

            ////this.AllowDrop = true;
            ////this.PreviewDrop += this.MainTabControl_PreviewDragEnter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The load button click.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void OpenButtonClick(object sender, RoutedEventArgs e)
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
            this.LoadFile(filename);
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            const string Filter = "|PNG files (*.png)|*.png" 
                                  + "JPEG files (*.jpg)|*.jpg"
                                  + "|TIFF files (*.tif)|*.tif"
                                  + "|Bitmaps (*.bmp)|*.bmp";
            var dialogue = new SaveFileDialog
            {
                DefaultExt = ".png",
                AddExtension = true,
                Filter = Filter
            };

            var result = dialogue.ShowDialog();

            if (result != true)
            {
                return;
            }

            var image = this.CombinedHeatmapView.ViewModel.HeatMapViewModel.GetHeatmapImage();

            var filename = dialogue.FileName;
            

            var format = GetImageFormat(filename);
            image.Save(filename, format);
        }


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
        /// TODO The load file.
        /// </summary>
        /// <param name="fileName">
        /// TODO The file name.
        /// </param>
        private void LoadFile(string fileName)
        {
            this.CombinedHeatmapView.ViewModel.HeatMapViewModel.InitializeUimfData(fileName);
        }

        /// <summary>
        /// TODO The main tab control_ preview drag enter.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void MainTabControl_PreviewDragEnter(object sender, DragEventArgs e)
        {
            var isCorrect = true;
            string[] filenames = { };
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }

                    var info = new FileInfo(filename);

                    if (info.Extension.ToLower() != ".uimf")
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }

            e.Effects = isCorrect ? DragDropEffects.All : DragDropEffects.None;

            if (isCorrect)
            {
                this.LoadFile(filenames[0]);
            }

            e.Handled = true;
        }

        #endregion
    }
}