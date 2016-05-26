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
        /// The checking.
        /// </summary>
        private bool checking;

        /// <summary>
        /// The current max m/z.
        /// </summary>
        private double currentMaxMz;

        /// <summary>
        /// The current min m/z.
        /// </summary>
        private double currentMinMz;

        /// <summary>
        /// The data reader.
        /// </summary>
        private DataReader dataReader;

        /// <summary>
        /// The end frame number.
        /// </summary>
        private int endFrameNumber;

        /// <summary>
        /// The end scan.
        /// </summary>
        private int endScan;

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
        /// The most recent height.
        /// </summary>
        private int mostRecentHeight;

        /// <summary>
        /// The most recent width.
        /// </summary>
        private int mostRecentWidth;

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
        private MzRange mzWindow;

        /// <summary>
        /// The range update list.
        /// </summary>
        private ConcurrentQueue<Range> rangeUpdateList;

        /// <summary>
        /// The scans.
        /// </summary>
        private int scans;

        /// <summary>
        /// The start frame number.
        /// </summary>
        private int startFrameNumber;

        /// <summary>
        /// The start scan.
        /// </summary>
        private int startScan;

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
            this.RangeUpdateList = new ConcurrentQueue<Range>();
            this.dataReader = new DataReader(uimfFile);
            this.TenthsOfNanoSecondsPerBin = 0.0;
            var global = this.dataReader.GetGlobalParams();
            this.Frames = this.dataReader.GetGlobalParams().NumFrames;
            this.MaxBins = global.Bins;
            this.Calibrator = this.dataReader.GetMzCalibrator(this.dataReader.GetFrameParams(1));
            this.TenthsOfNanoSecondsPerBin = Convert.ToDouble(this.dataReader.GetFrameParams(1).Values[FrameParamKeyType.AverageTOFLength].Value);
            this.MaxMz = this.Calibrator.BinToMZ(global.Bins);
            this.MinMz = this.Calibrator.BinToMZ(0);
            this.TotalMzRange = this.MaxMz - this.MinMz;
            this.Scans = this.dataReader.GetFrameParams(1).Scans;
            this.checking = false;
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
        /// Gets or sets the current max bin.
        /// </summary>
        public double CurrentMaxMz
        {
            get
            {
                return this.currentMaxMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMaxMz, value);
            }
        }

        /// <summary>
        /// Gets or sets the current min bin.
        /// </summary>
        public double CurrentMinMz
        {
            get
            {
                return this.currentMinMz;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMinMz, value);
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
        /// Gets or sets the end scan.
        /// </summary>
        public int EndScan
        {
            get
            {
                return this.endScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.endScan, value);
            }
        }

        /// <summary>
        /// Gets the frame data.
        /// </summary>
        public double[,] FrameData
        {
            get
            {
                return this.frameData;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.frameData, value);
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
        /// Gets the range update list.
        /// </summary>
        public ConcurrentQueue<Range> RangeUpdateList
        {
            get
            {
                return this.rangeUpdateList;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.rangeUpdateList, value);
            }
        }

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

            private set
            {
                if (this.startFrameNumber != value)
                {
                    while (true)
                    {
                        try
                        {
                            var calib = this.dataReader.GetMzCalibrator(this.dataReader.GetFrameParams(value));
                            this.MinMz = calib.BinToMZ(0);
                            this.MaxMz = calib.BinToMZ(this.MaxBins);
                            break;
                        }
                        catch (Exception)
                        {
                            //Let stuff cycle for a millisecond before trying again. Error occurs due to invalid operation where
                            //data reader is already active
                            Task.Delay(1);
                        }
                    }
                }
                this.RaiseAndSetIfChanged(ref this.startFrameNumber, value);
            }
        }

        /// <summary>
        /// Gets or sets the start scan.
        /// </summary>
        public int StartScan
        {
            get
            {
                return this.startScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.startScan, value);
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
        /// The check queue method that processes the queue of updates.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public void CheckQueue()
        {
            if (this.checking)
            {
                return;
            }

            this.checking = true;
            Range currentRange;

            while (this.RangeUpdateList.TryDequeue(out currentRange))
            {
                this.ProcessData(currentRange);
                Task.Delay(1);
            }

            this.ReadData();

            this.checking = false;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get m/z range for a given mz window.
        /// </summary>
        /// <param name="centerMz">
        /// The center mz.
        /// </param>
        /// <param name="partsPerMillionTolerance">
        /// The parts per million tolerance.
        /// </param>
        /// <returns>
        /// The <see cref="MzRange"/>.
        /// </returns>
        public MzRange GetMzRangeForMzWindow(double centerMz, double partsPerMillionTolerance)
        {
            this.MzCenter = centerMz;
            this.PartsPerMillion = partsPerMillionTolerance;
            var range = new MzRange();

            if (this.dataReader == null || this.Calibrator == null)
            {
                range.StartMz = 0;
                range.EndMz = this.CurrentMaxMz;
                this.mzWindow = range;
                return range;
            }

            var mzOffset = this.MzCenter * (this.PartsPerMillion / 1000000.0);

            range.StartMz = this.MzCenter - mzOffset;

            range.EndMz = this.MzCenter + mzOffset;

            this.mzWindow = range;
            return range;
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

        /// <summary>
        /// The read data.
        /// </summary>
        /// <param name="returnGatedData">
        /// The return gated data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public void ReadData(bool returnGatedData = false)
        {
            const int MAX_ATTEMPTS = 3;

            //if (this.CurrentMaxMz < 1)
            if (this.CurrentMaxMz < this.MinMz)
            {
                return;
            }

            //if (this.endScan < 1)
            if (this.endScan < this.MinMz)
            {
                return;
            }

            this.LoadingData = true;

            var frameParams = this.dataReader.GetFrameParams(this.startFrameNumber);
            UncompressedDeltaMz = this.dataReader.GetDeltaMz(1);

            if (frameParams == null)
            {
                // Frame number is out of range
                this.FrameData = new double[0, 0];
            }
            else
            {
                this.TotalMzRange = this.CurrentMaxMz - this.CurrentMinMz + 1;

                this.prevYPixels = this.ValuesPerPixelY;
                this.Calibrator = this.dataReader.GetMzCalibrator(frameParams);
                int currentMinBin = (int)Math.Floor(this.Calibrator.MZtoBin(this.CurrentMinMz));
                int currentMaxBin = (int)Math.Ceiling(this.Calibrator.MZtoBin(this.CurrentMaxMz));
                var totalBinRange = currentMaxBin - currentMinBin + 1;
                //this.ValuesPerPixelY = (int)(this.TotalMzRange / (double)this.mostRecentHeight);
                this.ValuesPerPixelY = (int)(totalBinRange / (double)this.mostRecentHeight);

                var totalScans = this.EndScan - this.StartScan + 1;
                this.ValuesPerPixelX = (int) (totalScans / (double) this.mostRecentWidth);

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

                bool exceptionEncountered = true;
                var exceptionMessage = string.Empty;

                int numTries = 0;

                // For pulling the spectrum data from the UIMF file
                
                try
                {
                    this.dataReader.GetSpectrum(
                        this.StartFrameNumber,
                        this.EndFrameNumber,
                        frametype,
                        this.StartScan,
                        this.EndScan,
                        out mzs,
                        out intensities);
                    this.MzArray = mzs;

                    this.MzIntensities = intensities;
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(exceptionMessage) && !string.IsNullOrEmpty(ex.Message))
                        exceptionMessage = ex.Message;
                }
                
                var currentStep = "starting";

                // For pulling frame data from the UIMF file
                try
                {
                    currentStep = "Populate temp by calling AccumulateFrameData for frames " + startFrameNumber + " through " + EndFrameNumber;

                    var temp = this.dataReader.AccumulateFrameData(
                        this.startFrameNumber,
                        this.EndFrameNumber,
                        false,
                        this.StartScan,
                        this.EndScan,
                        currentMinBin,
                        currentMaxBin,
                        (int)this.ValuesPerPixelX,
                        (int)this.ValuesPerPixelY);
                    this.FrameData = temp;
                    currentStep = "Initialize 2D array collapsedFrame with size " + (Frames - StartFrameNumber + 1) +"," + (this.EndScan - this.StartScan + 1);

                    var collapsedFrame =
                        new double[Frames - StartFrameNumber + 1, this.EndScan - this.StartScan + 1];

                    for (var i = 1; i < this.Frames + 1; i++)
                    {
                        currentStep = "Call to AccumulateFrameData for frame " + i;

                        var frame = this.dataReader.AccumulateFrameData(i, i, false,
                                                                        this.StartScan, this.EndScan, currentMinBin,
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

                    currentStep = "Populate uncompressed by calling AccumulateFrameData for frames " + startFrameNumber + " through " + EndFrameNumber;

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
                }
                catch (System.OutOfMemoryException ex)
                {
                    exceptionMessage = "Out of memory: " + ex.Message + "; " + currentStep;
                    numTries = MAX_ATTEMPTS;
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(exceptionMessage) && !string.IsNullOrEmpty(ex.Message))
                        exceptionMessage = ex.Message;
                }

                var arrayLength =
                    (int) Math.Round((currentMaxBin - currentMinBin + 1) / this.ValuesPerPixelY);

                var tof = new double[arrayLength];
                var mz = new double[arrayLength];
                var start = this.dataReader.GetBinForPixel(0);
                for (var i = 0; i < arrayLength; i++)
                {
                    tof[i] = this.dataReader.GetBinForPixel((int) Math.Round(i*ValuesPerPixelY));
                    mz[i] = this.calibrator.BinToMZ(tof[i]);
                    tof[i] = this.calibrator.MZtoTOF(mz[i])/10000.0;
                }
                this.BinToMzMap = mz;
                this.BinToTofMap = tof;
            }

            this.GateData();

            this.LoadingData = false;
        }

        /// <summary>
        /// The read data.
        /// </summary>
        /// <param name="startMz">
        /// The start bin.
        /// </param>
        /// <param name="endMz">
        /// The end bin.
        /// </param>
        /// <param name="startFrame">
        /// The start frame number.
        /// </param>
        /// <param name="endFrame">
        /// The end frame number.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="startScanValue">
        /// The start scan.
        /// </param>
        /// <param name="endScanValue">
        /// The end scan.
        /// </param>
        /// <param name="returnGatedData">
        /// Whether or not the returned data should be gated
        /// </param>
        /// <returns>
        /// The 2d array of doubles that represents the data, index 0 is bins, index 1 in scans.
        /// </returns>
        public void ReadData(
            double startMz, 
            double endMz, 
            int startFrame, 
            int endFrame, 
            int height, 
            int width, 
            int startScanValue = 0, 
            int endScanValue = 359, 
            bool returnGatedData = false)
        {
            this.UpdateScanRange(startScanValue, endScanValue);

            //this.CurrentMinMz = startMz < 0 ? 0 : startMz;
            //this.CurrentMaxMz = endMz > this.MaxBins ? this.MaxBins : endMz;
            this.CurrentMinMz = startMz < this.MinMz ? this.MinMz : startMz;
            this.CurrentMaxMz = endMz > this.MaxMz ? this.MaxMz : endMz;

            this.StartFrameNumber = startFrame;
            this.EndFrameNumber = endFrame;
            this.mostRecentHeight = height;
            this.mostRecentWidth = width;
        }

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

        /// <summary>
        /// The update scan range.
        /// </summary>
        /// <param name="startScanNew">
        /// The start scan new.
        /// </param>
        /// <param name="endScanNew">
        /// The end scan new.
        /// </param>
        public void UpdateScanRange(int startScanNew, int endScanNew)
        {
            this.EndScan = endScanNew > this.Scans ? this.Scans : endScanNew;

            this.StartScan = startScanNew < 0 ? 0 : startScanNew;
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
            if (this.LowGate <= 0)
            {
                this.GatedFrameData = this.FrameData;
                return;
            }

            var temp = new double[this.FrameData.GetLength(0), this.FrameData.GetLength(1)];

            for (var x = 0; x < temp.GetLength(0); x++)
            {
                for (var y = 0; y < temp.GetLength(1); y++)
                {
                    if (this.FrameData[x, y] > this.LowGate && this.FrameData[x, y] < this.HighGate)
                    {
                        temp[x, y] = this.FrameData[x, y];
                    }
                }
            }

            this.GatedFrameData = temp;
        }

        /// <summary>
        /// The process data.
        /// </summary>
        /// <param name="range">
        /// The range.
        /// </param>
        /// <exception cref="ArgumentException">
        /// thrown if the range type is set to a type that it cannot be cast to.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// thrown if an unknown range type is used.  The currently known types are Mz, Frame, and Scan.
        /// </exception>
        private void ProcessData(Range range)
        {
            switch (range.RangeType)
            {
                case RangeType.MzRange:
                    var mzRange = range as MzRange;
                    if (mzRange == null)
                    {
                        throw new ArgumentException(
                            "Range has it's RangeType set to MzRange but cannot be cast to MzRange", 
                            "range");
                    }

                    double min, max;

                    if (this.WindowMz && this.mzWindow != null)
                    {
                        min = this.mzWindow.StartMz;
                        max = this.mzWindow.EndMz;
                    }
                    else
                    {
                        min = this.MinMz;
                        max = this.MaxMz;
                    }

                    if (mzRange.StartMz < min)
                    {
                        mzRange.StartMz = min;
                    }

                    if (mzRange.EndMz > max)
                    {
                        mzRange.EndMz = max;
                    }

                    this.CurrentMinMz = mzRange.StartMz;
                    this.CurrentMaxMz = mzRange.EndMz;

                    break;
                case RangeType.FrameRange:
                    var frameRange = range as FrameRange;
                    if (frameRange == null)
                    {
                        throw new ArgumentException(
                            "Range has it's RangeType set to FrameRange but cannot be cast to FrameRange", 
                            "range");
                    }

                    this.StartFrameNumber = frameRange.StartFrame;
                    this.EndFrameNumber = frameRange.EndFrame;
                    break;
                case RangeType.ScanRange:
                    var scanRange = range as ScanRange;
                    if (scanRange == null)
                    {
                        throw new ArgumentException(
                            "Range has it's RangeType set to ScanRange but cannot be cast to ScanRange", 
                            "range");
                    }

                    this.StartScan = scanRange.StartScan;
                    this.EndScan = scanRange.EndScan;
                    break;
                default:
                    throw new NotImplementedException(
                        "Currently ProcessRangeData only supports types of MzRange, FrameRange, and ScanRange, "
                        + "but you passed something else and it scared us too much to continue.");
            }
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