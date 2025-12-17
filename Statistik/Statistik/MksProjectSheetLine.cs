using System;
using System.Collections.Generic;
using System.Text;

namespace CMaurer.Common
{
    public class MksProjectSheetLine
    {
        public const int PROJECTSHEET_INDEX_RESPONSIBLE = 0;
        public const int PROJECTSHEET_INDEX_DEPUTY = 1;
        public const int PROJECTSHEET_INDEX_SUPERVISOR = 2;
        public const int PROJECTSHEET_INDEX_SUPERVISOR_DEPUTY = 3;

        public string _functionModel = "";

        /// <summary>
        /// Contains the for entries for a function model: 
        /// index 0:    user
        /// index 1     deputy
        /// index 2     supervisor
        /// index 3     supervisor deputy
        /// </summary>
        public MksUserData[] _data = new MksUserData[4];

        public MksProjectSheetLine()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = new MksUserData();
            }
        }
    }
}
