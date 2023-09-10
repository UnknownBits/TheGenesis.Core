using System;
using System.Collections.Generic;
using System.Text;

namespace TheGenesis.Core.Utils.UUID.Generator
{
    internal static class ByteMarker
    {
        public static byte[] AddVariantMarker(this byte[] array)
        {
            array[8] &= 0x3f;
            array[8] |= 0x80;
            return array;
        }

        public static byte[] AddVersionMarker(this byte[] array, UUIDVersion version)
        {
            var versionBits = (byte)version;
            array[6] &= 0x0f;
            array[6] |= versionBits;
            return array;
        }

        public static byte[] TrimTo16Bytes(this byte[] array)
        {
            var result = new byte[16];
            Array.Copy(array, result, 16);
            return result;
        }
    }
}
