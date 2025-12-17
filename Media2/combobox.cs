
namespace Media
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Data;

    public class ComboBoxEntry
    {
        private string m_strKey;
        private string m_strValue;

        public ComboBoxEntry(string p_strKey, string p_strValue)
        {
            m_strKey = p_strKey;
            m_strValue = p_strValue;
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }
    }
}
