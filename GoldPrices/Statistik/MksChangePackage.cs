using System.Collections.Generic;

namespace CMaurer.Common
{
    public class MksChangePackage
    {
        public string _id;
        public string _summary;

        public List<MksChangePackageEntry> _changePackageEntries = new List<MksChangePackageEntry>();
    }
}
