// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !FEATURE_INLINEARRAYATTRIBUTE

namespace J2N
{
    internal readonly struct ParamsArray
    {
        private readonly object? _arg0;
        private readonly object? _arg1;
        private readonly object? _arg2;
        private readonly int _length;

        public ParamsArray(object? arg0)
        {
            _arg0 = arg0;
            _arg1 = null;
            _arg2 = null;

            _length = 1;
        }

        public ParamsArray(object? arg0, object? arg1)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = null;

            _length = 2;
        }

        public ParamsArray(object? arg0, object? arg1, object? arg2)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;

            _length = 3;
        }

        public int Length => _length;

        public object? this[int index] => index == 0 ? _arg0 : GetAtSlow(index);

        private object? GetAtSlow(int index)
        {
            if (index == 1)
                return _arg1;
            if (index == 2)
                return _arg2;
            ThrowHelper.ThrowIndexOutOfRangeException();
            return default;
        }
    }
}

#endif