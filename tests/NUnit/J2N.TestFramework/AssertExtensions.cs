// Based on: https://github.com/dotnet/runtime/blob/v10.0.0-rc.1.25451.107/src/libraries/Common/tests/TestUtilities/System/AssertExtensions.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using NUnit.Framework;
using System;
using System.Linq;

namespace J2N
{
    public static class AssertExtensions
    {
        public static void ThrowsAny(Type firstExceptionType, Type secondExceptionType, Action action)
        {
            ThrowsAnyInternal(action, firstExceptionType, secondExceptionType);
        }

        private static void ThrowsAnyInternal(Action action, params Type[] exceptionTypes)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Type exceptionType = e.GetType();
                if (exceptionTypes.Any(t => t.Equals(exceptionType)))
                    return;

                throw new AssertionException($"Expected one of: ({string.Join<Type>(", ", exceptionTypes)}) -> Actual: ({exceptionType}): {e}"); // Log message and callstack to help diagnosis
            }

            throw new AssertionException($"Expected one of: ({string.Join<Type>(", ", exceptionTypes)}) -> Actual: No exception thrown");
        }

        public static void ThrowsAny<TFirstExceptionType, TSecondExceptionType>(Action action)
            where TFirstExceptionType : Exception
            where TSecondExceptionType : Exception
        {
            ThrowsAnyInternal(action, typeof(TFirstExceptionType), typeof(TSecondExceptionType));
        }

        public static void ThrowsAny<TFirstExceptionType, TSecondExceptionType, TThirdExceptionType>(Action action)
            where TFirstExceptionType : Exception
            where TSecondExceptionType : Exception
            where TThirdExceptionType : Exception
        {
            ThrowsAnyInternal(action, typeof(TFirstExceptionType), typeof(TSecondExceptionType), typeof(TThirdExceptionType));
        }
    }
}
