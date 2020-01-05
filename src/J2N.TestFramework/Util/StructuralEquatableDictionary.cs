using J2N.Collections.Generic;
using System.Collections;
using System.Collections.Generic;

namespace J2N.Util
{
    public class StructuralEquatableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IStructuralEquatable
    {

        public bool Equals(object other, IEqualityComparer comparer)
        {
            return DictionaryEqualityComparer<TKey, TValue>.Equals(this, other, comparer);
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return DictionaryEqualityComparer<TKey, TValue>.GetHashCode(this, comparer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj, DictionaryEqualityComparer<TKey, TValue>.Default);
        }

        public override int GetHashCode()
        {
            return GetHashCode(DictionaryEqualityComparer<TKey, TValue>.Default);
        }
    }
}
