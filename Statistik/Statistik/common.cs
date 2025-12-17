namespace fsd
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    
    public class Util
	{
        public static DataRowVersion getDataRowVersion(DataRow p_row)
        {
            switch (p_row.RowState)
            {
                case DataRowState.Deleted:
                    return DataRowVersion.Original;

                case DataRowState.Added:
                case DataRowState.Detached:
                case DataRowState.Modified:
                case DataRowState.Unchanged:
                default:
                    return DataRowVersion.Current;
            }
        }

        public static  int getNextValueForColumn(DataTable p_dt, string p_strColumnName)
        {
            int nMax = 0;

            foreach (DataRow row in p_dt.Rows)
            {
                if (nMax < (int) row[p_strColumnName, Util.getDataRowVersion(row)])
                {
                    nMax = (int) row[p_strColumnName, Util.getDataRowVersion(row)];
                }
            }
            nMax++;

            return nMax;
        }

        public static int getNextMaxValueForTable(
            SqlConnection   p_sqlConnection, 
            SqlCommand      p_sqlCommand, 
            SqlDataReader   p_sqlDataReader,
            string          p_tableName)
        {
            int nNewPk = -1;

            SqlParameter p;
            CommandType commandTypeSaved = p_sqlCommand.CommandType;

            if (p_tableName.Length != 4)
            {
                throw new Exception("Bad tablename for new pk, must be of length exactly 4: " + p_tableName);
            }

            p_sqlCommand.CommandType = CommandType.StoredProcedure;
            p_sqlCommand.Parameters.Clear();

            p = new SqlParameter("@tablename", SqlDbType.Char,4);
            p.Value = p_tableName;
            p.Direction = ParameterDirection.Input;
            p_sqlCommand.Parameters.Add(p);

            p = new SqlParameter("@pk", SqlDbType.Int);
            p.Direction = ParameterDirection.Output;
            p_sqlCommand.Parameters.Add(p);

            p_sqlCommand.CommandText = "tprk_pk";
            p_sqlCommand.ExecuteNonQuery();

            p = p_sqlCommand.Parameters[1];
            nNewPk = (int) p.Value;
            p_sqlCommand.CommandType = commandTypeSaved;

            return nNewPk;
        }

        public static void setDefaults(DataGrid p_dg, DataTable p_dt)
        {
            DataView dv = new DataView(p_dt); //.DefaultView;

            dv.AllowNew = false;
            dv.AllowDelete = true;
            p_dg.DataSource = dv;
            p_dg.AllowSorting = false;
        }

        public static DataColumn createAndInsertDataColumn(DataTable p_dataTable, string p_strColumnName, 
            Type p_type, int p_nMaxLength)
        {
            return createAndInsertDataColumn(p_dataTable, p_strColumnName, p_type, false, p_nMaxLength);
        }    

        public static DataColumn createAndInsertDataColumn(DataTable p_dataTable, string p_strColumnName, 
            Type p_type)
        {
            return createAndInsertDataColumn(p_dataTable, p_strColumnName, p_type, false, -1);
        }    

        public static DataColumn createAndInsertDataColumn(DataTable p_dataTable, string p_strColumnName, 
            Type p_type, bool p_fIsReadOnly)
        {
            return createAndInsertDataColumn(p_dataTable, p_strColumnName, p_type, p_fIsReadOnly, -1);
        }
            
        public static DataColumn createAndInsertDataColumn(DataTable p_dataTable, string p_strColumnName, 
            Type p_type, bool p_fIsReadOnly, int p_nMaxLength)
        {
            DataColumn col = new DataColumn(p_strColumnName, p_type);
            col.ReadOnly = p_fIsReadOnly;
            col.AllowDBNull = false;
            p_dataTable.Columns.Add(col);
            if (p_nMaxLength != -1)
            {
                col.MaxLength = p_nMaxLength;
            }

            return col;
        }

        public static object getValueFromSqlDataReader(SqlDataReader p_reader, int p_nIndex)
        {
            string strDataTypeName = p_reader.GetDataTypeName(p_nIndex);

            if (p_reader.IsDBNull(p_nIndex))
            {
                return DBNull.Value;
            }
            else if (Db.IsCharField(strDataTypeName))
            {
                return p_reader.GetString(p_nIndex).Trim();
            }
            else if (strDataTypeName.Equals("int"))
            {
                return p_reader.GetInt32(p_nIndex).ToString();
            }
            else if (strDataTypeName.Equals("smallint"))
            {
                return p_reader.GetInt16(p_nIndex).ToString();
            }
            else if (strDataTypeName.Equals("tinyint"))
            {
                return p_reader.GetByte(p_nIndex).ToString();
            }
            else if (strDataTypeName.Equals("datetime"))
            {
                return p_reader.GetDateTime(p_nIndex).ToShortDateString();
            }
    
            throw new Exception("Unknown database column format: '" + strDataTypeName + "'");
        }

        public static void setColumns(SqlCommand p_sqlCommand, DataGrid p_dg, DataTable p_dt, ColumnDescription[] p_arColumnDescription)
        {
            setColumns(p_sqlCommand, p_dg, p_dt, p_arColumnDescription, false);
        }

        public static void setColumns(SqlCommand p_sqlCommand, DataGrid p_dg, DataTable p_dt, ColumnDescription[] p_arColumnDescription, bool p_fForce)
        {
            int i;
            bool bHasComboBox = false;
            DataGridColumnStyle[] arGcs = null;
            DataGridComboBoxColumn ComboTextCol = null;

            if (p_dg.TableStyles.Count == 0 || p_fForce)
            {
                if (p_fForce)
                {
                    p_dg.TableStyles.Clear();
                }

                DataGridTableStyle dgts = new DataGridTableStyle();
                dgts.MappingName = p_dt.TableName;
                dgts.AllowSorting = false;
                p_dg.TableStyles.Add(dgts);

                //
                // Check whether one of them needs a combo box
                //
                for (i = 0; i < dgts.GridColumnStyles.Count; i++)
                {
                    if (p_arColumnDescription[i].TableName.Length > 0)
                    {
                        bHasComboBox = true;
                        break;
                    }
                }

                if (bHasComboBox)
                {
                    arGcs = new DataGridColumnStyle[dgts.GridColumnStyles.Count];
                }

                for (i = 0; i < dgts.GridColumnStyles.Count; i++)
                {
                    if (bHasComboBox)
                    {
                        if (p_arColumnDescription[i].TableName.Length > 0)
                        {
                            //
                            // create the combo box stuff
                            //
                            ComboTextCol = new DataGridComboBoxColumn();

                            ComboTextCol.HeaderText = p_arColumnDescription[i].TableName;
                            ComboTextCol.Width = p_arColumnDescription[i].Width;

                            TableContents ds = new TableContents(p_sqlCommand, 
                                p_arColumnDescription[i].TableName, 
                                p_arColumnDescription[i].Pk, 
                                p_arColumnDescription[i].Description,
                                p_arColumnDescription[i].OrderBy);

                            DefTableCache dtc = ds[p_arColumnDescription[i].TableName];
                            ComboTextCol.MappingName = p_arColumnDescription[i].TableName;
                            ComboTextCol.ColumnComboBox.DataSource = dtc;
                            ComboTextCol.ReadOnly = p_arColumnDescription[i].ReadOnly;
                            ComboTextCol.ColumnComboBox.DisplayMember = "Description";
                            ComboTextCol.ColumnComboBox.ValueMember = "Pk";

                            dgts.PreferredRowHeight = ComboTextCol.ColumnComboBox.Height + 2;

                            arGcs[i] = ComboTextCol;
                        }
                        else
                        {
                            //
                            // use the existing one
                            //
                            dgts.GridColumnStyles[i].Width = p_arColumnDescription[i].Width;
                            dgts.GridColumnStyles[i].NullText = Db.s_strDbNullString;
                            arGcs[i] = dgts.GridColumnStyles[i];
                        }
                    }
                    else
                    {
                        dgts.GridColumnStyles[i].Width = p_arColumnDescription[i].Width;
                        dgts.GridColumnStyles[i].NullText = Db.s_strDbNullString;
                    }
                }
                if (bHasComboBox)
                {
                    dgts.GridColumnStyles.Clear();
                    dgts.GridColumnStyles.AddRange(arGcs);
                }
            }
        }

        public static void addToListIfNotExists(ArrayList p_arList, object p_o)
        {
            for (int i = 0; i < p_arList.Count; i++)
            {
                if (p_arList[i].Equals(p_o))
                {
                    return;
                }
            }
            p_arList.Add(p_o);
        }
    }
}
