using J2N.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace J2N.Util
{
    public class StructuralEquatableList<T> : List<T>, IStructuralEquatable
    {
        private readonly ListEqualityComparer<T> equalityComparer;

        public StructuralEquatableList(ListEqualityComparer<T> equalityComparer)
        {
            this.equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
        }

        public bool Equals(object other, IEqualityComparer comparer)
        {
            if (other is IList<T> otherList)
                return equalityComparer.Equals(this, otherList);
            return false;
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return equalityComparer.GetHashCode(this);
        }

        //public override bool Equals(object obj)
        //{
        //    return Equals(obj, equalityComparer);
        //}

        //public override int GetHashCode()
        //{
        //    return GetHashCode(equalityComparer);
        //}
    }
}
