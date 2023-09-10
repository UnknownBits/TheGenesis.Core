using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TheGenesis.Core.Utils.UUID
{
    public class RandomBasedGenerator
    {
        private readonly RandomNumberGenerator _generator;

        public RandomBasedGenerator(RandomNumberGenerator generator) => _generator = generator;

        public RandomBasedGenerator() : this(RandomNumberGenerator.Create()) { }

        public Guid Generate()
        {
            var data = new byte[16];
            _generator.GetBytes(data);
            data.AddVariantMarker().AddVersionMarker(UUIDVersion.Random);
            return data.ToGuidFromActuallyOrderedBytes();
        }
    }
}
