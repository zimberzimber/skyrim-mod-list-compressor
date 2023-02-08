using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SkyrimModListCompressor
{
    public class Mod
    {
        [Name("#Mod_Name")]
        public string Name { get; set; }

        [Name("#Mod_Nexus_URL")]
        public string NexusUrl { get; set; }

        [Name("#Mod_Version")]
        public string Version { get; set; }
    }

    public class ModComparer : IEqualityComparer<Mod>, IComparer<Mod>
    {
        public int Compare([AllowNull] Mod x, [AllowNull] Mod y)
        {
            if (Equals(x, y))
                return 0;

            if (x == null) return -1;
            if (y == null) return 1;

            int nameCompare = x.Name.CompareTo(y.Name);

            if (nameCompare != 0)
                return nameCompare;

            return x.Version.CompareTo(y.Version);
        }

        public bool Equals([AllowNull] Mod x, [AllowNull] Mod y)
        {
            if (x == y) return true;

            if (x == null) return false;
            if (y == null) return false;

            return x.Name.Equals(y.Name) && x.Version.Equals(y.Version);

        }
        public int GetHashCode([DisallowNull] Mod obj) => throw new NotImplementedException();
    }
}
