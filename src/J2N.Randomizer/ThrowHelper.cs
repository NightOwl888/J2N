// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// This file defines an internal static class used to throw exceptions in BCL code.
// The main purpose is to reduce code size.
//
// The old way to throw an exception generates quite a lot IL code and assembly code.
// Following is an example:
//     C# source
//          throw new ArgumentNullException(nameof(key), SR.ArgumentNull_Key);
//     IL code:
//          IL_0003:  ldstr      "key"
//          IL_0008:  ldstr      "ArgumentNull_Key"
//          IL_000d:  call       string System.Environment::GetResourceString(string)
//          IL_0012:  newobj     instance void System.ArgumentNullException::.ctor(string,string)
//          IL_0017:  throw
//    which is 21bytes in IL.
//
// So we want to get rid of the ldstr and call to Environment.GetResource in IL.
// In order to do that, I created two enums: ExceptionResource, ExceptionArgument to represent the
// argument name and resource name in a small integer. The source code will be changed to
//    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key, ExceptionResource.ArgumentNull_Key);
//
// The IL code will be 7 bytes.
//    IL_0008:  ldc.i4.4
//    IL_0009:  ldc.i4.4
//    IL_000a:  call       void System.ThrowHelper::ThrowArgumentNullException(valuetype System.ExceptionArgument)
//    IL_000f:  ldarg.0
//
// This will also reduce the Jitted code size a lot.
//
// It is very important we do this for generic classes because we can easily generate the same code
// multiple times for different instantiation.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace J2N
{
    [StackTraceHidden]
    internal class ThrowHelper
    {
        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_MustBeNonNegativeNonZero(int value, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(value, argument,
                                                    ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_MustBeNonNegativeNonZero(long value, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(value, argument,
                                                    ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException_Argument_MinMaxValue(ExceptionArgument min, ExceptionArgument max) // J2N TODO: API - this should be ArgumentException rather than ArgumentOutOfRangeException.
        {
            string minName = GetArgumentName(min);
            throw new ArgumentOutOfRangeException(minName, SR.Format(SR.Argument_MinMaxValue, minName, GetArgumentName(max)));
        }


        [DoesNotReturn]
        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(GetArgumentName(argument));
        }

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(object? actualValue, ExceptionArgument argument, ExceptionResource resource)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(argument), actualValue, GetResourceString(resource));
        }



#if false // Reflection-based implementation does not work for NativeAOT
        // This function will convert an ExceptionArgument enum value to the argument name string.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetArgumentName(ExceptionArgument argument)
        {
            Debug.Assert(Enum.IsDefined(argument),
                "The enum value is not defined, please check the ExceptionArgument Enum.");

            return argument.ToString();
        }
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetArgumentName(ExceptionArgument argument)
        {
            switch (argument)
            {
                case ExceptionArgument.buffer:
                    return "buffer";
                case ExceptionArgument.maxValue:
                    return "maxValue";
                case ExceptionArgument.minValue:
                    return "minValue";

                default:
                    Debug.Fail("The enum value is not defined, please check the ExceptionArgument Enum.");
                    return "";
            }
        }

#if false // Reflection-based implementation does not work for NativeAOT
        // This function will convert an ExceptionResource enum value to the resource string.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetResourceString(ExceptionResource resource)
        {
            Debug.Assert(Enum.IsDefined(resource),
                "The enum value is not defined, please check the ExceptionResource Enum.");

            return SR.GetResourceString(resource.ToString());
        }
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetResourceString(ExceptionResource resource)
        {
            switch (resource)
            {
                case ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero:
                    return SR.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero;
                default:
                    Debug.Fail("The enum value is not defined, please check the ExceptionResource Enum.");
                    return "";
            }
        }
    }

    //
    // The convention for this enum is using the argument name as the enum name
    //
    internal enum ExceptionArgument
    {
        buffer,
        maxValue,
        minValue,
    }

    //
    // The convention for this enum is using the resource name as the enum name
    //
    internal enum ExceptionResource
    {
        ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero,
    }
}