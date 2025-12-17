using System;
using System.Collections.Generic;
using System.Text;

namespace CMaurer.Common
{
    public class MksUserData
    {
        /// <summary>
        /// The user id in MKS (EFCEX4)
        /// </summary>
        public string _mksId = "";

        /// <summary>
        /// The user name in MKS (Maurer, Christoph)
        /// </summary>
        public string _humanReadableName = "";

        /// <summary>
        /// The email address christoph.maurer@continental-corporation.com
        /// </summary>
        public string _email = "";

        /// <summary>
        /// The SAFE topic
        /// </summary>
        public string _safeTopic = "";

        public override string ToString()
        {
            return base.ToString() + ": " + _mksId + ":" + _humanReadableName + ":" + _email;
        }

    }
}

