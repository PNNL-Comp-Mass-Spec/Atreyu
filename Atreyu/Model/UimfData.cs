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

    using UIMFLibrary;

    /// <summary>
    /// TODO The uimf data.
    /// </summary>
    public class UimfData : IDisposable
    {
        #region Fields

        /// <summary>
        /// TODO The _data reader.
        /// </summary>
        private DataReader _dataReader;

        /// <summary>
        /// TODO The _start bin.
        /// </summary>
        private int _startBin;

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
            var global = this._dataReader.GetGlobalParameters();
            this.Frames = this._dataReader.GetGlobalParameters().NumFrames;
            this.MaxBins = global.Bins;
            this.TotalBins = this.MaxBins;
            this.Scans = this._dataReader.GetFrameParameters(1).Scans;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the frame data.
        /// </summary>
        public double[,] FrameData { get; set; }

        /// <summary>
        /// Gets or sets the frames.
        /// </summary>
        public int Frames { get; set; }

        /// <summary>
        /// Gets or sets the max bins.
        /// </summary>
        public int MaxBins { get; set; }

        /// <summary>
        /// Gets or sets the scans.
        /// </summary>
        public int Scans { get; set; }

        /// <summary>
        /// Gets or sets the total bins.
        /// </summary>
        public int TotalBins { get; set; }

        /// <summary>
        /// Gets or sets the values per pixel x.
        /// </summary>
        public double ValuesPerPixelX { get; set; }

        /// <summary>
        /// Gets or sets the values per pixel y.
        /// </summary>
        public double ValuesPerPixelY { get; set; }

        /// <summary>
        /// Gets or sets the current max bin.
        /// </summary>
        public int currentMaxBin { get; set; }

        /// <summary>
        /// Gets or sets the current min bin.
        /// </summary>
        public int currentMinBin { get; set; }

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
        public double[,] ReadData(
            int startBin, 
            int endBin, 
            int startFrameNumber, 
            int endFrameNumber, 
            int height, 
            int startScan = 0, 
            int endScan = 359)
        {
            if (endScan > this.Scans)
            {
                endScan = this.Scans;
            }

            if (startScan < 0)
            {
                startScan = 0;
            }

            this.TotalBins = this.currentMaxBin - this.currentMinBin + 1;
            this.ValuesPerPixelY = this.TotalBins / (double)height;

            if (this.ValuesPerPixelY < 1)
            {
                this.ValuesPerPixelY = 1;
            }

            if (this.ValuesPerPixelX < 1)
            {
                this.ValuesPerPixelX = 1;
            }
            
            this.FrameData = this._dataReader.AccumulateFrameData(
                startFrameNumber, 
                endFrameNumber, 
                false, 
                startScan, 
                endScan, 
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

        #endregion
    }
}