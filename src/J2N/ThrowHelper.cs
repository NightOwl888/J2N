﻿// Licensed to the .NET Foundation under one or more agreements.
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

using J2N.Text;
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
        //[DoesNotReturn]
        //internal static void ThrowUnreachableException()
        //{
        //    throw new UnreachableException();
        //}

        [DoesNotReturn]
        internal static void ThrowArithmeticException(string message)
        {
            throw new ArithmeticException(message);
        }

        [DoesNotReturn]
        internal static void ThrowAccessViolationException()
        {
            throw new AccessViolationException();
        }

        [DoesNotReturn]
        internal static void ThrowArrayTypeMismatchException()
        {
            throw new ArrayTypeMismatchException();
        }

        //[DoesNotReturn]
        //internal static void ThrowInvalidTypeWithPointersNotSupported(Type targetType)
        //{
        //    throw new ArgumentException(J2N.SR.Format(SR.Argument_InvalidTypeWithPointersNotSupported, targetType));
        //}

        [DoesNotReturn]
        internal static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }

        [DoesNotReturn]
        internal static void ThrowIndexArgumentOutOfRange()
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("index");
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException_DestinationTooShort()
        {
            throw new ArgumentException(SR.Argument_DestinationTooShort, "destination");
        }

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_InvalidTimeSpanStyles()
        //{
        //    throw new ArgumentException(SR.Argument_InvalidTimeSpanStyles, "styles");
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_InvalidEnumValue<TEnum>(TEnum value, [CallerArgumentExpression(nameof(value))] string argumentName = "")
        //{
        //    throw new ArgumentException(SR.Format(SR.Argument_InvalidEnumValue, value, typeof(TEnum).Name), argumentName);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_OverlapAlignmentMismatch()
        //{
        //    throw new ArgumentException(SR.Argument_OverlapAlignmentMismatch);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_ArgumentNull_TypedRefType()
        //{
        //    throw new ArgumentNullException("value", SR.ArgumentNull_TypedRefType);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_CannotExtractScalar(ExceptionArgument argument)
        //{
        //    throw GetArgumentException(ExceptionResource.Argument_CannotExtractScalar, argument);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_TupleIncorrectType(object obj)
        //{
        //    throw new ArgumentException(J2N.SR.Format(SR.ArgumentException_ValueTupleIncorrectType, obj.GetType()), "other");
        //}

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_MustBeNonNegative<T>(T actualValue, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(actualValue, argument,
                                                    ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegative);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_MustBeNonNegative(ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(argument,
                                                    ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegative);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_MustBeNonNegativeNonZero(int value, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(value, argument,
                                                    ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_MustBeNonNegativeNonZero(object? value, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(value, argument,
                                                    ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessException()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.index,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLess);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessException<T>(T actualValue)
        {
            throw GetArgumentOutOfRangeException(actualValue, ExceptionArgument.index,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLess);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessException<T>(T actualValue, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(actualValue, argument,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLess);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.index,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException<T>(T actualValue)
        {
            throw GetArgumentOutOfRangeException(actualValue, ExceptionArgument.index,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException<T>(T actualValue, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(actualValue, argument,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException_BadComparer(object? comparer)
        {
            throw new ArgumentException(SR.Format(SR.Arg_BogusIComparer, comparer));
        }

        [DoesNotReturn]
        internal static void ThrowIndexArgumentOutOfRange_NeedNonNegNumException()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.index,
                                                    ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        }

        [DoesNotReturn]
        internal static void ThrowIndexArgumentOutOfRange_NeedNonNegNumException(int index)
        {
            throw GetArgumentOutOfRangeException(index, ExceptionArgument.index,
                                                    ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        }

        [DoesNotReturn]
        internal static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.value,
                                                    ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        }

        [DoesNotReturn]
        internal static void ThrowLengthArgumentOutOfRange_ArgumentOutOfRange_NeedNonNegNum()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.length,
                                                    ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        }

        [DoesNotReturn]
        internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.startIndex,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
        }

        [DoesNotReturn]
        internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual<T>(T actualValue)
        {
            throw GetArgumentOutOfRangeException(actualValue, ExceptionArgument.startIndex,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
        }

        [DoesNotReturn]
        internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.startIndex,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLess);
        }

        [DoesNotReturn]
        internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess<T>(T actualValue)
        {
            throw GetArgumentOutOfRangeException(actualValue, ExceptionArgument.startIndex,
                                                    ExceptionResource.ArgumentOutOfRange_IndexMustBeLess);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexLengthString(int startIndex, int length) // J2N TODO: API - This should be converted to ArgumentException becasue we are dealing with 2 arguemtns
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.length,
                                                    ExceptionResource.ArgumentOutOfRange_IndexLength);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(int value, ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(value, argument,
                                                    ExceptionResource.ArgumentOutOfRange_IndexString);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexLengthArray(int startIndex, int length) // J2N TODO: API - This should be converted to ArgumentException becasue we are dealing with 2 arguemtns
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.startIndex,
                                                    ExceptionResource.ArgumentOutOfRange_IndexLengthArray);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexLengthArray(int startIndex, ExceptionArgument startArgument, int length) // J2N TODO: API - This should be converted to ArgumentException becasue we are dealing with 2 arguemtns
        {
            throw GetArgumentOutOfRangeException(startArgument,
                                                    ExceptionResource.ArgumentOutOfRange_IndexLengthArray);
        }

        [DoesNotReturn]
        internal static void ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.count,
                                                    ExceptionResource.ArgumentOutOfRange_Count);
        }

        [DoesNotReturn]
        internal static void ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count<T>(T actualValue)
        {
            throw GetArgumentOutOfRangeException(actualValue, ExceptionArgument.count,
                                                    ExceptionResource.ArgumentOutOfRange_Count);
        }

        [DoesNotReturn]
        internal static void ThrowRadixArgumentOutOfRange(int radix)
        {
            throw GetArgumentOutOfRangeException(radix, ExceptionArgument.radix,
                                                    ExceptionResource.ArgumentOutOfRange_Radix);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException_Argument_MinMaxValue(ExceptionArgument min, ExceptionArgument max) // J2N TODO: API - this should be ArgumentException rather than ArgumentOutOfRangeException.
        {
            string minName = GetArgumentName(min);
            throw new ArgumentOutOfRangeException(minName, SR.Format(SR.Argument_MinMaxValue, minName, GetArgumentName(max)));
        }

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_Year()
        //{
        //    throw GetArgumentOutOfRangeException(ExceptionArgument.year,
        //                                            ExceptionResource.ArgumentOutOfRange_Year);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_Month(int month)
        //{
        //    throw new ArgumentOutOfRangeException(nameof(month), month, SR.ArgumentOutOfRange_Month);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_DayNumber(int dayNumber)
        //{
        //    throw new ArgumentOutOfRangeException(nameof(dayNumber), dayNumber, SR.ArgumentOutOfRange_DayNumber);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_BadYearMonthDay()
        //{
        //    throw new ArgumentOutOfRangeException(null, SR.ArgumentOutOfRange_BadYearMonthDay);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_BadHourMinuteSecond()
        //{
        //    throw new ArgumentOutOfRangeException(null, SR.ArgumentOutOfRange_BadHourMinuteSecond);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_TimeSpanTooLong()
        //{
        //    throw new ArgumentOutOfRangeException(null, SR.Overflow_TimeSpanTooLong);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_RoundingDigits(string name)
        //{
        //    throw new ArgumentOutOfRangeException(name, SR.ArgumentOutOfRange_RoundingDigits);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_RoundingDigits_MathF(string name)
        //{
        //    throw new ArgumentOutOfRangeException(name, SR.ArgumentOutOfRange_RoundingDigits_MathF);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRange_Range<T>(string parameterName, T value, T minInclusive, T maxInclusive)
        //{
        //    throw new ArgumentOutOfRangeException(parameterName, value, SR.Format(SR.ArgumentOutOfRange_Range, minInclusive, maxInclusive));
        //}

        [DoesNotReturn]
        internal static void ThrowOverflowException()
        {
            throw new OverflowException();
        }

        //[DoesNotReturn]
        //internal static void ThrowOverflowException_NegateTwosCompNum()
        //{
        //    throw new OverflowException(SR.Overflow_NegateTwosCompNum);
        //}

        //[DoesNotReturn]
        //internal static void ThrowOverflowException_TimeSpanTooLong()
        //{
        //    throw new OverflowException(SR.Overflow_TimeSpanTooLong);
        //}

        //[DoesNotReturn]
        //internal static void ThrowOverflowException_TimeSpanDuration()
        //{
        //    throw new OverflowException(SR.Overflow_Duration);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_Arg_CannotBeNaN()
        //{
        //    throw new ArgumentException(SR.Arg_CannotBeNaN);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_Arg_CannotBeNaN(ExceptionArgument argument)
        //{
        //    throw new ArgumentException(SR.Arg_CannotBeNaN, GetArgumentName(argument));
        //}

        [DoesNotReturn]
        internal static void ThrowWrongKeyTypeArgumentException<T>(T key, Type targetType)
        {
            // Generic key to move the boxing to the right hand side of throw
            throw GetWrongKeyTypeArgumentException((object?)key, targetType);
        }

        [DoesNotReturn]
        internal static void ThrowWrongValueTypeArgumentException<T>([AllowNull] T value, Type targetType)
        {
            // Generic key to move the boxing to the right hand side of throw
            throw GetWrongValueTypeArgumentException((object?)value, targetType);
        }

        private static ArgumentException GetAddingDuplicateWithKeyArgumentException(object? key)
        {
            return new ArgumentException(SR.Format(SR.Argument_AddingDuplicateWithKey, key));
        }

        [DoesNotReturn]
        internal static void ThrowAddingDuplicateWithKeyArgumentException<T>([AllowNull] T key)
        {
            // Generic key to move the boxing to the right hand side of throw
            throw GetAddingDuplicateWithKeyArgumentException((object?)key);
        }

        [DoesNotReturn]
        internal static void ThrowKeyNotFoundException<T>(T key)
        {
            // Generic key to move the boxing to the right hand side of throw
            throw GetKeyNotFoundException((object?)key);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException(ExceptionResource resource)
        {
            throw GetArgumentException(resource);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException(ExceptionResource resource, ExceptionArgument argument)
        {
            throw GetArgumentException(resource, argument);
        }

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_HandleNotSync(string paramName)
        //{
        //    throw new ArgumentException(SR.Arg_HandleNotSync, paramName);
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_HandleNotAsync(string paramName)
        //{
        //    throw new ArgumentException(SR.Arg_HandleNotAsync, paramName);
        //}

        [DoesNotReturn]
        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(GetArgumentName(argument));
        }

        [DoesNotReturn]
        internal static void ThrowArgumentNullException(ExceptionResource resource)
        {
            throw new ArgumentNullException(GetResourceString(resource));
        }

        [DoesNotReturn]
        internal static void ThrowArgumentNullException(ExceptionArgument argument, ExceptionResource resource)
        {
            throw new ArgumentNullException(GetArgumentName(argument), GetResourceString(resource));
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
        {
            throw GetArgumentOutOfRangeException(argument, resource);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, int paramNumber, ExceptionResource resource)
        {
            throw GetArgumentOutOfRangeException(argument, paramNumber, resource);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException<T>(T actualValue, ExceptionArgument argument, ExceptionResource resource)
        {
            throw GetArgumentOutOfRangeException(actualValue, argument, resource);
        }

        //[DoesNotReturn]
        //internal static void ThrowEndOfFileException()
        //{
        //    throw CreateEndOfFileException();
        //}

        //internal static Exception CreateEndOfFileException() =>
        //    new EndOfStreamException(SR.IO_EOF_ReadBeyondEOF);

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException();
        }

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException(ExceptionResource resource)
        {
            throw GetInvalidOperationException(resource);
        }

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException(ExceptionResource resource, Exception e)
        {
            throw new InvalidOperationException(GetResourceString(resource), e);
        }

        //[DoesNotReturn]
        //internal static void ThrowNullReferenceException()
        //{
        //    throw new NullReferenceException(SR.Arg_NullArgumentNullRef);
        //}

        [DoesNotReturn]
        internal static void ThrowSerializationException(ExceptionResource resource)
        {
            throw new SerializationException(GetResourceString(resource));
        }

        [DoesNotReturn]
        internal static void ThrowRankException(ExceptionResource resource)
        {
            throw new RankException(GetResourceString(resource));
        }

        [DoesNotReturn]
        internal static void ThrowNotSupportedException(ExceptionResource resource)
        {
            throw new NotSupportedException(GetResourceString(resource));
        }

        //[DoesNotReturn]
        //internal static void ThrowNotSupportedException_UnseekableStream()
        //{
        //    throw new NotSupportedException(SR.NotSupported_UnseekableStream);
        //}

        //[DoesNotReturn]
        //internal static void ThrowNotSupportedException_UnreadableStream()
        //{
        //    throw new NotSupportedException(SR.NotSupported_UnreadableStream);
        //}

        //[DoesNotReturn]
        //internal static void ThrowNotSupportedException_UnwritableStream()
        //{
        //    throw new NotSupportedException(SR.NotSupported_UnwritableStream);
        //}

        [DoesNotReturn]
        internal static void ThrowObjectDisposedException(object? instance)
        {
            throw new ObjectDisposedException(instance?.GetType().FullName);
        }

        [DoesNotReturn]
        internal static void ThrowObjectDisposedException(Type? type)
        {
            throw new ObjectDisposedException(type?.FullName);
        }

        //[DoesNotReturn]
        //internal static void ThrowObjectDisposedException_StreamClosed(string? objectName)
        //{
        //    throw new ObjectDisposedException(objectName, SR.ObjectDisposed_StreamClosed);
        //}

        //[DoesNotReturn]
        //internal static void ThrowObjectDisposedException_FileClosed()
        //{
        //    throw new ObjectDisposedException(null, SR.ObjectDisposed_FileClosed);
        //}

        [DoesNotReturn]
        internal static void ThrowObjectDisposedException(ExceptionResource resource)
        {
            throw new ObjectDisposedException(null, GetResourceString(resource));
        }

        [DoesNotReturn]
        internal static void ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }

        [DoesNotReturn]
        internal static void ThrowAggregateException(List<Exception> exceptions)
        {
            throw new AggregateException(exceptions);
        }

        [DoesNotReturn]
        internal static void ThrowOutOfMemoryException()
        {
            throw new OutOfMemoryException();
        }

        [DoesNotReturn]
        internal static void ThrowDivideByZeroException()
        {
            throw new DivideByZeroException();
        }

        //[DoesNotReturn]
        //internal static void ThrowOutOfMemoryException_StringTooLong()
        //{
        //    throw new OutOfMemoryException(SR.OutOfMemory_StringTooLong);
        //}

        //[DoesNotReturn]
        //internal static void ThrowOutOfMemoryException_LockEnter_WaiterCountOverflow()
        //{
        //    throw new OutOfMemoryException(SR.Lock_Enter_WaiterCountOverflow_OutOfMemoryException);
        //}

        [DoesNotReturn]
        internal static void ThrowArgumentException_Argument_IncompatibleArrayType()
        {
            throw new ArgumentException(SR.Argument_IncompatibleArrayType);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException_Argument_IncompatibleArrayType(ExceptionArgument argument)
        {
            throw new ArgumentException(SR.Argument_IncompatibleArrayType, GetArgumentName(argument));
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException_Argument_InvalidCodePoint(int codePoint)
        {
            throw new ArgumentException(SR.Format(SR.Argument_InvalidCodePoint, codePoint));
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException_Argument_MinMaxValue(ExceptionArgument min, ExceptionArgument max)
        {
            throw new ArgumentException(SR.Format(SR.Argument_MinMaxValue, GetArgumentName(min), GetArgumentName(max)));
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException_Argument_MustNotBeThis(ExceptionArgument argument)
        {
            throw new ArgumentException(SR.Format(SR.Argument_MustNotBeThis, GetArgumentName(argument)));
        }

        //[DoesNotReturn]
        //internal static void ThrowArgumentException_InvalidHandle(string? paramName)
        //{
        //    throw new ArgumentException(SR.Arg_InvalidHandle, paramName);
        //}

        //[DoesNotReturn]
        //internal static void ThrowUnexpectedStateForKnownCallback(object? state)
        //{
        //    throw new ArgumentOutOfRangeException(nameof(state), state, SR.Argument_UnexpectedStateForKnownCallback);
        //}

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_InvalidOperation_EnumNotStarted()
        {
            throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
        }

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_InvalidOperation_EnumEnded()
        {
            throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
        }

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_EnumCurrent(int index)
        {
            throw GetInvalidOperationException_EnumCurrent(index);
        }

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
        {
            throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
        }

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen()
        {
            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
        }

        //[DoesNotReturn]
        //internal static void ThrowInvalidOperationException_InvalidOperation_NoValue()
        //{
        //    throw new InvalidOperationException(SR.InvalidOperation_NoValue);
        //}

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_ConcurrentOperationsNotSupported()
        {
            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
        }

        //[DoesNotReturn]
        //internal static void ThrowInvalidOperationException_HandleIsNotInitialized()
        //{
        //    throw new InvalidOperationException(SR.InvalidOperation_HandleIsNotInitialized);
        //}

        //[DoesNotReturn]
        //internal static void ThrowInvalidOperationException_HandleIsNotPinned()
        //{
        //    throw new InvalidOperationException(SR.InvalidOperation_HandleIsNotPinned);
        //}

        [DoesNotReturn]
        internal static void ThrowArraySegmentCtorValidationFailedExceptions(Array? array, int offset, int count)
        {
            throw GetArraySegmentCtorValidationFailedException(array, offset, count);
        }

        //[DoesNotReturn]
        //internal static void ThrowInvalidOperationException_InvalidUtf8()
        //{
        //    throw new InvalidOperationException(SR.InvalidOperation_InvalidUtf8);
        //}

        [DoesNotReturn]
        internal static void ThrowFormatException(ReadOnlySpan<char> value)
        {
            throw new FormatException(J2N.SR.Format(SR.Format_InvalidString, value.ToString()));
        }

        [DoesNotReturn]
        internal static void ThrowFormatException(string value)
        {
            throw new FormatException(J2N.SR.Format(SR.Format_InvalidString, value));
        }

        [DoesNotReturn]
        internal static void ThrowFormatException_BadFormatSpecifier()
        {
            throw new FormatException(SR.Argument_BadFormatSpecifier);
        }

        //[DoesNotReturn]
        //internal static void ThrowFormatException_NeedSingleChar()
        //{
        //    throw new FormatException(SR.Format_NeedSingleChar);
        //}

        //[DoesNotReturn]
        //internal static void ThrowFormatException_BadBoolean(ReadOnlySpan<char> value)
        //{
        //    throw new FormatException(SR.Format(SR.Format_BadBoolean, new string(value)));
        //}

        //[DoesNotReturn]
        //internal static void ThrowArgumentOutOfRangeException_PrecisionTooLarge()
        //{
        //    throw new ArgumentOutOfRangeException("precision", SR.Format(SR.Argument_PrecisionTooLarge, StandardFormat.MaxPrecision));
        //}

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException_SymbolDoesNotFit()
        {
            throw new ArgumentOutOfRangeException("symbol", SR.Argument_BadFormatSpecifier);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException_NeedNonNegNum(string paramName)
        {
            throw new ArgumentOutOfRangeException(paramName, SR.ArgumentOutOfRange_NeedNonNegNum);
        }

        //[DoesNotReturn]
        //internal static void ArgumentOutOfRangeException_Enum_Value()
        //{
        //    throw new ArgumentOutOfRangeException("value", SR.ArgumentOutOfRange_Enum);
        //}

        //[DoesNotReturn]
        //internal static void ThrowApplicationException(int hr)
        //{
        //    // Get a message for this HR
        //    Exception? ex = Marshal.GetExceptionForHR(hr);
        //    if (ex != null && !string.IsNullOrEmpty(ex.Message))
        //    {
        //        ex = new ApplicationException(ex.Message);
        //    }
        //    else
        //    {
        //        ex = new ApplicationException();
        //    }

        //    ex.HResult = hr;
        //    throw ex;
        //}

        [DoesNotReturn]
        internal static void ThrowFormatInvalidString()
        {
            throw new FormatException(SR.Format_InvalidString);
        }

        //[DoesNotReturn]
        //internal static void ThrowFormatInvalidString(int offset, ExceptionResource resource)
        //{
        //    throw new FormatException(SR.Format(SR.Format_InvalidStringWithOffsetAndReason, offset, GetResourceString(resource)));
        //}

        //[DoesNotReturn]
        //internal static void ThrowFormatIndexOutOfRange()
        //{
        //    throw new FormatException(SR.Format_IndexOutOfRange);
        //}

        //[DoesNotReturn]
        //internal static void ThrowSynchronizationLockException_LockExit()
        //{
        //    throw new SynchronizationLockException(SR.Lock_Exit_SynchronizationLockException);
        //}

        //internal static AmbiguousMatchException GetAmbiguousMatchException(MemberInfo memberInfo)
        //{
        //    Type? declaringType = memberInfo.DeclaringType;
        //    return new AmbiguousMatchException(SR.Format(SR.Arg_AmbiguousMatchException_MemberInfo, declaringType, memberInfo));
        //}

        //internal static AmbiguousMatchException GetAmbiguousMatchException(Attribute attribute)
        //{
        //    return new AmbiguousMatchException(SR.Format(SR.Arg_AmbiguousMatchException_Attribute, attribute));
        //}

        //internal static AmbiguousMatchException GetAmbiguousMatchException(CustomAttributeData customAttributeData)
        //{
        //    return new AmbiguousMatchException(SR.Format(SR.Arg_AmbiguousMatchException_CustomAttributeData, customAttributeData));
        //}

        private static Exception GetArraySegmentCtorValidationFailedException(Array? array, int offset, int count)
        {
            if (array == null)
                return new ArgumentNullException(nameof(array));
            if (offset < 0)
                return new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                return new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);

            Debug.Assert(array.Length - offset < count);
            return new ArgumentException(SR.Argument_InvalidOffLen);
        }

        private static ArgumentException GetArgumentException(ExceptionResource resource)
        {
            return new ArgumentException(GetResourceString(resource));
        }

        private static InvalidOperationException GetInvalidOperationException(ExceptionResource resource)
        {
            return new InvalidOperationException(GetResourceString(resource));
        }

        private static ArgumentException GetWrongKeyTypeArgumentException(object? key, Type targetType)
        {
            return new ArgumentException(SR.Format(SR.Arg_WrongType, key, targetType), nameof(key));
        }

        private static ArgumentException GetWrongValueTypeArgumentException(object? value, Type targetType)
        {
            return new ArgumentException(SR.Format(SR.Arg_WrongType, value, targetType), nameof(value));
        }

        private static KeyNotFoundException GetKeyNotFoundException(object? key)
        {
            return new KeyNotFoundException(SR.Format(SR.Arg_KeyNotFoundWithKey, key));
        }

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(argument), GetResourceString(resource));
        }

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(object? actualValue, ExceptionArgument argument, ExceptionResource resource)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(argument), actualValue, GetResourceString(resource));
        }

        private static ArgumentException GetArgumentException(ExceptionResource resource, ExceptionArgument argument)
        {
            return new ArgumentException(GetResourceString(resource), GetArgumentName(argument));
        }

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument, int paramNumber, ExceptionResource resource)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(argument) + "[" + paramNumber.ToString() + "]", GetResourceString(resource));
        }

        private static InvalidOperationException GetInvalidOperationException_EnumCurrent(int index)
        {
            return new InvalidOperationException(
                index < 0 ?
                SR.InvalidOperation_EnumNotStarted :
                SR.InvalidOperation_EnumEnded);
        }

        // Allow nulls for reference types and Nullable<U>, but not for value types.
        // Aggressively inline so the jit evaluates the if in place and either drops the call altogether
        // Or just leaves null test and call to the Non-returning ThrowHelper.ThrowArgumentNullException
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IfNullAndNullsAreIllegalThenThrow<T>(object? value, ExceptionArgument argName)
        {
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
            if (!(default(T) == null) && value == null)
                ThrowArgumentNullException(argName);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void ThrowForUnsupportedSimdVectorBaseType<TVector, T>()
        //    where TVector : ISimdVector<TVector, T>
        //{
        //    if (!TVector.IsSupported)
        //    {
        //        ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
        //    }
        //}

        //// Throws if 'T' is disallowed in Vector<T> in the Numerics namespace.
        //// If 'T' is allowed, no-ops. JIT will elide the method entirely if 'T'
        //// is supported and we're on an optimized release build.
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void ThrowForUnsupportedNumericsVectorBaseType<T>()
        //{
        //    if (!Vector<T>.IsSupported)
        //    {
        //        ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
        //    }
        //}

        //// Throws if 'T' is disallowed in Vector64<T> in the Intrinsics namespace.
        //// If 'T' is allowed, no-ops. JIT will elide the method entirely if 'T'
        //// is supported and we're on an optimized release build.
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void ThrowForUnsupportedIntrinsicsVector64BaseType<T>()
        //{
        //    if (!Vector64<T>.IsSupported)
        //    {
        //        ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
        //    }
        //}

        //// Throws if 'T' is disallowed in Vector128<T> in the Intrinsics namespace.
        //// If 'T' is allowed, no-ops. JIT will elide the method entirely if 'T'
        //// is supported and we're on an optimized release build.
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void ThrowForUnsupportedIntrinsicsVector128BaseType<T>()
        //{
        //    if (!Vector128<T>.IsSupported)
        //    {
        //        ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
        //    }
        //}

        //// Throws if 'T' is disallowed in Vector256<T> in the Intrinsics namespace.
        //// If 'T' is allowed, no-ops. JIT will elide the method entirely if 'T'
        //// is supported and we're on an optimized release build.
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void ThrowForUnsupportedIntrinsicsVector256BaseType<T>()
        //{
        //    if (!Vector256<T>.IsSupported)
        //    {
        //        ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
        //    }
        //}

        //// Throws if 'T' is disallowed in Vector512<T> in the Intrinsics namespace.
        //// If 'T' is allowed, no-ops. JIT will elide the method entirely if 'T'
        //// is supported and we're on an optimized release build.
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void ThrowForUnsupportedIntrinsicsVector512BaseType<T>()
        //{
        //    if (!Vector512<T>.IsSupported)
        //    {
        //        ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
        //    }
        //}

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
                case ExceptionArgument.action:
                    return "action";
                case ExceptionArgument.allocSize:
                    return "allocSize";
                case ExceptionArgument.appendable:
                    return "appendable";
                case ExceptionArgument.array:
                    return "array";
                case ExceptionArgument.arrayIndex:
                    return "arrayIndex";
                case ExceptionArgument.assembly:
                    return "assembly";
                case ExceptionArgument.bitSet:
                    return "bitSet";
                case ExceptionArgument.buffer:
                    return "buffer";
                case ExceptionArgument.capacity:
                    return "capacity";
                case ExceptionArgument.characterSequence:
                    return "characterSequence";
                case ExceptionArgument.charCount:
                    return "charCount";
                case ExceptionArgument.charSequence: // J2N TODO: Normalize to characterSequence
                    return "charSequence";
                case ExceptionArgument.codePoints:
                    return "codePoints";
                case ExceptionArgument.collection:
                    return "collection";
                case ExceptionArgument.comments:
                    return "comments";
                case ExceptionArgument.comparer:
                    return "comparer";
                case ExceptionArgument.comparison:
                    return "comparison";
                case ExceptionArgument.converter:
                    return "converter";
                case ExceptionArgument.count:
                    return "count";
                case ExceptionArgument.culture:
                    return "culture";
                case ExceptionArgument.currPos:
                    return "currPos";
                case ExceptionArgument.delimiters:
                    return "delimiters";
                case ExceptionArgument.destination:
                    return "destination";
                case ExceptionArgument.destinationIndex:
                    return "destinationIndex";
                case ExceptionArgument.dictionary:
                    return "dictionary";
                case ExceptionArgument.encoding:
                    return "encoding";
                case ExceptionArgument.enumerator:
                    return "enumerator";
                case ExceptionArgument.equals:
                    return "equals";
                case ExceptionArgument.fnCondition:
                    return "fnCondition";
                case ExceptionArgument.fnCreate:
                    return "fnCreate";
                case ExceptionArgument.fnUpdate:
                    return "fnUpdate";
                case ExceptionArgument.formatProvider:
                    return "formatProvider";
                case ExceptionArgument.hashSize:
                    return "hashSize";
                case ExceptionArgument.index:
                    return "index";
                case ExceptionArgument.index1:
                    return "index1";
                case ExceptionArgument.index2:
                    return "index2";
                case ExceptionArgument.index3:
                    return "index3";
                case ExceptionArgument.info:
                    return "info";
                case ExceptionArgument.input:
                    return "input";
                case ExceptionArgument.interfaceType:
                    return "interfaceType";
                case ExceptionArgument.item:
                    return "item";
                case ExceptionArgument.key:
                    return "key";
                case ExceptionArgument.length:
                    return "length";
                case ExceptionArgument.limit:
                    return "limit";
                case ExceptionArgument.list:
                    return "list";
                case ExceptionArgument.lockSize:
                    return "lockSize";
                case ExceptionArgument.match:
                    return "match";
                case ExceptionArgument.maxValue:
                    return "maxValue";
                case ExceptionArgument.memoryMappedFile:
                    return "memoryMappedFile";
                case ExceptionArgument.minValue:
                    return "minValue";
                case ExceptionArgument.name:
                    return "name";
                case ExceptionArgument.nbits:
                    return "nbits";
                case ExceptionArgument.newLength:
                    return "newLength";
                case ExceptionArgument.newLimit:
                    return "newLimit";
                case ExceptionArgument.newPosition:
                    return "newPosition";
                case ExceptionArgument.newValue:
                    return "newValue";
                case ExceptionArgument.offset:
                    return "offset";
                case ExceptionArgument.original:
                    return "original";
                case ExceptionArgument.other:
                    return "other";
                case ExceptionArgument.output:
                    return "output";
                case ExceptionArgument.owner:
                    return "owner";
                case ExceptionArgument.position:
                    return "position";
                case ExceptionArgument.position1:
                    return "position1";
                case ExceptionArgument.position2:
                    return "position2";
                case ExceptionArgument.priorityQueue:
                    return "priorityQueue";
                case ExceptionArgument.prefix:
                    return "prefix";
                case ExceptionArgument.properties:
                    return "properties";
                case ExceptionArgument.radix:
                    return "radix";
                case ExceptionArgument.random:
                    return "random";
                case ExceptionArgument.reader:
                    return "reader";
                case ExceptionArgument.s:
                    return "s";
                case ExceptionArgument.seq:
                    return "seq";
                case ExceptionArgument.set:
                    return "set";
                case ExceptionArgument.source:
                    return "source";
                case ExceptionArgument.sourceIndex:
                    return "sourceIndex";
                case ExceptionArgument.start:
                    return "start";
                case ExceptionArgument.startIndex:
                    return "startIndex";
                case ExceptionArgument.str:
                    return "str";
                case ExceptionArgument.structuralEqualityComparer:
                    return "structuralEqualityComparer";
                case ExceptionArgument.target:
                    return "target";
                case ExceptionArgument.targetType:
                    return "targetType";
                case ExceptionArgument.text:
                    return "text";
                case ExceptionArgument.threadStart:
                    return "threadStart";
                case ExceptionArgument.toStringFormatProvider:
                    return "toStringFormatProvider";
                case ExceptionArgument.type:
                    return "type";
                case ExceptionArgument.uiCulture:
                    return "uiCulture";
                case ExceptionArgument.value:
                    return "value";
                case ExceptionArgument.writer:
                    return "writer";
                case ExceptionArgument.year:
                    return "year";


                //case ExceptionArgument.obj:
                //    return "obj";




                //case ExceptionArgument.values:
                //    return "values";


                //case ExceptionArgument.task:
                //    return "task";
                //case ExceptionArgument.bytes:
                //    return "bytes";
                //case ExceptionArgument.byteIndex:
                //    return "byteIndex";
                //case ExceptionArgument.byteCount:
                //    return "byteCount";
                //case ExceptionArgument.ch:
                //    return "ch";
                //case ExceptionArgument.chars:
                //    return "chars";
                //case ExceptionArgument.charIndex:
                //    return "charIndex";



                //case ExceptionArgument.ownedMemory:
                //    return "ownedMemory";










                //case ExceptionArgument.exceptions:
                //    return "exceptions";
                //case ExceptionArgument.exception:
                //    return "exception";
                //case ExceptionArgument.pointer:
                //    return "pointer";

                //case ExceptionArgument.format:
                //    return "format";
                //case ExceptionArgument.formats:
                //    return "formats";


                //case ExceptionArgument.comparable:
                //    return "comparable";


                //case ExceptionArgument.comparisonType:
                //    return "comparisonType";
                //case ExceptionArgument.manager:
                //    return "manager";
                //case ExceptionArgument.sourceBytesToCopy:
                //    return "sourceBytesToCopy";
                //case ExceptionArgument.callBack:
                //    return "callBack";
                //case ExceptionArgument.creationOptions:
                //    return "creationOptions";
                //case ExceptionArgument.function:
                //    return "function";
                //case ExceptionArgument.scheduler:
                //    return "scheduler";
                //case ExceptionArgument.continuation:
                //    return "continuation";
                //case ExceptionArgument.continuationAction:
                //    return "continuationAction";
                //case ExceptionArgument.continuationFunction:
                //    return "continuationFunction";
                //case ExceptionArgument.tasks:
                //    return "tasks";
                //case ExceptionArgument.asyncResult:
                //    return "asyncResult";
                //case ExceptionArgument.beginMethod:
                //    return "beginMethod";
                //case ExceptionArgument.endMethod:
                //    return "endMethod";
                //case ExceptionArgument.endFunction:
                //    return "endFunction";
                //case ExceptionArgument.cancellationToken:
                //    return "cancellationToken";
                //case ExceptionArgument.continuationOptions:
                //    return "continuationOptions";
                //case ExceptionArgument.delay:
                //    return "delay";
                //case ExceptionArgument.millisecondsDelay:
                //    return "millisecondsDelay";
                //case ExceptionArgument.millisecondsTimeout:
                //    return "millisecondsTimeout";
                //case ExceptionArgument.stateMachine:
                //    return "stateMachine";
                //case ExceptionArgument.timeout:
                //    return "timeout";
                //case ExceptionArgument.type:
                //    return "type";

                //case ExceptionArgument.sourceArray:
                //    return "sourceArray";

                //case ExceptionArgument.destinationArray:
                //    return "destinationArray";
                //case ExceptionArgument.pHandle:
                //    return "pHandle";
                //case ExceptionArgument.handle:
                //    return "handle";

                //case ExceptionArgument.newSize:
                //    return "newSize";
                //case ExceptionArgument.lengths:
                //    return "lengths";
                //case ExceptionArgument.len:
                //    return "len";
                //case ExceptionArgument.keys:
                //    return "keys";
                //case ExceptionArgument.indices:
                //    return "indices";

                //case ExceptionArgument.endIndex:
                //    return "endIndex";
                //case ExceptionArgument.elementType:
                //    return "elementType";


                //case ExceptionArgument.codePoint:
                //    return "codePoint";

                //case ExceptionArgument.options:
                //    return "options";

                //case ExceptionArgument.suffix:
                //    return "suffix";

                //case ExceptionArgument.buffers:
                //    return "buffers";

                //case ExceptionArgument.stream:
                //    return "stream";
                //case ExceptionArgument.anyOf:
                //    return "anyOf";
                //case ExceptionArgument.overlapped:
                //    return "overlapped";
                //case ExceptionArgument.minimumBytes:
                //    return "minimumBytes";
                //case ExceptionArgument.arrayType:
                //    return "arrayType";
                //case ExceptionArgument.divisor:
                //    return "divisor";
                //case ExceptionArgument.factor:
                //    return "factor";

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
                case ExceptionResource.Arg_EmptySpan:
                    return SR.Arg_EmptySpan;
                case ExceptionResource.Arg_HSCapacityOverflow:
                    return SR.Arg_HSCapacityOverflow;
                case ExceptionResource.ArgumentNull_NullOrNullValue:
                    return SR.ArgumentNull_NullOrNullValue;
                case ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegative:
                    return SR.ArgumentOutOfRange_Generic_MustBeNonNegative;
                case ExceptionResource.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero:
                    return SR.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero;
                case ExceptionResource.ArgumentOutOfRange_Capacity:
                    return SR.ArgumentOutOfRange_Capacity;
                case ExceptionResource.ArgumentOutOfRange_OffsetOut:
                    return SR.ArgumentOutOfRange_OffsetOut;
                case ExceptionResource.ArgumentOutOfRange_StartIndexLargerThanLength:
                    return SR.ArgumentOutOfRange_StartIndexLargerThanLength;

                case ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual:
                    return SR.ArgumentOutOfRange_IndexMustBeLessOrEqual;
                case ExceptionResource.ArgumentOutOfRange_IndexMustBeLess:
                    return SR.ArgumentOutOfRange_IndexMustBeLess;
                //case ExceptionResource.ArgumentOutOfRange_IndexCount:
                //    return SR.ArgumentOutOfRange_IndexCount;
                //case ExceptionResource.ArgumentOutOfRange_IndexCountBuffer:
                //    return SR.ArgumentOutOfRange_IndexCountBuffer;
                case ExceptionResource.ArgumentOutOfRange_IndexLength:
                    return SR.ArgumentOutOfRange_IndexLength;
                case ExceptionResource.ArgumentOutOfRange_IndexLengthArray:
                    return SR.ArgumentOutOfRange_IndexLengthArray;
                case ExceptionResource.ArgumentOutOfRange_IndexString:
                    return SR.ArgumentOutOfRange_IndexString;
                case ExceptionResource.ArgumentOutOfRange_Count:
                    return SR.ArgumentOutOfRange_Count;
                case ExceptionResource.ArgumentOutOfRange_Radix:
                    return SR.ArgumentOutOfRange_Radix;
                //case ExceptionResource.ArgumentOutOfRange_Year:
                //    return SR.ArgumentOutOfRange_Year;
                case ExceptionResource.Arg_ArrayPlusOffTooSmall:
                    return SR.Arg_ArrayPlusOffTooSmall;
                //case ExceptionResource.Arg_ByteArrayTooSmallForValue:
                //    return SR.Arg_ByteArrayTooSmallForValue;
                case ExceptionResource.NotSupported_ReadOnlyCollection:
                    return SR.NotSupported_ReadOnlyCollection;
                case ExceptionResource.Arg_RankMultiDimNotSupported:
                    return SR.Arg_RankMultiDimNotSupported;
                case ExceptionResource.Arg_NonZeroLowerBound:
                    return SR.Arg_NonZeroLowerBound;
                //case ExceptionResource.ArgumentOutOfRange_GetCharCountOverflow:
                //    return SR.ArgumentOutOfRange_GetCharCountOverflow;
                case ExceptionResource.ArgumentOutOfRange_ListInsert:
                    return SR.ArgumentOutOfRange_ListInsert;
                case ExceptionResource.ArgumentOutOfRange_NeedNonNegNum:
                    return SR.ArgumentOutOfRange_NeedNonNegNum;

                case ExceptionResource.ArgumentOutOfRange_SmallCapacity:
                    return SR.ArgumentOutOfRange_SmallCapacity;
                case ExceptionResource.Argument_InvalidOffLen:
                    return SR.Argument_InvalidOffLen;
                //case ExceptionResource.Argument_CannotExtractScalar:
                //    return SR.Argument_CannotExtractScalar;
                case ExceptionResource.ArgumentOutOfRange_BiggerThanCollection:
                    return SR.ArgumentOutOfRange_BiggerThanCollection;
                case ExceptionResource.Serialization_MissingKeys:
                    return SR.Serialization_MissingKeys;
                //case ExceptionResource.Serialization_NullKey:
                //    return SR.Serialization_NullKey;
                case ExceptionResource.Serialization_MismatchedCount:
                    return SR.Serialization_MismatchedCount;
                case ExceptionResource.NotSupported_KeyCollectionSet:
                    return SR.NotSupported_KeyCollectionSet;
                case ExceptionResource.NotSupported_ValueCollectionSet:
                    return SR.NotSupported_ValueCollectionSet;
                //case ExceptionResource.InvalidOperation_NullArray:
                //    return SR.InvalidOperation_NullArray;
                //case ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted:
                //    return SR.TaskT_TransitionToFinal_AlreadyCompleted;
                //case ExceptionResource.TaskCompletionSourceT_TrySetException_NullException:
                //    return SR.TaskCompletionSourceT_TrySetException_NullException;
                //case ExceptionResource.TaskCompletionSourceT_TrySetException_NoExceptions:
                //    return SR.TaskCompletionSourceT_TrySetException_NoExceptions;
                //case ExceptionResource.NotSupported_StringComparison:
                //    return SR.NotSupported_StringComparison;
                case ExceptionResource.ConcurrentCollection_SyncRoot_NotSupported:
                    return SR.ConcurrentCollection_SyncRoot_NotSupported;
                //case ExceptionResource.Task_MultiTaskContinuation_NullTask:
                //    return SR.Task_MultiTaskContinuation_NullTask;
                //case ExceptionResource.InvalidOperation_WrongAsyncResultOrEndCalledMultiple:
                //    return SR.InvalidOperation_WrongAsyncResultOrEndCalledMultiple;
                //case ExceptionResource.Task_MultiTaskContinuation_EmptyTaskList:
                //    return SR.Task_MultiTaskContinuation_EmptyTaskList;
                //case ExceptionResource.Task_Start_TaskCompleted:
                //    return SR.Task_Start_TaskCompleted;
                //case ExceptionResource.Task_Start_Promise:
                //    return SR.Task_Start_Promise;
                //case ExceptionResource.Task_Start_ContinuationTask:
                //    return SR.Task_Start_ContinuationTask;
                //case ExceptionResource.Task_Start_AlreadyStarted:
                //    return SR.Task_Start_AlreadyStarted;
                //case ExceptionResource.Task_RunSynchronously_Continuation:
                //    return SR.Task_RunSynchronously_Continuation;
                //case ExceptionResource.Task_RunSynchronously_Promise:
                //    return SR.Task_RunSynchronously_Promise;
                //case ExceptionResource.Task_RunSynchronously_TaskCompleted:
                //    return SR.Task_RunSynchronously_TaskCompleted;
                //case ExceptionResource.Task_RunSynchronously_AlreadyStarted:
                //    return SR.Task_RunSynchronously_AlreadyStarted;
                //case ExceptionResource.AsyncMethodBuilder_InstanceNotInitialized:
                //    return SR.AsyncMethodBuilder_InstanceNotInitialized;
                //case ExceptionResource.Task_ContinueWith_ESandLR:
                //    return SR.Task_ContinueWith_ESandLR;
                //case ExceptionResource.Task_ContinueWith_NotOnAnything:
                //    return SR.Task_ContinueWith_NotOnAnything;
                //case ExceptionResource.Task_InvalidTimerTimeSpan:
                //    return SR.Task_InvalidTimerTimeSpan;
                //case ExceptionResource.Task_Delay_InvalidMillisecondsDelay:
                //    return SR.Task_Delay_InvalidMillisecondsDelay;
                //case ExceptionResource.Task_Dispose_NotCompleted:
                //    return SR.Task_Dispose_NotCompleted;
                //case ExceptionResource.Task_ThrowIfDisposed:
                //    return SR.Task_ThrowIfDisposed;
                //case ExceptionResource.Task_WaitMulti_NullTask:
                //    return SR.Task_WaitMulti_NullTask;
                //case ExceptionResource.ArgumentException_OtherNotArrayOfCorrectLength:
                //    return SR.ArgumentException_OtherNotArrayOfCorrectLength;
                //case ExceptionResource.ArgumentNull_Array:
                //    return SR.ArgumentNull_Array;
                //case ExceptionResource.ArgumentNull_SafeHandle:
                //    return SR.ArgumentNull_SafeHandle;
                //case ExceptionResource.ArgumentOutOfRange_EndIndexStartIndex:
                //    return SR.ArgumentOutOfRange_EndIndexStartIndex;
                //case ExceptionResource.ArgumentOutOfRange_Enum:
                //    return SR.ArgumentOutOfRange_Enum;
                //case ExceptionResource.ArgumentOutOfRange_HugeArrayNotSupported:
                //    return SR.ArgumentOutOfRange_HugeArrayNotSupported;
                case ExceptionResource.Argument_AddingDuplicate:
                    return SR.Argument_AddingDuplicate;
                case ExceptionResource.Argument_InvalidArgumentForComparison:
                    return SR.Argument_InvalidArgumentForComparison;
                //case ExceptionResource.Arg_LowerBoundsMustMatch:
                //    return SR.Arg_LowerBoundsMustMatch;
                //case ExceptionResource.Arg_MustBeType:
                //    return SR.Arg_MustBeType;
                case ExceptionResource.Arg_Need1DArray:
                    return SR.Arg_Need1DArray;
                //case ExceptionResource.Arg_Need2DArray:
                //    return SR.Arg_Need2DArray;
                //case ExceptionResource.Arg_Need3DArray:
                //    return SR.Arg_Need3DArray;
                //case ExceptionResource.Arg_NeedAtLeast1Rank:
                //    return SR.Arg_NeedAtLeast1Rank;
                //case ExceptionResource.Arg_RankIndices:
                //    return SR.Arg_RankIndices;
                //case ExceptionResource.Arg_RanksAndBounds:
                //    return SR.Arg_RanksAndBounds;
                case ExceptionResource.InvalidOperation_IComparerFailed:
                    return SR.InvalidOperation_IComparerFailed;
                //case ExceptionResource.NotSupported_FixedSizeCollection:
                //    return SR.NotSupported_FixedSizeCollection;
                //case ExceptionResource.Rank_MultiDimNotSupported:
                //    return SR.Rank_MultiDimNotSupported;
                //case ExceptionResource.Arg_TypeNotSupported:
                //    return SR.Arg_TypeNotSupported;
                //case ExceptionResource.Argument_SpansMustHaveSameLength:
                //    return SR.Argument_SpansMustHaveSameLength;
                //case ExceptionResource.Argument_InvalidFlag:
                //    return SR.Argument_InvalidFlag;
                //case ExceptionResource.CancellationTokenSource_Disposed:
                //    return SR.CancellationTokenSource_Disposed;
                //case ExceptionResource.Argument_AlignmentMustBePow2:
                //    return SR.Argument_AlignmentMustBePow2;
                //case ExceptionResource.ArgumentOutOfRange_NotGreaterThanBufferLength:
                //    return SR.ArgumentOutOfRange_NotGreaterThanBufferLength;
                //case ExceptionResource.InvalidOperation_SpanOverlappedOperation:
                //    return SR.InvalidOperation_SpanOverlappedOperation;
                //case ExceptionResource.InvalidOperation_TimeProviderNullLocalTimeZone:
                //    return SR.InvalidOperation_TimeProviderNullLocalTimeZone;
                //case ExceptionResource.InvalidOperation_TimeProviderInvalidTimestampFrequency:
                //    return SR.InvalidOperation_TimeProviderInvalidTimestampFrequency;
                //case ExceptionResource.Format_UnexpectedClosingBrace:
                //    return SR.Format_UnexpectedClosingBrace;
                //case ExceptionResource.Format_UnclosedFormatItem:
                //    return SR.Format_UnclosedFormatItem;
                //case ExceptionResource.Format_ExpectedAsciiDigit:
                //    return SR.Format_ExpectedAsciiDigit;
                //case ExceptionResource.Argument_HasToBeArrayClass:
                //    return SR.Argument_HasToBeArrayClass;
                //case ExceptionResource.InvalidOperation_IncompatibleComparer:
                //    return SR.InvalidOperation_IncompatibleComparer;
                case ExceptionResource.ArgumentOutOfRange_PositionMustBeLessThanLimit:
                    return SR.ArgumentOutOfRange_PositionMustBeLessThanLimit;
                case ExceptionResource.ArgumentOutOfRange_LimitMustBeLessThanCapacity:
                    return SR.ArgumentOutOfRange_LimitMustBeLessThanCapacity;
                case ExceptionResource.InvalidOperation_ViewFailedVersion:
                    return SR.InvalidOperation_ViewFailedVersion;
                case ExceptionResource.LurchTable_NeedLimitIntMaxValue:
                    return SR.LurchTable_NeedLimitIntMaxValue;
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
        action,
        allocSize,
        appendable,
        array,
        arrayIndex,
        assembly,
        bitSet,
        buffer,
        capacity,
        characterSequence,
        charCount,
        charSequence,
        codePoints,
        collection,
        comments,
        comparer,
        comparison,
        converter,
        count,
        culture,
        currPos,
        delimiters,
        destination,
        destinationIndex,
        dictionary,
        encoding,
        enumerator,
        equals,
        fnCondition,
        fnCreate,
        fnUpdate,
        formatProvider,
        hashSize,
        index,
        index1,
        index2,
        index3,
        info,
        input,
        interfaceType,
        item,
        key,
        length,
        limit,
        list,
        lockSize,
        match,
        maxValue,
        memoryMappedFile,
        minValue,
        name,
        nbits,
        newLength,
        newLimit,
        newPosition,
        newValue,
        offset,
        original,
        other,
        output,
        owner,
        position,
        position1,
        position2,
        priorityQueue,
        prefix,
        properties,
        radix,
        random,
        reader,
        s,
        seq,
        set,
        source,
        sourceIndex,
        start,
        startIndex,
        str,
        structuralEqualityComparer,
        target,
        targetType,
        text,
        threadStart,
        toStringFormatProvider,
        type,
        uiCulture,
        value,
        writer,
        year,

        //obj,




        //values,


        //task,
        //bytes,
        //byteIndex,
        //byteCount,
        //ch,
        //chars,
        //charIndex,



        //ownedMemory,










        //exceptions,
        //exception,
        //pointer,

        //format,
        //formats,
        //culture,

        //comparable,


        //comparisonType,
        //manager,
        //sourceBytesToCopy,
        //callBack,
        //creationOptions,
        //function,
        //scheduler,
        //continuation,
        //continuationAction,
        //continuationFunction,
        //tasks,
        //asyncResult,
        //beginMethod,
        //endMethod,
        //endFunction,
        //cancellationToken,
        //continuationOptions,
        //delay,
        //millisecondsDelay,
        //millisecondsTimeout,
        //stateMachine,
        //timeout,
        //type,

        //sourceArray,

        //destinationArray,
        //pHandle,
        //handle,

        //newSize,
        //lengths,
        //len,
        //keys,
        //indices,

        //endIndex,
        //elementType,


        //codePoint,

        //options,

        //suffix,

        //buffers,

        //stream,
        //anyOf,
        //overlapped,
        //minimumBytes,
        //arrayType,
        //divisor,
        //factor,

    }

    //
    // The convention for this enum is using the resource name as the enum name
    //
    internal enum ExceptionResource
    {
        Arg_EmptySpan,
        Arg_HSCapacityOverflow,
        ArgumentNull_NullOrNullValue,
        ArgumentOutOfRange_Generic_MustBeNonNegative,
        ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero,
        ArgumentOutOfRange_Capacity,
        ArgumentOutOfRange_OffsetOut,
        ArgumentOutOfRange_StartIndexLargerThanLength,

        ArgumentOutOfRange_IndexMustBeLessOrEqual,
        ArgumentOutOfRange_IndexMustBeLess,
        //ArgumentOutOfRange_IndexCount,
        //ArgumentOutOfRange_IndexCountBuffer,
        ArgumentOutOfRange_IndexLength,
        ArgumentOutOfRange_IndexLengthArray,
        ArgumentOutOfRange_IndexString,
        ArgumentOutOfRange_Count,
        ArgumentOutOfRange_Radix,
        //ArgumentOutOfRange_Year,
        Arg_ArrayPlusOffTooSmall,
        //Arg_ByteArrayTooSmallForValue,
        NotSupported_ReadOnlyCollection,
        Arg_RankMultiDimNotSupported,
        Arg_NonZeroLowerBound,
        //ArgumentOutOfRange_GetCharCountOverflow,
        ArgumentOutOfRange_ListInsert,
        ArgumentOutOfRange_NeedNonNegNum,
        //ArgumentOutOfRange_NotGreaterThanBufferLength,
        ArgumentOutOfRange_SmallCapacity,
        Argument_InvalidOffLen,
        //Argument_CannotExtractScalar,
        ArgumentOutOfRange_BiggerThanCollection,
        Serialization_MissingKeys,
        //Serialization_NullKey,
        Serialization_MismatchedCount,
        NotSupported_KeyCollectionSet,
        NotSupported_ValueCollectionSet,
        //InvalidOperation_NullArray,
        //TaskT_TransitionToFinal_AlreadyCompleted,
        //TaskCompletionSourceT_TrySetException_NullException,
        //TaskCompletionSourceT_TrySetException_NoExceptions,
        //NotSupported_StringComparison,
        ConcurrentCollection_SyncRoot_NotSupported,
        //Task_MultiTaskContinuation_NullTask,
        //InvalidOperation_WrongAsyncResultOrEndCalledMultiple,
        //Task_MultiTaskContinuation_EmptyTaskList,
        //Task_Start_TaskCompleted,
        //Task_Start_Promise,
        //Task_Start_ContinuationTask,
        //Task_Start_AlreadyStarted,
        //Task_RunSynchronously_Continuation,
        //Task_RunSynchronously_Promise,
        //Task_RunSynchronously_TaskCompleted,
        //Task_RunSynchronously_AlreadyStarted,
        //AsyncMethodBuilder_InstanceNotInitialized,
        //Task_ContinueWith_ESandLR,
        //Task_ContinueWith_NotOnAnything,
        //Task_InvalidTimerTimeSpan,
        //Task_Delay_InvalidMillisecondsDelay,
        //Task_Dispose_NotCompleted,
        //Task_ThrowIfDisposed,
        //Task_WaitMulti_NullTask,
        //ArgumentException_OtherNotArrayOfCorrectLength,
        //ArgumentNull_Array,
        //ArgumentNull_SafeHandle,
        //ArgumentOutOfRange_EndIndexStartIndex,
        //ArgumentOutOfRange_Enum,
        //ArgumentOutOfRange_HugeArrayNotSupported,
        Argument_AddingDuplicate,
        Argument_InvalidArgumentForComparison,
        //Arg_LowerBoundsMustMatch,
        //Arg_MustBeType,
        Arg_Need1DArray,
        //Arg_Need2DArray,
        //Arg_Need3DArray,
        //Arg_NeedAtLeast1Rank,
        //Arg_RankIndices,
        //Arg_RanksAndBounds,
        InvalidOperation_IComparerFailed,
        //NotSupported_FixedSizeCollection,
        //Rank_MultiDimNotSupported,
        //Arg_TypeNotSupported,
        //Argument_SpansMustHaveSameLength,
        //Argument_InvalidFlag,
        //CancellationTokenSource_Disposed,
        //Argument_AlignmentMustBePow2,
        //InvalidOperation_SpanOverlappedOperation,
        //InvalidOperation_TimeProviderNullLocalTimeZone,
        //InvalidOperation_TimeProviderInvalidTimestampFrequency,
        //Format_UnexpectedClosingBrace,
        //Format_UnclosedFormatItem,
        //Format_ExpectedAsciiDigit,
        //Argument_HasToBeArrayClass,
        //InvalidOperation_IncompatibleComparer,
        ArgumentOutOfRange_PositionMustBeLessThanLimit,
        ArgumentOutOfRange_LimitMustBeLessThanCapacity,
        InvalidOperation_ViewFailedVersion,
        LurchTable_NeedLimitIntMaxValue,
    }
}
