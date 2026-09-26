using System;
using System.Collections.Generic;
using System.Text;

namespace CMaurer.Common
{
    public class MksChangePackageEntry
    {
        private MksChangePackage _changePackage = null;

        /*
         * these commands are generated from the data in this class:
         * 
         * si freeze --hostname=ffm-mks1 --port=7001 --sandbox=D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\project.pj D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\DIS.cidl
         * si thaw --hostname=ffm-mks1 --port=7001 --sandbox=D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\project.pj D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\DIS.cidl
         * 
         */

        /// <summary>
        /// G99BAB
        /// </summary>
        public string _devpath;

        /// <summary>
        /// "d:/mks/archives/src3/FORD_FB4/EBS/MSW/PBCService/PBCService_plugin/Pbc_Lib/project.pj"
        /// </summary>
        public string _project;

        /// <summary>
        /// "PbcLib_CAS__MOT_5.9.2.1-b_3.zip"
        /// </summary>
        public string _member;

        /// <summary>
        /// "1.1"
        /// </summary>
        public string _revision;

        public MksChangePackageEntry()
        {
        }

        public MksChangePackageEntry(MksChangePackage cp)
        {
            _changePackage = cp;
        }

        public override string ToString()
        {
            return base.ToString() + ": " + _project + ":" + _member + ":" + _revision;
        }

    }
}
