namespace fsd
{
    using System.Collections;
    using System.Data.SqlClient;
    using System;

    public class DefTableEntry
    {
        private int m_nPk;
        private string m_strValue;

        public DefTableEntry(int p_nPk, string p_strValue)
        {
            m_nPk = p_nPk;
            m_strValue = p_strValue;
        }

        public int Pk
        {
            get { return m_nPk; }
            set { m_nPk = value; }
        }

        public string Description
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }
    }

    public class DefTableCache : ArrayList
    {
        private string m_strTableName;
        private string m_strKeyName;
        private string m_strValueName;
        private string m_strOrderBy;

        public DefTableCache(string p_strTableName, string p_strKey, string p_strValue)
            : this(p_strTableName, p_strKey, p_strValue, null)
        {
        }

        public DefTableCache(string p_strTableName, string p_strKey, string p_strValue, string p_strOrderBy)
        {
            m_strTableName = p_strTableName;
            m_strKeyName = p_strKey;
            m_strValueName = p_strValue;
            m_strOrderBy = p_strOrderBy;
        }

        public string TableName { get { return m_strTableName; } }
        public string KeyName   { get { return m_strKeyName; } }
        public string ValueName { get { return m_strValueName; } }
        public string OrderBy   { get { return m_strOrderBy; } }

        public object getDescriptionForPk(int p_nPk)
        {
            for (int i = 0; i < this.Count; i++)
            {
                DefTableEntry e = (DefTableEntry) this[i];
                if (p_nPk == e.Pk)
                {
                    return e.Description;
                }
            }
            return DBNull.Value;
        }

        public object getPkForDescription(string p_strDescription)
        {
            for (int i = 0; i < this.Count; i++)
            {
                DefTableEntry e = (DefTableEntry) this[i];
                if (p_strDescription == e.Description)
                {
                    return e.Pk;
                }
            }
            return DBNull.Value;
        }
    }

    //
    // Use this wrapper to have only one instance of DefTableCache for each def table
    //
    public class TableContents
    {
        private static ArrayList s_list;

        public TableContents()
        {
        }

        public TableContents(SqlCommand p_sqlCommand, 
            string p_strTableName, string p_strPk, string p_strDescription, string p_strOrderBy)
        {
            DefTableCache dtl;

            if (s_list == null)
            {
                s_list = new ArrayList();
            }

            for (int i = 0; i < s_list.Count; i++)
            {
                dtl = (DefTableCache) s_list[i];

                if (dtl.TableName.Equals(p_strTableName))
                {
                    return;
                }
            }

            dtl = new DefTableCache(p_strTableName, p_strPk, p_strDescription);

            p_sqlCommand.CommandText = "SELECT " + p_strPk + "," + p_strDescription + " FROM " + p_strTableName;
            if (p_strOrderBy != null)
            {
                p_sqlCommand.CommandText += " ORDER BY " + p_strOrderBy;
            }
            SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                int pk = sqlDataReader.GetInt32(0);
                string description;

                string strDataTypeName = sqlDataReader.GetDataTypeName(1);

                if (Db.IsCharField(strDataTypeName))
                {
                    description = sqlDataReader.GetString(1).Trim();
                }
                else if (strDataTypeName.Equals("datetime"))
                {
                    description = sqlDataReader.GetDateTime(1).ToString();
                }
                else
                {
                    throw new Exception("Unknown database column format: '" + strDataTypeName + "'");
                }

                dtl.Add(new DefTableEntry(pk, description));
            }
            sqlDataReader.Close();

            s_list.Add(dtl);
        }

        public DefTableCache this[string p_strTableName]
        {
            get 
            { 
                for (int i = 0; i < s_list.Count; i++)
                {
                    DefTableCache dtl = (DefTableCache) s_list[i];

                    if (dtl.TableName.Equals(p_strTableName))
                    {
                        return dtl;
                    }
                }
                return null;
            }
        }

        static public void Refresh(SqlCommand p_sqlCommand, string strTableName)
        {
            DefTableCache oldDtc = new TableContents()[strTableName];

            if (oldDtc != null)
            {
                //
                // Clear the old cache
                //
                oldDtc.Clear();

                //
                // populate it
                //
                p_sqlCommand.CommandText = "SELECT " + oldDtc.KeyName + "," 
                    + oldDtc.ValueName + " FROM " + oldDtc.TableName;
                if (oldDtc.OrderBy != null)
                {
                    p_sqlCommand.CommandText += " ORDER BY " + oldDtc.OrderBy;
                }

                SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int pk = sqlDataReader.GetInt32(0);
                    string description = sqlDataReader.GetString(1);

                    oldDtc.Add(new DefTableEntry(pk, description));
                }
                sqlDataReader.Close();
            }
        }
    }
}

