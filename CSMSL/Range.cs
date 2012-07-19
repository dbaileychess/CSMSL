using System;

namespace CSMSL
{
    /// <summary>
    /// A default range with the type of double
    /// </summary>
    public class Range : Range<double>, IRange<double>
    {
        protected double _mean;

        protected double _width;

        public Range()
            : this(0, 0) { }

        public Range(double minimum, double maximum)
        {
            _min = minimum;
            _max = maximum;
            _width = _max - _min;
            _mean = (_max + _min) / 2.0;
        }

        public new double Maximum
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                _width = _max - _min;
                _mean = (_max + _min) / 2.0;
            }
        }

        public double Mean
        {
            get
            {
                return _mean;
            }
            set
            {
                _mean = value;
                _min = _mean - (_width / 2.0);
                _max = _mean + (_width / 2.0);
            }
        }

        public new double Minimum
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
                _width = _max - _min;
                _mean = (_max + _min) / 2.0;
            }
        }

        public double Width
        {
            get 
            { 
                return _width; 
            }
            set
            {
                _width = value;
                _min = _mean - (_width / 2.0);
                _max = _mean + (_width / 2.0);
            }
        }
    }

    /// <summary>
    /// A continuous, inclusive range of values
    /// </summary>
    public class Range<T> : IRange<T>
        where T : IComparable<T>, IEquatable<T>
    {
        protected T _max;
        protected T _min;

        public Range()
            : this(default(T), default(T)) { }

        public Range(IRange<T> range)
            : this(range.Minimum, range.Maximum) { }

        public Range(T minimum, T maximum)
        {
            _min = minimum;
            _max = maximum;
        }

        /// <summary>
        /// The maximum value of the range
        /// </summary>
        public T Maximum
        {
            get { return _max; }
            set { T _max = value; }
        }

        /// <summary>
        /// The minimum value of the range
        /// </summary>
        public T Minimum
        {
            get { return _min; }
            set { T _min = value; }
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

        /// <summary>
        /// Checks to see if this range is a proper super range of another range (inclusive)
        /// </summary>
        /// <param name="other">Thje other range to compare to</param>
        /// <returns>True if this range is fully encloses the other range, false otherwise</returns>
        public bool IsSuperRange(IRange<T> other)
        {
            return (_max.CompareTo(other.Maximum) >= 0 && _min.CompareTo(other.Minimum) <= 0);
        }

        /// <summary>
        /// Checks to see if this range is a proper sub range of another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if this range is fully enclosed by the other range, false otherwise</returns>
        public bool IsSubRange(IRange<T> other)
        {
            return (_max.CompareTo(other.Maximum) <= 0 && _min.CompareTo(other.Minimum) >= 0);
        }

        /// <summary>
        /// Checks to see if this range overlaps another range (inclusive)
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if the other range in any way overlaps this range, false otherwise</returns>
        public bool IsOverlaping(IRange<T> other)
        {
            return Contains(other.Minimum) || Contains(other.Maximum);
        }

        /// <summary>
        /// Determines if the item is contained within a range of values
        /// </summary>
        /// <param name="item">The item to compare against the range</param>
        /// <returns>True if the item is within the range (inclusive), false otherwise</returns>
        public bool Contains(T item)
        {
            return CompareTo(item).Equals(0);
        }

        /// <summary>
        /// Returns a formated string of the range [min - max]
        /// </summary>
        /// <returns>Format: [min - max]</returns>
        public override string ToString()
        {
            return string.Format("[{0} - {1}]", _min, _max);
        }

        /// <summary>
        /// Compares if two ranges are identical
        /// </summary>
        /// <param name="other">The other range to compare to</param>
        /// <returns>True if both the minimum and maximum values are equivalent, false otherwise</returns>
        public bool Equals(IRange<T> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if ((this == null) != (other == null)) return false;
            return _max.Equals(other.Maximum) && _min.Equals(other.Minimum);
        }
    }
}