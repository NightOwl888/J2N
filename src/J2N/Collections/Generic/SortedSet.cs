// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.ObjectModel;
using J2N.Numerics;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if FEATURE_SERIALIZABLE
using System.Runtime.Serialization;
#endif

using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
    // A binary search tree is a red-black tree if it satisfies the following red-black properties:
    // 1. Every node is either red or black
    // 2. Every leaf (nil node) is black
    // 3. If a node is red, the both its children are black
    // 4. Every simple path from a node to a descendant leaf contains the same number of black nodes
    //
    // The basic idea of a red-black tree is to represent 2-3-4 trees as standard BSTs but to add one extra bit of information
    // per node to encode 3-nodes and 4-nodes.
    // 4-nodes will be represented as:   B
    //                                 R   R
    //
    // 3 -node will be represented as:   B     or     B
    //                                 R   B        B   R
    //
    // For a detailed description of the algorithm, take a look at "Algorithms" by Robert Sedgewick.

    internal enum NodeColor : byte
    {
        Black,
        Red
    }

    internal delegate bool TreeWalkPredicate<T>(SortedSet<T>.Node node);

    internal enum TreeRotation : byte
    {
        Left,
        LeftRight,
        Right,
        RightLeft
    }

    /// <summary>
    /// Represents a collection of objects that is maintained in sorted order.
    /// <para/>
    /// <see cref="SortedSet{T}"/> adds the following features to <see cref="System.Collections.Generic.SortedSet{T}"/>:
    /// <list type="bullet">
    ///     <item><description>
    ///         Overrides the <see cref="Equals(object)"/> and <see cref="GetHashCode()"/> methods to compare lists
    ///         using structural equality by default. Also, <see cref="IStructuralEquatable"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Overrides the <see cref="ToString()"/> methods to list the contents of the list
    ///         by default. Also, <see cref="IFormatProvider"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Uses <see cref="Comparer{T}.Default"/> by default, which provides some specialized equality comparisons
    ///         for specific types to match the behavior of Java.
    ///     </description></item>
    /// </list>
    /// <para/>
    /// Usage Note: This class is intended to be a direct replacement for <see cref="System.Collections.Generic.SortedSet{T}"/> in order
    /// to provide default structural equality and formatting behavior similar to Java. Note that the <see cref="ToString()"/>
    /// method uses the current culture by default to behave like other components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <remarks>
    /// <b>Java to .NET Method Mapping</b>
    /// <para/>
    /// The following table shows how common Java <see cref="ISet{T}"/> operations map to .NET <see cref="ISet{T}"/> operations:
    /// <list type="table">
    ///   <listheader>
    ///     <term>Java Operation</term>
    ///     <description>.NET Operation</description>
    ///   </listheader>
    ///   <item>
    ///     <term><c>set1.containsAll(set2)</c></term>
    ///     <description><see cref="IsSupersetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>set1.containsAll(set2) &amp;&amp; !set1.equals(set2)</c></term>
    ///     <description><see cref="IsProperSupersetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>set2.containsAll(set1)</c></term>
    ///     <description><see cref="IsSubsetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>set2.containsAll(set1) &amp;&amp; !set2.equals(set1)</c></term>
    ///     <description><see cref="IsProperSubsetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>Collections.disjoint(set1, set2)</c></term>
    ///     <description><c>!<see cref="Overlaps(IEnumerable{T})"/></c></description>
    ///   </item>
    ///   <item>
    ///     <term><c>!Collections.disjoint(set1, set2)</c></term>
    ///     <description><see cref="Overlaps(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>EnumSet.complementOf(enumSet)</c></term>
    ///     <description><see cref="SymmetricExceptWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>removeAll(other)</c></term>
    ///     <description><see cref="ExceptWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>retainAll(other)</c></term>
    ///     <description><see cref="IntersectWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>addAll(other)</c></term>
    ///     <description><see cref="UnionWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>equals(other)</c></term>
    ///     <description><see cref="SetEquals(IEnumerable{T})"/> or <see cref="Equals(object)"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "by design name choice")]
    [SuppressMessage("Style", "IDE0018:Inline variable declaration", Justification = "Following Microsoft's code style")]
    [SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "Following Microsoft's code style")]
    [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Following Microsoft's code style")]
    [SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "Following Microsoft's code styles")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public partial class SortedSet<T> : ISet<T>, ICollection<T>, ICollection, INavigableCollection<T>,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyCollection<T>,
#endif
#if FEATURE_READONLYSET
        IReadOnlySet<T>,
#endif
        IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
        , ISerializable, IDeserializationCallback
#endif
    {
        #region Local variables/constants

        private Node? root;
        private IComparer<T> comparer = default!;
        private int count;
        private int version;

        // J2N: Added caching fields for Min/Max

        private T? cachedMin;
        private T? cachedMax;

        private int minVersion = -1;
        private int maxVersion = -1;

#if FEATURE_SERIALIZABLE
        private const string ComparerName = "Comparer"; // Do not rename (binary serialization)
        private const string CountName = "Count"; // Do not rename (binary serialization)
        private const string ItemsName = "Items"; // Do not rename (binary serialization)
        private const string VersionName = "Version"; // Do not rename (binary serialization)

        // J2N NOTE: The constants in StringComparerMetadataSerializer are also used to round-trip any StringComparer metadata

        //needed for enumerator
        private const string TreeName = "Tree";
        private const string NodeValueName = "Item";
        private const string EnumStartName = "EnumStarted";
        private const string ReverseName = "Reverse";
        private const string EnumVersionName = "EnumVersion";

        private SerializationInfo? siInfo; //A temporary variable which we need during deserialization.
#endif

        internal const int StackAllocThreshold = 100;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}"/> class.
        /// </summary>
        public SortedSet()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}"/> class that uses a specified comparer.
        /// </summary>
        /// <param name="comparer">The default comparer to use for comparing objects.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <c>null</c>.</exception>
        /// <remarks>
        /// If <paramref name="comparer"/> does not implement <see cref="IComparable{T}"/>, you must specify
        /// an <see cref="IComparer{T}"/> object to be used.
        /// </remarks>
        public SortedSet(IComparer<T>? comparer)
        {
            this.comparer = comparer ?? Comparer<T>.Default;

            // J2N: Special-case Comparer<string>.Default and StringComparer (all options).
            // We wrap these comparers to ensure that alternate string comparison is available.
            if (typeof(T) == typeof(string) &&
                WrappedStringComparer.GetStringComparer(this.comparer) is IComparer<string> stringComparer)
            {
                this.comparer = (IComparer<T>)stringComparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}"/> class that contains elements copied
        /// from a specified enumerable collection.
        /// </summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <remarks>
        /// Duplicate elements in the enumerable collection are not copied into the new instance of the
        /// <see cref="SortedSet{T}"/> class, and no exceptions are thrown.
        /// <para/>
        /// This constructor is an <c>O(n log n)</c> operation, where <c>n</c> is the number of elements
        /// in the <paramref name="collection"/> parameter.
        /// </remarks>
        public SortedSet(IEnumerable<T> collection) : this(collection, Comparer<T>.Default) { /* Intentionally empty */ }


        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}"/> class that contains elements copied
        /// from a specified enumerable collection and that uses a specified comparer.
        /// </summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <param name="comparer">The default comparer to use for comparing objects.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        public SortedSet(IEnumerable<T> collection, IComparer<T>? comparer)
            : this(comparer)
        {
            if (collection is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

            // These are explicit type checks in the mold of HashSet. It would have worked better with
            // something like an ISorted<T> interface. (We could make this work for SortedList.Keys, etc.)
            SortedSet<T>? sortedSet = collection as SortedSet<T>;
            if (sortedSet != null && !(sortedSet is TreeSubSet) && ComparerEquals(Comparer, sortedSet.Comparer))
            {
                if (sortedSet.Count > 0)
                {
                    Debug.Assert(sortedSet.root != null);
                    this.count = sortedSet.count;
                    root = sortedSet.root!.DeepClone(this.count);
                }
                return;
            }

            if (TryGetSortedItems(collection, out T[]? sortedItems, out int sortedCount))
            {
                // We have a sorted array of items with no duplicates.
                EnsureTreeOrder(sortedItems, sortedCount); // J2N: ensure the order matches the underlying tree if we have a reverse view
                root = ConstructRootFromSortedArray(sortedItems!, 0, sortedCount - 1, null);
                this.count = sortedCount;
                return;
            }

            int count;
            T[] elements = EnumerableHelpers.ToArray(collection, out count);
            if (count > 0)
            {
                // If `comparer` is null, sets it to Comparer<T>.Default. We checked for this condition in the IComparer<T> constructor.
                // Array.Sort handles null comparers, but we need this later when we use `comparer.Compare` directly.
                comparer = this.comparer;
                Array.Sort(elements, 0, count, comparer);

                // Overwrite duplicates while shifting the distinct elements towards
                // the front of the array.
                int index = 1;
                for (int i = 1; i < count; i++)
                {
                    if (comparer.Compare(elements[i], elements[i - 1]) != 0)
                    {
                        elements[index++] = elements[i];
                    }
                }

                count = index;
                root = ConstructRootFromSortedArray(elements, 0, count - 1, null);
                this.count = count;
            }
        }

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}"/> class that contains serialized data.
        /// </summary>
        /// <param name="info">The object that contains the information that is required to serialize
        /// the <see cref="SortedSet{T}"/> object.</param>
        /// <param name="context">The structure that contains the source and destination of the serialized
        /// stream associated with the <see cref="SortedSet{T}"/> object.</param>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute an object that is transmitted over a stream.
        /// </remarks>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected SortedSet(SerializationInfo info, StreamingContext context)
        {
            siInfo = info;
        }
#endif

        #endregion

        #region Bulk operation helpers

        private void AddAllElements(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                if (!Contains(item))
                {
                    Add(item);
                }
            }
        }

        private void RemoveAllElements(IEnumerable<T> collection)
        {
            T? first = MinInternal;
            T? last = MaxInternal;
            IComparer<T> comparer = Comparer; // J2N: Use the outer comparer to ensure we do the correct range checks for reverse views, etc.
            foreach (T item in collection)
            {
                if (!(comparer.Compare(item!, first!) < 0 || comparer.Compare(item!, last!) > 0) && Contains(item))
                {
                    Remove(item);
                }
            }
        }

        private bool ContainsAllElements(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Does an in-order tree walk and calls the delegate for each node.
        /// </summary>
        /// <param name="action">
        /// The delegate to invoke on each node.
        /// If the delegate returns <c>false</c>, the walk is stopped.
        /// </param>
        /// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
        internal virtual bool InOrderTreeWalk(TreeWalkPredicate<T> action)
        {
            if (root == null)
            {
                return true;
            }

            // The maximum height of a red-black tree is 2 * log2(n+1).
            // See page 264 of "Introduction to algorithms" by Thomas H. Cormen
            // Note: It's not strictly necessary to provide the stack capacity, but we don't
            // want the stack to unnecessarily allocate arrays as it grows.

            var stack = new Stack<Node>(2 * (int)Log2(Count + 1));
            Node? current = root;

            while (current != null)
            {
                stack.Push(current);
                current = current.Left;
            }

            while (stack.Count != 0)
            {
                current = stack.Pop();
                if (!action(current))
                {
                    return false;
                }

                Node? node = current.Right;
                while (node != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }
            }

            return true;
        }

        /// <summary>
        /// Does a left-to-right breadth-first tree walk and calls the delegate for each node.
        /// </summary>
        /// <param name="action">
        /// The delegate to invoke on each node.
        /// If the delegate returns <c>false</c>, the walk is stopped.
        /// </param>
        /// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
        internal virtual bool BreadthFirstTreeWalk(TreeWalkPredicate<T> action)
        {
            if (root == null)
            {
                return true;
            }

            var processQueue = new Queue<Node>();
            processQueue.Enqueue(root);

            Node current;
            while (processQueue.Count != 0)
            {
                current = processQueue.Dequeue();
                if (!action(current))
                {
                    return false;
                }

                if (current.Left != null)
                {
                    processQueue.Enqueue(current.Left);
                }
                if (current.Right != null)
                {
                    processQueue.Enqueue(current.Right);
                }
            }

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements in the <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <remarks>Retrieving the value of this property is an <c>O(1)</c> operation.</remarks>
        public int Count
        {
            get
            {
                VersionCheck(updateCount: true);
                return count;
            }
        }

        /// <summary>
        /// Gets the <see cref="IComparer{T}"/> object that is used to order the values in the <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <remarks>
        /// The returned comparer can be either J2N's default comparer of the type for a <see cref="SortedSet{T}"/>,
        /// or the comparer used for its constructor.
        /// <para/>
        /// Retrieving the value of this property is an <c>O(1)</c> operation.
        /// </remarks>
        public IComparer<T> Comparer => ComparerInternal;

        internal virtual IComparer<T> ComparerInternal
        {
            get
            {
                Debug.Assert(comparer is not null, "The comparer should never be null.");
                // J2N: We must unwrap the comparer before returning it to the user.
                if (typeof(T) == typeof(string))
                {
                    return (IComparer<T>)InternalStringComparer.GetUnderlyingComparer((IComparer<string?>)comparer!); // [!]: asserted above
                }
                else
                {
                    return comparer!; // [!]: asserted above
                }
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        #endregion

        #region Properties for Alternate Lookup

        // J2N: Indicates whether the current view is reversed
        internal virtual bool IsReversed => false;

        // This calls the correct layer to get the *outermost* comparer (a user comparer or a comparer wrapper around a BCL StringComparer)
        internal IComparer<T> RawComparer => comparer;

        // J2N: This is state from TreeSubSet exposed to allow range checks in Alternate Lookup
        internal virtual SortedSet<T> UnderlyingSet => this;

        internal virtual bool HasLowerBound => false;
        internal virtual bool HasUpperBound => false;

        internal virtual bool LowerBoundInclusive => true; // J2N: Default to true to match the BCL
        internal virtual bool UpperBoundInclusive => true; // J2N: Default to true to match the BCL

        internal virtual T? LowerBound => default;
        internal virtual T? UpperBound => default;

        #endregion

        #region Subclass helpers

        /// <summary>
        /// Indicates the physical lowest value regardless of whether the view is reversed.
        /// This aligns with the structure of the underlying tree.
        /// </summary>
        internal virtual T? LowerValue
        {
            get
            {
                // J2N: Added caching to the value so we don't have to traverse the tree again unless the set is mutated.
                if (minVersion == version)
                    return cachedMin;

                if (root == null)
                {
                    cachedMin = default;
                }
                else
                {
                    Node current = root;
                    while (current.Left != null)
                    {
                        current = current.Left;
                    }

                    cachedMin = current.Item;
                }

                minVersion = version;
                return cachedMin;
            }
        }

        /// <summary>
        /// Indicates the physical highest value regardless of whether the view is reversed.
        /// This aligns with the structure of the underlying tree.
        /// </summary>
        internal virtual T? UpperValue
        {
            get
            {
                // J2N: Added caching to the value so we don't have to traverse the tree again unless the set is mutated.
                if (maxVersion == version)
                    return cachedMax;

                if (root == null)
                {
                    cachedMax = default;
                }
                else
                {
                    Node current = root;
                    while (current.Right != null)
                    {
                        current = current.Right;
                    }

                    cachedMax = current.Item;
                }

                maxVersion = version;
                return cachedMax;
            }
        }

        // Virtual function for TreeSubSet, which may need to reverse an array order to match the underlying tree forward order.
        internal virtual void EnsureTreeOrder(T[] array, int length) { /* Intentionally empty */ }

        // Virtual function for TreeSubSet, which may need to update its count.
        internal virtual void VersionCheck(bool updateCount = false) { }
        // Virtual function for TreeSubSet, which may need the count variable of the parent set.
        internal virtual int TotalCount() { return Count; }

        // Virtual function for TreeSubSet, which may need to do range checks.
        internal virtual bool IsWithinRange(T item) => true;

        internal virtual bool IsTooLow(T item) => false;

        internal virtual bool IsTooHigh(T item) => false;

        #endregion

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlySet{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="SortedSet{T}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="SortedSet{T}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlySet{T}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="SortedSet{T}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlySet<T> AsReadOnly()
            => new ReadOnlySet<T>(this);

        #endregion AsReadOnly

        #region Java TreeSet-like Members

        /// <summary>
        /// Gets the entry in the <see cref="SortedSet{T}"/> whose value
        /// is the predecessor of the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the predecessor of.</param>
        /// <param name="result">The predecessor, if any.</param>
        /// <returns><c>true</c> if a predecessor to <paramref name="item"/> exists; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>strict predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>lower()</c> method in the JDK.
        /// </remarks>
        public bool TryGetPredecessor([AllowNull] T item, [MaybeNullWhen(false)] out T result) => DoTryGetPredecessor(item, out result);

        internal virtual bool DoTryGetPredecessor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
        {
            Node? current = root, match = null;

            while (current != null)
            {
                int comp = comparer.Compare(item!, current.Item);

                if (comp > 0)
                {
                    match = current;
                    current = current.Right;
                }
                else if (comp == 0)
                {
                    current = current.Left;
                    while (current != null)
                    {
                        match = current;
                        current = current.Right;
                    }
                }
                else
                    current = current.Left;
            }

            if (match == null)
            {
                result = default!;
                return false;
            }
            else
            {
                result = match.Item;
                return true;
            }
        }

        /// <summary>
        /// Gets the entry in the <see cref="SortedSet{T}"/> whose value
        /// is the successor of the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the successor of.</param>
        /// <param name="result">The successor, if any.</param>
        /// <returns><c>true</c> if a successor to <paramref name="item"/> exists; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>strict successor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>higher()</c> method in the JDK.
        /// </remarks>
        public bool TryGetSuccessor([AllowNull] T item, [MaybeNullWhen(false)] out T result) => DoTryGetSuccessor(item, out result);

        internal virtual bool DoTryGetSuccessor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
        {
            Node? current = root, match = null;

            while (current != null)
            {
                int comp = comparer.Compare(item!, current.Item);

                if (comp < 0)
                {
                    match = current;
                    current = current.Left;
                }
                else if (comp == 0)
                {
                    current = current.Right;
                    while (current != null)
                    {
                        match = current;
                        current = current.Left;
                    }
                }
                else
                    current = current.Right;
            }

            if (match == null)
            {
                result = default!;
                return false;
            }
            else
            {
                result = match.Item;
                return true;
            }
        }

        /// <summary>
        /// Gets the value in the <see cref="SortedSet{T}"/> whose value
        /// is the greatest element less than or equal to <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the floor of.</param>
        /// <param name="result">The floor, if any.</param>
        /// <returns><c>true</c> if a floor to <paramref name="item"/> exists; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>weak predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>floor()</c> method in the JDK.
        /// </remarks>
        public bool TryGetFloor([AllowNull] T item, [MaybeNullWhen(false)] out T result) => DoTryGetFloor(item, out result);

        internal virtual bool DoTryGetFloor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
        {
            Node? current = root;
            Node? candidate = null;

            while (current != null)
            {
                int cmp = comparer.Compare(item!, current.Item);

                if (cmp < 0)
                {
                    current = current.Left;
                }
                else
                {
                    candidate = current;
                    current = current.Right;
                }
            }

            if (candidate == null)
            {
                result = default!;
                return false;
            }

            result = candidate.Item;
            return true;
        }


        /// <summary>
        /// Gets the value in the <see cref="SortedSet{T}"/> whose value
        /// is the least element greater than or equal to <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the ceiling of.</param>
        /// <param name="result">The ceiling, if any.</param>
        /// <returns><c>true</c> if a ceiling to <paramref name="item"/> exists; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <b>weak successor</b> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>ceiling()</c> method in the JDK.
        /// </remarks>
        public bool TryGetCeiling([AllowNull] T item, [MaybeNullWhen(false)] out T result) => DoTryGetCeiling(item, out result);

        internal virtual bool DoTryGetCeiling([AllowNull] T item, [MaybeNullWhen(false)] out T result)
        {
            Node? current = root;
            Node? candidate = null;

            while (current != null)
            {
                int cmp = comparer.Compare(item!, current.Item);

                if (cmp > 0)
                {
                    current = current.Right;
                }
                else
                {
                    candidate = current;
                    current = current.Left;
                }
            }

            if (candidate == null)
            {
                result = default!;
                return false;
            }

            result = candidate.Item;
            return true;
        }

        #endregion

        #region ICollection<T> members

        /// <summary>
        /// Adds an element to the set and returns a value that indicates if it was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns><c>true</c> if item is added to the set; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="SortedSet{T}"/> class does not accept duplicate elements. If item is already
        /// in the set, this method returns <c>false</c> and does not throw an exception.
        /// <para/>
        /// If <see cref="Count"/> already equals the capacity of the <see cref="SortedSet{T}"/> object,
        /// the capacity is automatically adjusted to accommodate the new item.
        /// </remarks>
        public bool Add(T item) => AddIfNotPresent(item); // Hack so the implementation can be made virtual

        void ICollection<T>.Add(T item) => Add(item);

        internal virtual bool AddIfNotPresent(T item)
        {
            if (root == null)
            {
                // The tree is empty and this is the first item.
                root = new Node(item, NodeColor.Black);
                count = 1;
                version++;
                return true;
            }

            // Search for a node at bottom to insert the new node.
            // If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
            // We split 4-nodes along the search path.
            Node? current = root;
            Node? parent = null;
            Node? grandParent = null;
            Node? greatGrandParent = null;

            // Even if we don't actually add to the set, we may be altering its structure (by doing rotations and such).
            // So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
            version++;

            int order = 0;
            while (current != null)
            {
                order = comparer.Compare(item, current.Item);
                if (order == 0)
                {
                    // We could have changed root node to red during the search process.
                    // We need to set it to black before we return.
                    root.ColorBlack();
                    return false;
                }

                // Split a 4-node into two 2-nodes.
                if (current.Is4Node)
                {
                    current.Split4Node();
                    // We could have introduced two consecutive red nodes after split. Fix that by rotation.
                    if (Node.IsNonNullRed(parent))
                    {
                        InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
                    }
                }

                greatGrandParent = grandParent;
                grandParent = parent;
                parent = current;
                current = (order < 0) ? current.Left : current.Right;
            }

            Debug.Assert(parent != null);
            // We're ready to insert the new node.
            Node node = new Node(item, NodeColor.Red);
            if (order > 0)
            {
                parent!.Right = node;
            }
            else
            {
                parent!.Left = node;
            }

            // The new node will be red, so we will need to adjust colors if its parent is also red.
            if (parent.IsRed)
            {
                InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
            }

            // The root node is always black.
            root.ColorBlack();
            ++count;
            return true;
        }

        /// <summary>
        /// Removes a specified item from the <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><c>true</c> if the element is found and successfully removed; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the <see cref="SortedSet{T}"/> object does not contain the specified element, the object remains
        /// unchanged and no exception is thrown.
        /// <para/>
        /// <paramref name="item"/> can be <c>null</c> for reference types.
        /// <para/>
        /// This method is an <c>O(log n)</c> operation.
        /// </remarks>
        public bool Remove(T item) => DoRemove(item, out _); // Hack so the implementation can be made virtual

        internal virtual bool DoRemove(T item, [MaybeNullWhen(false)] out T removed)
        {
            if (root == null)
            {
                removed = default;
                return false;
            }

            // Search for a node and then find its successor.
            // Then copy the item from the successor to the matching node, and delete the successor.
            // If a node doesn't have a successor, we can replace it with its left child (if not empty),
            // or delete the matching node.
            //
            // In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
            // Following code will make sure the node on the path is not a 2-node.

            // Even if we don't actually remove from the set, we may be altering its structure (by doing rotations
            // and such). So update our version to disable any enumerators/subsets working on it.
            version++;

            Node? current = root;
            Node? parent = null;
            Node? grandParent = null;
            Node? match = null;
            Node? parentOfMatch = null;
            bool foundMatch = false;
            while (current != null)
            {
                if (current.Is2Node)
                {
                    // Fix up 2-node
                    if (parent == null)
                    {
                        // `current` is the root. Mark it red.
                        current.ColorRed();
                    }
                    else
                    {
                        Node sibling = parent.GetSibling(current);
                        if (sibling.IsRed)
                        {
                            // If parent is a 3-node, flip the orientation of the red link.
                            // We can achieve this by a single rotation.
                            // This case is converted to one of the other cases below.
                            Debug.Assert(parent.IsBlack);
                            if (parent.Right == sibling)
                            {
                                parent.RotateLeft();
                            }
                            else
                            {
                                parent.RotateRight();
                            }

                            parent.ColorRed();
                            sibling.ColorBlack(); // The red parent can't have black children.
                            // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
                            ReplaceChildOrRoot(grandParent, parent, sibling);
                            // `sibling` will become the grandparent of `current`.
                            grandParent = sibling;
                            if (parent == match)
                            {
                                parentOfMatch = sibling;
                            }

                            sibling = parent.GetSibling(current);
                        }

                        Debug.Assert(Node.IsNonNullBlack(sibling));

                        if (sibling.Is2Node)
                        {
                            parent.Merge2Nodes();
                        }
                        else
                        {
                            // `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
                            // We can change the color of `current` to red by some rotation.
                            Node newGrandParent = parent.Rotate(parent.GetRotation(current, sibling))!;

                            newGrandParent.Color = parent.Color;
                            parent.ColorBlack();
                            current.ColorRed();

                            ReplaceChildOrRoot(grandParent, parent, newGrandParent);
                            if (parent == match)
                            {
                                parentOfMatch = newGrandParent;
                            }
                        }
                    }
                }

                // We don't need to compare after we find the match.
                int order = foundMatch ? -1 : comparer.Compare(item, current.Item);
                if (order == 0)
                {
                    // Save the matching node.
                    foundMatch = true;
                    match = current;
                    parentOfMatch = parent;
                }

                grandParent = parent;
                parent = current;
                // If we found a match, continue the search in the right sub-tree.
                current = order < 0 ? current.Left : current.Right;
            }

            // Move successor to the matching node position and replace links.
            if (match != null)
            {
                removed = match.Item;
                ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
                --count;
            }
            else
            {
                removed = default;
            }

            root?.ColorBlack();
            return foundMatch;
        }

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        /// <remarks>
        /// This method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public virtual void Clear()
        {
            root = null;
            count = 0;
            ++version;
        }

        /// <summary>
        /// Determines whether the set contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate in the set.</param>
        /// <returns><c>true</c> if the set contains item; otherwise, <c>false</c>.</returns>
        /// <remarks>This method is an <c>O(log n)</c> operation.</remarks>
        public virtual bool Contains(T item) => FindNode(item) != null;

        /// <summary>
        /// Copies the complete <see cref="SortedSet{T}"/> to a compatible one-dimensional array,
        /// starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">A one-dimensional array that is the destination of the elements copied
        /// from the <see cref="SortedSet{T}"/>.</param>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="SortedSet{T}"/>
        /// exceeds the number of elements that the destination array can contain.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <remarks>The indexing of array must be zero-based.</remarks>
        public void CopyTo(T[] array) => CopyTo(array, 0, Count);

        /// <summary>
        /// Copies the complete <see cref="SortedSet{T}"/> to a compatible one-dimensional array,
        /// starting at the specified array index.
        /// </summary>
        /// <param name="array">A one-dimensional array that is the destination of the elements copied
        /// from the <see cref="SortedSet{T}"/>.The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentException">The number of elements in the source array is greater
        /// than the available space from <paramref name="index"/> to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <remarks>This method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        public void CopyTo(T[] array, int index) => CopyTo(array, index, Count);

        /// <summary>
        /// Copies the complete <see cref="SortedSet{T}"/> to a compatible one-dimensional array, starting
        /// at the beginning of the target array.
        /// </summary>
        /// <param name="array">A one-dimensional array that is the destination of the elements copied from
        /// the <see cref="SortedSet{T}"/>. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">The number of elements in the source array is greater
        /// than the available space from <paramref name="index"/> to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than zero.
        /// </exception>
        /// <remarks>This method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        public void CopyTo(T[] array, int index, int count)
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            if (count > array.Length - index)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

            // J2N: Ensure we always stay aligned with enumerator order
            int end = index + count;

            using var enumerator = GetEnumerator();
            while (index < end && enumerator.MoveNext())
            {
                array[index++] = enumerator.Current;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (array.Rank != 1)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            if (array.GetLowerBound(0) != 0)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
            if (array.Length - index < Count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

            T[]? tarray = array as T[];
            if (tarray != null)
            {
                CopyTo(tarray, index);
            }
            else
            {
                object?[]? objects = array as object[];
                if (objects == null)
                {
                    ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType(ExceptionArgument.array);
                }

                try
                {
                    // J2N: Ensure we always stay aligned with enumerator order
                    using (var enumerator = GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            objects[index++] = enumerator.Current;
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType(ExceptionArgument.array);
                }
            }
        }

        #endregion

        #region SpanAlternateLookup

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="SortedSet{T}"/>
        /// using a <typeparamref name="TAlternateSpan"/> instead of a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TAlternateSpan">The alternate type of <see cref="ReadOnlySpan{T}"/> instance for performing lookups.</typeparam>
        /// <returns>The created lookup instance.</returns>
        /// <exception cref="InvalidOperationException">The set's comparer is not compatible with <typeparamref name="TAlternateSpan"/>.</exception>
        /// <remarks>
        /// The set must be using a comparer that implements <see cref="ISpanAlternateComparer{TAlternateSpan, T}"/> with
        /// a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateSpan"/> and <typeparamref name="T"/>.
        /// If it doesn't, an exception will be thrown.
        /// <para/>
        /// The following <see cref="StringComparer" /> options have built-in support when
        /// <typeparamref name="TAlternateSpan"/> is <see cref="char"/>.
        /// <list type="bullet">
        ///     <item><description><see cref="StringComparer.Ordinal"/></description></item>
        ///     <item><description><see cref="StringComparer.OrdinalIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCultureIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCultureIgnoreCase"/></description></item>
        /// </list>
        /// </remarks>
        public SpanAlternateLookup<TAlternateSpan> GetSpanAlternateLookup<TAlternateSpan>()
        {
            if (!SpanAlternateLookup<TAlternateSpan>.IsCompatibleItem(this))
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
            }

            return new SpanAlternateLookup<TAlternateSpan>(this);
        }

        // This overload is a special case for the comparer to be passed down from SortedDictionary<TKey, TValue>,
        // since it requires an adapter. This is recursively called inside of this type to ensure the same comparer
        // is used by other view instances, even if it is not the SortedSet<T> comparer.
        internal SpanAlternateLookup<TAlternateSpan> GetSpanAlternateLookup<TAlternateSpan>(ISpanAlternateComparer<TAlternateSpan, T>? alternateComparer)
        {
            if (alternateComparer is null)
            {
                return new SpanAlternateLookup<TAlternateSpan>(this);
            }

            return new SpanAlternateLookup<TAlternateSpan>(this, alternateComparer!);
        }

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="SortedSet{T}"/>
        /// using a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateSpan"/> instead of a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TAlternateSpan">The alternate type of <see cref="ReadOnlySpan{T}"/> instance for performing lookups.</typeparam>
        /// <param name="lookup">The created lookup instance when the method returns <see langword="true"/>, or a default instance
        /// that should not be used if the method returns <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if a lookup could be created; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The set must be using a comparer that implements <see cref="ISpanAlternateComparer{TAlternateSpan, T}"/> with
        /// a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateSpan"/> and <typeparamref name="T"/>.
        /// If it doesn't, the method returns <see langword="false"/>.
        /// <para/>
        /// The following <see cref="StringComparer" /> options have built-in support when
        /// <typeparamref name="TAlternateSpan"/> is <see cref="char"/>.
        /// <list type="bullet">
        ///     <item><description><see cref="StringComparer.Ordinal"/></description></item>
        ///     <item><description><see cref="StringComparer.OrdinalIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCultureIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCultureIgnoreCase"/></description></item>
        /// </list>
        /// </remarks>
        public bool TryGetSpanAlternateLookup<TAlternateSpan>(out SpanAlternateLookup<TAlternateSpan> lookup)
        {
            if (SpanAlternateLookup<TAlternateSpan>.IsCompatibleItem(this))
            {
                lookup = new SpanAlternateLookup<TAlternateSpan>(this);
                return true;
            }

            lookup = default;
            return false;
        }

        /// <summary>
        /// Provides a type that may be used to perform operations on a <see cref="SortedSet{T}"/>
        /// using a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateSpan"/> instead of a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TAlternateSpan">The alternate type of instance for performing lookups.</typeparam>
        public readonly struct SpanAlternateLookup<TAlternateSpan>
        {
            private readonly ISpanAlternateComparer<TAlternateSpan, T>? _alternateComparer;
            private readonly bool _isUnderlying;

            /// <summary>Initialize the instance. The set must have already been verified to have a compatible comparer.</summary>
            internal SpanAlternateLookup(SortedSet<T> set)
            {
                Debug.Assert(set is not null);
                Debug.Assert(IsCompatibleItem(set!)); // [!]: asserted above
                Set = set!; // [!]: asserted above
                _isUnderlying = set == set!.UnderlyingSet; // [!]: asserted above
                _alternateComparer = null;
            }

            /// <summary>Initialize the instance. The comparer must have already been verified to be compatible with the set.</summary>
            internal SpanAlternateLookup(SortedSet<T> set, ISpanAlternateComparer<TAlternateSpan, T> alternateComparer)
            {
                Debug.Assert(set is not null);
                Debug.Assert(alternateComparer is not null); // [!]: asserted above
                Set = set!; // [!]: asserted above
                _isUnderlying = set == set!.UnderlyingSet; // [!]: asserted above
                _alternateComparer = alternateComparer;
            }

            /// <summary>Gets the <see cref="SortedSet{T}"/> against which this instance performs operations.</summary>
            public SortedSet<T> Set { get; }

            /// <summary>Checks whether the set has a comparer compatible with <typeparamref name="TAlternateSpan"/>.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool IsCompatibleItem(SortedSet<T> set)
            {
                Debug.Assert(set is not null);
                return set!.comparer is ISpanAlternateComparer<TAlternateSpan, T>; // [!]: asserted above
            }

            /// <summary>Gets the set's alternate comparer. The set must have already been verified as compatible.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ISpanAlternateComparer<TAlternateSpan, T> GetAlternateComparer()
            {
                if (_alternateComparer is null)
                {
                    Debug.Assert(IsCompatibleItem(Set));
                    return Unsafe.As<ISpanAlternateComparer<TAlternateSpan, T>>(Set.comparer)!;
                }

                // This path is for the SortedDictionary<TKey, TValue>, since it will pass its comparer through the constructor directly
                // to satisfy the generic closing types (something that KeyValuePairComparer cannot do).
                return _alternateComparer;
            }

            #region Add

            /// <summary>Adds the specified element to a set.</summary>
            /// <param name="item">The element to add to the set.</param>
            /// <returns><see langword="true"/> if the element is added to the set; <see langword="false"/> if the element is already present.</returns>
            public bool Add(ReadOnlySpan<TAlternateSpan> item)
            {
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                if (_isUnderlying)
                    return AddIfNotPresent(item, comparer);

                return Add_View(item, comparer);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private bool Add_View(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;
                SortedSet<T> underlying = set.UnderlyingSet;

                if (!IsWithinRange(item, comparer))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.item);
                }

                // Delegate to underlying set.
                // All views share the same Comparer instance. Therefore, passing the alternate comparer to the other instance is also safe.
                bool ret = underlying.GetSpanAlternateLookup(_alternateComparer).AddIfNotPresent(item, comparer);

                set.VersionCheck();

#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == underlying.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                return ret;
            }

            private bool AddIfNotPresent(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;

                if (set.root == null)
                {
                    // The tree is empty and this is the first item.
                    set.root = new Node(comparer.Create(item), NodeColor.Black);
                    set.count = 1;
                    set.version++;
                    return true;
                }

                // Search for a node at bottom to insert the new node.
                // If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
                // We split 4-nodes along the search path.
                Node? current = set.root;
                Node? parent = null;
                Node? grandParent = null;
                Node? greatGrandParent = null;

                // Even if we don't actually add to the set, we may be altering its structure (by doing rotations and such).
                // So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
                set.version++;

                int order = 0;
                while (current != null)
                {
                    order = comparer.Compare(item, current.Item);
                    if (order == 0)
                    {
                        // We could have changed root node to red during the search process.
                        // We need to set it to black before we return.
                        set.root.ColorBlack();
                        return false;
                    }

                    // Split a 4-node into two 2-nodes.
                    if (current.Is4Node)
                    {
                        current.Split4Node();
                        // We could have introduced two consecutive red nodes after split. Fix that by rotation.
                        if (Node.IsNonNullRed(parent))
                        {
                            set.InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
                        }
                    }

                    greatGrandParent = grandParent;
                    grandParent = parent;
                    parent = current;
                    current = (order < 0) ? current.Left : current.Right;
                }

                Debug.Assert(parent != null);
                // We're ready to insert the new node.
                Node node = new Node(comparer.Create(item), NodeColor.Red);
                if (order > 0)
                {
                    parent!.Right = node;
                }
                else
                {
                    parent!.Left = node;
                }

                // The new node will be red, so we will need to adjust colors if its parent is also red.
                if (parent.IsRed)
                {
                    set.InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
                }

                // The root node is always black.
                set.root.ColorBlack();
                ++set.count;
                return true;
            }

            #endregion Add

            #region TryAdd

            // J2N: Called from SortedDictionary<TKey, TValue>.SpanAlternateLookup<TAlternateKeySpan> only. We need to do tasks in the following order:
            //
            // 1. Traverse the tree to see if the node exists
            // 2. If not, instantiate the TKey value
            // 3. Create a KeyValuePair, populating both TKey and TValue
            // 4. Add the KeyValuePair to the SortedSet as type T
            //
            // This method requires both type info about the dictionary and the ability to traverse the tree and internal state of SortedSet<T>.
            // Since SortedSet<T> owns the tree and ReadOnlySpan<T> limits how we can instantiate the value, it is more sensible to do unsafe casts here than to
            // expose all of the internal state of SortedSet<T> to SortedDictionary<TKey, TValue>.SpanAlternateLookup<TAlternateKeySpan> to pull this off.
            internal bool TryAdd<TKey, TValue>(ReadOnlySpan<TAlternateSpan> key, TValue value, SortedDictionary<TKey, TValue>.AlternateKeyValuePairComparer<TAlternateSpan> comparer)
            {
#if DEBUG
                Debug.Assert(typeof(T) == typeof(KeyValuePair<TKey, TValue>));
#endif

                if (_isUnderlying)
                    return AddIfNotPresent(key, value, comparer);

                return TryAdd_View(key, value, comparer);
            }

            internal bool TryAdd_View<TKey, TValue>(ReadOnlySpan<TAlternateSpan> key, TValue value, SortedDictionary<TKey, TValue>.AlternateKeyValuePairComparer<TAlternateSpan> comparer)
            {
#if DEBUG
                Debug.Assert(typeof(T) == typeof(KeyValuePair<TKey, TValue>));
                Debug.Assert(_alternateComparer is not null);
#endif
                SortedSet<T> set = Set;
                SortedSet<T> underlying = set.UnderlyingSet;
                ISpanAlternateComparer<TAlternateSpan, T> cmp = Unsafe.As<SortedDictionary<TKey, TValue>.AlternateKeyValuePairComparer<TAlternateSpan>, ISpanAlternateComparer<TAlternateSpan, T>>(ref comparer);

                if (!IsWithinRange(key, cmp))
                {
                    //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.key);
                    return false;
                }

                // Delegate to underlying set.
                // All views share the same Comparer instance. Therefore, passing the alternate comparer to the other instance is also safe.
                bool ret = underlying.GetSpanAlternateLookup(_alternateComparer).AddIfNotPresent(key, value, comparer);

                set.VersionCheck();

#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == underlying.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                return ret;
            }


            private bool AddIfNotPresent<TKey, TValue>(ReadOnlySpan<TAlternateSpan> key, TValue value, SortedDictionary<TKey, TValue>.AlternateKeyValuePairComparer<TAlternateSpan> comparer)
            {
#if DEBUG
                Debug.Assert(typeof(T) == typeof(KeyValuePair<TKey, TValue>));
#endif

                SortedSet<T> set = Set;

                if (set.root == null)
                {
                    // The tree is empty and this is the first item.
                    KeyValuePair<TKey, TValue> kvpBlack = comparer.Create(key, value);
                    set.root = new Node(Unsafe.As<KeyValuePair<TKey,TValue>, T>(ref kvpBlack), NodeColor.Black);
                    set.count = 1;
                    set.version++;
                    return true;
                }

                // Search for a node at bottom to insert the new node.
                // If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
                // We split 4-nodes along the search path.
                Node? current = set.root;
                Node? parent = null;
                Node? grandParent = null;
                Node? greatGrandParent = null;

                // Even if we don't actually add to the set, we may be altering its structure (by doing rotations and such).
                // So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
                set.version++;

                int order = 0;
                while (current != null)
                {
                    ref T item = ref current.Item;
                    order = comparer.Compare(key, Unsafe.As<T, KeyValuePair<TKey,TValue>>(ref item));
                    if (order == 0)
                    {
                        // We could have changed root node to red during the search process.
                        // We need to set it to black before we return.
                        set.root.ColorBlack();
                        return false;
                    }

                    // Split a 4-node into two 2-nodes.
                    if (current.Is4Node)
                    {
                        current.Split4Node();
                        // We could have introduced two consecutive red nodes after split. Fix that by rotation.
                        if (Node.IsNonNullRed(parent))
                        {
                            set.InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
                        }
                    }

                    greatGrandParent = grandParent;
                    grandParent = parent;
                    parent = current;
                    current = (order < 0) ? current.Left : current.Right;
                }

                Debug.Assert(parent != null);
                // We're ready to insert the new node.
                KeyValuePair<TKey, TValue> kvpRed = comparer.Create(key, value);
                Node node = new Node(Unsafe.As<KeyValuePair<TKey, TValue>, T>(ref kvpRed), NodeColor.Red);
                if (order > 0)
                {
                    parent!.Right = node;
                }
                else
                {
                    parent!.Left = node;
                }

                // The new node will be red, so we will need to adjust colors if its parent is also red.
                if (parent.IsRed)
                {
                    set.InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
                }

                // The root node is always black.
                set.root.ColorBlack();
                ++set.count;
                return true;
            }

            #endregion TryAdd

            #region Remove

            /// <summary>Removes the specified element from a set.</summary>
            /// <param name="item">The element to remove.</param>
            /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.</returns>
            public bool Remove(ReadOnlySpan<TAlternateSpan> item)
            {
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                if (_isUnderlying)
                    return DoRemove(item, comparer, out _);

                return Remove_View(item, comparer, out _);
            }

            internal bool Remove(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T removed)
            {
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                if (_isUnderlying)
                    return DoRemove(item, comparer, out removed);

                return Remove_View(item, comparer, out removed);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private bool Remove_View(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer, [MaybeNullWhen(false)] out T removed)
            {
                SortedSet<T> set = Set;
                SortedSet<T> underlying = set.UnderlyingSet;

                if (!IsWithinRange(item, comparer))
                {
                    removed = default;
                    return false;
                }

                // Delegate to underlying set.
                // All views share the same Comparer instance. Therefore, passing the alternate comparer to the other instance is also safe.
                bool ret = underlying.GetSpanAlternateLookup(_alternateComparer).DoRemove(item, comparer, out removed);

                set.VersionCheck();

#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == underlying.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                return ret;
            }

            private bool DoRemove(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer, [MaybeNullWhen(false)] out T removed)
            {
                SortedSet<T> set = Set;

                if (set.root is null)
                {
                    removed = default;
                    return false;
                }

                // Search for a node and then find its successor.
                // Then copy the item from the successor to the matching node, and delete the successor.
                // If a node doesn't have a successor, we can replace it with its left child (if not empty),
                // or delete the matching node.
                //
                // In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
                // Following code will make sure the node on the path is not a 2-node.

                // Even if we don't actually remove from the set, we may be altering its structure (by doing rotations
                // and such). So update our version to disable any enumerators/subsets working on it.
                set.version++;

                Node? current = set.root;
                Node? parent = null;
                Node? grandParent = null;
                Node? match = null;
                Node? parentOfMatch = null;
                bool foundMatch = false;
                while (current != null)
                {
                    if (current.Is2Node)
                    {
                        // Fix up 2-node
                        if (parent == null)
                        {
                            // `current` is the root. Mark it red.
                            current.ColorRed();
                        }
                        else
                        {
                            Node sibling = parent.GetSibling(current);
                            if (sibling.IsRed)
                            {
                                // If parent is a 3-node, flip the orientation of the red link.
                                // We can achieve this by a single rotation.
                                // This case is converted to one of the other cases below.
                                Debug.Assert(parent.IsBlack);
                                if (parent.Right == sibling)
                                {
                                    parent.RotateLeft();
                                }
                                else
                                {
                                    parent.RotateRight();
                                }

                                parent.ColorRed();
                                sibling.ColorBlack(); // The red parent can't have black children.
                                                      // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
                                set.ReplaceChildOrRoot(grandParent, parent, sibling);
                                // `sibling` will become the grandparent of `current`.
                                grandParent = sibling;
                                if (parent == match)
                                {
                                    parentOfMatch = sibling;
                                }

                                sibling = parent.GetSibling(current);
                            }

                            Debug.Assert(Node.IsNonNullBlack(sibling));

                            if (sibling.Is2Node)
                            {
                                parent.Merge2Nodes();
                            }
                            else
                            {
                                // `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
                                // We can change the color of `current` to red by some rotation.
                                Node newGrandParent = parent.Rotate(parent.GetRotation(current, sibling))!;

                                newGrandParent.Color = parent.Color;
                                parent.ColorBlack();
                                current.ColorRed();

                                set.ReplaceChildOrRoot(grandParent, parent, newGrandParent);
                                if (parent == match)
                                {
                                    parentOfMatch = newGrandParent;
                                }
                            }
                        }
                    }

                    // We don't need to compare after we find the match.
                    int order = foundMatch ? -1 : comparer.Compare(item, current.Item);
                    if (order == 0)
                    {
                        // Save the matching node.
                        foundMatch = true;
                        match = current;
                        parentOfMatch = parent;
                    }

                    grandParent = parent;
                    parent = current;
                    // If we found a match, continue the search in the right sub-tree.
                    current = order < 0 ? current.Left : current.Right;
                }

                // Move successor to the matching node position and replace links.
                if (match != null)
                {
                    removed = match.Item;
                    set.ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
                    --set.count;
                }
                else
                {
                    removed = default;
                }

                set.root?.ColorBlack();

                return foundMatch;
            }

            #endregion Remove

            #region Contains

            /// <summary>Determines whether a set contains the specified element.</summary>
            /// <param name="item">The element to locate in the set.</param>
            /// <returns><see langword="true"/> if the set contains the specified element; otherwise, <see langword="false"/>.</returns>
            public bool Contains(ReadOnlySpan<TAlternateSpan> item)
            {
                if (_isUnderlying)
                    return FindNode(item) is not null;

                return Contains_View(item);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private bool Contains_View(ReadOnlySpan<TAlternateSpan> item)
            {
                SortedSet<T> set = Set;
                set.VersionCheck();
#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == set.UnderlyingSet.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                return FindNode_View(item) is not null;
            }

            #endregion Contains

            #region TryGetValue

            /// <summary>Searches the set for a given value and returns the equal value it finds, if any.</summary>
            /// <param name="equalValue">The value to search for.</param>
            /// <param name="actualValue">The value from the set that the search found, or the default value of
            /// <typeparamref name="T"/> when the search yielded no match.</param>
            /// <returns>A value indicating whether the search was successful.</returns>
            public bool TryGetValue(ReadOnlySpan<TAlternateSpan> equalValue, [MaybeNullWhen(false)] out T actualValue)
            {
                Node? node = _isUnderlying ? FindNode(equalValue) : FindNode_View(equalValue);
                if (node is not null)
                {
                    actualValue = node.Item;
                    return true;
                }

                actualValue = default!;
                return false;
            }

            #endregion TryGetValue

            #region TryGetPredecessor

            /// <summary>
            /// Gets the entry in the <see cref="SortedSet{T}"/> whose value
            /// is the predecessor of the specified <paramref name="item"/>.
            /// </summary>
            /// <param name="item">The entry to get the predecessor of.</param>
            /// <param name="result">The predecessor, if any.</param>
            /// <returns><see langword="true"/> if a predecessor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <c>strict predecessor</c> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>lower()</c> method in the JDK.
            /// </remarks>
            public bool TryGetPredecessor(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result) =>
                _isUnderlying ? DoTryGetPredecessor(item, out result) : DoTryGetPredecessor_View(item, out result);

            private bool DoTryGetPredecessor(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                Node? current = set.root, match = null;

                while (current != null)
                {
                    int comp = comparer.Compare(item, current.Item);

                    if (comp > 0)
                    {
                        match = current;
                        current = current.Right;
                    }
                    else if (comp == 0)
                    {
                        current = current.Left;
                        while (current != null)
                        {
                            match = current;
                            current = current.Right;
                        }
                    }
                    else
                        current = current.Left;
                }

                if (match == null)
                {
                    result = default!;
                    return false;
                }
                else
                {
                    result = match.Item;
                    return true;
                }
            }

            private bool DoTryGetPredecessor_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
                => Set.IsReversed ? TryGetSuccessorCore_View(item, out result) : TryGetPredecessorCore_View(item, out result);

            private bool TryGetPredecessorCore_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                set.VersionCheck();
#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == set.UnderlyingSet.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                // If item is at or below lower bound, no strict predecessor exists
                if (IsTooLow(item, comparer))
                {
                    result = default!;
                    return false;
                }

                Node? current = set.root;
                Node? match = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item, current.Item);

                    if (cmp > 0)
                    {
                        match = current;
                        current = current.Right;
                    }
                    else
                    {
                        current = current.Left;
                    }
                }

                // Final safety check: candidate must be within view
                if (match == null || set.IsTooLow(match.Item))
                {
                    result = default!;
                    return false;
                }

                result = match.Item;
                return true;
            }

            #endregion TryGetPredecessor

            #region TryGetSuccessor

            /// <summary>
            /// Gets the entry in the <see cref="SortedSet{T}"/> whose value
            /// is the successor of the specified <paramref name="item"/>.
            /// </summary>
            /// <param name="item">The entry to get the successor of.</param>
            /// <param name="result">The successor, if any.</param>
            /// <returns><see langword="true"/> if a successor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <c>strict successor</c> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>higher()</c> method in the JDK.
            /// </remarks>
            public bool TryGetSuccessor(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result) =>
                _isUnderlying ? DoTryGetSuccessor(item, out result) : DoTryGetSuccessor_View(item, out result);

            private bool DoTryGetSuccessor(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                Node? current = set.root, match = null;

                while (current != null)
                {
                    int comp = comparer.Compare(item, current.Item);

                    if (comp < 0)
                    {
                        match = current;
                        current = current.Left;
                    }
                    else if (comp == 0)
                    {
                        current = current.Right;
                        while (current != null)
                        {
                            match = current;
                            current = current.Left;
                        }
                    }
                    else
                        current = current.Right;
                }

                if (match == null)
                {
                    result = default!;
                    return false;
                }
                else
                {
                    result = match.Item;
                    return true;
                }
            }

            private bool DoTryGetSuccessor_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
                => Set.IsReversed ? TryGetPredecessorCore_View(item, out result) : TryGetSuccessorCore_View(item, out result);

            private bool TryGetSuccessorCore_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                set.VersionCheck();
#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == set.UnderlyingSet.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                // If item is at or above upper bound, no strict successor exists
                if (IsTooHigh(item, comparer))
                {
                    result = default!;
                    return false;
                }

                Node? current = set.root;
                Node? match = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item, current.Item);

                    if (cmp < 0)
                    {
                        match = current;
                        current = current.Left;
                    }
                    else
                    {
                        current = current.Right;
                    }
                }

                // Final safety check
                if (match == null || set.IsTooHigh(match.Item))
                {
                    result = default!;
                    return false;
                }

                result = match.Item;
                return true;
            }

            #endregion TryGetSuccessor

            #region TryGetFloor

            /// <summary>
            /// Gets the value in the <see cref="SortedSet{T}"/> whose value
            /// is the greatest element less than or equal to <paramref name="item"/>.
            /// </summary>
            /// <param name="item">The entry to get the floor of.</param>
            /// <param name="result">The floor, if any.</param>
            /// <returns><see langword="true"/> if a floor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <c>weak predecessor</c> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>floor()</c> method in the JDK.
            /// </remarks>
            public bool TryGetFloor(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result) =>
                _isUnderlying ? DoTryGetFloor(item, out result) :DoTryGetFloor_View(item, out result);

            private bool DoTryGetFloor(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                Node? current = set.root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item, current.Item);

                    if (cmp < 0)
                    {
                        current = current.Left;
                    }
                    else
                    {
                        candidate = current;
                        current = current.Right;
                    }
                }

                if (candidate == null)
                {
                    result = default!;
                    return false;
                }

                result = candidate.Item;
                return true;
            }

            private bool DoTryGetFloor_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
                => Set.IsReversed ? TryGetCeilingCore_View(item, out result) : TryGetFloorCore_View(item, out result);

            private bool TryGetFloorCore_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                set.VersionCheck();
#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == set.UnderlyingSet.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                Node? current = set.root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item, current.Item);

                    if (cmp < 0)
                    {
                        current = current.Left;
                    }
                    else
                    {
                        candidate = current;
                        current = current.Right;
                    }
                }

                if (candidate == null || set.IsTooLow(candidate.Item))
                {
                    result = default!;
                    return false;
                }

                result = candidate.Item;
                return true;
            }

            #endregion TryGetFloor

            #region TryGetCeiling

            /// <summary>
            /// Gets the value in the <see cref="SortedSet{T}"/> whose value
            /// is the least element greater than or equal to <paramref name="item"/>.
            /// </summary>
            /// <param name="item">The entry to get the ceiling of.</param>
            /// <param name="result">The ceiling, if any.</param>
            /// <returns><see langword="true"/> if a ceiling to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <b>weak successor</b> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>ceiling()</c> method in the JDK.
            /// </remarks>
            public bool TryGetCeiling(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result) =>
                _isUnderlying ? DoTryGetCeiling(item, out result) : DoTryGetCeiling_View(item, out result);

            private bool DoTryGetCeiling(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                Node? current = set.root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item, current.Item);

                    if (cmp > 0)
                    {
                        current = current.Right;
                    }
                    else
                    {
                        candidate = current;
                        current = current.Left;
                    }
                }

                if (candidate == null)
                {
                    result = default!;
                    return false;
                }

                result = candidate.Item;
                return true;
            }

            private bool DoTryGetCeiling_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
                => Set.IsReversed ? TryGetFloorCore_View(item, out result) : TryGetCeilingCore_View(item, out result);

            private bool TryGetCeilingCore_View(ReadOnlySpan<TAlternateSpan> item, [MaybeNullWhen(false)] out T result)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                set.VersionCheck();
#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == set.UnderlyingSet.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif

                Node? current = set.root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item, current.Item);

                    if (cmp > 0)
                    {
                        current = current.Right;
                    }
                    else
                    {
                        candidate = current;
                        current = current.Left;
                    }
                }

                if (candidate == null || set.IsTooHigh(candidate.Item))
                {
                    result = default!;
                    return false;
                }

                result = candidate.Item;
                return true;
            }

            #endregion TryGetCeiling

            #region GetView

            /// <summary>
            /// Returns a view of a subset in a <see cref="SortedSet{T}"/>.
            /// <para/>
            /// Usage Note: In Java, the <paramref name="toItem"/> of TreeSet.subSet() is exclusive. To match the behavior, call
            /// <see cref="GetView(ReadOnlySpan{TAlternateSpan}, bool, ReadOnlySpan{TAlternateSpan}, bool)"/>, setting <c>fromInclusive</c> to <see langword="true"/>
            /// and <c>toInclusive</c> to <see langword="false"/>.
            /// </summary>
            /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
            /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <exception cref="ArgumentException"><paramref name="fromItem"/> is after <paramref name="toItem"/>
            /// in the current view order according to the comparer.</exception>
            /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
            /// specified by <paramref name="fromItem"/> and <paramref name="toItem"/>.</exception>
            /// <remarks>
            /// This method returns a view of the range of elements that fall between <paramref name="fromItem"/> and
            /// <paramref name="toItem"/> (inclusive), as defined by the current view order and the comparer.
            /// This method does not copy elements from the <see cref="SortedSet{T}"/>, but provides a window into the
            /// underlying <see cref="SortedSet{T}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
            /// <para/>
            /// This corresponds to the <c>subSet()</c> method in the JDK.
            /// </remarks>
            public SortedSet<T> GetView(ReadOnlySpan<TAlternateSpan> fromItem, ReadOnlySpan<TAlternateSpan> toItem)
                => DoGetView(fromItem, fromInclusive: true, ExceptionArgument.fromItem, toItem, toInclusive: true, ExceptionArgument.toItem);

            /// <summary>
            /// Returns a view of a subset in a <see cref="SortedSet{T}"/>.
            /// <para/>
            /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="fromInclusive"/>
            /// set to <see langword="true"/> and <paramref name="toInclusive"/> set to <see langword="false"/>.
            /// </summary>
            /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
            /// <param name="fromInclusive">If <see langword="true"/>, <paramref name="fromItem"/> will be included in the range;
            /// otherwise, it is an exclusive bound.</param>
            /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
            /// <param name="toInclusive">If <see langword="true"/>, <paramref name="toItem"/> will be included in the range;
            /// otherwise, it is an exclusive bound.</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <exception cref="ArgumentException"><paramref name="fromItem"/> is after <paramref name="toItem"/>
            /// in the current view order according to the comparer.</exception>
            /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
            /// specified by <paramref name="fromItem"/> and <paramref name="toItem"/>.</exception>
            /// <remarks>
            /// This method returns a view of the range of elements that fall between <paramref name="fromItem"/> and
            /// <paramref name="toItem"/>, as defined by the current view order and the comparer. Each bound may either be inclusive
            /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="fromInclusive"/>
            /// and <paramref name="toInclusive"/>. This method does not copy elements from the
            /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
            /// <para/>
            /// This corresponds to the <c>subSet()</c> method in the JDK.
            /// </remarks>
            public SortedSet<T> GetView(ReadOnlySpan<TAlternateSpan> fromItem, bool fromInclusive, ReadOnlySpan<TAlternateSpan> toItem, bool toInclusive)
                => DoGetView(fromItem, fromInclusive, ExceptionArgument.fromItem, toItem, toInclusive, ExceptionArgument.toItem);

            internal SortedSet<T> DoGetView(ReadOnlySpan<TAlternateSpan> fromItem, bool fromInclusive, ExceptionArgument fromArgumentName, ReadOnlySpan<TAlternateSpan> toItem, bool toInclusive, ExceptionArgument toArgumentName)
            {
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                return _isUnderlying
                    ? DoGetView(fromItem, fromInclusive, fromArgumentName, toItem, toInclusive, toArgumentName, comparer)
                    : DoGetView_View(fromItem, fromInclusive, fromArgumentName, toItem, toInclusive, toArgumentName, comparer);
            }

            internal SortedSet<T> DoGetView_View(ReadOnlySpan<TAlternateSpan> fromItem, bool fromInclusive, ExceptionArgument fromArgumentName, ReadOnlySpan<TAlternateSpan> toItem, bool toInclusive, ExceptionArgument toArgumentName, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                bool reverse = Set.IsReversed;

                ReadOnlySpan<TAlternateSpan> lower = reverse ? toItem : fromItem;
                ReadOnlySpan<TAlternateSpan> upper = reverse ? fromItem : toItem;
                bool lowerInclusive = reverse ? toInclusive : fromInclusive;
                bool upperInclusive = reverse ? fromInclusive : toInclusive;
                ExceptionArgument lowerArgumentName = reverse ? toArgumentName : fromArgumentName;
                ExceptionArgument upperArgumentName = reverse ? fromArgumentName : toArgumentName;

                if (!IsWithinRange(lower, lowerInclusive, comparer))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(lowerArgumentName);
                }
                if (!IsWithinRange(upper, upperInclusive, comparer))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(upperArgumentName);
                }

                return DoGetView(lower, lowerInclusive, lowerArgumentName, upper, upperInclusive, upperArgumentName, comparer);
            }

            internal SortedSet<T> DoGetView(ReadOnlySpan<TAlternateSpan> fromItem, bool fromInclusive, ExceptionArgument fromArgumentName, ReadOnlySpan<TAlternateSpan> toItem, bool toInclusive, ExceptionArgument toArgumentName, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                // J2N: We instantiate the upper instance prior to comparing to see whether we should
                // throw when lowerValue is greater than upperValue. This is so we don't have
                // to change the design of the ISpanAlternateComparer interface to allow matching 2
                // ReadOnlySpan instances. We must get two instances anyway because TreeSubSet requires
                // them as fields, so there is no harm in doing it this way.

                if (!TryGetValue(toItem, out T? to))
                {
                    to = comparer.Create(toItem);
                }
                if (comparer.Compare(fromItem, to) > 0)
                {
                    ThrowHelper.ThrowArgumentException_SortedSet_LowerValueGreaterThanUpperValue(fromArgumentName, toArgumentName);
                }
                if (!TryGetValue(fromItem, out T? from))
                {
                    from = comparer.Create(fromItem);
                }

                SortedSet<T> set = Set;
                return new TreeSubSet(set.UnderlyingSet, from, fromInclusive, to, toInclusive, true, true, set.IsReversed);
            }

            #endregion GetView

            #region GetViewBefore

            /// <summary>
            /// Returns the view of a subset in a <see cref="SortedSet{T}"/> with no lower bound.
            /// <para/>
            /// Usage Note: To match the default behavior of the JDK, call the <see cref="GetViewBefore(ReadOnlySpan{TAlternateSpan}, bool)"/>
            /// overload with <c>inclusive</c> set to <see langword="false"/>.
            /// </summary>
            /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <remarks>
            /// This method returns a view of the range of elements that fall before <paramref name="toItem"/>
            /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
            /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
            /// <para/>
            /// This corresponds to the <c>headSet()</c> method in the JDK.
            /// </remarks>
            public SortedSet<T> GetViewBefore(ReadOnlySpan<TAlternateSpan> toItem)
                => DoGetViewBefore(toItem, inclusive: true, ExceptionArgument.toItem);

            /// <summary>
            /// Returns the view of a subset in a <see cref="SortedSet{T}"/> with no lower bound.
            /// <para/>
            /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="inclusive"/>
            /// set to <see langword="false"/>.
            /// </summary>
            /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
            /// <param name="inclusive">If <see langword="true"/>, <paramref name="toItem"/> will be included in the range;
            /// otherwise, it is an exclusive upper bound.</param>
            /// <returns>
            /// This method returns a view of the range of elements that fall before <paramref name="toItem"/>, as defined
            /// by the current view order and comparer. The upper bound may either be inclusive (<see langword="true"/>)
            /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>.
            /// This method does not copy elements from the <see cref="SortedSet{T}"/>, but provides a window into
            /// the underlying <see cref="SortedSet{T}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
            /// <para/>
            /// This corresponds to the <c>headSet()</c> method in the JDK.
            /// </returns>
            public SortedSet<T> GetViewBefore(ReadOnlySpan<TAlternateSpan> toItem, bool inclusive)
                => DoGetViewBefore(toItem, inclusive, ExceptionArgument.toItem);

            internal SortedSet<T> DoGetViewBefore(ReadOnlySpan<TAlternateSpan> toItem, bool inclusive, ExceptionArgument toArgumentName)
            {
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                return _isUnderlying
                    ? DoGetViewBefore(toItem, inclusive, toArgumentName, comparer)
                    : DoGetViewBefore_View(toItem, inclusive, toArgumentName, comparer);
            }

            internal SortedSet<T> DoGetViewBefore(ReadOnlySpan<TAlternateSpan> toItem, bool inclusive, ExceptionArgument toArgumentName, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                if (!TryGetValue(toItem, out T? upper))
                {
                    upper = comparer.Create(toItem);
                }

                SortedSet<T> set = Set;
                return new TreeSubSet(set.UnderlyingSet, default, true, upper, inclusive, false, true, set.IsReversed);
            }

            internal SortedSet<T> DoGetViewBefore_View(ReadOnlySpan<TAlternateSpan> toItem, bool inclusive, ExceptionArgument toArgumentName, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                if (!IsWithinRange(toItem, inclusive, comparer))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(toArgumentName);
                }

                return !Set.IsReversed
                    ? GetViewBeforeCore_View(toItem, inclusive, comparer)
                    : GetViewAfterCore_View(toItem, inclusive, comparer);
            }

            internal SortedSet<T> GetViewBeforeCore_View(ReadOnlySpan<TAlternateSpan> toItem, bool inclusive, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;
                T? upper;
                bool upperInclusive;

                // Fast path - no upper bound, no equality possible
                if (!set.HasUpperBound)
                {
                    if (!TryGetValue(toItem, out upper))
                        upper = comparer.Create(toItem);
                    upperInclusive = inclusive;
                }
                else
                {
                    // Compute comparison ONCE
                    int cmp = comparer.Compare(toItem, set.UpperBound!);

                    if (cmp < 0)
                    {
                        // Override with new value
                        if (!TryGetValue(toItem, out upper))
                            upper = comparer.Create(toItem);
                        upperInclusive = inclusive;
                    }
                    else if (cmp > 0)
                    {
                        // Clipped by upper bound
                        upper = set.UpperBound;
                        upperInclusive = set.UpperBoundInclusive;
                    }
                    else // cmp == 0
                    {
                        // Rare equality case
                        upper = set.UpperBound;
                        upperInclusive = set.UpperBoundInclusive && inclusive;
                    }
                }

                return new TreeSubSet(set.UnderlyingSet, set.LowerBound, set.LowerBoundInclusive, upper, upperInclusive, set.HasLowerBound, true, set.IsReversed);
            }

            #endregion GetViewBefore

            #region GetViewAfter

            /// <summary>
            /// Returns a view of a subset in a <see cref="SortedSet{T}"/> with no upper bound.
            /// </summary>
            /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <remarks>
            /// This method returns a view of the range of elements that fall after <paramref name="fromItem"/>
            /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
            /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
            /// <para/>
            /// This corresponds to the <c>tailSet()</c> method in the JDK.
            /// </remarks>
            public SortedSet<T> GetViewAfter(ReadOnlySpan<TAlternateSpan> fromItem)
                => DoGetViewAfter(fromItem, inclusive: true, ExceptionArgument.fromItem);

            /// <summary>
            /// Returns a view of a subset in a <see cref="SortedSet{T}"/> with no upper bound.
            /// </summary>
            /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
            /// <param name="inclusive">If <see langword="true"/>, <paramref name="fromItem"/> will be included in the range;
            /// otherwise, it is an exclusive lower bound.</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <remarks>
            /// This method returns a view of the range of elements that fall after <paramref name="fromItem"/>, as defined
            /// by the current view order and comparer. The lower bound may either be inclusive (<see langword="true"/>)
            /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>. This method
            /// does not copy elements from the <see cref="SortedSet{T}"/>, but provides a window into the underlying
            /// <see cref="SortedSet{T}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
            /// <para/>
            /// This corresponds to the <c>tailSet()</c> method in the JDK.
            /// </remarks>
            public SortedSet<T> GetViewAfter(ReadOnlySpan<TAlternateSpan> fromItem, bool inclusive)
                => DoGetViewAfter(fromItem, inclusive, ExceptionArgument.fromItem);

            internal SortedSet<T> DoGetViewAfter(ReadOnlySpan<TAlternateSpan> toItem, bool inclusive, ExceptionArgument fromArgumentName)
            {
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                return _isUnderlying
                    ? DoGetViewAfter(toItem, inclusive, fromArgumentName, comparer)
                    : DoGetViewAfter_View(toItem, inclusive, fromArgumentName, comparer);
            }

            internal SortedSet<T> DoGetViewAfter(ReadOnlySpan<TAlternateSpan> fromItem, bool inclusive, ExceptionArgument fromArgumentName, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                if (!TryGetValue(fromItem, out T? lower))
                {
                    lower = comparer.Create(fromItem);
                }

                SortedSet<T> set = Set;
                return new TreeSubSet(set.UnderlyingSet, lower, inclusive, default, true, true, false, set.IsReversed);
            }

            internal SortedSet<T> DoGetViewAfter_View(ReadOnlySpan<TAlternateSpan> fromItem, bool inclusive, ExceptionArgument fromArgumentName, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                if (!IsWithinRange(fromItem, inclusive, comparer))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(fromArgumentName);
                }

                return !Set.IsReversed
                    ? GetViewAfterCore_View(fromItem, inclusive, comparer)
                    : GetViewBeforeCore_View(fromItem, inclusive, comparer);
            }

            internal SortedSet<T> GetViewAfterCore_View(ReadOnlySpan<TAlternateSpan> fromItem, bool inclusive, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;
                T? lower;
                bool lowerInclusive;

                if (!set.HasLowerBound)
                {
                    if (!TryGetValue(fromItem, out lower))
                        lower = comparer.Create(fromItem);
                    lowerInclusive = inclusive;
                }
                else
                {
                    // Compute comparison ONCE
                    int cmp = comparer.Compare(fromItem, set.LowerBound!);
                    if (cmp > 0)
                    {
                        // Override with new value
                        if (!TryGetValue(fromItem, out lower))
                            lower = comparer.Create(fromItem);
                        lowerInclusive = inclusive;
                    }
                    else if (cmp < 0)
                    {
                        // Clipped by lower bound
                        lower = set.LowerBound;
                        lowerInclusive = set.LowerBoundInclusive;
                    }
                    else // cmp == 0
                    {
                        // Rare equality case
                        lower = set.LowerBound;
                        lowerInclusive = set.LowerBoundInclusive && inclusive;
                    }
                }

                return new TreeSubSet(set.UnderlyingSet, lower, lowerInclusive, set.UpperBound, set.UpperBoundInclusive, true, set.HasUpperBound, set.IsReversed);
            }

            #endregion GetViewAfter

            #region FindNode

            [MethodImpl(MethodImplOptions.NoInlining)]
            private Node? FindNode_View(ReadOnlySpan<TAlternateSpan> item)
            {
                SortedSet<T> set = Set;
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                if (!IsWithinRange(item, comparer))
                {
                    return null;
                }

                set.VersionCheck();

#if DEBUG
                Debug.Assert(set.versionUpToDate() && set.root == set.UnderlyingSet.FindRange(set.LowerBound, set.UpperBound, set.LowerBoundInclusive, set.UpperBoundInclusive, set.HasLowerBound, set.HasUpperBound));
#endif
                return FindNode(item, comparer);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Node? FindNode(ReadOnlySpan<TAlternateSpan> item)
            {
                ISpanAlternateComparer<TAlternateSpan, T> comparer = GetAlternateComparer();

                return FindNode(item, comparer);
            }

            /// <summary>Finds the item in the set and returns a reference to the found item, or a null reference if not found.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private Node? FindNode(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                Node? current = Set.root;

                while (current != null)
                {
                    int order = comparer.Compare(item, current.Item);
                    if (order == 0)
                    {
                        return current;
                    }

                    current = order < 0 ? current.Left : current.Right;
                }

                return null;
            }

            #endregion

            #region Bounds Checking

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool IsWithinRange(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;

                // Check whether too low
                if (set.HasLowerBound)
                {
                    int c = comparer.Compare(item, set.LowerBound!);
                    if (c < 0 || (c == 0 && !set.LowerBoundInclusive))
                        return false;
                }

                // Check whether too high
                if (set.HasUpperBound)
                {
                    int c = comparer.Compare(item, set.UpperBound!);
                    if (c > 0 || (c == 0 && !set.UpperBoundInclusive))
                        return false;
                }

                return true;
            }

            private bool IsWithinRange(ReadOnlySpan<TAlternateSpan> item, bool inclusive, ISpanAlternateComparer<TAlternateSpan, T> comparer)
                => inclusive ? IsWithinRange(item, comparer) : IsWithinClosedRange(item, comparer);


            private bool IsWithinClosedRange(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;

                if (set.HasLowerBound)
                {
                    if (comparer.Compare(item!, set.LowerBound!) < 0)
                        return false;
                }

                if (set.HasUpperBound)
                {
                    if (comparer.Compare(item!, set.UpperBound!) > 0)
                        return false;
                }

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool IsTooHigh(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;

                if (set.HasUpperBound)
                {
                    int c = comparer.Compare(item!, set.UpperBound!);
                    if (c > 0 || (c == 0 && !set.UpperBoundInclusive))
                        return true;
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool IsTooLow(ReadOnlySpan<TAlternateSpan> item, ISpanAlternateComparer<TAlternateSpan, T> comparer)
            {
                SortedSet<T> set = Set;

                if (set.HasLowerBound)
                {
                    int c = comparer.Compare(item, set.LowerBound!);
                    if (c < 0 || (c == 0 && !set.LowerBoundInclusive))
                        return true;
                }
                return false;
            }

            #endregion Bounds Checking
        }

        #endregion SpanAlternateLookup

        #region IEnumerable<T> members

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <returns>An enumerator that iterates through the <see cref="SortedSet{T}"/> in sorted order.</returns>
        /// <remarks>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to
        /// the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably
        /// invalidated and the next call to <see cref="Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/>
        /// throws an <see cref="InvalidOperationException"/>.
        /// <para/>
        /// This method is an <c>O(log n)</c> operation.
        /// </remarks>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this, IsReversed); // J2N: Pass through the IsReversed property to align the enumeration order with the view

        internal Enumerator GetEnumeratorInternal(bool reverse) => new Enumerator(this, IsReversed ^ reverse);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Tree-specific operations

        // After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
        // It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
        // need to split again in the next node.
        // By the time we need to split again, everything will be correctly set.
        private void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(grandParent != null);

            bool parentIsOnRight = grandParent!.Right == parent;
            bool currentIsOnRight = parent!.Right == current;

            Node newChildOfGreatGrandParent;
            if (parentIsOnRight == currentIsOnRight)
            {
                // Same orientation, single rotation
                newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft() : grandParent.RotateRight();
            }
            else
            {
                // Different orientation, double rotation
                newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight() : grandParent.RotateRightLeft();
                // Current node now becomes the child of `greatGrandParent`
                parent = greatGrandParent;
            }

            // `grandParent` will become a child of either `parent` of `current`.
            grandParent.ColorRed();
            newChildOfGreatGrandParent.ColorBlack();

            ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
        }

        /// <summary>
        /// Replaces the child of a parent node, or replaces the root if the parent is <c>null</c>.
        /// </summary>
        /// <param name="parent">The (possibly <c>null</c>) parent.</param>
        /// <param name="child">The child node to replace.</param>
        /// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
        private void ReplaceChildOrRoot(Node? parent, Node child, Node newChild)
        {
            if (parent != null)
            {
                parent.ReplaceChild(child, newChild);
            }
            else
            {
                root = newChild;
            }
        }

        /// <summary>
        /// Replaces the matching node with its successor.
        /// </summary>
        private void ReplaceNode(Node match, Node parentOfMatch, Node successor, Node parentOfSuccessor)
        {
            Debug.Assert(match != null);

            if (successor == match)
            {
                // This node has no successor. This can only happen if the right child of the match is null.
                Debug.Assert(match.Right == null);
                successor = match.Left!;
            }
            else
            {
                Debug.Assert(parentOfSuccessor != null);
                Debug.Assert(successor.Left == null);
                Debug.Assert((successor.Right == null && successor.IsRed) || (successor.Right!.IsRed && successor.IsBlack));

                successor.Right?.ColorBlack();

                if (parentOfSuccessor != match)
                {
                    // Detach the successor from its parent and set its right child.
                    parentOfSuccessor!.Left = successor.Right;
                    successor.Right = match!.Right;
                }

                successor.Left = match!.Left;
            }

            if (successor != null)
            {
                successor.Color = match.Color;
            }

            ReplaceChildOrRoot(parentOfMatch, match, successor!);
        }

        internal virtual Node? FindNode(T item)
        {
            Node? current = root;
            while (current != null)
            {
                int order = comparer.Compare(item, current.Item);
                if (order == 0)
                {
                    return current;
                }

                current = order < 0 ? current.Left : current.Right;
            }

            return null;
        }

        /// <summary>
        /// Searches for an item and returns its zero-based index in this set.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item's zero-based index in this set, or -1 if it isn't found.</returns>
        /// <remarks>
        /// <para>
        /// This implementation is based off of http://en.wikipedia.org/wiki/Binary_Tree#Methods_for_storing_binary_trees.
        /// </para>
        /// <para>
        /// This method is used with the <see cref="BitHelper"/> class. Note that this implementation is
        /// completely different from <see cref="TreeSubSet"/>'s, and that the two should not be mixed.
        /// </para>
        /// </remarks>
        internal virtual int InternalIndexOf(T item)
        {
            Node? current = root;
            int count = 0;
            while (current != null)
            {
                int order = comparer.Compare(item, current.Item);
                if (order == 0)
                {
                    return count;
                }

                current = order < 0 ? current.Left : current.Right;
                count = order < 0 ? (2 * count + 1) : (2 * count + 2);
            }

            return -1;
        }

        internal Node? FindRange(T? from, T? to) => FindRange(from, to, lowerBoundInclusive: true, upperBoundInclusive: true, lowerBoundActive: true, upperBoundActive: true);

        internal Node? FindRange(T? from, T? to, bool lowerBoundInclusive, bool upperBoundInclusive, bool lowerBoundActive, bool upperBoundActive)
        {
            Node? current = root;
            while (current != null)
            {
                int comp;
                if (lowerBoundActive && ((comp = comparer.Compare(from!, current.Item!)) > 0 || !lowerBoundInclusive && comp == 0))
                {
                    current = current.Right;
                }
                else
                {
                    if (upperBoundActive && ((comp = comparer.Compare(to!, current.Item!)) < 0 || !upperBoundInclusive && comp == 0))
                    {
                        current = current.Left;
                    }
                    else
                    {
                        return current;
                    }
                }
            }

            return null;
        }

        // J2N: We need to override for views to ensure the underlying set version is updated
        internal virtual void UpdateVersion() => ++version;

        /// <summary>
        /// Returns an <see cref="IEqualityComparer{T}"/> object that can be used to create a collection that contains individual sets.
        /// </summary>
        public static IEqualityComparer<SortedSet<T>> CreateSetComparer() => CreateSetComparer(memberEqualityComparer: null);

        /// <summary>
        /// Returns an <see cref="IEqualityComparer{T}"/> object, according to a specified comparer, that can be used to create a collection that contains individual sets.
        /// </summary>
        public static IEqualityComparer<SortedSet<T>> CreateSetComparer(IEqualityComparer<T>? memberEqualityComparer)
        {
            return new SortedSetEqualityComparer<T>(memberEqualityComparer);
        }

        /// <summary>
        /// Decides whether two sets have equal contents, using a fallback comparer if the sets do not have equivalent equality comparers.
        /// </summary>
        /// <param name="set1">The first set.</param>
        /// <param name="set2">The second set.</param>
        /// <param name="comparer">The fallback comparer to use if the sets do not have equal comparers.</param>
        /// <returns><c>true</c> if the sets have equal contents; otherwise, <c>false</c>.</returns>
        internal static bool SortedSetEquals(SortedSet<T>? set1, SortedSet<T>? set2, IComparer<T> comparer)
        {
            if (set1 == null)
            {
                return set2 == null;
            }

            if (set2 == null)
            {
                Debug.Assert(set1 != null);
                return false;
            }

            if (ComparerEquals(set1.Comparer, set2.Comparer))
            {
                return set1.Count == set2.Count && set1.SetEquals(set2);
            }

            bool found = false;
            foreach (T item1 in set1)
            {
                found = false;
                foreach (T item2 in set2)
                {
                    if (comparer.Compare(item1, item2) == 0)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether two <see cref="IComparer{T}"/> instances are equal.
        /// </summary>
        /// <param name="a">The first <see cref="IComparer{T}"/>.</param>
        /// <param name="b">The second <see cref="IComparer{T}"/>.</param>
        /// <returns>A value indicating whether both comparers are equal.</returns>
        // J2N NOTE: This is equivalent to HasEqualComparer() in the BCL, but allows for comparing on any collection type.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ComparerEquals(IComparer<T> a, IComparer<T>? b)
            // Commonly, both comparers will be the default comparer (and reference-equal). Avoid a virtual method call to Equals() in that case.
            => a == b || a.Equals(b);

        /// <summary>
        /// Determines whether an <see cref="IComparer{T}"/> is equivalent to an <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="a">The first <see cref="IComparer{T}"/>.</param>
        /// <param name="b">The second <see cref="IComparer{T}"/>.</param>
        /// <returns>A value indicating whether both comparers are equal.</returns>
        private static bool ComparerEquals(IComparer<T> a, IEqualityComparer<T>? b)
        {
            // Commonly, both comparers will be the default comparer (and reference-equal). Avoid a virtual method call to Equals() in that case.
            if (a == b)
                return true;

            // Currently, only StringComparer instances can be compared for equivalence.
            if (typeof(T) == typeof(string) && a is StringComparer sca && b is StringComparer scb)
                return a.Equals(b);

            return false;
        }

        #endregion

        #region ISet members

        /// <summary>
        /// Modifies the current <see cref="SortedSet{T}"/> object so that it contains all
        /// elements that are present in either the current object or the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Any duplicate elements in <paramref name="other"/> are ignored.
        /// <para/>
        /// If the collection represented by <paramref name="other"/> is a <see cref="SortedSet{T}"/>,
        /// <see cref="SCG.SortedSet{T}"/>, <see cref="IDistinctSortedCollection{T}"/>, or
        /// <see cref="ISortedCollection{T}"/> with the same equality comparer as the current <see cref="SortedSet{T}"/> object,
        /// this is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the <see cref="Count"/> of the current set
        /// and <c>m</c> is the <see cref="SCG.ICollection{T}.Count"/> of <paramref name="other"/>. Otherwise,
        /// this is an O(<c>m</c> log <c>n</c>) operation.
        /// </remarks>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // Other is already the empty set; return.
            if (other is ICollection<T> genericCollection)
            {
                if (genericCollection.Count == 0)
                    return;
            }
            else if (other is ICollection c)
            {
                if (c.Count == 0)
                    return;
            }

            IDistinctSortedCollection<T>? asSorted;
            if (other is not SCG.SortedSet<T> otherSortedSet)
                asSorted = other as IDistinctSortedCollection<T>;
            else
                asSorted = new BclSortedSetAdapter(otherSortedSet);

            bool thisIsView = this is ICollectionView thisView && thisView.IsView;

            if (thisIsView)
                VersionCheck();

            // If asSorted is null, this is not a distinct sorted collection.
            if (asSorted is null && !thisIsView && other is ISortedCollection<T> otherSortedNonDistinct)
            {
                UnionWithSortedNonDistinctCollection(otherSortedNonDistinct);
                return;
            }

            if (asSorted != null && !thisIsView && Count == 0)
            {
                SortedSet<T> dummy = new SortedSet<T>(asSorted, UnderlyingSet.Comparer);
                root = dummy.root;
                count = dummy.count;
                version++;
                return;
            }

            IComparer<T>? comparer;

            // This actually hurts if N is much greater than M. The / 2 is arbitrary.
            if (asSorted != null && !thisIsView && ComparerEquals(comparer = Comparer, asSorted.Comparer) && (asSorted.Count > this.Count / 2))
            {
                // First do a merge sort to an array.
                T[] merged = new T[asSorted.Count + this.Count];
                int c = 0;
                IEnumerator<T> mine = this.GetEnumerator();
                IEnumerator<T> theirs = asSorted.GetEnumerator();
                bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
                while (!mineEnded && !theirsEnded)
                {
                    int comp = comparer.Compare(mine.Current, theirs.Current);
                    if (comp < 0)
                    {
                        merged[c++] = mine.Current;
                        mineEnded = !mine.MoveNext();
                    }
                    else if (comp == 0)
                    {
                        merged[c++] = theirs.Current;
                        mineEnded = !mine.MoveNext();
                        theirsEnded = !theirs.MoveNext();
                    }
                    else
                    {
                        merged[c++] = theirs.Current;
                        theirsEnded = !theirs.MoveNext();
                    }
                }

                if (!mineEnded || !theirsEnded)
                {
                    IEnumerator<T> remaining = (mineEnded ? theirs : mine);
                    do
                    {
                        merged[c++] = remaining.Current;
                    }
                    while (remaining.MoveNext());
                }

                // now merged has all c elements

                // safe to gc the root, we  have all the elements
                root = null;

                EnsureTreeOrder(merged, c); // J2N: ensure the order matches the underlying tree if we have a reverse view
                root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
                count = c;
                version++;
            }
            else
            {
                AddAllElements(other);
            }
        }

        private void UnionWithSortedNonDistinctCollection(ISortedCollection<T> other)
        {
            Debug.Assert(other != null);

            if (Count ==  0)
            {
                SortedSet<T> dummy = new SortedSet<T>(other!, UnderlyingSet.Comparer);
                root = dummy.root;
                count = dummy.count;
                version++;
                return;
            }

            IComparer<T>? comparer;

            if (ComparerEquals(comparer = Comparer, other!.Comparer) && (other.Count > this.Count / 2))
            {
                MergeWithSortedNonDistinctCollection(other!, comparer);
                return;
            }

            AddAllElements(other!);
        }

        private void MergeWithSortedNonDistinctCollection(ISortedCollection<T> other, IComparer<T> comparer)
        {
            // Preconditions already checked:
            // - treeSubset == null
            // - comparer equality
            // - other.Count > this.Count / 2

            T[] merged = new T[other.Count + this.Count];
            int c = 0;

            IEnumerator<T> mine = this.GetEnumerator();
            IEnumerator<T> theirs = other.GetEnumerator();

            bool mineEnded = !mine.MoveNext();
            bool theirsEnded = !theirs.MoveNext();

            bool hasPrevOther = false;
            T prevOther = default!;

            while (!mineEnded && !theirsEnded)
            {
                T theirsCurrent = theirs.Current;

                // Collapse duplicates in "other"
                if (hasPrevOther && comparer.Compare(prevOther, theirsCurrent) == 0)
                {
                    theirsEnded = !theirs.MoveNext();
                    continue;
                }

                int comp = comparer.Compare(mine.Current, theirsCurrent);

                if (comp < 0)
                {
                    merged[c++] = mine.Current;
                    mineEnded = !mine.MoveNext();
                }
                else if (comp == 0)
                {
                    merged[c++] = theirsCurrent;
                    mineEnded = !mine.MoveNext();

                    prevOther = theirsCurrent;
                    hasPrevOther = true;
                    theirsEnded = !theirs.MoveNext();
                }
                else
                {
                    merged[c++] = theirsCurrent;
                    prevOther = theirsCurrent;
                    hasPrevOther = true;
                    theirsEnded = !theirs.MoveNext();
                }
            }

            // Remaining elements
            if (!mineEnded)
            {
                do
                {
                    merged[c++] = mine.Current;
                }
                while (mine.MoveNext());
            }
            else if (!theirsEnded)
            {
                do
                {
                    T current = theirs.Current;

                    if (!hasPrevOther || comparer.Compare(prevOther, current) != 0)
                    {
                        merged[c++] = current;
                        prevOther = current;
                        hasPrevOther = true;
                    }
                }
                while (theirs.MoveNext());
            }

            // Replace tree
            root = null;
            EnsureTreeOrder(merged, c); // J2N: ensure the order matches the underlying tree if we have a reverse view
            root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
            count = c;
            version++;
        }

        private static Node? ConstructRootFromSortedArray(ReadOnlySpan<T> arr, int startIndex, int endIndex, Node? redNode)
        {
            // You're given a sorted array... say 1 2 3 4 5 6
            // There are 2 cases:
            // -  If there are odd # of elements, pick the middle element (in this case 4), and compute
            //    its left and right branches
            // -  If there are even # of elements, pick the left middle element, save the right middle element
            //    and call the function on the rest
            //    1 2 3 4 5 6 -> pick 3, save 4 and call the fn on 1,2 and 5,6
            //    now add 4 as a red node to the lowest element on the right branch
            //             3                       3
            //         1       5       ->     1        5
            //           2       6             2     4   6
            //    As we're adding to the leftmost of the right branch, nesting will not hurt the red-black properties
            //    Leaf nodes are red if they have no sibling (if there are 2 nodes or if a node trickles
            //    down to the bottom

            // This is done recursively because the iterative way to do this ends up wasting more space than it saves in stack frames
            // Only some base cases are handled below.

            int size = endIndex - startIndex + 1;
            Node root;

            switch (size)
            {
                case 0:
                    return null;
                case 1:
                    root = new Node(arr[startIndex], NodeColor.Black);
                    if (redNode != null)
                    {
                        root.Left = redNode;
                    }
                    break;
                case 2:
                    root = new Node(arr[startIndex], NodeColor.Black);
                    root.Right = new Node(arr[endIndex], NodeColor.Black);
                    root.Right.ColorRed();
                    if (redNode != null)
                    {
                        root.Left = redNode;
                    }
                    break;
                case 3:
                    root = new Node(arr[startIndex + 1], NodeColor.Black);
                    root.Left = new Node(arr[startIndex], NodeColor.Black);
                    root.Right = new Node(arr[endIndex], NodeColor.Black);
                    if (redNode != null)
                    {
                        root.Left.Left = redNode;
                    }
                    break;
                default:
                    int midpt = ((startIndex + endIndex) / 2);
                    root = new Node(arr[midpt], NodeColor.Black);
                    root.Left = ConstructRootFromSortedArray(arr, startIndex, midpt - 1, redNode);
                    root.Right = size % 2 == 0 ?
                        ConstructRootFromSortedArray(arr, midpt + 2, endIndex, new Node(arr[midpt + 1], NodeColor.Red)) :
                        ConstructRootFromSortedArray(arr, midpt + 1, endIndex, null);
                    break;

            }

            return root;
        }

        private bool TryGetSortedItems(IEnumerable<T> source, out T[] items, out int count)
        {
            items = null!;
            count = 0;

            if (TryGetDistinctSortedCollection(source, out IDistinctSortedCollection<T>? sortedSet))
            {
                if (ComparerEquals(Comparer, sortedSet.Comparer))
                {
                    items = EnumerableHelpers.ToArray(source, out count);
                    return true;
                }
                return false;
            }

            if (TryGetSortedCollection(source, out ISortedCollection<T>? sortedCollection))
            {
                IComparer<T>? comparer = Comparer;
                if (ComparerEquals(comparer, sortedCollection.Comparer))
                {
                    items = EnumerableHelpers.ToDistinctArray(source, out count, comparer);
                    return true;
                }
                return false;
            }

            return false;
        }

        private static bool TryGetDistinctSortedCollection(IEnumerable<T> source, [MaybeNullWhen(false)] out IDistinctSortedCollection<T> distinctSortedCollection)
        {
            if (source is IDistinctSortedCollection<T> sorted)
            {
                distinctSortedCollection = sorted;
                return true;
            }

            if (source is SCG.SortedSet<T> bclSortedSet)
            {
                distinctSortedCollection = new BclSortedSetAdapter(bclSortedSet);
                return true;
            }

            // J2N TODO: Add support for SCG.SortedDictionary<T,>.KeyCollection
            distinctSortedCollection = default;
            return false;
        }

        private static bool TryGetSortedCollection(IEnumerable<T> source, [MaybeNullWhen(false)] out ISortedCollection<T> sortedCollection)
        {
            if (source is ISortedCollection<T> sorted)
            {
                sortedCollection = sorted;
                return true;
            }

            sortedCollection = default;

            // J2N TODO: Add support for SCG.SortedDictionary<,T>.ValueCollection
            //Type type = source.GetType();
            //if (!type.IsGenericType)
            //    return false;

            //Type def = type.GetGenericTypeDefinition();

            //// SortedDictionary values
            //if (def == typeof(SCG.SortedDictionary<,>.ValueCollection) &&
            //    ComparerEquals(Comparer, Comparer<T>.Default))
            //{
            //    return true;
            //}

            return false;
        }

        private sealed class BclSortedSetAdapter : IDistinctSortedCollection<T>
        {
            private readonly SCG.SortedSet<T> set;

            public BclSortedSetAdapter(SCG.SortedSet<T> sortedSet)
            {
                Debug.Assert(sortedSet != null);
                this.set = sortedSet!; // [!] asserted above
            }

            public int Count => set.Count;

            bool ICollection<T>.IsReadOnly => ((ICollection<T>)set).IsReadOnly;

            public IComparer<T> Comparer => set.Comparer;

            public void Add(T item) => set.Add(item);

            public void Clear() => set.Clear();

            public bool Contains(T item) => set.Contains(item);

            public void CopyTo(T[] array, int index) => set.CopyTo(array, index);

            public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

            public bool Remove(T item) => set.Remove(item);

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)set).GetEnumerator();
        }

        /// <summary>
        /// Modifies the current <see cref="SortedSet{T}"/> object so that it contains only elements
        /// that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method ignores any duplicate elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by the <paramref name="other"/> parameter is a <see cref="SortedSet{T}"/>,
        /// <see cref="SCG.SortedSet{T}"/>, <see cref="IDistinctSortedCollection{T}"/> with the same equality comparer
        /// as the current <see cref="SortedSet{T}"/> object, this is an O(<c>n</c>) operation.
        /// If the collection is an <see cref="ISortedCollection{T}"/> with the same equality comparer as the current
        /// <see cref="SortedSet{T}"/> object, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c>
        /// is <see cref="Count"/> and <c>m</c> is the number of elements in <paramref name="other"/>. Otherwise,
        /// this method is an O(<c>m</c> * log <c>n</c> + <c>k</c> * log <c>k</c>) operation, where <c>k</c> is the
        /// number of elements in the intersection. For range-restricted views, this method falls
        /// back to incremental insertion per element to preserve range validation and projection invariants.
        /// </remarks>
        public virtual void IntersectWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (Count == 0)
                return;

            // If other is empty, we can simply call Clear() to save some steps.
            if (other is ICollection<T> genericCollection)
            {
                if (genericCollection.Count == 0)
                {
                    Clear();
                    return;
                }
            }
            else if (other is ICollection c)
            {
                if (c.Count == 0)
                {
                    Clear();
                    return;
                }
            }

            if (other == this)
                return;

            // HashSet<T> optimizations can't be done until equality comparers and comparers are related

            IDistinctSortedCollection<T>? asSorted;
            if (other is not SCG.SortedSet<T> otherSortedSet)
                asSorted = other as IDistinctSortedCollection<T>;
            else
                asSorted = new BclSortedSetAdapter(otherSortedSet);

            bool thisIsView = this is ICollectionView view && view.IsView;

            if (thisIsView)
                VersionCheck();

            // If asSorted is null, this is not a distinct sorted collection.
            if (asSorted == null && !thisIsView && other is ISortedCollection<T> otherSortedNonDistinct)
            {
                IntersectWithSortedNonDistinctCollection(otherSortedNonDistinct);
                return;
            }

            IComparer<T>? comparer;
            if (asSorted != null && !thisIsView && ComparerEquals(comparer = Comparer, asSorted.Comparer))
            {
                // First do a merge sort to an array.
                T[] merged = new T[Math.Min(this.Count, asSorted.Count)];
                int c = 0;
                IEnumerator<T> mine = this.GetEnumerator();
                IEnumerator<T> theirs = asSorted.GetEnumerator();
                bool mineEnded = !mine.MoveNext(), theirsEnded = !theirs.MoveNext();
                T? last = MaxInternal;

                while (!mineEnded && !theirsEnded && comparer.Compare(theirs.Current!, last!) <= 0)
                {
                    int comp = comparer.Compare(mine.Current, theirs.Current);
                    if (comp < 0)
                    {
                        mineEnded = !mine.MoveNext();
                    }
                    else if (comp == 0)
                    {
                        merged[c++] = theirs.Current;
                        mineEnded = !mine.MoveNext();
                        theirsEnded = !theirs.MoveNext();
                    }
                    else
                    {
                        theirsEnded = !theirs.MoveNext();
                    }
                }

                // now merged has all c elements

                // safe to gc the root, we  have all the elements
                root = null;

                EnsureTreeOrder(merged, c); // J2N: ensure the order matches the underlying tree if we have a reverse view
                root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
                count = c;
                version++;
            }
            else
            {
                IntersectWithEnumerable(other);
            }
        }

        private void IntersectWithSortedNonDistinctCollection(ISortedCollection<T> other)
        {
            Debug.Assert(other != null);
            Debug.Assert(Count > 0);

            IComparer<T> comparer = Comparer;
            if (ComparerEquals(comparer, other!.Comparer)) // [!] asserted above
            {
                // Result cannot exceed the smaller of this.Count or other.Count
                T[] merged = new T[Math.Min(this.Count, other.Count)];
                int c = 0;

                IEnumerator<T> mine = this.GetEnumerator();
                IEnumerator<T> theirs = other!.GetEnumerator(); // [!] asserted above

                bool mineEnded = !mine.MoveNext();
                bool theirsEnded = !theirs.MoveNext();

                bool hasPrevOther = false;
                T prevOther = default!;

                // Optional pruning using Last
                T last = MaxInternal!;

                while (!mineEnded && !theirsEnded &&
                       comparer.Compare(theirs.Current, last) <= 0)
                {
                    T theirsCurrent = theirs.Current;

                    // Collapse duplicates in "other"
                    if (hasPrevOther && comparer.Compare(theirsCurrent, prevOther) == 0)
                    {
                        theirsEnded = !theirs.MoveNext();
                        continue;
                    }

                    int comp = comparer.Compare(mine.Current, theirsCurrent);

                    if (comp < 0)
                    {
                        mineEnded = !mine.MoveNext();
                    }
                    else if (comp == 0)
                    {
                        merged[c++] = mine.Current;

                        mineEnded = !mine.MoveNext();

                        prevOther = theirsCurrent;
                        hasPrevOther = true;
                        theirsEnded = !theirs.MoveNext();
                    }
                    else
                    {
                        prevOther = theirsCurrent;
                        hasPrevOther = true;
                        theirsEnded = !theirs.MoveNext();
                    }
                }

                // Rebuild tree from merged intersection
                root = null;
                EnsureTreeOrder(merged, c); // J2N: ensure the order matches the underlying tree if we have a reverse view
                root = ConstructRootFromSortedArray(merged, 0, c - 1, null);
                count = c;
                version++;
            }
            else
            {
                IntersectWithEnumerable(other);
            }
        }

        // J2N: Optimized fallback to reduce allocations and tree noise with Add()
        internal virtual void IntersectWithEnumerable(IEnumerable<T> other)
        {
            int? otherCount = null;
            if (other is ICollection<T> genericCollection)
            {
                otherCount = genericCollection.Count;
            }
            else if (other is ICollection c)
            {
                otherCount = c.Count;
            }

            // J2N: Clear() is unnecessary here because we do it at the top of IntersectWith()
            // So we should never reach here if otherCount is 0 (if other is a collection)
            Debug.Assert(otherCount is null || otherCount > 0);

            List<T> toSave = new(otherCount.HasValue ? Math.Min(this.Count, otherCount.Value) : this.Count);
            foreach (T item in other)
            {
                if (Contains(item))
                {
                    toSave.Add(item);
                }
            }

            bool thisIsView = this is ICollectionView view && view.IsView;

            if (!thisIsView)
            {
                int count = toSave.Count;

                if (count > 0)
                {
                    T[] elements = toSave._items;
                    Array.Sort(elements, 0, count, comparer);

                    // Overwrite duplicates while shifting the distinct elements towards
                    // the front of the array.
                    int index = 1;
                    for (int i = 1; i < count; i++)
                    {
                        if (comparer.Compare(elements[i], elements[i - 1]) != 0)
                        {
                            elements[index++] = elements[i];
                        }
                    }

                    count = index;
                    root = null;
                    root = ConstructRootFromSortedArray(elements, 0, count - 1, null);
                    this.count = count;
                    version++;
                }
                else
                {
                    Clear();
                }
            }
            else
            {
                // J2N: Rebuilding the root of a TreeSubSet is not valid, as the underlying set may
                // have elements outside of the view. So, we must use Clear() and Add() to ensure validity.
                Clear();
                foreach (T item in toSave)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Removes all elements that are in a specified collection from the current <see cref="SortedSet{T}"/> object.
        /// </summary>
        /// <param name="other">The collection of items to remove from the <see cref="SortedSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method removes any element in the current <see cref="SortedSet{T}"/> that is also in <paramref name="other"/>.
        /// Duplicate values in <paramref name="other"/> are ignored.
        /// <para/>
        /// If the collection represented by the <paramref name="other"/> parameter is a <see cref="SortedSet{T}"/>,
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>, <see cref="SortedDictionary{TKey, TValue}"/>,
        /// <see cref="SCG.SortedSet{T}"/>, or <see cref="IDistinctSortedCollection{T}"/> with the same equality
        /// comparer as the current <see cref="SortedSet{T}"/> object, this method is an O(<c>k</c> log <c>n</c>)
        /// where <c>n</c> is <see cref="Count"/> and <c>k</c> is the number of elements in <paramref name="other"/>
        /// that fall within the range of the current set. Otherwise, this method is an O(<c>m</c> log <c>n</c>) operation,
        /// where <c>m</c> is the number of elements in the <paramref name="other"/> parameter.
        /// </remarks>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // J2N: Important - We call the Count property here on purpose rather than the count field
            // because it will do a version check and update the count if we are a TreeSubSet view.
            // We also call other.Count to ensure any other views are also up to date for the Min and Max
            // calls below.
            if (Count == 0)
                return;

            // Other is already the empty set; return.
            if (other is ICollection<T> genericCollection)
            {
                if (genericCollection.Count == 0)
                    return;
            }
            else if (other is ICollection c)
            {
                if (c.Count == 0)
                    return;
            }

            if (other == this)
            {
                Clear();
                return;
            }

            if (other is INavigableCollection<T> navigableCollection)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, navigableCollection.Comparer))
                {
                    // Outside range, no point in doing anything
                    if (navigableCollection.TryGetLast(out T? otherLast) &&
                        navigableCollection.TryGetFirst(out T? otherFirst) &&
                        comparer.Compare(otherLast, MinInternal!) >= 0 &&
                        comparer.Compare(otherFirst, MaxInternal!) <= 0)
                    {
                        T? first = MinInternal;
                        T? last = MaxInternal;
                        foreach (T item in other)
                        {
                            if (comparer.Compare(item!, first!) < 0)
                                continue;
                            if (comparer.Compare(item!, last!) > 0)
                                break;
                            Remove(item);
                        }
                    }
                    return;
                }
            }
            // J2N: RemoveAllElements() also uses Contains() to remove unnecessary calls to Remove() when there are duplicates in ISortedCollection<T>.
            else if (other is IDistinctSortedCollection<T> sortedCollection)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, sortedCollection.Comparer))
                {
                    T? first = MinInternal;
                    T? last = MaxInternal;
                    foreach (T item in other)
                    {
                        if (comparer.Compare(item!, first!) < 0)
                            continue;
                        if (comparer.Compare(item!, last!) > 0)
                            break;
                        Remove(item);
                    }
                    return;
                }
            }
            else if (other is SCG.SortedSet<T> bclSortedSet)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, bclSortedSet.Comparer))
                {
                    // Outside range, no point in doing anything
                    if (comparer.Compare(bclSortedSet.Max!, MinInternal!) >= 0 && comparer.Compare(bclSortedSet.Min!, MaxInternal!) <= 0)
                    {
                        T? first = MinInternal;
                        T? last = MaxInternal;
                        foreach (T item in bclSortedSet)
                        {
                            if (comparer.Compare(item!, first!) < 0)
                                continue;
                            if (comparer.Compare(item!, last!) > 0)
                                break;
                            Remove(item);
                        }
                    }
                    return;
                }
            }

            RemoveAllElements(other);
        }

        /// <summary>
        /// Modifies the current <see cref="SortedSet{T}"/> object so that it contains only elements
        /// that are present either in the current object or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Any duplicate elements in <paramref name="other"/> are ignored.
        /// <para/>
        /// If the other parameter is a <see cref="SortedSet{T}"/>, <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>,
        /// <see cref="IDistinctSortedCollection{T}"/> or <see cref="ISortedCollection{T}"/> with the same equality comparer as
        /// the current <see cref="SortedSet{T}"/> object, this method is an <c>O(n log m)</c> operation. Otherwise,
        /// this method is an <c>O(n log m) + O(n log n)</c> operation, where <c>n</c> is the number of elements
        /// in <paramref name="other"/> and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (Count == 0)
            {
                UnionWith(other);
                return;
            }

            if (other == this)
            {
                Clear();
                return;
            }

            if (other is IDistinctSortedCollection<T> distinctSortedCollection)
            {
                if (ComparerEquals(Comparer, distinctSortedCollection!.Comparer))
                {
                    SymmetricExceptWithSameComparer(distinctSortedCollection);
                    return;
                }
            }
            else if (other is ISortedCollection<T> sortedCollection)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, sortedCollection!.Comparer))
                {
                    SymmetricExceptWithSameComparer(sortedCollection, comparer);
                    return;
                }
            }

            int length;
            T[] elements = EnumerableHelpers.ToArray(other, out length);
            Array.Sort(elements, 0, length, comparer); // J2N: Always use forward comparer to match underlying tree
            SymmetricExceptWithSameComparer(elements, length);
        }

        private void SymmetricExceptWithSameComparer(IDistinctSortedCollection<T> other)
        {
            Debug.Assert(other != null);
            Debug.Assert(ComparerEquals(Comparer, other!.Comparer));

            if (other!.Count == 0) // [!] asserted above
            {
                return;
            }

            foreach (T item in other!)
            {
                SymmetricExceptWithValue(item);
            }
        }

        private void SymmetricExceptWithSameComparer(ISortedCollection<T> other, IComparer<T> comparer)
        {
            Debug.Assert(other != null);
            Debug.Assert(ComparerEquals(Comparer, other!.Comparer));

            if (other!.Count == 0) // [!] asserted above
            {
                return;
            }

            using var enumerator = other.GetEnumerator();
            bool hasNext = enumerator.MoveNext();
            Debug.Assert(hasNext);

            T current = enumerator.Current;
            while (true)
            {
                T next = default!;

                // Skip duplicates
                while (hasNext = enumerator.MoveNext())
                {
                    next = enumerator.Current;
                    if (comparer.Compare(next, current) != 0)
                        break;
                }

                // Process the current unique value
                SymmetricExceptWithValue(current);

                // If no more items, we are done.
                if (!hasNext)
                    break;

                current = next;
            }
        }

        private void SymmetricExceptWithSameComparer(T[] other, int count)
        {
            Debug.Assert(other != null);
            Debug.Assert(count >= 0 && count <= other!.Length);

            if (count == 0)
            {
                return;
            }

            T previous = other![0];
            for (int i = 0; i < count; i++)
            {
                while (i < count && i != 0 && comparer.Compare(other[i], previous) == 0)
                    i++;
                if (i >= count)
                    break;
                T current = other[i];
                SymmetricExceptWithValue(current);
                previous = current;
            }
        }

        internal virtual void SymmetricExceptWithValue(T item)
        {
            bool result = Contains(item) ? Remove(item) : Add(item);
            Debug.Assert(result);
        }

        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}"/> object is a subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <returns><c>true</c> if the current <see cref="SortedSet{T}"/> object is a subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>An empty set is a subset of any other collection, including an empty set; therefore, this method returns true
        /// if the collection represented by the current <see cref="SortedSet{T}"/> object is empty, even if the <paramref name="other"/> parameter is an empty set.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is greater than the number of elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by <paramref name="other"/> is a <see cref="SortedSet{T}"/>,
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>, or <see cref="SCG.SortedSet{T}"/> with the same equality comparer as the
        /// current <see cref="SortedSet{T}"/> object, this method is an O(<c>n</c>) operation. If the collection represented by <paramref name="other"/>
        /// is a <see cref="HashSet{T}"/>, <see cref="OrderedHashSet{T}"/>, <see cref="SCG.HashSet{T}"/>, or <see cref="ISortedCollection{T}"/> this method
        /// is an O(<c>n</c> log <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c> is the number of elements in <paramref name="other"/>.
        /// Otherwise, this method is an O(<c>n</c> log <c>n</c> + <c>m</c> log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/> and
        /// <c>m</c> is the number of elements in <paramref name="other"/>.
        /// </remarks>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (Count == 0)
            {
                return true;
            }

            // for sorted sets, sorted dictionaries, or sorted dictionary key collections with the same comparer
            if (other is INavigableCollection<T> navigableCollection)
            {
                if (ComparerEquals(Comparer, navigableCollection.Comparer))
                {
                    if (count > navigableCollection.Count)
                        return false;

                    return IsSubsetOfNavigableCollectionWithSameComparer(navigableCollection);
                }
            }
            else if (other is IDistinctSortedCollection<T> distinctSortedCollection)
            {
                if (ComparerEquals(Comparer, distinctSortedCollection.Comparer))
                {
                    if (count > distinctSortedCollection.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(distinctSortedCollection);
                }
            }
            else if (other is ISortedCollection<T> sortedCollection)
            {
                if (ComparerEquals(Comparer, sortedCollection.Comparer))
                {
                    // Non-distinct, so we cannot optimize on count here

                    return IsSubsetOfCollectionWithSameComparer(sortedCollection);
                }
            }
            else if (other is HashSet<T> hashSet)
            {
                if (ComparerEquals(Comparer, hashSet.EqualityComparer))
                {
                    if (count > hashSet.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(hashSet);
                }
            }
            else if (other is OrderedHashSet<T> orderedHashSet)
            {
                if (ComparerEquals(Comparer, orderedHashSet.EqualityComparer))
                {
                    if (count > orderedHashSet.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(orderedHashSet);
                }
            }
            else if (other is SCG.SortedSet<T> bclSortedSet)
            {
                if (ComparerEquals(Comparer, bclSortedSet.Comparer))
                {
                    if (count > bclSortedSet.Count)
                        return false;

                    return IsSubsetOfBclSortedSetWithSameComparer(bclSortedSet);
                }
            }
            else if (other is SCG.HashSet<T> bclHashSet)
            {
                if (ComparerEquals(Comparer, bclHashSet.Comparer))
                {
                    if (count > bclHashSet.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(bclHashSet);
                }
            }

            // Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
            ElementCount result = CheckUniqueAndUnfoundElements(other, false);
            return result.UniqueCount == Count && result.UnfoundCount >= 0;
        }

        private bool IsSubsetOfNavigableCollectionWithSameComparer(INavigableCollection<T> navigableCollection)
        {
            // J2N: If other is a view, GetView() may throw, so we fall back.
            if (navigableCollection is ICollectionView view && view.IsView)
                return IsSubsetOfCollectionWithSameComparer(navigableCollection);

            // J2N: We cannot make any assumptions about the whether the inclusivity of the other collection is the same as this one,
            // so we override it. The Contains() call will weed out the bounds if they are different.
            INavigableCollection<T> prunedOther = navigableCollection.GetView(MinInternal, fromInclusive: true, MaxInternal, toInclusive: true);
            foreach (T item in this)
            {
                if (!prunedOther.Contains(item))
                    return false;
            }
            return true;
        }

        private bool IsSubsetOfBclSortedSetWithSameComparer(SCG.SortedSet<T> bclSortedSet)
        {
            // J2N: If this is not exactly SCG.SortedSet<T>, we assume it is a view.
            // J2N: If other is a view, GetViewBetween() may throw, so we fall back.
            if (bclSortedSet.GetType() != typeof(SCG.SortedSet<T>))
                return IsSubsetOfCollectionWithSameComparer(bclSortedSet);

            SCG.SortedSet<T> prunedOther = bclSortedSet.GetViewBetween(LowerValue!, UpperValue!);
            foreach (T item in this)
            {
                if (!prunedOther.Contains(item))
                    return false;
            }
            return true;
        }

        private bool IsSubsetOfCollectionWithSameComparer(ICollection<T> collection)
        {
            foreach (T item in this)
            {
                if (!collection.Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}"/> object is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <returns><c>true</c> if the current <see cref="SortedSet{T}"/> object is a proper subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper subset of any other collection. Therefore, this method returns <c>true</c> if the
        /// collection represented by the current <see cref="SortedSet{T}"/> object is empty unless the other parameter
        /// is also an empty set.
        /// <para/>
        /// This method always returns false if <see cref="Count"/> is greater than or equal to the number of elements
        /// in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by <paramref name="other"/> is a <see cref="SortedSet{T}"/>,
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>, or <see cref="SCG.SortedSet{T}"/> with the same equality comparer as the
        /// current <see cref="SortedSet{T}"/> object, this method is an O(<c>n</c>) operation. If the collection represented by <paramref name="other"/>
        /// is a <see cref="HashSet{T}"/>, <see cref="OrderedHashSet{T}"/>, <see cref="SCG.HashSet{T}"/>, or <see cref="ISortedCollection{T}"/> this method
        /// is an O(<c>n</c> log <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c> is the number of elements in <paramref name="other"/>.
        /// Otherwise, this method is an O(<c>n</c> log <c>n</c> + <c>m</c> log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/> and
        /// <c>m</c> is the number of elements in <paramref name="other"/>.
        /// </remarks>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (other is ICollection<T> genericCollection)
            {
                if (Count == 0)
                    return genericCollection.Count > 0;
            }
            else if (other is ICollection c)
            {
                if (Count == 0)
                    return c.Count > 0;
            }

            // for sorted sets, sorted dictionaries, or sorted dictionary key collections with the same comparer
            if (other is INavigableCollection<T> navigableCollection)
            {
                if (ComparerEquals(Comparer, navigableCollection.Comparer))
                {
                    if (count >= navigableCollection.Count)
                        return false;

                    return IsSubsetOfNavigableCollectionWithSameComparer(navigableCollection);
                }
            }
            else if (other is IDistinctSortedCollection<T> distinctSortedCollection)
            {
                if (ComparerEquals(Comparer, distinctSortedCollection.Comparer))
                {
                    if (count >= distinctSortedCollection.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(distinctSortedCollection);
                }
            }
            else if (other is HashSet<T> hashSet)
            {
                if (ComparerEquals(Comparer, hashSet.EqualityComparer))
                {
                    if (count >= hashSet.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(hashSet);
                }
            }
            else if (other is OrderedHashSet<T> orderedHashSet)
            {
                if (ComparerEquals(Comparer, orderedHashSet.EqualityComparer))
                {
                    if (count >= orderedHashSet.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(orderedHashSet);
                }
            }
            else if (other is SCG.SortedSet<T> bclSortedSet)
            {
                if (ComparerEquals(Comparer, bclSortedSet.Comparer))
                {
                    if (count >= bclSortedSet.Count)
                        return false;

                    return IsSubsetOfBclSortedSetWithSameComparer(bclSortedSet);
                }
            }
            else if (other is SCG.HashSet<T> bclHashSet)
            {
                if (ComparerEquals(Comparer, bclHashSet.Comparer))
                {
                    if (count >= bclHashSet.Count)
                        return false;

                    return IsSubsetOfCollectionWithSameComparer(bclHashSet);
                }
            }

            // Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
            ElementCount result = CheckUniqueAndUnfoundElements(other, false);
            return result.UniqueCount == Count && result.UnfoundCount > 0;
        }

        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}"/> object is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <returns><c>true</c> if the current <see cref="SortedSet{T}"/> object is a superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// All collections, including the empty set, are supersets of the empty set. Therefore, this method returns
        /// <c>true</c> if the collection represented by the <paramref name="other"/> parameter is empty, even if
        /// the current <see cref="SortedSet{T}"/> object is empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than the number of elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by <paramref name="other"/> is a <see cref="SortedSet{T}"/>,
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>, or <see cref="SCG.SortedSet{T}"/> with the same equality comparer as the
        /// current <see cref="SortedSet{T}"/> object, this method is an O(<c>n</c>) operation. If the collection represented by <paramref name="other"/>
        /// is a <see cref="HashSet{T}"/>, <see cref="OrderedHashSet{T}"/>, <see cref="SCG.HashSet{T}"/>, or <see cref="ISortedCollection{T}"/> this method
        /// is an O(<c>n</c> log <c>m</c>) operation, where <c>n</c> is the number of elements in <paramref name="other"/> and <c>m</c> is <see cref="Count"/>.
        /// Otherwise, this method is an O(<c>n</c> log <c>n</c> + <c>m</c> log <c>n</c>) operation, where <c>n</c> is the number of elements in
        /// <paramref name="other"/> and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (other is ICollection<T> genericCollection)
            {
                if (genericCollection.Count == 0)
                    return true;
            }
            else if (other is ICollection c)
            {
                if (c.Count == 0)
                    return true;
            }

            if (other is INavigableCollection<T> navigableCollection)
            {
                if (ComparerEquals(Comparer, navigableCollection.Comparer))
                {
                    if (Count < navigableCollection.Count)
                        return false;

                    return IsSupersetOfNavigableCollectionWithSameComparer(navigableCollection);
                }
            }
            else if (other is IDistinctSortedCollection<T> distinctSortedCollection)
            {
                if (ComparerEquals(Comparer, distinctSortedCollection.Comparer))
                {
                    if (Count < distinctSortedCollection.Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(distinctSortedCollection);
                }
            }
            else if (other is ISortedCollection<T> sortedCollection)
            {
                if (ComparerEquals(Comparer, sortedCollection.Comparer))
                {
                    // Non-distinct, so we cannot optimize on count here

                    return IsSupersetOfEnumerableWithSameComparer(sortedCollection);
                }
            }
            else if (other is HashSet<T> hashSet)
            {
                if (ComparerEquals(Comparer, hashSet.EqualityComparer))
                {
                    if (Count < hashSet.Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(hashSet);
                }
            }
            else if (other is OrderedHashSet<T> orderedHashSet)
            {
                if (ComparerEquals(Comparer, orderedHashSet.EqualityComparer))
                {
                    if (Count < orderedHashSet.Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(orderedHashSet);
                }
            }
            else if (other is SCG.SortedSet<T> bclSortedSet)
            {
                if (ComparerEquals(Comparer, bclSortedSet.Comparer))
                {
                    if (Count < bclSortedSet.Count)
                        return false;

                    return IsSupersetOfBclSortedSetWithSameComparer(bclSortedSet);
                }
            }
            else if (other is SCG.HashSet<T> bclHashSet)
            {
                if (ComparerEquals(Comparer, bclHashSet.Comparer))
                {
                    if (Count < bclHashSet.Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(bclHashSet);
                }
            }

            // and a third for everything else
            return ContainsAllElements(other);
        }

        private bool IsSupersetOfNavigableCollectionWithSameComparer(INavigableCollection<T> navigableCollection)
        {
            // J2N: If this is a view, GetView() may throw, so we fall back.
            if (this is ICollectionView view && view.IsView)
                return IsSupersetOfEnumerableWithSameComparer(navigableCollection);

            if (!navigableCollection.TryGetFirst(out T? otherFirst) || !navigableCollection.TryGetLast(out T? otherLast))
            {
                // Other is empty, so we are done.
                return true;
            }

            // J2N: We cannot make any assumptions about the whether the inclusivity of this collection is the same as the other one,
            // so we use explicit bounds. The Contains() call will weed out the bounds if they are different.
            SortedSet<T> pruned = GetView(otherFirst, fromInclusive: true, otherLast, toInclusive: true);
            foreach (T item in navigableCollection)
            {
                if (!pruned.Contains(item))
                    return false;
            }
            return true;
        }

        private bool IsSupersetOfBclSortedSetWithSameComparer(SCG.SortedSet<T> bclSortedSet)
        {
            // J2N: If other is a view, GetView() may throw, so we fall back.
            if (this is ICollectionView view && view.IsView)
                return IsSupersetOfEnumerableWithSameComparer(bclSortedSet);

            SortedSet<T> pruned = GetView(bclSortedSet.Min, bclSortedSet.Max);
            foreach (T item in bclSortedSet)
            {
                if (!pruned.Contains(item))
                    return false;
            }
            return true;
        }

        private bool IsSupersetOfEnumerableWithSameComparer(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                if (!Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}"/> object is a proper superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <returns><c>true</c> if the current <see cref="SortedSet{T}"/> object is a proper superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper superset of any other collection. Therefore, this method returns <c>true</c>
        /// if the collection represented by the other parameter is empty unless the current <see cref="SortedSet{T}"/> collection is also empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than or equal to the number of elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by <paramref name="other"/> is a <see cref="SortedSet{T}"/>,
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>, or <see cref="SCG.SortedSet{T}"/> with the same equality comparer as the
        /// current <see cref="SortedSet{T}"/> object, this method is an O(<c>n</c>) operation. If the collection represented by <paramref name="other"/>
        /// is a <see cref="HashSet{T}"/>, <see cref="OrderedHashSet{T}"/>, <see cref="SCG.HashSet{T}"/>, or <see cref="ISortedCollection{T}"/> this method
        /// is an O(<c>n</c> log <c>m</c>) operation, where <c>n</c> is the number of elements in <paramref name="other"/> and <c>m</c> is <see cref="Count"/>.
        /// Otherwise, this method is an O(<c>n</c> log <c>n</c> + <c>m</c> log <c>n</c>) operation, where <c>n</c> is the number of elements in
        /// <paramref name="other"/> and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (Count == 0)
                return false;

            if (other is ICollection<T> genericCollection)
            {
                if (genericCollection.Count == 0)
                    return true;
            }
            else if (other is ICollection c)
            {
                if (c.Count == 0)
                    return true;
            }

            if (other is INavigableCollection<T> navigableCollection)
            {
                if (ComparerEquals(Comparer, navigableCollection.Comparer))
                {
                    if (navigableCollection.Count >= Count)
                        return false;

                    return IsSupersetOfNavigableCollectionWithSameComparer(navigableCollection);
                }
            }
            else if (other is IDistinctSortedCollection<T> distinctSortedCollection)
            {
                if (ComparerEquals(Comparer, distinctSortedCollection.Comparer))
                {
                    if (distinctSortedCollection.Count >= Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(distinctSortedCollection);
                }
            }
            else if (other is HashSet<T> hashSet)
            {
                if (ComparerEquals(Comparer, hashSet.EqualityComparer))
                {
                    if (hashSet.Count >= Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(hashSet);
                }
            }
            else if (other is OrderedHashSet<T> orderedHashSet)
            {
                if (ComparerEquals(Comparer, orderedHashSet.EqualityComparer))
                {
                    if (orderedHashSet.Count >= Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(orderedHashSet);
                }
            }
            else if (other is SCG.SortedSet<T> bclSortedSet)
            {
                if (ComparerEquals(Comparer, bclSortedSet.Comparer))
                {
                    if (bclSortedSet.Count >= Count)
                        return false;

                    return IsSupersetOfBclSortedSetWithSameComparer(bclSortedSet);
                }
            }
            else if (other is SCG.HashSet<T> bclHashSet)
            {
                if (ComparerEquals(Comparer, bclHashSet.Comparer))
                {
                    if (bclHashSet.Count >= Count)
                        return false;

                    return IsSupersetOfEnumerableWithSameComparer(bclHashSet);
                }
            }

            // Worst case: I mark every element in my set and see if I've counted all of them. O(M log N).
            // slight optimization, put it into a HashSet and then check can do it in O(N+M)
            // but slower in better cases + wastes space
            ElementCount result = CheckUniqueAndUnfoundElements(other, true);
            return result.UniqueCount < Count && result.UnfoundCount == 0;
        }

        /// <summary>
        /// Determines whether the current <see cref="SortedSet{T}"/> object and the specified
        /// collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <returns><c>true</c> if the current <see cref="SortedSet{T}"/> object is equal to <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method ignores the order of elements and any duplicate elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by <paramref name="other"/> is a <see cref="SortedSet{T}"/>,
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>, or <see cref="IDistinctSortedCollection{T}"/>
        /// with the same equality comparer as the current <see cref="SortedSet{T}"/> object, this method is an O(log <c>n</c>)
        /// operation. If the collection represented by <paramref name="other"/> is a <see cref="HashSet{T}"/>,
        /// <see cref="OrderedHashSet{T}"/>, or <see cref="SCG.HashSet{T}"/> with the same equality comparer as the
        /// current <see cref="SortedSet{T}"/> object, this method is an O(n) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of
        /// elements in other and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // Early bounds check:
            // If this set is empty, other must be empty.
            if (Count == 0)
            {
                if (other is ICollection<T> genericCollection)
                    return genericCollection!.Count == 0; // [!] asserted above
                else if (other is ICollection c)
                    return c!.Count == 0;
            }

            // A set is equal to itself.
            if (other == this)
                return true;

            IDistinctSortedCollection<T>? distinctSortedCollection;
            if (other is not SCG.SortedSet<T> otherSortedSet)
                distinctSortedCollection = other as IDistinctSortedCollection<T>;
            else
                distinctSortedCollection = new BclSortedSetAdapter(otherSortedSet);

            if (distinctSortedCollection != null)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, distinctSortedCollection.Comparer))
                {
                    IEnumerator<T> mine = GetEnumerator();
                    IEnumerator<T> theirs = distinctSortedCollection.GetEnumerator();
                    bool mineEnded = !mine.MoveNext();
                    bool theirsEnded = !theirs.MoveNext();
                    while (!mineEnded && !theirsEnded)
                    {
                        if (comparer.Compare(mine.Current, theirs.Current) != 0)
                        {
                            return false;
                        }
                        mineEnded = !mine.MoveNext();
                        theirsEnded = !theirs.MoveNext();
                    }
                    return mineEnded && theirsEnded;
                }
            }
            else if (other is ISortedCollection<T> sortedCollection)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, sortedCollection.Comparer))
                    return SetEqualsSortedNonDistinctCollectionWithSameComparer(sortedCollection, comparer);
            }
            else if (other is HashSet<T> hashSet)
            {
                if (ComparerEquals(Comparer, hashSet.EqualityComparer))
                    return SetEqualsDistinctCollectionWithSameComparer(hashSet);
            }
            else if (other is OrderedHashSet<T> orderedHashSet)
            {
                if (ComparerEquals(Comparer, orderedHashSet.EqualityComparer))
                    return SetEqualsDistinctCollectionWithSameComparer(orderedHashSet);
            }
            else if (other is SCG.HashSet<T> bclHashSet)
            {
                if (ComparerEquals(Comparer, bclHashSet.Comparer))
                    return SetEqualsDistinctCollectionWithSameComparer(bclHashSet);
            }

            // Worst case: I mark every element in my set and see if I've counted all of them. O(size of the other collection).
            ElementCount result = CheckUniqueAndUnfoundElements(other, returnIfUnfound: true);
            return result.UniqueCount == Count && result.UnfoundCount == 0;
        }

        private bool SetEqualsSortedNonDistinctCollectionWithSameComparer(ISortedCollection<T> other, IComparer<T> comparer)
        {
            Debug.Assert(other != null);

            IEnumerator<T> mine = GetEnumerator();
            IEnumerator<T> theirs = other!.GetEnumerator(); // [!] asserted above

            bool mineEnded = !mine.MoveNext();
            bool theirsEnded = !theirs.MoveNext();

            Debug.Assert(!mineEnded); // Count > 0 guarantees this

            int matched = 0;
            bool hasPrevOther = false;
            T prevOther = default!;

            while (!mineEnded && !theirsEnded)
            {
                T currentOther = theirs.Current;

                // Collapse duplicates in 'other' using the comparer
                if (hasPrevOther && comparer.Compare(prevOther, currentOther) == 0)
                {
                    theirsEnded = !theirs.MoveNext();
                    continue;
                }

                int cmp = comparer.Compare(mine.Current, currentOther);
                if (cmp != 0)
                    return false;

                matched++;

                mineEnded = !mine.MoveNext();

                prevOther = currentOther;
                hasPrevOther = true;
                theirsEnded = !theirs.MoveNext();
            }

            // If we still have items in 'other', they must all be duplicates
            while (!theirsEnded)
            {
                if (!hasPrevOther || comparer.Compare(prevOther, theirs.Current) != 0)
                    return false;

                theirsEnded = !theirs.MoveNext();
            }

            // Must have consumed all of 'mine' and matched exactly Count elements
            return mineEnded && matched == Count;
        }

        private bool SetEqualsDistinctCollectionWithSameComparer(ICollection<T> collection)
        {
            Debug.Assert(collection != null);

            if (Count != collection!.Count) // [!] asserted above
                return false;

            foreach (T item in this)
            {
                if (!collection.Contains(item))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the current <see cref="SortedSet{T}"/> object and a specified collection share common elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="SortedSet{T}"/> object.</param>
        /// <returns><see langword="true"/> if the <see cref="SortedSet{T}"/> object and <paramref name="other"/> share at
        /// least one common element; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Any duplicate elements in <paramref name="other"/> are ignored.
        /// <para/>
        /// This method is an O(<c>n</c> log <c>m</c>) operation, wheer <c>m</c> is <see cref="Count"/> and <c>n</c> is the
        /// number of elements in <paramref name="other"/>.
        /// </remarks>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (Count == 0)
                return false;

            if (other is ICollection<T> genericCollection)
            {
                if (genericCollection.Count == 0)
                    return false;
            }
            else if (other is ICollection c)
            {
                if (c.Count == 0)
                    return false;
            }

            if (other is INavigableCollection<T> navigableCollection)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, navigableCollection.Comparer) &&
                    (!navigableCollection.TryGetLast(out T? otherLast) || 
                    !navigableCollection.TryGetFirst(out T? otherFirst) ||
                    comparer.Compare(MinInternal!, otherLast!) > 0 ||
                    comparer.Compare(MaxInternal!, otherFirst!) < 0))
                {
                    return false;
                }
            }
            else if (other is SCG.SortedSet<T> bclSortedSet)
            {
                IComparer<T> comparer = Comparer;
                if (ComparerEquals(comparer, bclSortedSet.Comparer) &&
                    (comparer.Compare(MinInternal!, bclSortedSet.Max!) > 0 || comparer.Compare(MaxInternal!, bclSortedSet.Min!) < 0))
                {
                    return false;
                }
            }

            foreach (T item in other)
            {
                if (Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This works similar to HashSet's CheckUniqueAndUnfound (description below), except that the bit
        /// array maps differently than in the HashSet. We can only use this for the bulk boolean checks.
        ///
        /// Determines counts that can be used to determine equality, subset, and superset. This
        /// is only used when other is an IEnumerable and not a HashSet. If other is a HashSet
        /// these properties can be checked faster without use of marking because we can assume
        /// other has no duplicates.
        ///
        /// The following count checks are performed by callers:
        /// 1. Equals: checks if UnfoundCount = 0 and uniqueFoundCount = Count; i.e. everything
        /// in other is in this and everything in this is in other
        /// 2. Subset: checks if UnfoundCount >= 0 and uniqueFoundCount = Count; i.e. other may
        /// have elements not in this and everything in this is in other
        /// 3. Proper subset: checks if UnfoundCount > 0 and uniqueFoundCount = Count; i.e
        /// other must have at least one element not in this and everything in this is in other
        /// 4. Proper superset: checks if unfound count = 0 and uniqueFoundCount strictly less
        /// than Count; i.e. everything in other was in this and this had at least one element
        /// not contained in other.
        ///
        /// An earlier implementation used delegates to perform these checks rather than returning
        /// an ElementCount struct; however this was changed due to the perf overhead of delegates.
        /// </summary>
        internal unsafe ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
        {
            ElementCount result;

            // need special case in case this has no elements.
            if (Count == 0)
            {
                int numElementsInOther = 0;
                foreach (T item in other)
                {
                    numElementsInOther++;
                    // break right away, all we want to know is whether other has 0 or 1 elements
                    break;
                }
                result.UniqueCount = 0;
                result.UnfoundCount = numElementsInOther;
                return result;
            }

            int originalLastIndex = Count;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

            Span<int> span = stackalloc int[StackAllocThreshold];
            BitHelper bitHelper =
                intArrayLength <= StackAllocThreshold ?
                new BitHelper(span.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            // count of items in other not found in this
            int UnfoundCount = 0;
            // count of unique items in other found in this
            int uniqueFoundCount = 0;

            foreach (T item in other)
            {
                int index = InternalIndexOf(item);
                if (index >= 0)
                {
                    if (!bitHelper.IsMarked(index))
                    {
                        // item hasn't been seen yet
                        bitHelper.MarkBit(index);
                        uniqueFoundCount++;
                    }
                }
                else
                {
                    UnfoundCount++;
                    if (returnIfUnfound)
                    {
                        break;
                    }
                }
            }

            result.UniqueCount = uniqueFoundCount;
            result.UnfoundCount = UnfoundCount;
            return result;
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate
        /// from a <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <param name="match">The delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements that were removed from the <see cref="SortedSet{T}"/> collection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        public int RemoveWhere(Predicate<T> match)
        {
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            List<T> matches = new List<T>(this.Count);

            BreadthFirstTreeWalk(n =>
            {
                if (match(n.Item))
                {
                    matches.Add(n.Item);
                }
                return true;
            });

            // Enumerate the results of the breadth-first walk in reverse in an attempt to lower cost.
            int actuallyRemoved = 0;
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                if (Remove(matches[i]))
                {
                    actuallyRemoved++;
                }
            }

            return actuallyRemoved;
        }

        #endregion

        #region INavigableCollection<T> members

        IComparer<T> ISortedCollection<T>.Comparer => Comparer;

        bool INavigableCollection<T>.TryGetPredecessor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            => TryGetPredecessor(item, out result);

        bool INavigableCollection<T>.TryGetSuccessor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            => TryGetSuccessor(item, out result);

        bool INavigableCollection<T>.TryGetFloor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            => TryGetFloor(item, out result);

        bool INavigableCollection<T>.TryGetCeiling([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            => TryGetCeiling(item, out result);

        IEnumerable<T> INavigableCollection<T>.Reverse()
            => Reverse();

        INavigableCollection<T> INavigableCollection<T>.GetView([AllowNull] T fromItem, [AllowNull] T toItem)
            => GetView(fromItem, toItem);

        INavigableCollection<T> INavigableCollection<T>.GetView([AllowNull] T fromItem, bool fromInclusive, [AllowNull] T toItem, bool toInclusive)
            => GetView(fromItem, fromInclusive, toItem, toInclusive);

        INavigableCollection<T> INavigableCollection<T>.GetViewBefore([AllowNull] T toItem)
            => GetViewBefore(toItem);

        INavigableCollection<T> INavigableCollection<T>.GetViewBefore([AllowNull] T toItem, bool inclusive)
            => GetViewBefore(toItem, inclusive);

        INavigableCollection<T> INavigableCollection<T>.GetViewAfter([AllowNull] T fromItem)
            => GetViewAfter(fromItem);

        INavigableCollection<T> INavigableCollection<T>.GetViewAfter([AllowNull] T fromItem, bool inclusive)
            => GetViewAfter(fromItem, inclusive);

        INavigableCollection<T> INavigableCollection<T>.GetViewDescending()
            => GetViewDescending();

        #endregion INavigableCollection<T> members

        #region ISorted members

        /// <summary>
        /// Gets the minimum value in the <see cref="SortedSet{T}"/>, as defined by the comparer.
        /// </summary>
        /// <remarks>
        /// If the <see cref="SortedSet{T}"/> has no elements, then the <see cref="Min"/> property returns
        /// the default value of <typeparamref name="T"/>.
        /// </remarks>
        [Obsolete("This property is deprecated because the name does not match the behavior in descending order views. Use TryGetFirst(out T) instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public T? Min => MinInternal;

        /// <summary>
        /// Indicates the lowest logical value (which may be reversed)
        /// </summary>
        internal virtual T? MinInternal => LowerValue;

        /// <summary>
        /// Gets the maximum value in the <see cref="SortedSet{T}"/>, as defined by the comparer.
        /// </summary>
        /// <remarks>
        /// If the <see cref="SortedSet{T}"/> has no elements, then the <see cref="Max"/> property returns
        /// the default value of <typeparamref name="T"/>.
        /// </remarks>
        [Obsolete("This property is deprecated because the name does not match the behavior in descending order views. Use TryGetLast(out T) instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public T? Max => MaxInternal;

        /// <summary>
        /// Indicates the maximum logical value (which may be reversed)
        /// </summary>
        internal virtual T? MaxInternal => UpperValue;

        /// <summary>
        /// Gets the first (lowest) value in the <see cref="SortedSet{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="result">Upon successful return, contains the first (lowest) value.</param>
        /// <returns><see langword="true"/> if a first value exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>first()</c> method in the JDK.
        /// </remarks>
        public bool TryGetFirst([MaybeNullWhen(false)] out T result) => DoTryGetFirst(out result);

        internal virtual bool DoTryGetFirst([MaybeNullWhen(false)] out T result)
        {
            if (count > 0)
            {
                result = MinInternal!;
                return true;
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Gets the last (highest) value in the <see cref="SortedSet{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="result">Upon successful return, contains the last (highest) value.</param>
        /// <returns><see langword="true"/> if a last value exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>last()</c> method in the JDK.
        /// </remarks>
        public bool TryGetLast([MaybeNullWhen(false)] out T result) => DoTryGetLast(out result);

        internal virtual bool DoTryGetLast([MaybeNullWhen(false)] out T result)
        {
            if (count > 0)
            {
                result = MaxInternal!;
                return true;
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Removes the first (lowest) value in the <see cref="SortedSet{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/>  if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollFirst()</c> method in the JDK.
        /// </remarks>
        public bool RemoveFirst([MaybeNullWhen(false)] out T value) => DoRemoveFirst(out value);

        internal virtual bool DoRemoveFirst([MaybeNullWhen(false)] out T value)
        {
            if (count == 0)
            {
                value = default;
                return false;
            }

            return DoRemove(MinInternal!, out value);
        }

        /// <summary>
        /// Removes the last (highest) value in the <see cref="SortedSet{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/>  if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollLast()</c> method in the JDK.
        /// </remarks>
        public bool RemoveLast([MaybeNullWhen(false)] out T value) => DoRemoveLast(out value);

        internal virtual bool DoRemoveLast([MaybeNullWhen(false)] out T value)
        {
            if (count == 0)
            {
                value = default;
                return false;
            }

            return DoRemove(MaxInternal!, out value);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that iterates over the
        /// <see cref="SortedSet{T}"/> in reverse order.
        /// </summary>
        /// <returns>An enumerable that iterates over the <see cref="SortedSet{T}"/> in reverse order.</returns>
        /// <remarks>
        /// This corresponds roughly to the <c>descendingIterator()</c> method in the JDK.
        /// </remarks>
        public IEnumerable<T> Reverse()
        {
            Enumerator e = new Enumerator(this, reverse: !IsReversed);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedSet{T}"/>.
        /// <para/>
        /// Usage Note: In Java, the upper bound of TreeSet.subSet() is exclusive. To match the behavior, call
        /// <see cref="GetViewBetween(T, bool, T, bool)"/>, setting <c>lowerValueInclusive</c> to <see langword="true"/>
        /// and <c>upperValueInclusive</c> to <see langword="false"/>.
        /// </summary>
        /// <param name="lowerValue">The lowest desired value in the view.</param>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="lowerValue"/> is more than <paramref name="upperValue"/>
        /// according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="lowerValue"/> and <paramref name="upperValue"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="lowerValue"/> and
        /// <paramref name="upperValue"/> (inclusive), as defined by the comparer. This method does not copy elements from the
        /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        [Obsolete("This method is deprecated because the argument names do not match the behavior in descending order views. Use GetView(T, T) instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual SortedSet<T> GetViewBetween(T? lowerValue, T? upperValue)
            => DoGetView(lowerValue, fromInclusive: true, ExceptionArgument.lowerValue, upperValue, toInclusive: true, ExceptionArgument.upperValue);

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedSet{T}"/>.
        /// <para/>
        /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="lowerValueInclusive"/>
        /// set to <see langword="true"/> and <paramref name="upperValueInclusive"/> set to <see langword="false"/>.
        /// </summary>
        /// <param name="lowerValue">The lowest value in the range for the view.</param>
        /// <param name="lowerValueInclusive">If <see langword="true"/>, <paramref name="lowerValue"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <param name="upperValueInclusive">If <see langword="true"/>, <paramref name="upperValue"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="lowerValue"/> is more than <paramref name="upperValue"/>
        /// according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="lowerValue"/> and <paramref name="upperValue"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="lowerValue"/> and
        /// <paramref name="upperValue"/>, as defined by the comparer. Each bound may either be inclusive
        /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="lowerValueInclusive"/>
        /// and <paramref name="upperValueInclusive"/>. This method does not copy elements from the
        /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        [Obsolete("This method is deprecated because the argument names do not match the behavior in descending order views. Use GetView(T, bool, T, bool) instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual SortedSet<T> GetViewBetween(T? lowerValue, bool lowerValueInclusive, T? upperValue, bool upperValueInclusive)
            => DoGetView(lowerValue, fromInclusive: lowerValueInclusive, ExceptionArgument.lowerValue, upperValue, toInclusive: upperValueInclusive, ExceptionArgument.upperValue);

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedSet{T}"/>.
        /// <para/>
        /// Usage Note: In Java, the <paramref name="toItem"/> of TreeSet.subSet() is exclusive. To match the behavior, call
        /// <see cref="GetView(T, bool, T, bool)"/>, setting <c>fromInclusive</c> to <see langword="true"/>
        /// and <c>toInclusive</c> to <see langword="false"/>.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromItem"/> is after <paramref name="toItem"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromItem"/> and <paramref name="toItem"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromItem"/> and
        /// <paramref name="toItem"/> (inclusive), as defined by the current view order and the comparer.
        /// This method does not copy elements from the <see cref="SortedSet{T}"/>, but provides a window into the
        /// underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        public SortedSet<T> GetView([AllowNull] T fromItem, [AllowNull] T toItem)
            => DoGetView(fromItem, fromInclusive: true, fromArgumentName: ExceptionArgument.fromItem, toItem, toInclusive: true, toArgumentName: ExceptionArgument.toItem);

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedSet{T}"/>.
        /// <para/>
        /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="fromInclusive"/>
        /// set to <see langword="true"/> and <paramref name="toInclusive"/> set to <see langword="false"/>.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="fromInclusive">If <see langword="true"/>, <paramref name="fromItem"/> will be included in the range;
        /// otherwise, it is an exclusive bound.</param>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="toInclusive">If <see langword="true"/>, <paramref name="toItem"/> will be included in the range;
        /// otherwise, it is an exclusive bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromItem"/> is after <paramref name="toItem"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromItem"/> and <paramref name="toItem"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromItem"/> and
        /// <paramref name="toItem"/>, as defined by the current view order and the comparer. Each bound may either be inclusive
        /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="fromInclusive"/>
        /// and <paramref name="toInclusive"/>. This method does not copy elements from the
        /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        public SortedSet<T> GetView([AllowNull] T fromItem, bool fromInclusive, [AllowNull] T toItem, bool toInclusive)
            => DoGetView(fromItem, fromInclusive, ExceptionArgument.fromItem, toItem, toInclusive, ExceptionArgument.toItem);

        internal virtual SortedSet<T> DoGetView([AllowNull] T fromItem, bool fromInclusive, ExceptionArgument fromArgumentName, [AllowNull] T toItem, bool toInclusive, ExceptionArgument toArgumentName)
        {
            // J2N: Use forward-only comparer here, since this method requires the paramters to be passed in value
            // order regardless of whether the set is reversed or not.
            if (comparer.Compare(fromItem!, toItem!) > 0)
            {
                ThrowHelper.ThrowArgumentException_SortedSet_LowerValueGreaterThanUpperValue(fromArgumentName, toArgumentName);
            }
            return new TreeSubSet(UnderlyingSet, fromItem, lowerBoundInclusive: fromInclusive, toItem, upperBoundInclusive: toInclusive, true, true, IsReversed);
        }

        /// <summary>
        /// Returns the view of a subset in a <see cref="SortedSet{T}"/> with no lower bound.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call the <see cref="GetViewBefore(T, bool)"/>
        /// overload with <c>inclusive</c> set to <see langword="false"/>.
        /// </summary>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall before <paramref name="toItem"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>headSet()</c> method in the JDK.
        /// </remarks>
        public SortedSet<T> GetViewBefore([AllowNull] T toItem)
            => DoGetViewBefore(toItem, inclusive: true, ExceptionArgument.toItem);

        /// <summary>
        /// Returns the view of a subset in a <see cref="SortedSet{T}"/> with no lower bound.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="inclusive"/>
        /// set to <see langword="false"/>.
        /// </summary>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="toItem"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>
        /// This method returns a view of the range of elements that fall before <paramref name="toItem"/>, as defined
        /// by the current view order and comparer. The upper bound may either be inclusive (<see langword="true"/>)
        /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>.
        /// This method does not copy elements from the <see cref="SortedSet{T}"/>, but provides a window into
        /// the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>headSet()</c> method in the JDK.
        /// </returns>
        public SortedSet<T> GetViewBefore([AllowNull] T toItem, bool inclusive)
            => DoGetViewBefore(toItem, inclusive, ExceptionArgument.toItem);

        internal virtual SortedSet<T> DoGetViewBefore([AllowNull] T toItem, bool inclusive, ExceptionArgument toArgumentName)
        {
            return new TreeSubSet(UnderlyingSet, default, true, toItem, inclusive, false, true, IsReversed);
        }

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedSet{T}"/> with no upper bound.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromItem"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="SortedSet{T}"/>, but provides a window into the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>tailSet()</c> method in the JDK.
        /// </remarks>
        public SortedSet<T> GetViewAfter([AllowNull] T fromItem)
            => DoGetViewAfter(fromItem, inclusive: true, ExceptionArgument.fromItem);

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedSet{T}"/> with no upper bound.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="fromItem"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromItem"/>, as defined
        /// by the current view order and comparer. The lower bound may either be inclusive (<see langword="true"/>)
        /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>.
        /// This method does not copy elements from the <see cref="SortedSet{T}"/>, but provides a window into
        /// the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>tailSet()</c> method in the JDK.
        /// </remarks>
        public SortedSet<T> GetViewAfter([AllowNull] T fromItem, bool inclusive)
            => DoGetViewAfter(fromItem, inclusive, ExceptionArgument.fromItem);

        internal virtual SortedSet<T> DoGetViewAfter([AllowNull] T fromItem, bool inclusive, ExceptionArgument fromArgumentName)
        {
            return new TreeSubSet(UnderlyingSet, fromItem, inclusive, default, true, true, false, IsReversed);
        }

        /// <summary>
        /// Returns a reverse order view of the elements of the current <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <returns>A view that contains the values of the current <see cref="SortedSet{T}"/> in reverse order.</returns>
        /// <remarks>
        /// This method returns a reverse order view of the range of elements of this <see cref="SortedSet{T}"/>, as defined
        /// by the comparer.This method does not copy elements from the <see cref="SortedSet{T}"/>, but provides a window into
        /// the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>descendingSet()</c> method in the JDK.
        /// </remarks>
        public SortedSet<T> GetViewDescending()
        {
            // J2N: Inherit the settings from the current view.
            // These are defaulted in SortedSet<T> to the full range with inclusive bounds, but may be different for existing views.
            return new TreeSubSet(UnderlyingSet, LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive, HasLowerBound, HasUpperBound, !IsReversed);
        }

#if DEBUG
        /// <summary>
        /// debug status to be checked whenever any operation is called
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Following Microsoft's code styles")]
        internal virtual bool versionUpToDate()
        {
            return true;
        }
#endif

#if FEATURE_SERIALIZABLE
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => GetObjectData(info, context);

        /// <summary>
        /// Implements the <see cref="ISerializable"/> interface and returns the data that you must have to serialize a
        /// <see cref="SortedSet{T}"/> object.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> object that contains the information that is required
        /// to serialize the <see cref="SortedSet{T}"/> object.</param>
        /// <param name="context">A <see cref="StreamingContext"/> structure that contains the source and destination
        /// of the serialized stream associated with the <see cref="SortedSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <c>null</c>.</exception>
        /// <remarks>Calling this method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);

            info.AddValue(CountName, count); // This is the length of the bucket array.
            info.AddValue(ComparerName, Comparer, typeof(IComparer<T>)); // J2N: Ensure we call the Comparer property to unwrap the comparer for serialization.

            // J2N: Add metadata to the serialization blob so we can rehydrate the WrappedStringComparer properly
            if (typeof(T) == typeof(string) && StringComparerDescriptor.TryDescribe(Comparer, out StringComparerDescriptor descriptor))
            {
                info.AddValue(ref descriptor);
            }

            info.AddValue(VersionName, version);

            if (root != null)
            {
                T[] items = new T[Count];
                CopyTo(items, 0);
                info.AddValue(ItemsName, items, typeof(T[]));
            }
        }

        void IDeserializationCallback.OnDeserialization(object? sender) => OnDeserialization(sender);

        /// <summary>
        /// Implements the <see cref="ISerializable"/> interface, and raises the deserialization
        /// event when the deserialization is completed.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="SerializationException">The <see cref="SerializationInfo"/> object associated
        /// with the current <see cref="SortedSet{T}"/> object is invalid.</exception>
        /// <remarks>Calling this method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        protected virtual void OnDeserialization(object? sender)
        {
            if (comparer != null)
            {
                return; // Somebody had a dependency on this class and fixed us up before the ObjectManager got to it.
            }

            if (siInfo == null)
            {
                ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_InvalidOnDeser);
            }

            comparer = (IComparer<T>)siInfo.GetValue(ComparerName, typeof(IComparer<T>))!;

            // J2N:Try to wrap the comparer with WrappedStringComparer
            if (typeof(T) == typeof(string) && StringComparerMetadataSerializer.TryGetKnownStringComparer(comparer, siInfo, out IComparer<string?>? stringComparer))
            {
                comparer = (IComparer<T>)stringComparer;
            }

            int savedCount = siInfo.GetInt32(CountName);

            if (savedCount != 0)
            {
                T[]? items = (T[]?)siInfo.GetValue(ItemsName, typeof(T[]));

                if (items == null)
                {
                    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingValues);
                }

                for (int i = 0; i < items.Length; i++)
                {
                    Add(items[i]);
                }
            }

            version = siInfo.GetInt32(VersionName);
            if (count != savedCount)
            {
                ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MismatchedCount);
            }

            siInfo = null;
        }

#endif

        #endregion

        #region Helper classes

        internal sealed class Node
        {
            public Node(T item, NodeColor color)
            {
                Item = item;
                Color = color;
            }

            public static bool IsNonNullBlack(Node? node) => node != null && node.IsBlack;

            public static bool IsNonNullRed(Node? node) => node != null && node.IsRed;

            public static bool IsNullOrBlack(Node? node) => node == null || node.IsBlack;

            public T Item; // J2N: Changed to a field so we can directly access it with Unsafe

            public Node? Left { get; set; }

            public Node? Right { get; set; }

            public NodeColor Color { get; set; }

            public bool IsBlack => Color == NodeColor.Black;

            public bool IsRed => Color == NodeColor.Red;

            public bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);

            public bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

            public void ColorBlack() => Color = NodeColor.Black;

            public void ColorRed() => Color = NodeColor.Red;

            public Node DeepClone(int count)
            {
#if DEBUG
                Debug.Assert(count == GetCount());
#endif

#if FEATURE_STACK_TRYPOP
                Node newRoot = ShallowClone();

                var pendingNodes = new Stack<(Node source, Node target)>(2 * Log2(count) + 2);
                pendingNodes.Push((this, newRoot));

                while (pendingNodes.TryPop(out var next))
                {
                    Node clonedNode;

                    if (next.source.Left is Node left)
                    {
                        clonedNode = left.ShallowClone();
                        next.target.Left = clonedNode;
                        pendingNodes.Push((left, clonedNode));
                    }

                    if (next.source.Right is Node right)
                    {
                        clonedNode = right.ShallowClone();
                        next.target.Right = clonedNode;
                        pendingNodes.Push((right, clonedNode));
                    }
                }
#else
                // J2N: This was code from .NET 5
                // Breadth-first traversal to recreate nodes, preorder traversal to replicate nodes.

                var originalNodes = new Stack<Node>(2 * Log2(count) + 2);
                var newNodes = new Stack<Node>(2 * Log2(count) + 2);
                Node newRoot = ShallowClone();

                Node? originalCurrent = this;
                Node newCurrent = newRoot;

                while (originalCurrent != null)
                {
                    originalNodes.Push(originalCurrent);
                    newNodes.Push(newCurrent);
                    newCurrent.Left = originalCurrent.Left?.ShallowClone();
                    originalCurrent = originalCurrent.Left;
                    newCurrent = newCurrent.Left!;
                }

                while (originalNodes.Count != 0)
                {
                    originalCurrent = originalNodes.Pop();
                    newCurrent = newNodes.Pop();

                    Node? originalRight = originalCurrent.Right;
                    Node? newRight = originalRight?.ShallowClone();
                    newCurrent.Right = newRight;

                    while (originalRight != null)
                    {
                        originalNodes.Push(originalRight);
                        newNodes.Push(newRight!);
                        newRight!.Left = originalRight.Left?.ShallowClone();
                        originalRight = originalRight.Left;
                        newRight = newRight.Left;
                    }
                }
#endif
                return newRoot;
            }

            /// <summary>
            /// Gets the rotation this node should undergo during a removal.
            /// </summary>
            public TreeRotation GetRotation(Node current, Node sibling)
            {
                Debug.Assert(IsNonNullRed(sibling.Left) || IsNonNullRed(sibling.Right));
#if DEBUG
                Debug.Assert(HasChildren(current, sibling));
#endif

                bool currentIsLeftChild = Left == current;
                return IsNonNullRed(sibling.Left) ?
                    (currentIsLeftChild ? TreeRotation.RightLeft : TreeRotation.Right) :
                    (currentIsLeftChild ? TreeRotation.Left : TreeRotation.LeftRight);
            }

            /// <summary>
            /// Gets the sibling of one of this node's children.
            /// </summary>
            public Node GetSibling(Node node)
            {
                Debug.Assert(node != null);
                Debug.Assert(node == Left ^ node == Right);

                return node == Left ? Right! : Left!;
            }

            public Node ShallowClone() => new Node(Item, Color);

            public void Split4Node()
            {
                Debug.Assert(Left != null);
                Debug.Assert(Right != null);

                ColorRed();
                Left!.ColorBlack();
                Right!.ColorBlack();
            }

            /// <summary>
            /// Does a rotation on this tree. May change the color of a grandchild from red to black.
            /// </summary>
            public Node? Rotate(TreeRotation rotation)
            {
                Node removeRed;
                switch (rotation)
                {
                    case TreeRotation.Right:
                        removeRed = Left!.Left!;
                        Debug.Assert(removeRed.IsRed);
                        removeRed.ColorBlack();
                        return RotateRight();
                    case TreeRotation.Left:
                        removeRed = Right!.Right!;
                        Debug.Assert(removeRed.IsRed);
                        removeRed.ColorBlack();
                        return RotateLeft();
                    case TreeRotation.RightLeft:
                        Debug.Assert(Right!.Left!.IsRed);
                        return RotateRightLeft();
                    case TreeRotation.LeftRight:
                        Debug.Assert(Left!.Right!.IsRed);
                        return RotateLeftRight();
                    default:
                        Debug.Fail($"{nameof(rotation)}: {rotation} is not a defined {nameof(TreeRotation)} value.");
                        return null;
                }
            }

            /// <summary>
            /// Does a left rotation on this tree, making this node the new left child of the current right child.
            /// </summary>
            public Node RotateLeft()
            {
                Node child = Right!;
                Right = child.Left;
                child.Left = this;
                return child;
            }

            /// <summary>
            /// Does a left-right rotation on this tree. The left child is rotated left, then this node is rotated right.
            /// </summary>
            public Node RotateLeftRight()
            {
                Node child = Left!;
                Node grandChild = child.Right!;

                Left = grandChild.Right;
                grandChild.Right = this;
                child.Right = grandChild.Left;
                grandChild.Left = child;
                return grandChild;
            }

            /// <summary>
            /// Does a right rotation on this tree, making this node the new right child of the current left child.
            /// </summary>
            public Node RotateRight()
            {
                Node child = Left!;
                Left = child.Right;
                child.Right = this;
                return child;
            }

            /// <summary>
            /// Does a right-left rotation on this tree. The right child is rotated right, then this node is rotated left.
            /// </summary>
            public Node RotateRightLeft()
            {
                Node child = Right!;
                Node grandChild = child.Left!;

                Right = grandChild.Left;
                grandChild.Left = this;
                child.Left = grandChild.Right;
                grandChild.Right = child;
                return grandChild;
            }

            /// <summary>
            /// Combines two 2-nodes into a 4-node.
            /// </summary>
            public void Merge2Nodes()
            {
                Debug.Assert(IsRed);
                Debug.Assert(Left!.Is2Node);
                Debug.Assert(Right!.Is2Node);

                // Combine two 2-nodes into a 4-node.
                ColorBlack();
                Left.ColorRed();
                Right.ColorRed();
            }

            /// <summary>
            /// Replaces a child of this node with a new node.
            /// </summary>
            /// <param name="child">The child to replace.</param>
            /// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
            public void ReplaceChild(Node child, Node newChild)
            {
#if DEBUG
                Debug.Assert(HasChild(child));
#endif

                if (Left == child)
                {
                    Left = newChild;
                }
                else
                {
                    Right = newChild;
                }
            }

#if DEBUG
            private int GetCount() => 1 + (Left?.GetCount() ?? 0) + (Right?.GetCount() ?? 0);

            private bool HasChild(Node child) => child == Left || child == Right;

            private bool HasChildren(Node child1, Node child2)
            {
                Debug.Assert(child1 != child2);

                return (Left == child1 && Right == child2)
                    || (Left == child2 && Right == child1);
            }
#endif
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="SortedSet{T}"/> object.
        /// </summary>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
        /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
        /// <para/>
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// <para/>
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
        /// <see cref="SortedSet{T}.Enumerator.Current"/> property is undefined. Therefore, you must call the
        /// <see cref="SortedSet{T}.Enumerator.MoveNext()"/> method to advance the enumerator to the first element
        /// of the collection before reading the value of <see cref="SortedSet{T}.Enumerator.Current"/>.
        /// <para/>
        /// The <see cref="SortedSet{T}.Enumerator.Current"/> property returns the same object until
        /// <see cref="SortedSet{T}.Enumerator.MoveNext()"/> is called. <see cref="SortedSet{T}.Enumerator.MoveNext()"/>
        /// sets <see cref="SortedSet{T}.Enumerator.Current"/> to the next element.
        /// <para/>
        /// If <see cref="SortedSet{T}.Enumerator.MoveNext()"/> passes the end of the collection, the enumerator is
        /// positioned after the last element in the collection and <see cref="SortedSet{T}.Enumerator.MoveNext()"/>
        /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="SortedSet{T}.Enumerator.MoveNext()"/>
        /// also return <c>false</c>. If the last call to <see cref="SortedSet{T}.Enumerator.MoveNext()"/> returned false,
        /// <see cref="SortedSet{T}.Enumerator.Current"/> is undefined. You cannot set <see cref="SortedSet{T}.Enumerator.Current"/>
        /// to the first element of the collection again; you must create a new enumerator object instead.
        /// <para/>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
        /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
        /// to <see cref="SortedSet{T}.Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
        /// <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
        /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
        /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
        /// reading and writing, you must implement your own synchronization.
        /// <para/>
        /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        public struct Enumerator : IEnumerator<T>, IEnumerator
#if FEATURE_SERIALIZABLE
            , ISerializable, IDeserializationCallback
#endif
        {
            private /* readonly */ SortedSet<T> _tree;
            private /* readonly */ int _version;

            private /* readonly */ Stack<Node> _stack;
            private Node? _current;
            static readonly Node dummyNode = new Node(default!, NodeColor.Red);

            private /* readonly */ bool _reverse;

#if FEATURE_SERIALIZABLE
#pragma warning disable IDE0044 // Add readonly modifier
            private SerializationInfo? _siInfo;
#pragma warning restore IDE0044 // Add readonly modifier
#endif

            internal Enumerator(SortedSet<T> set)
                : this(set, reverse: false)
            {
            }

            internal Enumerator(SortedSet<T> set, bool reverse)
            {
                _tree = set;
                set.VersionCheck();
                _version = set.version;

                // 2 log(n + 1) is the maximum height.
                _stack = new Stack<Node>(2 * (int)Log2(set.TotalCount() + 1));
                _current = null;
                _reverse = reverse;
#if FEATURE_SERIALIZABLE
                _siInfo = null;
#endif

                Initialize();
            }

#if FEATURE_SERIALIZABLE

            [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
            private Enumerator(SerializationInfo info, StreamingContext context)
            {
                _tree = null!;
                _version = -1;
                _current = null;
                _reverse = false;
                _stack = null!;
                _siInfo = info;
            }

            [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);
                info.AddValue(TreeName, _tree, typeof(SortedSet<T>));
                info.AddValue(EnumVersionName, _version);
                info.AddValue(ReverseName, _reverse);
                info.AddValue(EnumStartName, !NotStartedOrEnded);
                info.AddValue(NodeValueName, (_current == null ? dummyNode.Item : _current.Item), typeof(T));

            }

            void IDeserializationCallback.OnDeserialization(object? sender)
            {
                if (_siInfo == null)
                {
                    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_InvalidOnDeser);
                }

                _tree = (SortedSet<T>)_siInfo.GetValue(TreeName, typeof(SortedSet<T>))!;
                _version = _siInfo.GetInt32(EnumVersionName);
                _reverse = _siInfo.GetBoolean(ReverseName);
                bool EnumStarted = _siInfo.GetBoolean(EnumStartName);
                _stack = new Stack<Node>(2 * Log2(_tree.Count + 1));
                _current = null;
                if (EnumStarted)
                {
                    T item = (T)_siInfo.GetValue(NodeValueName, typeof(T))!;
                    Initialize();
                    //go until it reaches the value we want
                    while (this.MoveNext())
                    {
                        if (_tree.Comparer.Compare(this.Current, item) == 0)
                            break;
                    }
                }
            }
#endif

            private void Initialize()
            {
                _current = null;
                Node? node = _tree.root;
                Node? next, other;
                while (node != null)
                {
                    next = (_reverse ? node.Right : node.Left);
                    other = (_reverse ? node.Left : node.Right);
                    if (_tree.IsWithinRange(node.Item))
                    {
                        _stack.Push(node);
                        node = next;
                    }
                    else if (next == null || !_tree.IsWithinRange(next.Item))
                    {
                        node = other;
                    }
                    else
                    {
                        node = next;
                    }
                }
            }


            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="SortedSet{T}"/> collection.
            /// </summary>
            /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
            /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            /// <remarks>
            /// After an enumerator is created, the enumerator is positioned before the first element in the collection, and
            /// the first call to the <see cref="MoveNext()"/> method advances the enumerator to the first element of the collection.
            /// <para/>
            /// If <see cref="MoveNext()"/> passes the end of the collection, the enumerator is positioned after the last element
            /// in the collection and <see cref="MoveNext()"/> returns <c>false</c>. When the enumerator is at this position,
            /// subsequent calls to <see cref="MoveNext()"/> also return <c>false</c>.
            /// <para/>
            /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
            /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
            /// to <see cref="MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an <see cref="InvalidOperationException"/>.
            /// </remarks>
            public bool MoveNext()
            {
                // Make sure that the underlying subset has not been changed since
                _tree.VersionCheck();

                if (_version != _tree.version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                if (_stack.Count == 0)
                {
                    _current = null;
                    return false;
                }

                _current = _stack.Pop();
                Node? node = (_reverse ? _current.Left : _current.Right);
                Node? next, other;
                while (node != null)
                {
                    next = (_reverse ? node.Right : node.Left);
                    other = (_reverse ? node.Left : node.Right);
                    if (_tree.IsWithinRange(node.Item))
                    {
                        _stack.Push(node);
                        node = next;
                    }
                    else if (other == null || !_tree.IsWithinRange(other.Item))
                    {
                        node = next;
                    }
                    else
                    {
                        node = other;
                    }
                }
                return true;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="SortedSet{T}.Enumerator"/>.
            /// </summary>
            public void Dispose() { /* Intentionally blank */ }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <remarks>
            /// <see cref="Current"/> is undefined under any of the following conditions:
            /// <list type="bullet">
            ///     <item><description>
            ///         The enumerator is positioned before the first element of the collection.
            ///         That happens after an enumerator is created or after the <see cref="IEnumerator.Reset()"/>
            ///         method is called. The <see cref="MoveNext()"/> method must be called to advance the
            ///         enumerator to the first element of the collection before reading the value of the
            ///         <see cref="Current"/> property.
            ///     </description></item>
            ///     <item><description>
            ///         The last call to <see cref="MoveNext()"/> returned <c>false</c>, which indicates the
            ///         end of the collection and that the enumerator is positioned after the last element
            ///         of the collection.
            ///     </description></item>
            ///     <item><description>
            ///         The enumerator is invalidated due to changes made in the collection, such as adding,
            ///         modifying, or deleting elements.
            ///     </description></item>
            /// </list>
            /// <para/>
            /// <see cref="Current"/> does not move the position of the enumerator, and consecutive calls to
            /// <see cref="Current"/> return the same object until either <see cref="MoveNext()"/> or
            /// <see cref="IEnumerator.Reset()"/> is called.
            /// </remarks>
            public T Current
            {
                get
                {
                    if (_current != null)
                    {
                        return _current.Item;
                    }
                    return default!; // Should only happen when accessing Current is undefined behavior
                }
            }

            object? IEnumerator.Current
            {
                get
                {
                    if (_current == null)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return _current.Item;
                }
            }

            internal bool NotStartedOrEnded => _current == null;

            internal void Reset()
            {
                if (_version != _tree.version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                _stack.Clear();
                Initialize();
            }

            void IEnumerator.Reset() => Reset();
        }

        internal struct ElementCount
        {
            internal int UniqueCount;
            internal int UnfoundCount;
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Searches the set for a given value and returns the equal value it finds, if any.
        /// </summary>
        /// <param name="equalValue">The value to search for.</param>
        /// <param name="actualValue">The value from the set that the search found, or the default value of <typeparamref name="T"/> when the search yielded no match.</param>
        /// <returns>A value indicating whether the search was successful.</returns>
        /// <remarks>
        /// This can be useful when you want to reuse a previously stored reference instead of
        /// a newly constructed one (so that more sharing of references can occur) or to look up
        /// a value that has more complete data than the value you currently have, although their
        /// comparer functions indicate they are equal.
        /// </remarks>
        public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
        {
            Node? node = FindNode(equalValue);
            if (node != null)
            {
                actualValue = node.Item;
                return true;
            }
            actualValue = default!;
            return false;
        }

        // Used for set checking operations (using enumerables) that rely on counting
        private static int Log2(int value) => BitOperation.Log2((uint)value);

        #endregion

        #region Structural Equality

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current set
        /// using rules provided by the specified <paramref name="comparer"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to determine
        /// whether the current object and <paramref name="other"/> are structurally equal.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is structurally equal to the current set;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public virtual bool Equals(object? other, IEqualityComparer comparer)
            => SetEqualityComparer<T>.Equals(this, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current set using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current set.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => SetEqualityComparer<T>.GetHashCode(this, comparer);

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current set
        /// using rules similar to those in the JDK's AbstractSet class. Two sets are considered
        /// equal when they both contain the same objects (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="ISet{T}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, SetEqualityComparer<T>.Default);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(SetEqualityComparer<T>.Default);

        #endregion

        #region ToString

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
            => CollectionUtil.ToString(formatProvider, format, this);

        /// <summary>
        /// Returns a string that represents the current set using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        public override string ToString()
            => ToString("{0}", StringFormatter.CurrentCulture);

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string format)
            => ToString(format, StringFormatter.CurrentCulture);

        #endregion
    }
}
