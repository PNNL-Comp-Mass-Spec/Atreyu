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
        public static PeakSet FindPeaks(IEnumerable<KeyValuePair<double, double>> data, int numberOfTopPeaks = 0)
        {
            var dataList = data.ToList();
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

            // I am not sure what the library does with this but in the example exe file that came with the library
            // they used half of the length of the list in their previous examples and this seems to work.
            // To be extra clear to future programmers, changing this number causes memory out of bounds from the peak detector
            var originalpeakLocation = tempFrameList.Count / 2;

            var allPeaks = peakDetector.FindPeaks(
                finderOptions, 
                tempFrameList.OrderBy(x => x.Key).ToList(), 
                originalpeakLocation, 
                out smoothedY);

            IEnumerable<clsPeak> peakSet;

            // if a few peaks are wanted, it is better that we trim them up front, before we calculate the extra information
            if (numberOfTopPeaks > 0)
            {
                var topPeaks = allPeaks.OrderByDescending(peak => smoothedY[peak.LocationIndex]).Take(numberOfTopPeaks);
                peakSet = topPeaks;
            }
            else
            {
                peakSet = allPeaks;
            }

            var calculatedPeakSet =
                peakSet.Select(peak => CalculatePeakInformation(peak, tempFrameList, smoothedY, Precision))
                    .Where(p => p.ResolvingPower > 0 && !double.IsInfinity(p.ResolvingPower));
            
            return new PeakSet(calculatedPeakSet);

            ////var tempList =
            ////    datapointList.Where(x => !double.IsInfinity(x.ResolvingPower)).OrderByDescending(x => x.Intensity).Take(10);
        }

        /// <summary>
        /// Calculates peak information.
        /// </summary>
        /// <param name="peak">
        /// The peak to calculate.
        /// </param>
        /// <param name="originalList">
        /// The original list given to the peak finder.
        /// </param>
        /// <param name="smoothedIntensityValues">
        /// The smoothed intensity values from the peak finder.
        /// </param>
        /// <param name="precisionUsed">
        /// The precision used when passing data to the peak finder so it can handle numbers with doubles.
        /// </param>
        /// <returns>
        /// The <see cref="PeakInformation"/>.
        /// </returns>
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

            List<KeyValuePair<int, double>> leftSidePoints;
            List<KeyValuePair<int, double>> rightSidePoints;
            var allPoints = ExtractPointInformation(
                peak,
                originalList,
                smoothedIntensityValues,
                precisionUsed,
                out leftSidePoints,
                out rightSidePoints);

            // find the left side half max
            var leftMidpoint = FindMidPoint(
                originalList,
                smoothedIntensityValues,
                leftSidePoints,
                ref currPoint,
                halfmax,
                Tolerance,
                ref currPointIndex,
                Side.LeftSide);

            // find the right side of the half max
            var rightMidPoint = FindMidPoint(
                originalList,
                smoothedIntensityValues,
                rightSidePoints,
                ref currPoint,
                halfmax,
                Tolerance,
                ref currPointIndex,
                Side.RightSide);

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

        /// <summary>
        /// Finds the left or right half max point.
        /// </summary>
        /// <param name="originalList">
        /// The original list.
        /// </param>
        /// <param name="smoothedIntensityValues">
        /// The smoothed intensity values.
        /// </param>
        /// <param name="sidePoints">
        /// The points making up the side you want to search for the mid point.
        /// </param>
        /// <param name="currPoint">
        /// The current point (designed to chain together.
        /// </param>
        /// <param name="halfmax">
        /// The half max of the peak.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance for calculation.
        /// </param>
        /// <param name="currPointIndex">
        /// The index of the original list for the <see cref="currPoint"/>.
        /// </param>
        /// <param name="sideToFind">
        /// The side to find.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double FindMidPoint(
            List<KeyValuePair<int, double>> originalList,
            IReadOnlyList<double> smoothedIntensityValues,
            IEnumerable<KeyValuePair<int, double>> sidePoints,
            ref KeyValuePair<int, double> currPoint,
            double halfmax,
            double tolerance,
            ref int currPointIndex,
            Side sideToFind)
        {
            double midpoint = 0;
            foreach (var sidePoint in sidePoints)
            {
                var prevPoint = currPoint;
                currPoint = sidePoint;
                var prevPointIndex = currPointIndex;

                currPointIndex = originalList.BinarySearch(
                    currPoint,
                    Comparer<KeyValuePair<int, double>>.Create((left, right) => left.Key - right.Key));

                switch (sideToFind)
                {
                    case Side.LeftSide:
                        // value is too small to be the half-max point
                        if (smoothedIntensityValues[currPointIndex] < halfmax)
                        {
                            continue;
                        }

                        // we got lucky and have a point at the exact half way point
                        if (Math.Abs(smoothedIntensityValues[currPointIndex] - halfmax) < tolerance)
                        {
                            midpoint = currPoint.Key;
                            continue;
                        }

                        break;
                    case Side.RightSide:
                        if (smoothedIntensityValues[currPointIndex] > halfmax
                            || smoothedIntensityValues[currPointIndex] < 0)
                        {
                            continue;
                        }

                        if (Math.Abs(smoothedIntensityValues[currPointIndex] - halfmax) < tolerance)
                        {
                            midpoint = currPoint.Key;
                            continue;
                        }

                        break;
                }

                // We didn't get luky and the two points we have are above and below where the half max would be, so we need to calculate it.
                // Having the redundant argument names improves readability for the formula (which is broken out for future test cases
                // ReSharper disable RedundantArgumentName
                midpoint = GetX(
                    yValue: halfmax,
                    x1: prevPoint.Key,
                    y1: smoothedIntensityValues[prevPointIndex],
                    x2: currPoint.Key,
                    y2: smoothedIntensityValues[currPointIndex]);
                // ReSharper restore RedundantArgumentName
                break;
            }

            return midpoint;
        }

        /// <summary>
        /// Gets an x value given a y value and two x,y points.
        /// </summary>
        /// <param name="yValue">
        /// The y value for the point we want to calculate an x for.
        /// </param>
        /// <param name="x1">
        /// The x value of the first point in the slope.
        /// </param>
        /// <param name="y1">
        /// The y value of the first point in the slope.
        /// </param>
        /// <param name="x2">
        /// The x value of the second point in the slope.
        /// </param>
        /// <param name="y2">
        /// The y value of the second point in the slope.
        /// </param>
        /// <returns>
        /// The the calculated c value, represented as a <see cref="double"/>.
        /// </returns>
        private static double GetX(double yValue, double x1, double y1, double x2, double y2)
        {
            /*  Point/Slope solved for X:
             *      (              (yValue - y1) )
             * x1 + ( (x2 - x1) * ---------------)
             *      (                (y2 - y1)   )
             */
            return x1 + ((x2 - x1) * ((yValue - y1) / (y2 - y1)));
        }

        /// <summary>
        /// Gets information for points, required for calculations.
        /// </summary>
        /// <param name="peak">
        /// The peak found.
        /// </param>
        /// <param name="originalList">
        /// The original list given to the peak finder.
        /// </param>
        /// <param name="smoothedIntensityValues">
        /// The smoothed intensity values output by the peak finder.
        /// </param>
        /// <param name="precisionUsed">
        /// The precision used.
        /// </param>
        /// <param name="leftSidePoints">
        /// The list to store the left side points.
        /// </param>
        /// <param name="rightSidePoints">
        /// The list to store the right side points.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        private static List<PointInformation> ExtractPointInformation(
            clsPeak peak,
            IReadOnlyList<KeyValuePair<int, double>> originalList,
            IReadOnlyList<double> smoothedIntensityValues,
            int precisionUsed,
            out List<KeyValuePair<int, double>> leftSidePoints,
            out List<KeyValuePair<int, double>> rightSidePoints)
        {
            var allPoints = new List<PointInformation>();

            // build left side
            leftSidePoints = new List<KeyValuePair<int, double>>();
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

            // build right side
            rightSidePoints = new List<KeyValuePair<int, double>>();
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

            return allPoints;
        }

        #endregion
    }
}