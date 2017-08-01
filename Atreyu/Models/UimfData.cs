using System.Diagnostics;
using System.Threading;

namespace Atreyu.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ReactiveUI;

    using UIMFLibrary;

    /// <summary>
    /// The uimf data.
    /// </summary>
    public class UimfData : ReactiveObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// The bin to mz map.
        /// </summary>
        private double[] binToMzMap;

        /// <summary>
        /// The calibrator.
        /// </summary>
        private MzCalibrator calibrator;

        /// <summary>
        /// The data reader.
        /// </summary>
        private DataReader dataReader;

        /// <summary>
        /// The end frame number.
        /// </summary>
        private int endFrameNumber;

        /// <summary>
        /// The frame data.
        /// </summary>
        private double[,] frameData;

        /// <summary>
        /// The frame intercept.
        /// </summary>
        private double frameIntercept;

        /// <summary>
        /// The frame slope.
        /// </summary>
        private double frameSlope;

        /// <summary>
        /// The frame type.
        /// </summary>
        private string frameType;

        /// <summary>
        /// The frames.
        /// </summary>
        private int frames;

        /// <summary>
        /// The gated frame data.
        /// </summary>
        private double[,] gatedFrameData;

        /// <summary>
        /// The high gate.
        /// </summary>
        private double highGate = double.PositiveInfinity;

        /// <summary>
        /// Backing field for a property that indicates whether this is currently loading data or not.
        /// </summary>
        private bool loadingData;

        /// <summary>
        /// The gate.
        /// </summary>
        private double lowGate;

        /// <summary>
        /// The max bins.
        /// </summary>
        private int maxBins;


        /// <summary>
        /// The mz array.
        /// </summary>
        private double[] mzArray;

        /// <summary>
        /// The mz intensities.
        /// </summary>
        private int[] mzIntensities;

        /// <summary>
        /// The mz window that will be enforced if <see cref="WindowMz"/> it true.
        /// </summary>
        private Range<double> mzWindow;

        /// <summary>
        /// The scans.
        /// </summary>
        private int scans;

        /// <summary>
        /// The start frame number.
        /// </summary>
        private int startFrameNumber;

        /// <summary>
        /// The total bins.
        /// </summary>
        private double totalMzRange;

        /// <summary>
        /// The values per pixel x.
        /// </summary>
        private double valuesPerPixelX;

        /// <summary>
        /// The values per pixel y.
        /// </summary>
        private double valuesPerPixelY;

        private ReaderWriterLockSlim readerWriterLock;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UimfData"/> class. 
        /// </summary>
        /// <param name="uimfFile">
        /// The data file that this will rely on for it's entire existence, you want a different file? Create a new class.
        /// </param>
        public UimfData(string uimfFile)
        {
            this.readerWriterLock = new ReaderWriterLockSlim();
            this.dataReader = new DataReader(uimfFile);
            this.TenthsOfNanoSecondsPerBin = 0.0;
            var global = this.dataReader.GetGlobalParams();
            this.Frames = this.dataReader.GetGlobalParams().NumFrames;
            this.MaxBins = global.Bins;
            var frameCalibrator = this.dataReader.GetFrameParams(1);
            this.Calibrator = this.dataReader.GetMzCalibrator(frameCalibrator);
            this.TenthsOfNanoSecondsPerBin = Convert.ToDouble(frameCalibrator.Values[FrameParamKeyType.AverageTOFLength].Value);
            this.MaxMz = this.Calibrator.BinToMZ(global.Bins);
            this.MinMz = this.Calibrator.BinToMZ(0);
            this.TotalMzRange = this.MaxMz - this.MinMz;
            this.Scans = this.dataReader.GetFrameParams(1).Scans;

            this.Ranges = (MinMz, MaxMz, 1, frameCalibrator.Scans);
            this.StartFrameNumber = 1;
            this.EndFrameNumber = 1;
            this.WhenAnyValue(x => x.StartFrameNumber).Subscribe(i =>
            {
                this.readerWriterLock.EnterReadLock();
                var calib = this.dataReader.GetMzCalibrator(this.dataReader.GetFrameParams(i));
                this.readerWriterLock.ExitReadLock();
                this.MinMz = calib.BinToMZ(0);
                this.MaxMz = calib.BinToMZ(this.MaxBins);
            });
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the bin to mz map.
        /// </summary>
        public double[] BinToMzMap
        {
            get
            {
                return this.binToMzMap;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.binToMzMap, value);
            }
        }

        private (double CurrentMinMz, double CurrentMaxMz, int StartScan, int EndScan) ranges;

        public (double CurrentMinMz, double CurrentMaxMz, int StartScan, int EndScan) Ranges
        {
            get => this.ranges;
            set => this.RaiseAndSetIfChanged(ref this.ranges, value);
        }

        /// <summary>
        /// Gets the m/z calibrator that converts TOF to m/z.
        /// </summary>
        public MzCalibrator Calibrator
        {
            get
            {
                return this.calibrator;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.calibrator, value);
            }
        }


        /// <summary>
        /// Gets the end frame number.
        /// </summary>
        public int EndFrameNumber
        {
            get
            {
                return this.endFrameNumber;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.endFrameNumber, value);
            }
        }


        /// <summary>
        /// Gets the frame intercept.
        /// </summary>
        public double FrameIntercept
        {
            get
            {
                return this.frameIntercept;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.frameIntercept, value);
            }
        }

        /// <summary>
        /// Gets the frame slope.
        /// </summary>
        public double FrameSlope
        {
            get
            {
                return this.frameSlope;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.frameSlope, value);
            }
        }

        /// <summary>
        /// Gets the frame type.
        /// </summary>
        public string FrameType
        {
            get
            {
                return this.frameType;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.frameType, value);
            }
        }

        /// <summary>
        /// Gets the frames.
        /// </summary>
        public int Frames
        {
            get
            {
                return this.frames;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.frames, value);
            }
        }

        /// <summary>
        /// Gets the gated frame data.
        /// </summary>
        public double[,] GatedFrameData
        {
            get
            {
                return this.gatedFrameData;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.gatedFrameData, value);
            }
        }

        /// <summary>
        /// Gets the high gate.
        /// </summary>
        public double HighGate
        {
            get
            {
                return this.highGate;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.highGate, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this class is currently loading data or not.
        /// </summary>
        public bool LoadingData
        {
            get
            {
                return this.loadingData;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.loadingData, value);
            }
        }

        /// <summary>
        /// Gets the gate.
        /// </summary>
        public double LowGate
        {
            get
            {
                return this.lowGate;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.lowGate, value);
            }
        }

        /// <summary>
        /// Gets or sets the max bins.
        /// </summary>
        public int MaxBins
        {
            get
            {
                return this.maxBins;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.maxBins, value);
            }
        }

        public double MaxMz
        {
            get { return this.maxMz; }
            private set
            {
                this.RaiseAndSetIfChanged(ref this.maxMz, value);
            }
        }

        public double MinMz
        {
            get { return this.minMz; }
            private set
            {
                this.RaiseAndSetIfChanged(ref this.minMz, value);
            }
        }

        private double maxMz;
        private double minMz;

        /// <summary>
        /// Gets the mz array.
        /// </summary>
        public double[] MzArray
        {
            get
            {
                return this.mzArray;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.mzArray, value);
            }
        }

        /// <summary>
        /// Gets or sets the mz center.
        /// </summary>
        public double MzCenter { get; set; }

        /// <summary>
        /// Gets the mz intensities.
        /// </summary>
        public int[] MzIntensities
        {
            get
            {
                return this.mzIntensities;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.mzIntensities, value);
            }
        }

        /// <summary>
        /// Gets or sets the parts per million.
        /// </summary>
        public double PartsPerMillion { get; set; }

        /// <summary>
        /// Gets the scans.
        /// </summary>
        public int Scans
        {
            get
            {
                return this.scans;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.scans, value);
            }
        }

        /// <summary>
        /// Gets the start frame number.
        /// </summary>
        public int StartFrameNumber
        {
            get
            {
                return this.startFrameNumber;
            }

            set
            {
               
                this.RaiseAndSetIfChanged(ref this.startFrameNumber, value);
            }
        }

        /// <summary>
        /// Gets the total bins currently queried.
        /// </summary>
        public double TotalMzRange
        {
            get
            {
                return this.totalMzRange;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.totalMzRange, value);
            }
        }

        /// <summary>
        /// Gets the values per pixel x.
        /// </summary>
        public double ValuesPerPixelX
        {
            get
            {
                return this.valuesPerPixelX;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.valuesPerPixelX, value);
            }
        }

        /// <summary>
        /// Gets the values per pixel y.
        /// </summary>
        public double ValuesPerPixelY
        {
            get
            {
                return this.valuesPerPixelY;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.valuesPerPixelY, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether window mz.
        /// </summary>
        public bool WindowMz { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

       
        public Range<double> GetMzRangeForMzWindow(double centerMz, double partsPerMillionTolerance)
        {
            this.MzCenter = centerMz;
            this.PartsPerMillion = partsPerMillionTolerance;


            if (this.dataReader == null || this.Calibrator == null)
            {
                var range = new Range<double>(0, Ranges.CurrentMaxMz);
                this.mzWindow = range;
                return range;
            }
            else
            {
                var mzOffset = this.MzCenter * (this.PartsPerMillion / 1000000.0);
                var range = new Range<double>(this.MzCenter - mzOffset, this.MzCenter + mzOffset);

                this.mzWindow = range;
                return range;

            }

        }

        /// <summary>
        /// Get full total ion chromatogram with the key based on scan number
        /// </summary>
        /// <param name="frameNumber">
        /// The frame number.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public List<ScanInfo> GetFullScanInfo(int frameNumber)
        {
            return this.dataReader.GetFrameScans(frameNumber);
        }

        private double prevYPixels;
        //private double[,] _uncompressed;
        private double[] binToTofMap;

        private object syncRoot = new object();

       /// <summary>
       /// 
       /// </summary>
       /// <param name="scanRange"></param>
       /// <param name="mzRange"></param>
       /// <param name="frameRange"></param>
       /// <param name="height"></param>
       /// <param name="width"></param>
       /// <param name="returnGatedData"></param>
       /// <returns></returns>
        public double[,] ReadData((double CurrentMinMz, double CurrentMaxMz, int StartScan, int EndScan) ranges, Range<int> frameRange, double height, double width, bool returnGatedData = false)
        {
            lock (syncRoot)
            {
                this.LoadingData = true;

                var frameParams = this.dataReader.GetFrameParams(this.startFrameNumber);
                UncompressedDeltaMz = this.dataReader.GetDeltaMz(1);

                if (frameParams == null)
                {
                    throw new Exception($"Frame: {this.startFrameNumber}");
                }
                else
                {
                    this.TotalMzRange = ranges.CurrentMinMz - ranges.CurrentMaxMz + 1;

                    this.prevYPixels = this.ValuesPerPixelY;
                    this.Calibrator = this.dataReader.GetMzCalibrator(frameParams);
                    int currentMinBin = (int)Math.Floor(this.Calibrator.MZtoBin(ranges.CurrentMinMz));
                    int currentMaxBin = (int)Math.Ceiling(this.Calibrator.MZtoBin(ranges.Item2));
                    var totalBinRange = currentMaxBin - currentMinBin + 1;
                    //this.ValuesPerPixelY = (int)(this.TotalMzRange / (double)this.mostRecentHeight);
                    this.ValuesPerPixelY = (totalBinRange / (double)height);

                    var totalScans = ranges.EndScan - ranges.StartScan + 1;
                    this.ValuesPerPixelX = (totalScans / (double)width);

                    if (this.ValuesPerPixelY < 1)
                    {
                        this.ValuesPerPixelY = 1;
                    }

                    if (this.ValuesPerPixelX < 1)
                    {
                        this.ValuesPerPixelX = 1;
                    }

                    this.FrameSlope = frameParams.GetValueDouble(FrameParamKeyType.CalibrationSlope);
                    this.FrameIntercept = frameParams.GetValueDouble(FrameParamKeyType.CalibrationIntercept);

                    this.FrameType = frameParams.GetValue(FrameParamKeyType.FrameType);
                    this.FrameIntercept = frameParams.GetValueDouble(FrameParamKeyType.CalibrationIntercept);

                    this.Calibrator = this.dataReader.GetMzCalibrator(frameParams);

                    //var gMaxBin = this.dataReader.GetGlobalParams().Bins;
                    //var gMinBin = 1;
                    //
                    //if (!(this.currentMaxMz == gMaxBin &&
                    //    this.currentMinMz == 1))
                    //{
                    //    var validRange = gMaxBin - gMinBin + 1;
                    //    if (this.prevYPixels > 1)
                    //    {
                    //        validRange = (int)Math.Round(validRange / this.prevYPixels);
                    //    }
                    //
                    //    var binRange = gMaxBin - gMinBin;
                    //    var lowerPct = (this.currentMinMz - gMinBin) / (double)binRange;
                    //    var upperPct = (this.currentMaxMz - gMinBin) / (double)binRange;
                    //    var newMinBin = this.dataReader.GetPixelMZ((int)Math.Floor(lowerPct * validRange));
                    //    var newMaxBin = this.dataReader.GetPixelMZ((int)Math.Ceiling(upperPct * validRange));
                    //    this.currentMinMz = (int)newMinBin;
                    //    this.currentMaxMz = (int)newMaxBin;
                    //}

                    //int currentMinBin = (int) Math.Floor(this.Calibrator.MZtoBin(this.CurrentMinMz));
                    //int currentMaxBin = (int) Math.Ceiling(this.Calibrator.MZtoBin(this.CurrentMaxMz));

                    var frametype = GetFrameType(this.frameType);
                    double[] mzs;
                    int[] intensities;

                    // For pulling the spectrum data from the UIMF file

                    this.dataReader.GetSpectrum(
                        this.StartFrameNumber,
                        this.EndFrameNumber,
                        frametype,
                        ranges.Item3,
                        ranges.Item4,
                        out mzs,
                        out intensities);
                    this.MzArray = mzs;

                    this.MzIntensities = intensities;


                    


                    var collapsedFrame =
                        new double[Frames - StartFrameNumber + 1, ranges.Item4 - ranges.Item3 + 1];

                    for (var i = 1; i < this.Frames + 1; i++)
                    {

                        var frame = this.dataReader.AccumulateFrameData(i, i, false,
                            ranges.Item3, ranges.Item4, currentMinBin,
                            currentMaxBin,
                            this.ValuesPerPixelX, this.ValuesPerPixelY);
                        for (int scan = 0; scan < frame.GetLength(0); scan++)
                        {
                            for (int mzindex = 0; mzindex < frame.GetLength(1); mzindex++)
                            {
                                collapsedFrame[i - 1, scan] += frame[scan, mzindex];
                            }
                        }
                    }
                    FrameCollapsed = collapsedFrame;


                    //var uncompressed = this.dataReader.AccumulateFrameData(
                    //    this.startFrameNumber,
                    //    this.endFrameNumber,
                    //    false,
                    //    this.startScan,
                    //    this.endScan,
                    //    currentMinBin,
                    //    currentMaxBin,
                    //    1,
                    //    1);
                    //this.Uncompressed = uncompressed;
                    //exceptionEncountered = false;

                    var arrayLength =
                        (int)Math.Round((currentMaxBin - currentMinBin + 1) / this.ValuesPerPixelY);

                    var tof = new double[arrayLength];
                    var mz = new double[arrayLength];
                    var start = this.dataReader.GetBinForPixel(0);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        tof[i] = this.dataReader.GetBinForPixel((int)Math.Round(i * ValuesPerPixelY));
                        mz[i] = this.calibrator.BinToMZ(tof[i]);
                        tof[i] = this.calibrator.MZtoTOF(mz[i]) / 10000.0;
                    }
                    this.BinToMzMap = mz;
                    this.BinToTofMap = tof;

                    this.GateData();

                    var frameData = this.dataReader.AccumulateFrameData(
                        this.StartFrameNumber,
                        this.EndFrameNumber,
                        false,
                        ranges.Item3,
                        ranges.Item4,
                        currentMinBin,
                        currentMaxBin,
                        (int)this.ValuesPerPixelX,
                        (int)this.ValuesPerPixelY);

                    return frameData;
                }
            }
           
        }

        public double Height { get; set; }

        public double Width { get; set; }

        /// <summary>
        /// The update high gate.
        /// </summary>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        public void UpdateHighGate(double newValue)
        {
            this.HighGate = newValue;
            this.GateData();
        }

        /// <summary>
        /// The update low gate.
        /// </summary>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        public void UpdateLowGate(double newValue)
        {
            if (this.frameData == null)
            {
                return;
            }

            this.LowGate = newValue;
            this.GateData();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dataReader != null)
                {
                    this.dataReader.Dispose();
                    this.dataReader = null;
                }
            }
        }

        /// <summary>
        /// The get frame type.
        /// </summary>
        /// <param name="frameTypeString">
        /// The frame type.
        /// </param>
        /// <returns>
        /// The <see cref="FrameType"/>.
        /// </returns>
        private static DataReader.FrameType GetFrameType(string frameTypeString)
        {
            var temp = frameTypeString.ToLower();
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
                    return DataReader.FrameType.MS1;

                    ////throw new NotImplementedException(
                    ////    "Only the MS1, MS2, Calibration, and Prescan frame types have been implemented in this version");
            }
        }

        /// <summary>
        /// The gate data.
        /// </summary>
        private void GateData()
        {
            //if (this.LowGate <= 0)
            //{
            //    this.GatedFrameData = this.FrameData;
            //    return;
            //}

            //var temp = new double[this.FrameData.GetLength(0), this.FrameData.GetLength(1)];

            //for (var x = 0; x < temp.GetLength(0); x++)
            //{
            //    for (var y = 0; y < temp.GetLength(1); y++)
            //    {
            //        if (this.FrameData[x, y] > this.LowGate && this.FrameData[x, y] < this.HighGate)
            //        {
            //            temp[x, y] = this.FrameData[x, y];
            //        }
            //    }
            //}

            //this.GatedFrameData = temp;
        }


        #endregion

        //public double[,] Uncompressed { get { return _uncompressed; } set
        //{
        //    this.RaiseAndSetIfChanged(ref this._uncompressed, value);
        //} }

        public double[] BinToTofMap
        {
            get
            {
                return this.binToTofMap;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.binToTofMap, value);
            }
        }

        public double UncompressedDeltaMz { get; set; }

        public double[,] FrameCollapsed { get; set; }

        public double TenthsOfNanoSecondsPerBin { get; set; }

        internal void UpdateTofTime(int frameNumber)
        {
            TenthsOfNanoSecondsPerBin =
                Convert.ToDouble(
                    this.dataReader.GetFrameParams(1).Values[FrameParamKeyType.AverageTOFLength].Value);
        }
    }
}