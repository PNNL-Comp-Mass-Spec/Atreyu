using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Atreyu.Models;
using ReactiveUI;
using ReactiveUI.Legacy;
using UIMFLibrary;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Atreyu.ViewModels
{
    // The purpose of this class is to provide a way for the user to manually
    // reset the MZ calibration from the UIMF file and update the file as well.
    public class ToFCalibratorViewModel : ReactiveObject
    {
        private const double TOLERANCE = 0.00001;

        private double _tof1;
        private double _mz1 = 0;

        private double _tof2;
        private double _mz2 = 0;

        private double _calibSlope;
        private double _calibInt;

        private bool _buttonEnable;
        private Visibility _calibVisible;
        private UimfData data;
        private bool _reload;

        public Visibility CalibVisible
        {
            get { return _calibVisible; }
            set { this.RaiseAndSetIfChanged(ref this._calibVisible, value); }
        }

        public ToFCalibratorViewModel()
        {
            CalibVisible = Visibility.Hidden;
            CalculateCalibrationCommand = ReactiveCommand.Create(() => CalculateCalibration());
            PerformCalibrationCommand = ReactiveCommand.Create(() => PerformCalibration());
        }

        private void CalculateCalibration()
        {
            this.CalibSlope = (Math.Sqrt(Mz2) - Math.Sqrt(Mz1))/(ToF2 - ToF1)/10000.0;
            this.CalibInt = ToF2 - Math.Sqrt(Mz2)/(this.CalibSlope * 10000.0);
        }

        private void PerformCalibration()
        {
            CalculateCalibration();

            var dataReader = new DataReader(FileName);
            var fileName = Path.Combine(Path.GetDirectoryName(FileName), (Path.GetFileNameWithoutExtension(FileName) + "updated.uimf"));
            if(File.Exists(fileName))
                File.Delete(fileName);
            File.Copy(FileName, fileName);
            using (var dataWriter = new DataWriter(fileName))
            {
                //dataWriter.InsertGlobal(dataReader.GetGlobalParams());
                var numFrames = dataReader.GetGlobalParams().NumFrames;
                for (int i = 1; i <= numFrames; i++)
                {
                    //    dataWriter.InsertFrame(i, dataReader.GetFrameParams(i));
                    //    dataWriter.InsertScan(i, dataReader.GetFrameParams(i), dataReader.)

                    dataWriter.UpdateCalibrationCoefficients(1, (float)(CalibSlope * 10000.0), (float)(CalibInt / 10000.0));
                }
            }
            this.NewFileName = fileName;
            this.ReloadUIMF = true;
        }

        public double ToF1
        {
            get { return _tof1;}
            set
            {
                this.RaiseAndSetIfChanged(ref this._tof1, value);
                CheckButtonEnable();
            }
        }

        public double Mz1
        {
            get { return _mz1;}
            set
            {
                this.RaiseAndSetIfChanged(ref this._mz1, value);
                CheckButtonEnable();
            }
        }

        public double ToF2
        {
            get { return _tof2; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._tof2, value);
                CheckButtonEnable();
            }
        }

        public double Mz2
        {
            get { return _mz2; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._mz2, value);
                CheckButtonEnable();
            }
        }

        public double CalibSlope
        {
            get { return _calibSlope; }
            set { this.RaiseAndSetIfChanged(ref this._calibSlope, value); 
                CheckButtonEnable();}
        }

        public double CalibInt
        {
            get { return _calibInt; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._calibInt, value);
                CheckButtonEnable();
            }
        }

        public bool ButtonEnable
        {
            get { return _buttonEnable; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._buttonEnable, value);
            }
        }

        public void CheckButtonEnable()
        {
            if (ToF1 != ToF2 && Math.Abs(Mz1 - Mz2) > TOLERANCE)
            {
                ButtonEnable = true;
            }
            else
            {
                ButtonEnable = false;
            }
        }

        public string FileName { get; set; }

        /// <summary>
        /// The magic numbers are from the UIMFDataReader to convert from the slope itself to the k or t0
        /// </summary>
        /// <param name="data"></param>
        public void UpdateExistingCalib(UimfData data, string file)
        {
            this.data = data;
            if (data != null && data.Calibrator != null)
            {
                CalibSlope = data.Calibrator.K;
                CalibInt = data.Calibrator.T0;
            }
        }

        public ReactiveCommand<Unit, Unit> CalculateCalibrationCommand { get; }
        public ReactiveCommand<Unit, Unit> PerformCalibrationCommand { get;  }

        public bool ReloadUIMF { get { return this._reload; } set
        {
            this.RaiseAndSetIfChanged(ref this._reload, value);
        } }

        public string NewFileName { get; set; }
    }
}
