namespace fsd
{
    public class ColumnDescription
    {
        private int     m_nWidth;
        private bool    m_bReadOnly;
        private string  m_strTableName;
        private string  m_strPk;
        private string  m_strDescription;
        private bool    m_bVisible;
        private string  m_strOrderBy;

        public ColumnDescription() 
            : this(80, false, "", "", "", false)
        {
        }

        public ColumnDescription(bool p_fReadOnly) 
            : this(80, p_fReadOnly, "", "", "", false)
        {
        }

        public ColumnDescription(int p_nWidth) 
            : this(p_nWidth, false, "", "", "", false)
        {
        }

        public ColumnDescription(int p_nWidth, bool p_fReadOnly) 
            : this(p_nWidth, p_fReadOnly, "", "", "", false)
        {
        }

        public ColumnDescription(int p_nWidth, bool p_fReadOnly, bool p_bVisible) 
            : this(p_nWidth, p_fReadOnly, "", "", "", p_bVisible)
        {
        }

        public ColumnDescription(int p_nWidth, string p_strTableName)
            : this(p_nWidth, false, p_strTableName, p_strTableName, p_strTableName + "_id", false)
        {
        }

        public ColumnDescription(int p_nWidth, string p_strTableName, string p_strPk, string p_strDescription)
            : this(p_nWidth, false, p_strTableName, p_strPk, p_strDescription, false)
        {
        }
            
        public ColumnDescription(int p_nWidth, string p_strTableName, string p_strPk, string p_strDescription, string p_strOrderBy)
            : this(p_nWidth, false, p_strTableName, p_strPk, p_strDescription, false, p_strOrderBy)
        {
        }

        public ColumnDescription(int p_nWidth, bool p_fReadOnly, string p_strTableName, string p_strPk, string p_strDescription)
            : this(p_nWidth, p_fReadOnly, p_strTableName, p_strPk, p_strDescription, false)
        {
        }

        public ColumnDescription(int p_nWidth, bool p_fReadOnly, string p_strTableName, string p_strPk, 
            string p_strDescription, bool p_bVisible)
            : this(p_nWidth, p_fReadOnly, p_strTableName, p_strPk, p_strDescription, p_bVisible, null)
        {
        }
        public ColumnDescription(int p_nWidth, bool p_fReadOnly, string p_strTableName, string p_strPk, 
            string p_strDescription, bool p_bVisible, string p_strOrderBy)
        {
            m_nWidth = p_nWidth;
            m_bReadOnly = p_fReadOnly;
            m_strTableName = p_strTableName;
            m_strPk = p_strPk;
            m_strDescription = p_strDescription;
            m_bVisible = p_bVisible;
            m_strOrderBy = p_strOrderBy;
        }

        public int Width
        {
            //
            // Unfortumately, the width controls whether a column is visible or not.
            // This is very sad. Obviously, we want both readonly-visible and readonly-hidden columns.
            // In debug mode, always display the column
            //

            get { return Db.s_bDebug ? m_nWidth : (m_bReadOnly ? (m_bVisible ? m_nWidth : 0) : m_nWidth); }
            set { m_nWidth = value; }
        }
        public bool ReadOnly
        {
            get { return m_bReadOnly; }
            set { m_bReadOnly = value; }
        }
        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }
        public string TableName
        {
            get { return m_strTableName; }
        }
        public string Pk
        {
            get { return m_strPk; }
        }
        public string Description
        {
            get { return m_strDescription; }
        }
        public string OrderBy
        {
            get { return m_strOrderBy; }
        }
    }
}
