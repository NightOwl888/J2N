using BenchmarkDotNet.Attributes;
using J2N.Collections.Generic;
using System;
using System.Collections.Generic;

namespace J2N.Collections.Generic;

[MemoryDiagnoser]
[WarmupCount(3)]
[IterationCount(10)]
public class SortedSet_IsSubsetOf
{
    private SortedSet<int>? _set;
    private SortedSet<int>? _subset;
    private SortedSet<int>? _other;
    private SortedCollection<int> _sortedCollectionWithDuplicates;

    [Params(1_000, 10_000, 100_000)]
    public int Size;

    [GlobalSetup]
    public void Setup()
    {
        _set = new SortedSet<int>();
        for (int i = 0; i < Size; i++)
            _set.Add(i * 2);

        _subset = _set.GetViewBetween(100, Size);
        _other = new SortedSet<int>();

        for (int i = 0; i < Size; i++)
            _other.Add(i * 2 + 1); // mostly misses

        _sortedCollectionWithDuplicates = new SortedCollection<int>();

        for (int i = 0; i < Size; i++)
        {
            _sortedCollectionWithDuplicates.Add(i);

            if (i % 5 == 0)
            {
                _sortedCollectionWithDuplicates.Add(i);
                _sortedCollectionWithDuplicates.Add(i);
                _sortedCollectionWithDuplicates.Add(i);
                _sortedCollectionWithDuplicates.Add(i);
            }
        }
    }

    [Benchmark]
    public bool ContainsLoop_Other()
    {
        foreach (var item in _other!)
        {
            if (!_set!.Contains(item))
                return false;
        }
        return true;
    }

    [Benchmark]
    public bool ContainsLoop_Subset()
    {
        foreach (var item in _subset!)
        {
            if (!_set!.Contains(item))
                return false;
        }
        return true;
    }

    [Benchmark]
    public bool ContainsLoop_SortedCollectionWithDuplicates()
    {
        foreach (var item in _sortedCollectionWithDuplicates!)
        {
            if (!_set!.Contains(item))
                return false;
        }
        return true;
    }

    [Benchmark]
    public bool BitHelperFallback_Other()
    {
        var result = _set!.CheckUniqueAndUnfoundElements(_other!, false);
        return result.UniqueCount == _other!.Count && result.UnfoundCount >= 0;
    }

    [Benchmark]
    public bool BitHelperFallback_Subset()
    {
        var result = _set!.CheckUniqueAndUnfoundElements(_subset!, false);
        return result.UniqueCount == _subset!.Count && result.UnfoundCount >= 0;
    }

    [Benchmark]
    public bool BitHelperFallback_SortedCollectionWithDuplicates()
    {
        var result = _set!.CheckUniqueAndUnfoundElements(_sortedCollectionWithDuplicates!, false);
        return result.UniqueCount == _sortedCollectionWithDuplicates!.Count && result.UnfoundCount >= 0;
    }
}
