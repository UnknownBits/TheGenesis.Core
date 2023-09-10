using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TheGenesis.Core.Utils.UUID
{
    public class NameBasedGenerator : IDisposable
    {
        private HashAlgorithm? _hashAlgorithm;
        private readonly UUIDVersion _version;
        private static readonly Guid[] NameSpaceGuids = {
            Guid.Empty, // None
            Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"), // DNS
            Guid.Parse("6ba7b811-9dad-11d1-80b4-00c04fd430c8"), // URL
            Guid.Parse("6ba7b812-9dad-11d1-80b4-00c04fd430c8"), // IOD
            Guid.Parse("6ba7b814-9dad-11d1-80b4-00c04fd430c8"), // X500
        };

        public enum HashType { Md5 = 1, Sha1 = 2 }

        public NameBasedGenerator() : this(HashType.Md5) { }

        public NameBasedGenerator(HashType hashType)
        {
            _hashAlgorithm = hashType == HashType.Md5 ? (HashAlgorithm)MD5.Create() : SHA1.Create();
            _version = hashType == HashType.Md5 ? UUIDVersion.NameBasedWithMd5 : UUIDVersion.NamedBasedWithSha1;
        }

        public Guid Generate(string name) => Generate(UUIDNameSpace.None, name);

        public Guid Generate(UUIDNameSpace nameSpace, string name) => Generate(NameSpaceGuids[(int)nameSpace], name);

        public Guid Generate(Guid customNamespaceGuid, string name)
        {
            byte[] array = (Guid.Empty == customNamespaceGuid) ? new byte[0] : customNamespaceGuid.ToActuallyOrderedBytes();
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            byte[] array2 = new byte[array.Length + bytes.Length];
            if (array.Length != 0)
                Array.Copy(array, array2, array.Length);
            Array.Copy(bytes, 0, array2, array.Length, bytes.Length);
            return _hashAlgorithm!
                .ComputeHash(array2)
                .TrimTo16Bytes()
                .AddVariantMarker()
                .AddVersionMarker(_version)
                .ToGuidFromActuallyOrderedBytes();
        }

        ~NameBasedGenerator() { _hashAlgorithm?.Dispose(); }

        public void Dispose() { if (_hashAlgorithm != null) { _hashAlgorithm.Dispose(); _hashAlgorithm = null; } }
    }
}
