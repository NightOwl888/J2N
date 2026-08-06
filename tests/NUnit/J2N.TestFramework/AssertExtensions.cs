// Based on: https://github.com/dotnet/runtime/blob/v10.0.0-rc.1.25451.107/src/libraries/Common/tests/TestUtilities/System/AssertExtensions.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#nullable enable

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

        public static void AssertNoABDeadlock<TLeft, TRight>(
            Func<TLeft> createLeft,
            Func<TRight> createRight,
            Action<TLeft, TRight> operation,
            int iterations = 1_000)
        {
            TLeft left1 = createLeft();
            TLeft left2 = createLeft();

            TRight right1 = createRight();
            TRight right2 = createRight();

            var barrier = new Barrier(2);

            Exception? ex1 = null;
            Exception? ex2 = null;

            var t1 = Task.Factory.StartNew(() =>
            {
                barrier.SignalAndWait();

                try
                {
                    for (int i = 0; i < iterations; i++)
                        operation(left1, right2);
                }
                catch (Exception ex)
                {
                    ex1 = ex;
                }
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                barrier.SignalAndWait();

                try
                {
                    for (int i = 0; i < iterations; i++)
                        operation(left2, right1);
                }
                catch (Exception ex)
                {
                    ex2 = ex;
                }
            });

            Assert.IsTrue(
                Task.WaitAll(new Task[] { t1, t2 }, TimeSpan.FromSeconds(4)),
                "Possible deadlock.");

            Assert.IsNull(ex1);
            Assert.IsNull(ex2);
        }
    }
}
