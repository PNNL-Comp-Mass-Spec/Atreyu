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
        /// TODO The _data reader.
        /// </summary>
        private DataReader _dataReader;

        /// <summary>
        /// TODO The bin to mz map.
        /// </summary>
        private double[] binToMzMap;

        /// <summary>
        /// TODO The current max bin.
        /// </summary>
        private int currentMaxBin;

        /// <summary>
        /// TODO The current min bin.
        /// </summary>
        private int currentMinBin;

        /// <summary>
        /// TODO The end scan.
        /// </summary>
        private int endScan;

        /// <summary>
        /// TODO The endframe number.
        /// </summary>
        private int endframeNumber;

        ///// <summary>
        ///// TODO The _start bin.
        ///// </summary>
        // private int _startBin;

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
        /// TODO The scans.
        /// </summary>
        private int scans;

        /// <summary>
        /// TODO The startframe number.
        /// </summary>
        private int startframeNumber;

        /// <summary>
        /// TODO The startscan.
        /// </summary>
        private int startscan;

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
        /// </param>
        public UimfData(string uimfFile)
        {
            this._dataReader = new DataReader(uimfFile);
            var global = this._dataReader.GetGlobalParams();
            this.Frames = this._dataReader.GetGlobalParams().NumFrames;
            this.MaxBins = global.Bins;
            this.TotalBins = this.MaxBins;
            this.Scans = this._dataReader.GetFrameParams(1).Scans;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the bin to mz map.
        /// </summary>
        public double[] BinToMzMap
        {
            get
            {
                return this.binToMzMap;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.binToMzMap, value);
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
                return this.endframeNumber;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.endframeNumber, value);
            }
        }

        /// <summary>
        /// Gets the end scan.
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
        /// Gets or sets the frame data.
        /// </summary>
        public double[,] FrameData
        {
            get
            {
                return this.frameData;
            }

            set
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
        /// Gets or sets the frame type.
        /// </summary>
        public string FrameType
        {
            get
            {
                return this.frameType;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.frameType, value);
            }
        }

        /// <summary>
        /// Gets or sets the frames.
        /// </summary>
        public int Frames
        {
            get
            {
                return this.frames;
            }

            set
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
        /// Gets or sets the gate.
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
        /// Gets or sets the scans.
        /// </summary>
        public int Scans
        {
            get
            {
                return this.scans;
            }

            set
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
                return this.startframeNumber;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.startframeNumber, value);
            }
        }

        /// <summary>
        /// Gets the start scan.
        /// </summary>
        public int StartScan
        {
            get
            {
                return this.startscan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.startscan, value);
            }
        }

        /// <summary>
        /// Gets or sets the total bins currently queried.
        /// </summary>
        public int TotalBins
        {
            get
            {
                return this.totalBins;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.totalBins, value);
            }
        }

        /// <summary>
        /// Gets or sets the values per pixel x.
        /// </summary>
        public double ValuesPerPixelX
        {
            get
            {
                return this.valuesPerPixelX;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.valuesPerPixelX, value);
            }
        }

        /// <summary>
        /// Gets or sets the values per pixel y.
        /// </summary>
        public double ValuesPerPixelY
        {
            get
            {
                return this.valuesPerPixelY;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.valuesPerPixelY, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        public async Task<double[,]> ReadData(bool returnGatedData = false)
        {
            if (this.CurrentMaxBin < 1) return new double[0,0];
            if (this.endScan < 1) return new double[0, 0];
            var frameParams = this._dataReader.GetFrameParams(this.startframeNumber);
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
                            var temp = this._dataReader.AccumulateFrameData(
                                this.startframeNumber,
                                this.EndFrameNumber,
                                false,
                                this.StartScan,
                                this.EndScan,
                                this.CurrentMinBin,
                                this.CurrentMaxBin,
                                (int)this.ValuesPerPixelX,
                                (int)this.ValuesPerPixelY);

                            var arrayLength = (int)Math.Round((this.CurrentMaxBin - this.currentMinBin + 1) / this.ValuesPerPixelY);

                            var tof = new double[arrayLength];
                            var mz = new double[arrayLength];
                            var calibrator = this._dataReader.GetMzCalibrator(frameParams);

                            for (var i = 0; i < arrayLength; i++)
                            {
                                tof[i] = this._dataReader.GetPixelMZ(i);
                                mz[i] = calibrator.TOFtoMZ(tof[i] * 10);
                            }

                            this.BinToMzMap = mz;

                            this.FrameData = temp;
                        });
            }

            this.GateData();

            return returnGatedData ? this.GatedFrameData : this.FrameData;
        }

        private int mostRecentHeight;
        private int mostRecentWidth;


        /// <summary>
        /// TODO The read data.
        /// </summary>
        /// <param name="startBin">
        /// TODO The start bin.
        /// </param>
        /// <param name="endBin">
        /// TODO The end bin.
        /// </param>
        /// <param name="startFrameNumber">
        /// TODO The start frame number.
        /// </param>
        /// <param name="endFrameNumber">
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
            int startFrameNumber, 
            int endFrameNumber, 
            int height, 
            int width, 
            int startScanValue = 0, 
            int endScanValue = 359, 
            bool returnGatedData = false)
        {
            this.UpdateScanRange(startScanValue, endScanValue);

            this.CurrentMinBin = startBin < 0 ? 0 : startBin;
            this.CurrentMaxBin = endBin > this.MaxBins ? this.MaxBins : endBin;

            this.StartFrameNumber = startFrameNumber;
            this.EndFrameNumber = endFrameNumber;
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
            if (this.frameData == null) { return; }

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
                if (this._dataReader != null)
                {
                    this._dataReader.Dispose();
                    this._dataReader = null;
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

        #endregion
    }
}