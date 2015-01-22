﻿// --------------------------------------------------------------------------------------------------------------------
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

        ///// <summary>
        ///// TODO The _start bin.
        ///// </summary>
        //private int _startBin;

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

        private double[,] frameData;
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

        private int frames;
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

        private int maxBins;

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

        private int scans;

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

        private int totalBins;

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


        private double valuesPerPixelX;

        private double valuesPerPixelY;

        private int currentMaxBin;

        private int currentMinBin;

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

            this.TotalBins = this.CurrentMaxBin - this.CurrentMinBin + 1;
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