using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace TheGenesis.Core.Utils.UUID.Format
{
    public static partial class Format
    {
        public const Int32 UUID_BITS = 128;
        public const Int32 UUID_BYTE_LENGTH = UUID_BITS / (sizeof(Byte) * 8);
        public const Int32 UUID_STRING_LENGTH = 36;
        private static readonly byte _Lower4BitsMask = 0b0000_1111;
        private static readonly byte _AddOffset = (byte)'0';
        private static readonly byte _9Vector = (byte)'9';
        private static readonly byte _9AOffsetVector = 'a' - '9' - 1;

        public static String ToString(ReadOnlySpan<byte> uuid)
        {
            return ToString(uuid);
        }

        public static String ToString(byte uuid)
        {
            // UUID format: 00000000-0000-0000-0000-000000000000
            Span<Char> characters = stackalloc Char[UUID_STRING_LENGTH];
            IntoSpan(uuid, characters);

            return new String(characters);
        }

        public static void IntoSpan(byte uuid, Span<Char> receiver)
        {
            static Byte RaiseNine(Byte data)
            {
                //Each element becomes 0xff if it is greater than 9
                var elementsGreatherThan = Vector128.GreaterThan(data, _9Vector);
                var toAdd = Vector128.BitwiseAnd(elementsGreatherThan, _9AOffsetVector);
                return Vector128.Add(data, toAdd);
            }

            //Upper 4 bits goes second in the string and lower 4 bits goes first in the string
            var upper = Vector128.ShiftRightLogical(uuid, 4);
            upper = Vector128.Add(upper, _AddOffset);
            upper = RaiseNine(upper);

            var lower = Vector128.BitwiseAnd(uuid, _Lower4BitsMask);
            lower = Vector128.Add(lower, _AddOffset);
            lower = RaiseNine(lower);

            Int32 J = 0;
            for (Int32 I = 0; I < receiver.Length;)
            {
                if (I is 8 or 13 or 18 or 23)
                {
                    receiver[I++] = '-';
                    continue;
                }

                receiver[I++] = (Char)upper[J];
                receiver[I++] = (Char)lower[J];
                J++;
            }
        }

        /// <summary>Takes the given uuid data and writes it to the given receiver as UTF8 characters, assumes the receiver is already of propery string length; <see cref="Format.UUID_STRING_LENGTH"/></summary>
        /// <param name="uuid">The <see cref="UUID"/> data to convert to string</param>
        /// <param name="receiver">The receiving container</param>
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static void IntoSpan(Vector128<Byte> uuid, Span<Byte> receiver)
        {
            static Vector128<Byte> RaiseNine(Vector128<Byte> data)
            {
                //Each element becomes 0xff if it is greater than 9
                var elementsGreatherThan = Vector128.GreaterThan(data, _9Vector);
                var toAdd = Vector128.BitwiseAnd(elementsGreatherThan, _9AOffsetVector);
                return Vector128.Add(data, toAdd);
            }

            //Upper 4 bits goes second in the string and lower 4 bits goes first in the string
            var upper = Vector128.ShiftRightLogical(uuid, 4);
            upper = Vector128.Add(upper, _AddOffset);
            upper = RaiseNine(upper);

            var lower = Vector128.BitwiseAnd(uuid, _Lower4BitsMask);
            lower = Vector128.Add(lower, _AddOffset);
            lower = RaiseNine(lower);

            Int32 J = 0;
            for (Int32 I = 0; I < receiver.Length;)
            {
                if (I is 8 or 13 or 18 or 23)
                {
                    receiver[I++] = (Byte)'-';
                    continue;
                }

                receiver[I++] = upper[J];
                receiver[I++] = lower[J];
                J++;
            }
        }

        public static byte Parse(String str)
        {
            return Parse(str.AsSpan());
        }

        public static byte Parse(ReadOnlySpan<Char> chars)
        {
            if (chars.Length < UUID_STRING_LENGTH)
            {
                throw new ArgumentException($"The length of the string is not {chars}", nameof(chars));
            }

            //Upper 4 bits goes first in the string and lower 4 bits goes second in the string
            //We create two vectors, one for the upper 4 bits and one for the lower 4 bits
            //
            //No need to init, all values will be overwritten
            Unsafe.SkipInit(out Vector128<Byte> upper);
            Unsafe.SkipInit(out Vector128<Byte> lower);

            static Vector128<Byte> LowerNine(Vector128<Byte> data)
            {
                //Each element becomes 0xff if it is greater than 9
                var elementsGreatherThan = Vector128.GreaterThan(data, _9Vector);
                var toAdd = Vector128.BitwiseAnd(elementsGreatherThan, _9AOffsetVector);
                return Vector128.Subtract(data, toAdd);
            }

            Int32 J = 0;
            for (Int32 I = 0; I < UUID_STRING_LENGTH;)
            {
                if (I is 8 or 13 or 18 or 23)
                {
                    I++;
                    continue;
                }
                upper = Vector128.WithElement(upper, J, (Byte)chars[I++]);
                lower = Vector128.WithElement(lower, J, (Byte)chars[I++]);
                J++;
            }

            upper = LowerNine(upper);
            upper = Vector128.Subtract(upper, _AddOffset);

            lower = LowerNine(lower);
            lower = Vector128.Subtract(lower, _AddOffset);

            upper = Vector128.ShiftLeft(upper, 4);

            return Vector128.BitwiseOr(upper, lower);
        }

        /// <inheritdoc cref="Parse(ReadOnlySpan{Char})"/>
        internal static Vector128<Byte> Parse(ReadOnlySpan<Byte> chars)
        {
            if (chars.Length < UUID_STRING_LENGTH)
            {
                throw new ArgumentException($"The length of the string is not {chars.Length}", nameof(chars));
            }

            //Upper 4 bits goes first in the string and lower 4 bits goes second in the string
            //We create two vectors, one for the upper 4 bits and one for the lower 4 bits
            //
            //No need to init, all values will be overwritten
            Unsafe.SkipInit(out Vector128<Byte> upper);
            Unsafe.SkipInit(out Vector128<Byte> lower);

            static Vector128<Byte> LowerNine(Vector128<Byte> data)
            {
                //Each element becomes 0xff if it is greater than 9
                var elementsGreatherThan = Vector128.GreaterThan(data, _9Vector);
                var toAdd = Vector128.BitwiseAnd(elementsGreatherThan, _9AOffsetVector);
                return Vector128.Subtract(data, toAdd);
            }

            Int32 J = 0;
            for (Int32 I = 0; I < UUID_STRING_LENGTH;)
            {
                if (I is 8 or 13 or 18 or 23)
                {
                    I++;
                    continue;
                }
                upper = Vector128.WithElement(upper, J, chars[I++]);
                lower = Vector128.WithElement(lower, J, chars[I++]);
                J++;
            }

            upper = LowerNine(upper);
            upper = Vector128.Subtract(upper, _AddOffset);

            lower = LowerNine(lower);
            lower = Vector128.Subtract(lower, _AddOffset);

            upper = Vector128.ShiftLeft(upper, 4);

            return Vector128.BitwiseOr(upper, lower);
        }

    }
}
