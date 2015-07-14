// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeakFinder.cs" company="Pacific Northwest National Laboratory">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Pacific Northwest National Laboratory
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// <summary>
//   The peak finder class file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MagnitudeConcavityPeakFinder;

    using Utilities.Models;

    /// <summary>
    /// The peak finder class, gives additional information over what the magnitude peak finder provides.
    /// </summary>
    public static class PeakFinder
    {
        #region Public Methods and Operators

        /// <summary>
        /// Find the peaks in the data set passed and returns a list of peak information.
        /// </summary>
        /// <param name="dataList">
        /// The data List.
        /// </param>
        /// <param name="numberOfTopPeaks">
        /// The number of peaks to return, starting with the highest intensity, if less than one it will return all peaks.
        /// </param>
        /// <returns>
        /// The <see cref="System.Collections.Generic.List{T}"/> of peaks.
        /// </returns>
        public static PeakSet FindPeaks(IReadOnlyList<KeyValuePair<double, double>> dataList, int numberOfTopPeaks = 0)
        {
            var datapointList = new PeakSet();
            const int Precision = 100000;

            var peakDetector = new PeakDetector();

            var finderOptions = PeakDetector.GetDefaultSICPeakFinderOptions();

            List<double> smoothedY;

            // Create a new dictionary so we don't modify the original one
            var tempFrameList = new List<KeyValuePair<int, double>>(dataList.Count);

            // We have to give it integers for the double, but we need this to handle doubles, so we will multiply the key by the precision
            // and later get the correct value back by dividing it out again
            // resultant linq quesry is less readable than this.
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < dataList.Count; i++)
            {
                tempFrameList.Add(new KeyValuePair<int, double>((int)(dataList[i].Key * Precision), dataList[i].Value));
            }

            // I am not sure what this does but in the example exe file that came with the library
            // they used half of the length of the list in their previous examples and this seems to work
            var originalpeakLocation = tempFrameList.Count / 2;

            var allPeaks = peakDetector.FindPeaks(
                finderOptions, 
                tempFrameList.OrderBy(x => x.Key).ToList(), 
                originalpeakLocation, 
                out smoothedY);

            IEnumerable<clsPeak> peakSet;

            if (numberOfTopPeaks > 0)
            {
                var topPeaks = allPeaks.OrderByDescending(peak => smoothedY[peak.LocationIndex]).Take(numberOfTopPeaks);
                peakSet = topPeaks;
            }
            else
            {
                peakSet = allPeaks;
            }

            foreach (var peak in peakSet)
            {
                var temp = CalculatePeakInformation(peak, tempFrameList, smoothedY, Precision);

                if (temp.ResolvingPower > 0 && !double.IsInfinity(temp.ResolvingPower))
                {
                    datapointList.Peaks.Add(temp);
                }
            }

            return datapointList;

            ////var tempList =
            ////    datapointList.Where(x => !double.IsInfinity(x.ResolvingPower)).OrderByDescending(x => x.Intensity).Take(10);
        }

        private static PeakInformation CalculatePeakInformation(
            clsPeak peak,
            List<KeyValuePair<int, double>> originalList,
            IReadOnlyList<double> smoothedIntensityValues,
            int precisionUsed)
        {
            const double Tolerance = 0.01;
            var centerPoint = originalList.ElementAt(peak.LocationIndex);
            var offsetCenter = centerPoint.Key; // + firstpoint.Key; 
            var intensity = centerPoint.Value;
            var smoothedPeakIntensity = smoothedIntensityValues[peak.LocationIndex];

            var realCenter = (double)offsetCenter / precisionUsed;
            var halfmax = smoothedPeakIntensity / 2.0;

            var currPoint = new KeyValuePair<int, double>(0, 0);
            var currPointIndex = 0;
            double leftMidpoint = 0;
            double rightMidPoint = 0;

            var allPoints = new List<PointInformation>();
            var leftSidePoints = new List<KeyValuePair<int, double>>();
            for (var l = peak.LeftEdge; l < peak.LocationIndex && l < originalList.Count; l++)
            {
                leftSidePoints.Add(originalList[l]);
                allPoints.Add(
                    new PointInformation
                        {
                            Location = (double)originalList[l].Key / precisionUsed,
                            Intensity = originalList[l].Value,
                            SmoothedIntensity = smoothedIntensityValues[l]
                        });
            }

            var rightSidePoints = new List<KeyValuePair<int, double>>();
            for (var r = peak.LocationIndex; r < peak.RightEdge && r < originalList.Count; r++)
            {
                rightSidePoints.Add(originalList[r]);
                allPoints.Add(
                    new PointInformation
                        {
                            Location = (double)originalList[r].Key / precisionUsed,
                            Intensity = originalList[r].Value,
                            SmoothedIntensity = smoothedIntensityValues[r]
                        });
            }

            // find the left side half max
            foreach (var leftSidePoint in leftSidePoints)
            {
                var prevPoint = currPoint;
                currPoint = leftSidePoint;
                var prevPointIndex = currPointIndex;

                currPointIndex = originalList.BinarySearch(
                    currPoint,
                    Comparer<KeyValuePair<int, double>>.Create((left, right) => left.Key - right.Key));

                if (smoothedIntensityValues[currPointIndex] < halfmax)
                {
                    continue;
                }

                if (Math.Abs(smoothedIntensityValues[currPointIndex] - halfmax) < Tolerance)
                {
                    leftMidpoint = currPoint.Key;
                    continue;
                }

                ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                double a1 = prevPoint.Key;
                double a2 = currPoint.Key;
                double c = halfmax;
                double b1 = smoothedIntensityValues[prevPointIndex];
                double b2 = smoothedIntensityValues[currPointIndex];

                leftMidpoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                break;
            }

            // find the right side of the half max
            foreach (var rightSidePoint in rightSidePoints)
            {
                var prevPoint = currPoint;
                currPoint = rightSidePoint;
                var prevPointIndex = currPointIndex;
                currPointIndex = originalList.BinarySearch(
                    currPoint,
                    Comparer<KeyValuePair<int, double>>.Create((left, right) => left.Key - right.Key));

                if (smoothedIntensityValues[currPointIndex] > halfmax || smoothedIntensityValues[currPointIndex] < 0)
                {
                    continue;
                }

                if (Math.Abs(smoothedIntensityValues[currPointIndex] - halfmax) < Tolerance)
                {
                    rightMidPoint = currPoint.Key;
                    continue;
                }

                ////var slope = (prevPoint.Key - currPoint.Key) / (prevPoint.Value - currPoint.Value);
                double a1 = prevPoint.Key;
                double a2 = currPoint.Key;
                double c = halfmax;
                double b1 = smoothedIntensityValues[prevPointIndex];
                double b2 = smoothedIntensityValues[currPointIndex];

                rightMidPoint = a1 + ((a2 - a1) * ((c - b1) / (b2 - b1)));
                break;
            }

            var correctedRightMidPoint = rightMidPoint / precisionUsed;
            var correctedLeftMidPoint = leftMidpoint / precisionUsed;
            var fullWidthHalfMax = correctedRightMidPoint - correctedLeftMidPoint;
            var resolution = realCenter / fullWidthHalfMax;

            var temp = new PeakInformation
                           {
                               AreaUnderThePeak = peak.Area,
                               FullWidthHalfMax = fullWidthHalfMax,
                               Intensity = intensity,
                               LeftMidpoint = correctedLeftMidPoint,
                               PeakCenter = realCenter,
                               RightMidpoint = correctedRightMidPoint,
                               ResolvingPower = resolution,
                               SmoothedIntensity = smoothedPeakIntensity,
                               TotalDataPointSet = allPoints
                           };
            return temp;
        }

        #endregion
    }
}