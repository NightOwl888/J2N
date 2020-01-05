using J2N.Collections.Generic;
using System.Collections;

namespace J2N.Util
{
    public class StructuralEquatableSet<T> : System.Collections.Generic.HashSet<T>, IStructuralEquatable
    {
        public bool Equals(object other, IEqualityComparer comparer)
        {
            return SetEqualityComparer<T>.Equals(this, other, comparer);
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return SetEqualityComparer<T>.GetHashCode(this, comparer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj, SetEqualityComparer<T>.Default);
        }

        public override int GetHashCode()
        {
            return GetHashCode(SetEqualityComparer<T>.Default);
        }
    }

    //public class StructuralEquatableSet<T> : HashSet<T>, IStructuralEquatable
    //{
    //    private readonly SetEqualityComparer<T> equalityComparer;

    //    public StructuralEquatableSet(SetEqualityComparer<T> equalityComparer)
    //    {
    //        this.equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
    //    }

    //    public bool Equals(object other, IEqualityComparer comparer)
    //    {
    //        if (other is ISet<T> otherSet)
    //            return equalityComparer.Equals(this, otherSet);
    //        return false;
    //    }

    //    public int GetHashCode(IEqualityComparer comparer)
    //    {
    //        return equalityComparer.GetHashCode(this);
    //    }

    //    //public override bool Equals(object obj)
    //    //{
    //    //    return Equals(obj, equalityComparer);
    //    //}

    //    //public override int GetHashCode()
    //    //{
    //    //    return GetHashCode(equalityComparer);
    //    //}
    //}
}
