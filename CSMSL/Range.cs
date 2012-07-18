using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL
{
    /// <summary>
    /// A default range with the type of double
    /// </summary>
    public class Range : Range<double>
    {
        public double Mean
        {
            get
            {
                return (_max + _min) / 2;
            }
        }

        public Range()
        {
            _min = 0;
            _max = 0;            
        }

        public Range(double minimum, double maximum)
        {
            _min = minimum;
            _max = maximum;
        }

    }

    /// <summary>
    /// A continuous, inclusive range of values 
    /// </summary>
    public class Range<T> where T: IComparable<T>
    {
        protected T _min;

        /// <summary>
        /// The minimum value of the range
        /// </summary>
        public T Minimum
        {
            get { return _min; }
            set { T _min = value; }
        }

        protected T _max;

        /// <summary>
        /// The maximum value of the range
        /// </summary>
        public T Maximum
        {
            get { return _max; }
            set { T _max = value; }
        }

        public Range()
        {
            _min = default(T);
            _max = default(T);
        }

        public Range(T minimum, T maximum)
        {
            _min = minimum;
            _max = maximum;
        }

        /// <summary>
        /// Determines if the item is contained within a range of values
        /// </summary>
        /// <param name="item">The item to compare against the range</param>
        /// <returns>True if the item is within the range (inclusive), false otherwise</returns>
        public bool IsWithin(T item)
        {
            return this.CompareTo(item).Equals(0);
        }

        /// <summary>
        /// Determines whether an item is below, above, or contained within a range of values
        /// </summary>
        /// <param name="item">The item to compare against the range</param>
        /// <returns>-1 if item is below the range, 1 if item is above the range, 0 otherwise</returns>
        public int CompareTo(T item)
        {
            if (_min.CompareTo(item) > 0) return -1;
            if (_max.CompareTo(item) < 0) return 1;
            return 0;
        }        
        
    }
}
