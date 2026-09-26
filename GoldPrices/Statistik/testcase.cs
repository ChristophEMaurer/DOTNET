namespace fsd
{
    using System.Collections;

    public class CTFUN
    {
        private int m_nTFUN;
        private string m_strTFUN_NAME;

        public CTFUN(int p_nTFUN, string p_strFunctionName)
        {
            m_nTFUN = p_nTFUN;
            m_strTFUN_NAME = p_strFunctionName.Trim();
        }

        public int TFUN
        {
            get { return m_nTFUN; }
        }
        public string TFUN_NAME
        {
            get { return m_strTFUN_NAME; }
        }

        public override bool Equals(object p_o)
        {
            CTFUN tfun = (CTFUN) p_o;

            return tfun.m_nTFUN == m_nTFUN;
        }

        public override int GetHashCode()
        {
            return m_nTFUN;
        }
    }

    public class CTTAB
    {
        private int m_nTTAB;
        private string m_strTTAB_NAME;
        private string m_strDTTY_ID;

        public CTTAB(int p_nTTAB, string p_strTTAB_NAME, string p_strDTTY_ID)
        {
            m_nTTAB = p_nTTAB;
            m_strTTAB_NAME = p_strTTAB_NAME.Trim();
            m_strDTTY_ID = p_strDTTY_ID.Trim();
        }

        public override int GetHashCode()
        {
            return m_nTTAB;
        }

        public override bool Equals(object p_o)
        {
            CTTAB ttab = (CTTAB) p_o;
            return ttab.m_nTTAB == m_nTTAB;
        }

        public int TTAB { get { return m_nTTAB; } }
        public string TTAB_NAME { get { return m_strTTAB_NAME; } }
        public string DTTY_ID { get { return m_strDTTY_ID; } }
    }

    public class CTTFU
    {
        private CTFUN m_tfun;
        private CTTAB m_ttab;
        private string m_strTTFU_COMMENT;
        private string m_strTTFU_FLAG;

        public CTTFU(CTFUN p_tfun, CTTAB p_ttab, string p_strTTFU_COMMENT, string p_strTTFU_FLAG)
        {
            m_tfun = p_tfun;
            m_ttab = p_ttab;
            m_strTTFU_COMMENT = p_strTTFU_COMMENT.Trim();
            m_strTTFU_FLAG = p_strTTFU_FLAG.Trim();
        }

        public override bool Equals(object p_o)
        {
            CTTFU ttfu = (CTTFU) p_o;
            return m_tfun.Equals(ttfu.TFUN) && m_ttab.Equals(ttfu.TTAB);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public CTFUN TFUN{ get { return m_tfun; } }
        public CTTAB TTAB{ get { return m_ttab; } }
        public string TTFU_COMMENT{ get { return m_strTTFU_COMMENT; } }
        public string TTFU_FLAG{ get { return m_strTTFU_FLAG; } }
    }


    /*
     * Which test case data to generate for a given table
     */
    public class TableData
    {
        public enum GENERATE_MODE
        {
            ALL,        // ignore m_arTTCD, generate entire table
            SELECTION     // use m_arTTCD to generate only the test case data in the list
        }

        private GENERATE_MODE m_mode;

        // The table
        private CTTAB  m_ttab;

        // The IDs of all test case data from this table to generate
        private ArrayList m_arTTCD;

        public TableData(CTTAB p_ttab, GENERATE_MODE p_mode)
        {
            m_ttab = p_ttab;  
            m_mode = p_mode;
            m_arTTCD = new ArrayList();
        }

        public override int GetHashCode()
        {
            return m_ttab.TTAB;
        }

        public override bool Equals(object p_o)
        {
            TableData td = (TableData) p_o;
            return m_ttab.Equals(td.TTAB);
        }

        public CTTAB TTAB { get { return m_ttab; } }
        public GENERATE_MODE Mode 
        { 
            get { return m_mode; } 
        }
        public ArrayList TTCDList { get { return m_arTTCD; } }

        public void AddTTCD(int p_nTTCD)
        {
            m_arTTCD.Add(p_nTTCD);
        }
    }
}
