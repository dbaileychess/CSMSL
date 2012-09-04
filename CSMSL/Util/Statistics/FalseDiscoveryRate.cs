using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Util.Statistics
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

        public double MaxFDR { get; set; }

        public double ActualFDR { get; private set; }

        public bool RemoveDuplicateItems { get; set; }

        public List<TSource> ForwardHits { get; private set; }

        public List<TSource> DecoyHits { get; private set; }

        public FalseDiscoveryRate(IEnumerable<TSource> items, double maxFDR = 0.01, bool removeDuplicateItems = false)
        {
            OriginalItems = items;
            MaxFDR = maxFDR;
            RemoveDuplicateItems = removeDuplicateItems;
            ActualFDR = double.NaN;
        }

        /// <summary>
        /// Filter the internal list of items and store the results in this object
        /// </summary>
        public void Filter()
        {
            Filter(MaxFDR, RemoveDuplicateItems);
        }

        /// <summary>
        /// Filter the internal list of items at a given Maximum FDR rate and store the results in this object
        /// </summary>
        /// <param name="maxFDR">The maximum FDR to accept</param>
        /// <param name="removeDuplicateItems">Calculate FDR on only unique items</param>
        public void Filter(double maxFDR = 0.01, bool removeDuplicateItems = false)
        {
            MaxFDR = maxFDR;
            RemoveDuplicateItems = removeDuplicateItems;
            ForwardHits = new List<TSource>();
            DecoyHits = new List<TSource>();

            long forward = 0;
            long decoy = 0;

            // Filter the original items at the max FDR rate, not removing decoys (will do later)
            foreach (TSource hit in Filter(OriginalItems, MaxFDR, false, RemoveDuplicateItems))
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
                ActualFDR = (double)decoy / (double)forward;
            }
            else
            {
                ActualFDR = double.NaN;
            }
        }

        #endregion Instance Variables

        #region Static Methods

        /// <summary>
        /// Filters a list using the object in the list's default comparer
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <param name="maxFDR">The max fdr rate [0.0 - 1.0] to keep items</param>
        /// <param name="removeDecoys">Remove the decoy items from the filtered list</param>
        /// <param name="uniqueItems">Remove non-unique items before filtering</param>
        /// <returns>A FDR Filtered list of items that are below the stated maxFDR level, can contain decoys if specified</returns>
        public static IEnumerable<TSource> Filter(IEnumerable<TSource> items, double maxFDR = 0.01, bool removeDecoys = false, bool uniqueItems = false)
        {
            // Calls the other method, passing in the default comparer of the objects in the list
            return Filter(items, Comparer<TSource>.Default, maxFDR, removeDecoys, uniqueItems);
        }

        /// <summary>
        /// Filters a list using the specified comparer
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <param name="maxFDR">>The max fdr rate [0.0 - 1.0] to keep items</param>
        /// <param name="comparer">The comparer used to sort the items before filtering (you want the things you like to be at the top of the list)</param>
        /// <param name="removeDecoys">Remove the decoy items from the filtered list</param>
        /// <param name="uniqueItems">Remove non-unique items before filtering</param>
        /// <returns>A FDR Filtered list of items that are below the stated maxFDR level, can contain decoys if specified</returns>
        public static IEnumerable<TSource> Filter(IEnumerable<TSource> items, Comparer<TSource> comparer, double maxFDR = 0.01, bool removeDecoys = false, bool uniqueItems = false)
        {
            // Keep track of forward and decoy hits
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
                sorteditems = sorteditems.Distinct<TSource>().ToList<TSource>();
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
                double fdr = (double)decoy / (double)forward;

                // Check to see if the local fdr rate is larger than the max
                if (fdr > maxFDR)
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
                currentCutoff = item.FDRScoreMetric;
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
            else
            {
                // Loop over each item
                foreach (TSource item in originalSortedItems)
                {
                    // If the item's fdr metric is lower or equal to the cutoff value
                    if (item.FDRScoreMetric.CompareTo(storedCutoff) <= 0)
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

            yield break;
        }

        #endregion Static Methods
    }
}