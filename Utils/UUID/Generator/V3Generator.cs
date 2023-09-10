using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TheGenesis.Core.Utils.UUID.Generator
{
    public class V3Generator : IDisposable
    {
        private static readonly Guid[] NameSpaceGuids = {
            Guid.Empty,
            Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"), // DNS
            Guid.Parse("6ba7b811-9dad-11d1-80b4-00c04fd430c8"), // URL
            Guid.Parse("6ba7b812-9dad-11d1-80b4-00c04fd430c8"), // IOD
            Guid.Parse("6ba7b814-9dad-11d1-80b4-00c04fd430c8"), // X500
        };

        private HashAlgorithm? _hashAlgorithm;
        private readonly UUIDVersion _version;

        public enum HashType
        {
            Md5 = 1,
            Sha1 = 2
        }

        /// <summary>
        /// Creates an instance of name based RFC4122 UUID generator. 
        /// Either Version-3 (MD5 hashing) or Version-5 (SHA-1 hashing) variant is instantiated based on  <paramref name="hashType"/>.
        /// </summary>
        /// <param name="hashType">Hashing algorithm type be used (MD5 or SHA-1)</param>
        public V3Generator(HashType hashType)
        {
            _hashAlgorithm = hashType == HashType.Md5 ? (HashAlgorithm)MD5.Create() : SHA1.Create();
            _version = hashType == HashType.Md5 ? UUIDVersion.NameBasedWithMd5 : UUIDVersion.NamedBasedWithSha1;
        }

        /// <summary>
        /// Creates an instance of name based RFC4122 UUID Version-3 generator, using MD5 hashing.
        /// </summary>
        public NameBasedGenerator()
            : this(HashType.Md5)
        {
        }

        /// <summary>
        /// Generates RFC4122 name based UUID without any namespace for the given <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name to use when generating UUID</param>
        /// <returns>RFC4122 UUID generated using the <paramref name="name"/></returns>
        public Guid GenerateGuid(string name) => GenerateGuid(UUIDNameSpace.None, name);

        /// <summary>
        /// Generates RFC4122 name based UUID with the given <paramref name="nameSpace"/> and <paramref name="name"/>
        /// </summary>
        /// <param name="nameSpace">RFC4122 suggested standard namespace for the UUID, or None</param>
        /// <param name="name">The name to use when generating UUID</param>
        /// <returns>RFC4122 UUID generated using the <paramref name="nameSpace"/> and the <paramref name="name"/></returns>
        public Guid GenerateGuid(UUIDNameSpace nameSpace, string name) => GenerateGuid(NameSpaceGuids[(int)nameSpace], name);

        /// <summary>
        /// Generates RFC4122 name based UUID with the given <paramref name="customNamespaceGuid"/> and <paramref name="name"/>
        /// </summary>
        /// <param name="customNamespaceGuid">The custom namespace (as GUID) to use when generating UUID</param>
        /// <param name="name">The name to use when generating UUID</param>
        /// <returns>RFC4122 UUID generated using the <paramref name="customNamespaceGuid"/> and the <paramref name="name"/></returns>
        public Guid GenerateGuid(Guid customNamespaceGuid, string name)
        {
            var nsBytes = Guid.Empty == customNamespaceGuid ? new byte[0] : customNamespaceGuid.ToActuallyOrderedBytes();
            var nameBytes = Encoding.UTF8.GetBytes(name);
            var data = new byte[nsBytes.Length + nameBytes.Length];
            if (nsBytes.Length > 0)
            {
                Array.Copy(nsBytes, data, nsBytes.Length);
            }
            Array.Copy(nameBytes, 0, data, nsBytes.Length, nameBytes.Length);

            var result = _hashAlgorithm!
                .ComputeHash(data)
                .TrimTo16Bytes()
                .AddVariantMarker()
                .AddVersionMarker(_version);

            return result.ToGuidFromActuallyOrderedBytes();
        }

        public void Dispose()
        {
            if (_hashAlgorithm != null)
            {
                _hashAlgorithm.Dispose();
                _hashAlgorithm = null;
            }
        }

        ~NameBasedGenerator()
        {
            _hashAlgorithm?.Dispose();
        }
    }
}
