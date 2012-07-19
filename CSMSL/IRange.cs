using System;

namespace CSMSL
{
    public interface IRange<T> : IEquatable<IRange<T>> where T : IComparable<T>
    {
        T Minimum { get; set; }

        T Maximum { get; set; }

        bool Contains(T item);

        int CompareTo(T item);

        bool IsSubRange(IRange<T> other);

        bool IsSuperRange(IRange<T> other);

        bool IsOverlaping(IRange<T> other);
    }
}