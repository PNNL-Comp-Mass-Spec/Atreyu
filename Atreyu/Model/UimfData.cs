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
        /// TODO The frames.
        /// </summary>
        private int frames;

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

            private set
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

            private set
            {
                this.RaiseAndSetIfChanged(ref this.startscan, value);
            }
        }

        /// <summary>
        /// Gets or sets the total bins.
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

        /// <summary>
        /// </summary>
        /// <param name="startBin">
        /// </param>
        /// <param name="endBin">
        /// </param>
        /// <param name="startFrameNumber">
        /// </param>
        /// <param name="endFrameNumber">
        /// </param>
        /// <param name="height">
        /// </param>
        /// <param name="startScan">
        /// </param>
        /// <param name="endScan">
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        [Obsolete("It is suggested that you use ReadData that specifies a width instead")]
        public double[,] ReadData(
            int startBin, 
            int endBin, 
            int startFrameNumber, 
            int endFrameNumber, 
            int height, 
            int startScan = 0, 
            int endScan = 359)
        {            
            UpdateScanRange(startScan, endScan);
            
            this.TotalBins = this.CurrentMaxBin - this.CurrentMinBin + 1;
            this.ValuesPerPixelY = this.TotalBins / (double)height;

            var totalScans = this.EndScan - this.StartScan + 1;

            // this.valuesPerPixelX = totalScans / (double)width;
            if (this.ValuesPerPixelY < 1)
            {
                this.ValuesPerPixelY = 1;
            }

            if (this.ValuesPerPixelX < 1)
            {
                this.ValuesPerPixelX = 1;
            }

            this.StartFrameNumber = startFrameNumber;
            this.EndFrameNumber = endFrameNumber;

            var frameParams = this._dataReader.GetFrameParams(startFrameNumber);

            this.FrameSlope = frameParams.GetValueDouble(FrameParamKeyType.CalibrationSlope);
            this.FrameIntercept = frameParams.GetValueDouble(FrameParamKeyType.CalibrationIntercept);

            this.FrameData = this._dataReader.AccumulateFrameData(
                startFrameNumber, 
                endFrameNumber, 
                false, 
                this.StartScan, 
                this.EndScan, 
                startBin, 
                endBin, 
                this.ValuesPerPixelX, 
                this.ValuesPerPixelY);

            return this.FrameData;
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
        /// <param name="startScan">
        /// TODO The start scan.
        /// </param>
        /// <param name="endScan">
        /// TODO The end scan.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public double[,] ReadData(
            int startBin, 
            int endBin, 
            int startFrameNumber, 
            int endFrameNumber, 
            int height, 
            int width, 
            int startScan = 0, 
            int endScan = 359)
        {
            UpdateScanRange(startScan, endScan);

            this.TotalBins = this.CurrentMaxBin - this.CurrentMinBin + 1;
            this.ValuesPerPixelY = this.TotalBins / (double)height;

            var totalScans = this.EndScan - this.StartScan + 1;
            this.valuesPerPixelX = totalScans / (double)width;

            if (this.ValuesPerPixelY < 1)
            {
                this.ValuesPerPixelY = 1;
            }

            if (this.ValuesPerPixelX < 1)
            {
                this.ValuesPerPixelX = 1;
            }

            this.StartFrameNumber = startFrameNumber;
            this.EndFrameNumber = endFrameNumber;

            var frameParams = this._dataReader.GetFrameParams(startFrameNumber);

            this.FrameSlope = frameParams.GetValueDouble(FrameParamKeyType.CalibrationSlope);
            this.FrameIntercept = frameParams.GetValueDouble(FrameParamKeyType.CalibrationIntercept);

            this.FrameData = this._dataReader.AccumulateFrameData(
                startFrameNumber, 
                endFrameNumber, 
                false, 
                this.StartScan, 
                this.EndScan, 
                startBin, 
                endBin, 
                this.ValuesPerPixelX, 
                this.ValuesPerPixelY);

            return this.FrameData;
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

        public void UpdateScanRange(int startScanNew, int endScanNew)
        {
            this.EndScan = endScanNew > this.Scans ? this.Scans : endScanNew;

            this.StartScan = startScanNew < 0 ? 0 : startScanNew;
        }

        #endregion
    }
}