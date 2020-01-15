// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace J2N.Collections.ObjectModel.Tests
{
    public class CollectionTestBase
    {
        protected static readonly int[] s_intArray = new[] { -4, 5, -2, 3, 1, 2, -1, -3, 0, 4, -5, 3, 3 };
        protected static readonly int[] s_excludedFromIntArray = new int[] { 100, -34, 42, int.MaxValue, int.MinValue };

        [Flags]
        protected enum IListApi
        {
            None = 0,
            IndexerGet = 0x1,
            IndexerSet = 0x2,
            Count = 0x4,
            IsReadOnly = 0x8,
            Clear = 0x10,
            Contains = 0x20,
            CopyTo = 0x40,
            GetEnumeratorGeneric = 0x80,
            IndexOf = 0x100,
            Insert = 0x200,
            RemoveAt = 0x400,
            GetEnumerator = 0x800,
            End
        }

        protected class CallTrackingIList<T> : IList<T>, ICollection<T>
        {
            private IListApi _expectedApiCalls;
            private IListApi _calledMembers;

            public CallTrackingIList(IListApi expectedApiCalls)
            {
                _expectedApiCalls = expectedApiCalls;
            }

            public void AssertAllMembersCalled()
            {
                if (_expectedApiCalls != _calledMembers)
                {
                    for (IListApi i = (IListApi)1; i < IListApi.End; i = (IListApi)((int)i << 1))
                    {
                        Assert.Equal(_expectedApiCalls & i, _calledMembers & i);
                    }
                }
            }

            public T this[int index]
            {
                get
                {
                    _calledMembers |= IListApi.IndexerGet;
                    return default(T);
                }
                set
                {
                    _calledMembers |= IListApi.IndexerSet;
                }
            }

            public int Count
            {
                get
                {
                    _calledMembers |= IListApi.Count;
                    return 1;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    _calledMembers |= IListApi.IsReadOnly;
                    return false;
                }
            }

            public void Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                _calledMembers |= IListApi.Clear;
            }

            public bool Contains(T item)
            {
                _calledMembers |= IListApi.Contains;
                return false;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _calledMembers |= IListApi.CopyTo;
            }

            public IEnumerator<T> GetEnumerator()
            {
                _calledMembers |= IListApi.GetEnumeratorGeneric;
                return null;
            }

            public int IndexOf(T item)
            {
                _calledMembers |= IListApi.IndexOf;
                return -1;
            }

            public void Insert(int index, T item)
            {
                _calledMembers |= IListApi.Insert;
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                _calledMembers |= IListApi.RemoveAt;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                _calledMembers |= IListApi.GetEnumerator;
                return null;
            }
        }




        [Flags]
        protected enum ISetApi
        {
            None = 0,
            Count = 0x1,
            IsReadOnly = 0x2,
            Clear = 0x4, //0x10,
            Contains = 0x8, //0x20,
            CopyTo = 0x10, //0x40,
            GetEnumeratorGeneric = 0x20, //0x80,
            GetEnumerator = 0x40, //0x800,
            ExceptWith = 0x100,
            IntersectWith = 0x200,
            IsProperSubsetOf = 0x400,
            IsProperSupersetOf = 0x800,
            IsSubsetOf = 0x1600,
            IsSupersetOf = 0x3200,
            Overlaps = 0x6400,
            SetEquals = 0x12800,
            SymmetricExceptWith = 0x25600,
            UnionWith = 0x51200,
            End
        }

        protected class CallTrackingISet<T> : ISet<T>
        {
            private ISetApi _expectedApiCalls;
            private ISetApi _calledMembers;

            public CallTrackingISet(ISetApi expectedApiCalls)
            {
                _expectedApiCalls = expectedApiCalls;
            }

            public void AssertAllMembersCalled()
            {
                if (_expectedApiCalls != _calledMembers)
                {
                    for (ISetApi i = (ISetApi)1; i < ISetApi.End; i = (ISetApi)((int)i << 1))
                    {
                        Assert.Equal(_expectedApiCalls & i, _calledMembers & i);
                    }
                }
            }

            public int Count
            {
                get
                {
                    _calledMembers |= ISetApi.Count;
                    return 1;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    _calledMembers |= ISetApi.IsReadOnly;
                    return false;
                }
            }

            public bool Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                _calledMembers |= ISetApi.Clear;
            }

            public bool Contains(T item)
            {
                _calledMembers |= ISetApi.Contains;
                return false;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _calledMembers |= ISetApi.CopyTo;
            }

            public void ExceptWith(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.ExceptWith;
            }

            public IEnumerator<T> GetEnumerator()
            {
                _calledMembers |= ISetApi.GetEnumeratorGeneric;
                return null;
            }

            public void IntersectWith(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.IntersectWith;
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.IsProperSubsetOf;
                return false;
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.IsProperSupersetOf;
                return false;
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.IsSubsetOf;
                return false;
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.IsSupersetOf;
                return false;
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.Overlaps;
                return false;
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.SetEquals;
                return false;
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.SymmetricExceptWith;
            }

            public void UnionWith(IEnumerable<T> other)
            {
                _calledMembers |= ISetApi.UnionWith;
            }

            void ICollection<T>.Add(T item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                _calledMembers |= ISetApi.GetEnumerator;
                return null;
            }
        }
    }
}
