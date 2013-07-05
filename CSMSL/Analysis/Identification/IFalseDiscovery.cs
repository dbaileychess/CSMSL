using System;

namespace CSMSL.Analysis.Identification
{
    /// <summary>
    /// An interface to filter lists based on the detection of decoy hits that are known to be wrong
    /// </summary>
    /// <typeparam name="T">The type of scoring metric of the object (must implement IComparable)</typeparam>
    public interface IFalseDiscovery<T> where T : IComparable<T>
    {
        /// <summary>
        /// States whether the object is a Decoy hit (Known False Positive) or a Foward Hit (Unknown True Positive)
        /// </summary>
        bool IsDecoy { get; }

        /// <summary>
        /// The scoring metric for the object
        /// </summary>
        T FdrScoreMetric { get; }
    }
}
