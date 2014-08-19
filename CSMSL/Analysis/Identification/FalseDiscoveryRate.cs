// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (FalseDiscoveryRate.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Analysis.Identification
{
    /// <summary>
    /// An extension method to perform False Discovery Rate Filtering (FDR) on any type of lists.
    /// Derek Bailey
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the list (must implement IFalseDiscovery)</typeparam>
    /// <typeparam name="TMetric">The type of scoring metric of the objects in the list (must implement IComparable)</typeparam>
    public class FalseDiscoveryRate<TSource, TMetric>
        where TSource : IFalseDiscovery<TMetric>
        where TMetric : IComparable<TMetric>
    {
        #region Instance Variables

        public IEnumerable<TSource> OriginalItems { get; set; }

        public double MaxFdr { get; set; }

        public double ActualFdr { get; private set; }

        public bool RemoveDuplicateItems { get; set; }

        public List<TSource> ForwardHits { get; private set; }

        public List<TSource> DecoyHits { get; private set; }

        public FalseDiscoveryRate(IEnumerable<TSource> items, double maxFdr = 0.01, bool removeDuplicateItems = false)
        {
            OriginalItems = items;
            MaxFdr = maxFdr;
            RemoveDuplicateItems = removeDuplicateItems;
            ActualFdr = double.NaN;
        }

        /// <summary>
        /// Filter the internal list of items and store the results in this object
        /// </summary>
        public void Filter(bool removeDuplicateItems = false)
        {
            Filter(MaxFdr, RemoveDuplicateItems);
        }

        /// <summary>
        /// Filter the internal list of items at a given Maximum FDR rate and store the results in this object
        /// </summary>
        /// <param name="maxFdr">The maximum FDR to accept</param>
        /// <param name="removeDuplicateItems">Calculate FDR on only unique items</param>
        public void Filter(double maxFdr = 0.01, bool removeDuplicateItems = false)
        {
            MaxFdr = maxFdr;
            RemoveDuplicateItems = removeDuplicateItems;
            ForwardHits = new List<TSource>();
            DecoyHits = new List<TSource>();

            long forward = 0;
            long decoy = 0;

            // Filter the original items at the max FDR rate, not removing decoys (will do later)
            foreach (TSource hit in Filter(OriginalItems, MaxFdr, false, RemoveDuplicateItems))
            {
                if (hit.IsDecoy)
                {
                    DecoyHits.Add(hit);
                    decoy++;
                }
                else
                {
                    ForwardHits.Add(hit);
                    forward++;
                }
            }

            if (forward > 0)
            {
                // The actual FDR is just the decoys count over the forward hits
                ActualFdr = decoy/(double) forward;
            }
            else
            {
                ActualFdr = double.NaN;
            }
        }

        #endregion Instance Variables

        #region Static Methods

        /// <summary>
        /// Filters a list using the object in the list's default comparer
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <param name="maxFdr">The max fdr rate [0.0 - 1.0] to keep items</param>
        /// <param name="removeDecoys">Remove the decoy items from the filtered list</param>
        /// <param name="uniqueItems">Remove non-unique items before filtering</param>
        /// <returns>A FDR Filtered list of items that are below the stated maxFDR level, can contain decoys if specified</returns>
        public static IEnumerable<TSource> Filter(IEnumerable<TSource> items, double maxFdr = 0.01, bool removeDecoys = false, bool uniqueItems = false)
        {
            // Calls the other method, passing in the default comparer of the objects in the list
            return Filter(items, Comparer<TSource>.Default, maxFdr, removeDecoys, uniqueItems);
        }

        /// <summary>
        /// Filters a list using the specified comparer
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <param name="maxFdr">>The max fdr rate [0.0 - 1.0] to keep items</param>
        /// <param name="comparer">The comparer used to sort the items before filtering (you want the things you like to be at the top of the list)</param>
        /// <param name="removeDecoys">Remove the decoy items from the filtered list</param>
        /// <param name="uniqueItems">Remove non-unique items before filtering</param>
        /// <returns>A FDR Filtered list of items that are below the stated maxFDR level, can contain decoys if specified</returns>
        public static IEnumerable<TSource> Filter(IEnumerable<TSource> items, Comparer<TSource> comparer, double maxFdr = 0.01, bool removeDecoys = false, bool uniqueItems = false)
        {
            //// Keep track of forward and decoy hits
            long forward = 0;
            long decoy = 0;

            // Set the cutoff metrics to the default
            TMetric storedCutoff = default(TMetric);
            TMetric currentCutoff = default(TMetric);

            // Set if the max FDR has been hit before to false (we are currently at a 0% FDR)
            bool hitOnce = false;

            // Sort the input data based on the comparer given
            List<TSource> sorteditems = items.ToList();
            sorteditems.Sort(comparer);
            List<TSource> originalSortedItems = new List<TSource>(sorteditems);

            // Filter away so only unique values (the best unique value) remains in the sorted list
            if (uniqueItems)
            {
                sorteditems = sorteditems.Distinct().ToList();
            }

            // Problem could arise if the top hit is a decoy, so check if it is
            if (sorteditems.Count > 0 && sorteditems[0].IsDecoy)
            {
                yield break;
            }

            // Loop over each item, from best scoring to worst (due to the above sort)
            foreach (TSource item in sorteditems)
            {
                // Check to see if the item is a decoy or not
                if (item.IsDecoy) decoy++;
                else forward++;

                // Calculate the local fdr rate
                double fdr = decoy/(double) forward;

                // Check to see if the local fdr rate is larger than the max
                if (fdr > maxFdr)
                {
                    // If this is the first time reaching the fdr max
                    if (!hitOnce)
                    {
                        // Save the cutoff value and mark that the fdr max has been hit once
                        storedCutoff = currentCutoff;
                        hitOnce = true;
                    }
                    else
                    {
                        // This is the second time hitting the MaxFDR, so we must exit
                        // First check if to see if the stored cutoff is bigger than the current cutoff,
                        // if so, set the stored cutoff to the current cutoff value
                        if (storedCutoff.CompareTo(currentCutoff) > 0)
                            storedCutoff = currentCutoff;
                        break;
                    }
                }

                // Set the cutoff value for the last item analyzed (which is the score metric)
                currentCutoff = item.FdrScoreMetric;
            }

            // If the max fdr was never hit once, every item passes FDR filtering then return all items
            if (!hitOnce)
            {
                // Loop over each item
                foreach (TSource item in originalSortedItems)
                {
                    // Skip decoys if told to
                    if (removeDecoys && item.IsDecoy)
                    {
                        continue;
                    }

                    // Return item
                    yield return item;
                }
                yield break;
            }

            // Loop over each item
            foreach (TSource item in originalSortedItems)
            {
                // If the item's fdr metric is lower or equal to the cutoff value
                if (item.FdrScoreMetric.CompareTo(storedCutoff) <= 0)
                {
                    // Skip decoys if told to
                    if (removeDecoys && item.IsDecoy)
                    {
                        continue;
                    }

                    // return that item, it passes the fdr filter
                    yield return item;
                }
                else
                {
                    // An item was above the cutoff value, so there are no more items to look at, exit immediately
                    yield break;
                }
            }
        }

        public static int Count(IList<TSource> items, double maxFdr = 0.01, bool removeDecoys = false, bool uniqueItems = false)
        {
            return Count(items, Comparer<TSource>.Default, Comparer<TMetric>.Default, maxFdr, removeDecoys, uniqueItems);
        }

        public static int Count(IList<TSource> items, Comparer<TSource> comparer, Comparer<TMetric> metricComparer, double maxFdr = 0.01, bool removeDecoys = false, bool uniqueItems = false, bool preSorted = false)
        {
            TMetric threshold = CalculateThreshold(items, comparer, maxFdr, uniqueItems, preSorted);

            if (removeDecoys)
                return items.Count(item => metricComparer.Compare(item.FdrScoreMetric, threshold) <= 0 && !item.IsDecoy);
            return items.Count(item => metricComparer.Compare(item.FdrScoreMetric, threshold) <= 0);
        }

        public static TMetric CalculateThreshold(IEnumerable<TSource> items, double maxFdr = 0.01,
            bool uniqueItems = false)
        {
            return CalculateThreshold(items, Comparer<TSource>.Default, maxFdr, uniqueItems);
        }

        public static TMetric CalculateThreshold(IEnumerable<TSource> items, Comparer<TSource> comparer,
            double maxFdr = 0.01, bool keepUniqueItemsOnly = false, bool preSorted = false)
        {
            // Set the cutoff metrics to the default
            TMetric storedCutoff = default(TMetric);

            // Sort the input data based on the comparer given
            List<TSource> sorteditems = items.ToList();

            if (!preSorted)
                sorteditems.Sort(comparer);

            int count = sorteditems.Count;

            if (count == 0)
                return storedCutoff;

            // Filter away so only unique values (the best unique value) remains in the sorted list
            if (keepUniqueItemsOnly)
            {
                sorteditems = sorteditems.Distinct().ToList();
            }

            // Keep track of forward and decoy hits
            double forward = 0;
            double decoy = 0;

            double[] fdrs = new double[count];
            double[] qvalues = new double[count];

            // Loop over each item, from best scoring to worst (due to the above sort)
            for (int index = 0; index < count; index++)
            {
                TSource item = sorteditems[index];

                if (item.IsDecoy) decoy++;
                else forward++;

                // Calculate the local fdr rate
                double fdr = decoy/forward;

                fdrs[index] = fdr;
                qvalues[index] = fdr;
            }

            // Start with the worst scoring item and calculating q-values by iterating backwards.
            // Start with the minimum Q at positive infinity,
            double minQ = double.PositiveInfinity;
            for (int index = count - 1; index >= 0; index--)
            {
                double fdr = fdrs[index];
                if (fdr > minQ)
                    qvalues[index] = minQ;
                else
                    minQ = qvalues[index];
            }

            // Filter the list until the q-value is > maximum false discovery rate allowed
            for (int index = 0; index < count; index++)
            {
                if (qvalues[index] <= maxFdr)
                    continue;
                if (index == 0)
                    return sorteditems[0].FdrScoreMetric;
                return sorteditems[index - 1].FdrScoreMetric;
            }

            // If we made it this far, everything passed
            return sorteditems[count - 1].FdrScoreMetric;
        }

        #endregion Static Methods
    }
}