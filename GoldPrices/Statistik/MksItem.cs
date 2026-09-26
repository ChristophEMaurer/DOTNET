using System.Collections.Generic;

namespace CMaurer.Common
{
    public class MksItem
    {
        public MksItem _parent = null;

        public int _type;
        public string _id;

        /// <summary>
        /// Attached SAFEs, TASKs or PFCRs
        /// </summary>
        public List<MksItem> _subItems = new List<MksItem>();

        /// <summary>
        /// all change package attached to a SAFE via TASKS or PFCRs, sub-SAFEs are not considered.
        /// </summary>
        public List<MksChangePackage> _changePackages = new List<MksChangePackage>();

        public bool _hasChangePackageEntriesRecursive = false;

        public MksItem()
        {
        }

        public MksItem(MksItem parent)
        {
            _parent = parent;
        }

        public string GetTypeAsString()
        {
            string name = "dont know";

            switch (_type)
            {
                case IntegritySupport.TYPE_PFCR:
                    name = "PFCR";
                    break;

                case IntegritySupport.TYPE_SAFE:
                    name = "SAFE";
                    break;
                case IntegritySupport.TYPE_TASK:
                    name = "TASK";
                    break;
            }

            return name;
        }

        public override string ToString()
        {
            string text = _type + ":" + _id + ", cp;" + _changePackages.Count + ";cpes:" + _hasChangePackageEntriesRecursive;

            return text;
        }
    }
}
