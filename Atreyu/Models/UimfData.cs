// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UimfData.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The uimf data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using ReactiveUI;

    using UIMFLibrary;

    /// <summary>
    /// TODO The uimf data.
    /// </summary>
    public class UimfData : ReactiveObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// TODO The bin to mz map.
        /// </summary>
        private double[] binToMzMap;

        /// <summary>
        /// TODO The checking.
        /// </summary>
        private bool checking;

        /// <summary>
        /// TODO The current max bin.
        /// </summary>
        private int currentMaxBin;

        /// <summary>
        /// TODO The current min bin.
        /// </summary>
        private int currentMinBin;

        /// <summary>
        /// TODO The data reader.
        /// </summary>
        private DataReader dataReader;

        /// <summary>
        /// TODO The end frame number.
        /// </summary>
        private int endFrameNumber;

        /// <summary>
        /// TODO The end scan.
        /// </summary>
        private int endScan;

        /// <summary>
        /// TODO The frame data.
        /// </summary>
        private double[,] frameData;

        /// <summary>
        /// TODO The frame intercept.
        /// </summary>
        private double frameIntercept;

        /// <summary>
        /// TODO The frame slope.
        /// </summary>
        private double frameSlope;

        /// <summary>
        /// TODO The frame type.
        /// </summary>
        private string frameType;

        /// <summary>
        /// TODO The frames.
        /// </summary>
        private int frames;

        /// <summary>
        /// TODO The gated frame data.
        /// </summary>
        private double[,] gatedFrameData;

        /// <summary>
        /// TODO The high gate.
        /// </summary>
        private double highGate = double.PositiveInfinity;

        /// <summary>
        /// TODO The gate.
        /// </summary>
        private double lowGate;

        /// <summary>
        /// TODO The max bins.
        /// </summary>
        private int maxBins;

        /// <summary>
        /// TODO The most recent height.
        /// </summary>
        private int mostRecentHeight;

        /// <summary>
        /// TODO The most recent width.
        /// </summary>
        private int mostRecentWidth;

        private MzCalibrator calibrator;

        /// <summary>
        /// TODO The range update list.
        /// </summary>
        private ConcurrentQueue<Range> rangeUpdateList;

        /// <summary>
        /// TODO The scans.
        /// </summary>
        private int scans;

        /// <summary>
        /// TODO The start frame number.
        /// </summary>
        private int startFrameNumber;

        /// <summary>
        /// TODO The start scan.
        /// </summary>
        private int startScan;

        /// <summary>
        /// TODO The total bins.
        /// </summary>
        private int totalBins;

        /// <summary>
        /// TODO The values per pixel x.
        /// </summary>
        private double valuesPerPixelX;

        /// <summary>
        /// TODO The values per pixel y.
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
            var global = this.dataReader.GetGlobalParams();
            this.Frames = this.dataReader.GetGlobalParams().NumFrames;
            this.MaxBins = global.Bins;
            this.TotalBins = this.MaxBins;
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
        public int CurrentMaxBin
        {
            get
            {
                return this.currentMaxBin;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMaxBin, value);
            }
        }

        /// <summary>
        /// Gets or sets the current min bin.
        /// </summary>
        public int CurrentMinBin
        {
            get
            {
                return this.currentMinBin;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMinBin, value);
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
        public int TotalBins
        {
            get
            {
                return this.totalBins;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.totalBins, value);
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The check queue.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CheckQueue()
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
                await Task.Delay(1);
            }

            await this.ReadData();

            this.checking = false;
        }

        /// <summary>
        /// TODO The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// TODO The read data.
        /// </summary>
        /// <param name="returnGatedData">
        /// TODO The return gated data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<double[,]> ReadData(bool returnGatedData = false)
        {
            if (this.CurrentMaxBin < 1)
            {
                return new double[0, 0];
            }

            if (this.endScan < 1)
            {
                return new double[0, 0];
            }

            var frameParams = this.dataReader.GetFrameParams(this.startFrameNumber);

            if (frameParams == null)
            {
                // Frame number is out of range
                this.FrameData = new double[0, 0];
            }
            else
            {
                this.TotalBins = this.CurrentMaxBin - this.CurrentMinBin + 1;

                this.ValuesPerPixelY = (int)(this.TotalBins / (double)this.mostRecentHeight);

                var totalScans = this.EndScan - this.StartScan + 1;
                this.ValuesPerPixelX = (int)(totalScans / (double)this.mostRecentWidth);

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

                await Task.Run(
                    () =>
                        {
                            var temp = this.dataReader.AccumulateFrameData(
                                this.startFrameNumber, 
                                this.EndFrameNumber, 
                                false, 
                                this.StartScan, 
                                this.EndScan, 
                                this.CurrentMinBin, 
                                this.CurrentMaxBin, 
                                (int)this.ValuesPerPixelX, 
                                (int)this.ValuesPerPixelY);

                            var arrayLength =
                                (int)Math.Round((this.CurrentMaxBin - this.currentMinBin + 1) / this.ValuesPerPixelY);

                            var tof = new double[arrayLength];
                            var mz = new double[arrayLength];
                            this.Calibrator = this.dataReader.GetMzCalibrator(frameParams);

                            for (var i = 0; i < arrayLength; i++)
                            {
                                tof[i] = this.dataReader.GetPixelMZ(i);
                                mz[i] = calibrator.TOFtoMZ(tof[i] * 10);
                            }

                            this.BinToMzMap = mz;

                            this.FrameData = temp;
                        });
            }

            this.GateData();

            return returnGatedData ? this.GatedFrameData : this.FrameData;
        }

        /// <summary>
        /// TODO The read data.
        /// </summary>
        /// <param name="startBin">
        /// TODO The start bin.
        /// </param>
        /// <param name="endBin">
        /// TODO The end bin.
        /// </param>
        /// <param name="startFrame">
        /// TODO The start frame number.
        /// </param>
        /// <param name="endFrame">
        /// TODO The end frame number.
        /// </param>
        /// <param name="height">
        /// TODO The height.
        /// </param>
        /// <param name="width">
        /// TODO The width.
        /// </param>
        /// <param name="startScanValue">
        /// TODO The start scan.
        /// </param>
        /// <param name="endScanValue">
        /// TODO The end scan.
        /// </param>
        /// <param name="returnGatedData">
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public async Task<double[,]> ReadData(
            int startBin, 
            int endBin, 
            int startFrame, 
            int endFrame, 
            int height, 
            int width, 
            int startScanValue = 0, 
            int endScanValue = 359, 
            bool returnGatedData = false)
        {
            this.UpdateScanRange(startScanValue, endScanValue);

            this.CurrentMinBin = startBin < 0 ? 0 : startBin;
            this.CurrentMaxBin = endBin > this.MaxBins ? this.MaxBins : endBin;

            this.StartFrameNumber = startFrame;
            this.EndFrameNumber = endFrame;
            this.mostRecentHeight = height;
            this.mostRecentWidth = width;
            return await this.ReadData(returnGatedData);
        }

        /// <summary>
        /// TODO The update high gate.
        /// </summary>
        /// <param name="newValue">
        /// TODO The new value.
        /// </param>
        public void UpdateHighGate(double newValue)
        {
            this.HighGate = newValue;
            this.GateData();
        }

        /// <summary>
        /// TODO The update low gate.
        /// </summary>
        /// <param name="newValue">
        /// TODO The new value.
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
        /// TODO The update scan range.
        /// </summary>
        /// <param name="startScanNew">
        /// TODO The start scan new.
        /// </param>
        /// <param name="endScanNew">
        /// TODO The end scan new.
        /// </param>
        public void UpdateScanRange(int startScanNew, int endScanNew)
        {
            this.EndScan = endScanNew > this.Scans ? this.Scans : endScanNew;

            this.StartScan = startScanNew < 0 ? 0 : startScanNew;
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The dispose.
        /// </summary>
        /// <param name="disposing">
        /// TODO The disposing.
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
        /// TODO The gate data.
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
        /// TODO The process data.
        /// </summary>
        /// <param name="range">
        /// TODO The range.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void ProcessData(Range range)
        {
            switch (range.RangeType)
            {
                case RangeType.BinRange:
                    var binRange = range as BinRange;
                    if (binRange == null)
                    {
                        throw new ArgumentException(
                            "Range has it's RangeType set to BinRange but cannot be cast to BinRange", 
                            "range");
                    }

                    this.CurrentMinBin = binRange.StartBin;
                    this.CurrentMaxBin = binRange.EndBin;
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
                        "Currently ProcessRangeData only supports types of BinRange, FrameRange, and ScanRange, "
                        + "but you passed something else and it scared us too much to continue.");
            }
        }

        #endregion
    }
}