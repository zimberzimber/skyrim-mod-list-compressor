using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SkyrimModListCompressor
{
    public class NonNexusMod
    {
        public string Name { get; set; }

        public string File { get; set; }

        public NonNexusMod(string name, string file)
        {
            Name = name;
            File = file;
        }
    }

    public class NonNexusModComparer : IEqualityComparer<NonNexusMod>, IComparer<NonNexusMod>
    {
        public int Compare([AllowNull] NonNexusMod x, [AllowNull] NonNexusMod y)
        {
            if (Equals(x, y))
                return 0;

            if (x == null) return -1;
            if (y == null) return 1;

            int fileCompare = x.File.CompareTo(y.File);
            if (fileCompare != 0)
                return fileCompare;

            return x.Name.CompareTo(y.Name);
        }

        public bool Equals([AllowNull] NonNexusMod x, [AllowNull] NonNexusMod y)
        {
            if (x == y) return true;

            if (x == null) return false;
            if (y == null) return false;

            return x.File.Equals(y.File) && x.Name.Equals(y.Name);

        }
        public int GetHashCode([DisallowNull] NonNexusMod obj) => throw new NotImplementedException();
    }
}
