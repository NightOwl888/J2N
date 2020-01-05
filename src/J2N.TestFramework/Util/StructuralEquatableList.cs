using J2N.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace J2N.Util
{
    public class StructuralEquatableList<T> : List<T>, IStructuralEquatable
    {
        public bool Equals(object other, IEqualityComparer comparer)
        {
            return ListEqualityComparer<T>.Equals(this, other, comparer);
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return ListEqualityComparer<T>.GetHashCode(this, comparer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj, ListEqualityComparer<T>.Default);
        }

        public override int GetHashCode()
        {
            return GetHashCode(ListEqualityComparer<T>.Default);
        }
    }
}
