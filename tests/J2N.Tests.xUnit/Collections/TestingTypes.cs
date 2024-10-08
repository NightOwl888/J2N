﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;

namespace J2N.Collections.Tests
{
    #region Comparers and Equatables

    // Use parity only as a hashcode so as to have many collisions.
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class BadIntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }

        public override bool Equals(object obj)
        {
            return obj is BadIntEqualityComparer; // Equal to all other instances of this type, not to anything else.
        }

        public override int GetHashCode()
        {
            return unchecked((int)0xC001CAFE); // Doesn't matter as long as its constant.
        }
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class EquatableBackwardsOrder : IEquatable<EquatableBackwardsOrder>, IComparable<EquatableBackwardsOrder>, IComparable
    {
        private int _value;

        public EquatableBackwardsOrder(int value)
        {
            _value = value;
        }

        public int CompareTo(EquatableBackwardsOrder other) //backwards from the usual integer ordering
        {
            return other._value - _value;
        }

        public override int GetHashCode() => _value;

        public override bool Equals(object obj)
        {
            EquatableBackwardsOrder other = obj as EquatableBackwardsOrder;
            return other != null && Equals(other);
        }

        public bool Equals(EquatableBackwardsOrder other)
        {
            return _value == other._value;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj != null && obj.GetType() == typeof(EquatableBackwardsOrder))
                return ((EquatableBackwardsOrder)obj)._value - _value;
            else return -1;
        }
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Comparer_SameAsDefaultComparer : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x - y;
        }

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Comparer_HashCodeAlwaysReturnsZero : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x - y;
        }

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return 0;
        }
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Comparer_ModOfInt : IEqualityComparer<int>, IComparer<int>
    {
        private int _mod;

        public Comparer_ModOfInt(int mod)
        {
            _mod = mod;
        }

        public Comparer_ModOfInt()
        {
            _mod = 500;
        }

        public int Compare(int x, int y)
        {
            return ((x % _mod) - (y % _mod));
        }

        public bool Equals(int x, int y)
        {
            return ((x % _mod) == (y % _mod));
        }

        public int GetHashCode(int x)
        {
            return (x % _mod);
        }
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Comparer_AbsOfInt : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return Math.Abs(x) - Math.Abs(y);
        }

        public bool Equals(int x, int y)
        {
            return Math.Abs(x) == Math.Abs(y);
        }

        public int GetHashCode(int x)
        {
            return Math.Abs(x);
        }
    }

    #endregion

    #region TestClasses

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public struct SimpleInt : IStructuralComparable, IStructuralEquatable, IComparable, IComparable<SimpleInt>
    {
        private int _val;
        public SimpleInt(int t)
        {
            _val = t;
        }
        public int Val
        {
            get { return _val; }
            set { _val = value; }
        }

        public int CompareTo(SimpleInt other)
        {
            return other.Val - _val;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(SimpleInt))
            {
                return ((SimpleInt)obj).Val - _val;
            }
            return -1;
        }

        public int CompareTo(object other, IComparer comparer)
        {
            if (other.GetType() == typeof(SimpleInt))
                return ((SimpleInt)other).Val - _val;
            return -1;
        }

        public bool Equals(object other, IEqualityComparer comparer)
        {
            if (other.GetType() == typeof(SimpleInt))
                return ((SimpleInt)other).Val == _val;
            return false;
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(_val);
        }
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class WrapStructural_Int : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return StructuralComparisons.StructuralComparer.Compare(x, y);
        }

        public bool Equals(int x, int y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(int obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class WrapStructural_SimpleInt : IEqualityComparer<SimpleInt>, IComparer<SimpleInt>
    {
        public int Compare(SimpleInt x, SimpleInt y)
        {
            return StructuralComparisons.StructuralComparer.Compare(x, y);
        }

        public bool Equals(SimpleInt x, SimpleInt y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(SimpleInt obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "by design")]
    public class GenericComparable : IComparable<GenericComparable>
    {
        private readonly int _value;

        public GenericComparable(int value)
        {
            _value = value;
        }

        public int CompareTo(GenericComparable other) => _value.CompareTo(other._value);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "by design")]
    public class NonGenericComparable : IComparable
    {
        private readonly GenericComparable _inner;

        public NonGenericComparable(int value)
        {
            _inner = new GenericComparable(value);
        }

        public int CompareTo(object other) =>
            _inner.CompareTo(((NonGenericComparable)other)._inner);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "by design")]
    public class BadlyBehavingComparable : IComparable<BadlyBehavingComparable>, IComparable
    {
        public int CompareTo(BadlyBehavingComparable other) => 1;

        public int CompareTo(object other) => -1;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "by design")]
    public class MutatingComparable : IComparable<MutatingComparable>, IComparable
    {
        private int _state;

        public MutatingComparable(int initialState)
        {
            _state = initialState;
        }

        public int State => _state;

        public int CompareTo(object other) => _state++;

        public int CompareTo(MutatingComparable other) => _state++;
    }

    public static class ValueComparable
    {
        // Convenience method so the compiler can work its type inference magic.
        public static ValueComparable<T> Create<T>(T value) where T : IComparable<T>
        {
            return new ValueComparable<T>(value);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "by design")]
    public struct ValueComparable<T> : IComparable<ValueComparable<T>> where T : IComparable<T>
    {
        public ValueComparable(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public int CompareTo(ValueComparable<T> other) =>
            Value.CompareTo(other.Value);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "by design")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA1067 doesn't fire on all target frameworks")]
    public class Equatable : IEquatable<Equatable>
    {
        public Equatable(int value)
        {
            Value = value;
        }

        public int Value { get; }

        // Equals(object) is not implemented on purpose.
        // EqualityComparer is only supposed to call through to the strongly-typed Equals since we implement IEquatable.

        public bool Equals(Equatable other)
        {
            return other != null && Value == other.Value;
        }

        public override int GetHashCode() => Value;
    }

    public struct NonEquatableValueType
    {
        public NonEquatableValueType(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "by design")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA1067 doesn't fire on all target frameworks")]
    public class DelegateEquatable : IEquatable<DelegateEquatable>
    {
        public DelegateEquatable()
        {
            EqualsWorker = _ => false;
        }

        public Func<DelegateEquatable, bool> EqualsWorker { get; set; }

        public bool Equals(DelegateEquatable other) => EqualsWorker(other);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "by design")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA1067 doesn't fire on all target frameworks")]
    public struct ValueDelegateEquatable : IEquatable<ValueDelegateEquatable>
    {
        public Func<ValueDelegateEquatable, bool> EqualsWorker { get; set; }

        public bool Equals(ValueDelegateEquatable other) => EqualsWorker(other);
    }

    public sealed class TrackingEqualityComparer<T> : IEqualityComparer<T>
    {
        public int EqualsCalls;
        public int GetHashCodeCalls;

        public bool Equals(T x, T y)
        {
            EqualsCalls++;
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            GetHashCodeCalls++;
            return EqualityComparer<T>.Default.GetHashCode(obj);
        }
    }

    public sealed class EqualityComparerConstantHashCode<T> : IEqualityComparer<T>
    {
        private readonly IEqualityComparer<T> _comparer;

        public EqualityComparerConstantHashCode(IEqualityComparer<T> comparer) => _comparer = comparer;

        public bool Equals(T x, T y) => _comparer.Equals(x, y);

        public int GetHashCode(T obj) => 42;
    }

    #endregion
}
