using System;
using System.Collections.Generic;
using System.Text;

namespace TheGenesis.Core.Utils.UUID
{
    public static class GuidExtensions
    {
        public static byte[] ToActuallyOrderedBytes(this Guid guid) => guid.ToByteArray().ChangeGuidByteOrders();

        public static Guid ToGuidFromActuallyOrderedBytes(this byte[] array) => new Guid(array.ChangeGuidByteOrders());

        public static long ToLeastSignificantBits(this Guid id)
        {
            var bytes = id.ToByteArray().ChangeGuidByteOrders();
            var boolArray = new bool[bytes.Length];
            for (var i = 0; i < bytes.Length; i++)
                boolArray[i] = GetBit(bytes[i]);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static Guid ToGuid(this long id)
        {
            var data = new byte[16];
            var sourceArray = BitConverter.GetBytes(id);
            Array.Copy(sourceArray, data, sourceArray.Length);
            return ToGuidFromActuallyOrderedBytes(data);
        }

        private static bool GetBit(byte b) => (b & 1) != 0;

        /// <summary>
        /// Swaps bytes in positions as:
        /// <![CDATA[0 <-> 3, 1 <-> 2, 4 <-> 5, 6 <-> 7]]>
        /// </summary>
        /// <param name="array"></param>
        private static byte[] ChangeGuidByteOrders(this byte[] array)
        {
            (array[7], array[6]) = (array[6], array[7]);
            (array[5], array[4]) = (array[4], array[5]);
            (array[3], array[0]) = (array[0], array[3]);
            (array[2], array[1]) = (array[1], array[2]);
            return array;
        }
    }
}
