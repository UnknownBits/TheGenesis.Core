using Newtonsoft.Json;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TheGenesis.Core.Utils.UUID
{
    /// <summary>An Unique Universal IDentifier, 128 bits of data</summary>
    [JsonConverter(typeof(UUIDJsonConverter))]
    public readonly struct UUID : IEquatable<UUID>
    {
        /// <summary>The data field</summary>
        internal readonly byte[] _Data;

        public UUID(String uuid)
        {
            this._Data = Format.Parse(uuid);
        }

        public UUID(byte[] data) : this(data.AsSpan()) { }
        public UUID(ReadOnlySpan<byte> data)
        {
            _Data = new byte[16];
            Array.Copy(data.ToArray(), _Data, 16);
        }

        public override String ToString()
        {
            return Format.ToString(this._Data);
        }
        public override bool Equals(object? obj)
        {
            return obj is UUID uuid && Equals(uuid);
        }
        public bool Equals(UUID other)
        {
            return this._Data == other._Data;
        }
        public static UUID Parse(String uuid)
        {
            return Parse(uuid.AsSpan());
        }
        public static UUID Parse(ReadOnlySpan<char> uuid)
        {
            Vector128<Byte> data = Format.Parse(uuid);
            return new UUID(data);
        }
        public static bool TryParse(string uuid, out UUID result)
        {
            return TryParse(uuid.AsSpan(), out result);
        }
        public static bool TryParse(ReadOnlySpan<char> uuid, out UUID result)
        {
            bool r = false;
            try {
                result = Parse(uuid);
                r = true;
            } catch(Exception) { result = default; }
            return r;
        }

    }
}
