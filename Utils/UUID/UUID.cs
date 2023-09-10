using System;
using System.Collections.Generic;
using System.Text;

namespace TheGenesis.Core.Utils.UUID
{
    public enum UUIDNameSpace
    {
        None = 0,
        Dns = 1,
        Url = 2,
        Oid = 3,
        X500 = 4
    }

    internal enum UUIDVersion
    {
        TimeBased = 0x10,
        NameBasedWithMd5 = 0x30,
        Random = 0x40,
        NamedBasedWithSha1 = 0x50
    }
}
