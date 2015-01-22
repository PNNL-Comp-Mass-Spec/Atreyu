﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Viewer
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Atreyu.ViewModels;

    using Falkor.Views.Atreyu;

    using Microsoft.Practices.Prism.PubSubEvents;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EventAggregator eventAggregator = new EventAggregator();

        private HeatMapViewModel heatMapViewModel;
        private HeatMapView heatMapView;

        private FrameManipulationViewModel frameManipulationViewModel;

        private FrameManipulationView frameManipulationView;

        private MzSpectraViewModel mzSpectraViewModel;

        private MzSpectraView mzSpectraView;

        private TotalIonChromatogramViewModel totalIonChromatogramViewModel;

        private TotalIonChromatogramView totalIonChromatogramView;

        private Button loadButton;

        public MainWindow()
        {
            InitializeComponent();

            heatMapViewModel = new HeatMapViewModel(eventAggregator);
            heatMapView = new HeatMapView(heatMapViewModel);
            Grid.SetColumn(this.heatMapView, 1);
            Grid.SetRow(this.heatMapView, 1);
            this.MainGrid.Children.Add(this.heatMapView);
            
            this.frameManipulationViewModel = new FrameManipulationViewModel(this.eventAggregator);
            this.frameManipulationView = new FrameManipulationView(this.frameManipulationViewModel);
            Grid.SetColumn(this.frameManipulationView, 1);
            Grid.SetRow(this.frameManipulationView, 0);
            this.MainGrid.Children.Add(this.frameManipulationView);

            this.mzSpectraViewModel = new MzSpectraViewModel(this.eventAggregator);
            this.mzSpectraView = new MzSpectraView(this.mzSpectraViewModel);
            var transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(90));
            //transform.Children.Add(new ScaleTransform(-1, 1));
            //transform.Children.Add(new );
            //this.mzSpectraView.RenderTransform = transform;
            Grid.SetColumn(this.mzSpectraView, 0);
            Grid.SetRow(this.mzSpectraView, 1);
            //this.mzSpectraView.VerticalAlignment = VerticalAlignment.Stretch;
            //this.mzSpectraView.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.MainGrid.Children.Add(this.mzSpectraView);

            this.totalIonChromatogramViewModel = new TotalIonChromatogramViewModel(this.eventAggregator);
            this.totalIonChromatogramView = new TotalIonChromatogramView(this.totalIonChromatogramViewModel);
            Grid.SetColumn(this.totalIonChromatogramView, 1);
            Grid.SetRow(this.totalIonChromatogramView, 3);
            this.MainGrid.Children.Add(this.totalIonChromatogramView);

            this.loadButton = new Button { Content = "Load" };
            this.loadButton.Click += this.LoadButtonClick;
            Grid.SetColumn(this.loadButton, 0);
            Grid.SetRow(this.loadButton, 0);
            this.MainGrid.Children.Add(this.loadButton);

            this.AllowDrop = true;
            this.PreviewDrop += this.MainTabControl_PreviewDragEnter;
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted", Justification = "Reviewed. Suppression is OK here.")]
        void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            var dialogue = new OpenFileDialog();
            dialogue.DefaultExt = ".uimf";
            dialogue.Filter = "Unified Ion Mobility File (*.uimf)|*.uimf";
            
            var result = dialogue.ShowDialog();

            if (result != true) return;

            var filename = dialogue.FileName;
            this.LoadFile(filename);
        }

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

        private void LoadFile(string fileName)
        {
            this.heatMapViewModel.InitializeUimfData(fileName);
            this.totalIonChromatogramViewModel.UpdateReference(this.heatMapViewModel.HeatMapData);
        }

    }
}
