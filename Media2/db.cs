using System.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Media
{
    public class Db
    {
        public const int COL_LEN_DEF_ID = 50;
        public const int COL_LEN_DEF_DESCRIPTION = 300;
        public const int COL_LEN_DEF_ORDER = 2;

        public const int COL_LEN_TMED_ID = 10;

        public const int NO_TMED_VALUES = 5;

        private static string s_strConnectionString;

        public static bool s_bDebug = false;
        public const string s_strDbNullString = "<NULL>";
        public const int NUMBER_COLUMN_WIDTH = 40;

        public const string s_strDEFTABLE_NONE = "none";
        public const string s_strDEFTABLE_DTYP_DEFAULT = "cd_video";
        public const string s_strDEFTABLE_DLAN_DEFAULT = "en";
        public const string s_strConnectionUser = "Server=cmaurer1;Database=media;user id=media;password=media";
        public const string s_strConnectionTrusted = "Server=cmaurer1;Database=media;Trusted_Connection=Yes";

        public static string ConnectionString 
        {
            set { s_strConnectionString = value; }
            get { return s_strConnectionString; }
        }

        public static SqlConnection getNewSqlConnection()
        {
            SqlConnection sqlConnection = new SqlConnection(s_strConnectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception o)
            {
                MessageBox.Show("Error: Login failed:" + o.ToString());
                throw o;
            }

            return sqlConnection;
        }

        //
        // TART
        //
        public static bool TARTDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TARTCheckDelete(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "DELETE FROM tart WHERE tart=" + (int) p_row["tart", Util.getDataRowVersion(p_row)];
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        public static bool TARTInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TARTCheckInsert(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "INSERT INTO tart (tart, tart_name)"
                    + " VALUES (" 
                    + p_row["tart"] + ","
                    + createSqlString(p_row["tart_name"], true) + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        private static bool TARTCheckDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, false, "tmed", "tart", p_row["tart", Util.getDataRowVersion(p_row)]))
                return false;
            if (!FkCheck(p_sqlCommand, false, "tent", "tart", p_row["tart", Util.getDataRowVersion(p_row)]))
                return false;
            return true;
        }

        private static bool TARTCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            return true;
        }

        public static bool TARTUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tart SET "
                + "tart_name=" + createSqlString(p_row["tart_name"], true)
                + " WHERE tart=" + (int) p_row["tart"];
            return ExecuteNonQuery(p_sqlCommand);
        }
        
        //
        // TENT
        //
        public static bool TENTDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TENTCheckDelete(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "DELETE FROM tent WHERE tent=" + (int) p_row["tent", Util.getDataRowVersion(p_row)];
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        public static bool TENTInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TENTCheckInsert(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "INSERT INTO tent (tent, tent_side, tent_no, tent_title, tmed, tart)"
                    + " VALUES (" 
                    + p_row["tent"] + ","
                    + createSqlString(p_row["tent_side"], true) + ","
                    + p_row["tent_no"] + ","
                    + createSqlString(p_row["tent_title"], true) + ","
                    + p_row["tmed"] + ","
                    + createSqlString(p_row["tart"]) + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        private static bool TENTCheckDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, false, "tent", "tmed", p_row["tent", Util.getDataRowVersion(p_row)]))
                return false;

            return true;
        }

        private static bool TENTCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, true, "tmed", "tmed", p_row["tmed"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "tart", "tart", p_row["tart"]))
                return false;

            return true;
        }

        public static bool TENTUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tent SET "
                + "tent_side=" + createSqlString(p_row["tent_side"], true)
                + ", tent_no=" + p_row["tent_no"]
                + ", tent_title=" + createSqlString(p_row["tent_title"], true)
                + ", tmed=" + p_row["tmed"]
                + ", tart=" + createSqlString(p_row["tart"])
                + " WHERE tent=" + (int) p_row["tent"];
            return ExecuteNonQuery(p_sqlCommand);
        }
        
        //
        // DEFTABLE
        //
        public static bool DEFTABLEDelete(SqlCommand p_sqlCommand, string p_strDefTableName, int p_nDEFTABLE)
        {
            if (p_strDefTableName.Equals("dtyp"))
            {
                if (!FkCheck(p_sqlCommand, false, "tmed", p_strDefTableName, p_nDEFTABLE))
                    return false;
            }
            else if (p_strDefTableName.Equals("dlan"))
            {
                if (!FkCheck(p_sqlCommand, false, "tmed", p_strDefTableName, p_nDEFTABLE))
                    return false;
            }

            p_sqlCommand.CommandText = "DELETE FROM " + p_strDefTableName + " WHERE " + p_strDefTableName + " = " + p_nDEFTABLE;
            return ExecuteNonQuery(p_sqlCommand);
        }

        public static bool DEFTABLEInsert(SqlCommand p_sqlCommand, string p_strDefTableName, DataRow p_row)
        {
            if (DEFTABLECheckInsert(p_sqlCommand, p_strDefTableName, p_row))
            {
                p_sqlCommand.CommandText = "INSERT INTO " + p_strDefTableName + " (" 
                    + p_strDefTableName + "," 
                    + p_strDefTableName + "_id," 
                    + p_strDefTableName + "_order,"
                    + p_strDefTableName + "_description)"
                    + " VALUES (" 
                    + (int) p_row["pk"] + ","
                    + createSqlString(p_row["id"], true) + ","
                    + createSqlString(p_row["order"], true) + ","
                    + createSqlString(p_row["description"], true) + ")";

                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        public static bool DEFTABLECheckInsert(SqlCommand p_sqlCommand, string p_strDefTableName, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, false, p_strDefTableName, p_row["pk"]))
                return false;

            return true;
        }

        public static bool DEFTABLEUpdate(SqlCommand p_sqlCommand, string p_strDefTableName, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE " + p_strDefTableName + " SET "
                + p_strDefTableName + "_id='"  + (string) p_row["id"] + "',"
                + p_strDefTableName + "_order="  + createSqlString(p_row["order"], true) + ","
                + p_strDefTableName + "_description="  + createSqlString(p_row["description"], true)
                + " WHERE " + p_strDefTableName + "=" + (int) p_row["pk"];
            return ExecuteNonQuery(p_sqlCommand);
        }

        //
        // TMED
        //
        public static bool TMEDDelete(SqlConnection p_sqlConnection, SqlCommand p_sqlCommand, DataRow p_row)
        {
                int nTMED = (int) p_row["tmed", Util.getDataRowVersion(p_row)];

                SqlTransaction transaction;

                transaction = p_sqlConnection.BeginTransaction();
                p_sqlCommand.Transaction = transaction;

                try
                {
                    p_sqlCommand.CommandText = "DELETE FROM tent WHERE tmed=" + nTMED;
                    p_sqlCommand.ExecuteNonQuery();
                    p_sqlCommand.CommandText = "DELETE FROM tmed WHERE tmed=" + nTMED;
                    p_sqlCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    return false;
                }

            return true;
        }

        public static bool TMEDInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TMEDCheckInsert(p_sqlCommand, p_row))
            {
                string ts = createSqlString(DateTime.Now.ToString(), true);

                p_sqlCommand.CommandText = "INSERT INTO tmed (tmed, tmed_id, tmed_title, tmed_created, tmed_modified, dtyp, dlan)"
                    + " VALUES (" 
                    + (int) p_row["tmed"] + ","
                    + createSqlString(p_row["tmed_id"], true) + ","
                    + createSqlString(p_row["tmed_title"], true, true) + ","
                    + ts + ","
                    + ts + ","
                    + (int) p_row["dtyp"] + ","
                    + (int) p_row["dlan"] + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        private static bool TMEDCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, true, "dtyp", "dtyp", p_row["dtyp"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "dlan", "dlan", p_row["dlan"]))
                return false;

            return true;
        }

        private static bool TMEDCheckDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, false, "tent", "tmed", p_row["tmed", Util.getDataRowVersion(p_row)]))
                return false;

            return true;
        }

        public static bool TMEDUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tmed SET "
                + "tmed_id=" + createSqlString(p_row["tmed_id"], true)
                + ", tmed_title=" + createSqlString(p_row["tmed_title"], true)
                + ", dtyp=" + p_row["dtyp"]
                + ", dlan=" + p_row["dlan"]
                + ", tart=" + createSqlString(p_row["tart"])
                + ", tmed_modified=" + createSqlString(DateTime.Now.ToString(), true)
                + " WHERE tmed=" + p_row["tmed"];

            return ExecuteNonQuery(p_sqlCommand);
        }

        public static string[] TMEDGetValuesForPk(int p_nTMED, SqlCommand p_sqlCommand)
        {
            string[] arValues = new string[NO_TMED_VALUES];

            p_sqlCommand.CommandText = "SELECT dtyp_description, tmed_id, tmed_title, tmed_date, tart_name"
                + " FROM tmed "
                + " INNER JOIN dtyp ON tmed.dtyp=dtyp.dtyp"
                + " LEFT OUTER JOIN tart ON tmed.tart=tart.tart"
                + " WHERE tmed = " + p_nTMED;

            SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();

            if (sqlDataReader.Read())
            {
                for (int i = 0; i < NO_TMED_VALUES; i++)
                {
                    arValues[i] = Util.getValueFromSqlDataReader(sqlDataReader, i).ToString();
                }
            }
            
            sqlDataReader.Close();

            return arValues;
        }

        public static string[] TARTGetValuesForPk(int p_nTART, SqlCommand p_sqlCommand)
        {
            string[] arValues = new string[1];

            p_sqlCommand.CommandText = "SELECT tart_name FROM tart WHERE tart = " + p_nTART;
            SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();

            if (sqlDataReader.Read())
            {
                arValues[0] = (string) Util.getValueFromSqlDataReader(sqlDataReader, 0);
            }
            
            sqlDataReader.Close();

            return arValues;
        }
        
        public static int GetNextIntValueForIntPk(string p_strTablename, string p_strFieldname, 
            string p_strPkName, int p_nPkValue, SqlCommand p_sqlCommand)
        {
            int nNextValue = 1;

            p_sqlCommand.CommandText = "SELECT MAX(" + p_strFieldname + ") FROM " + p_strTablename;

            if (p_strPkName != null)
            {
                p_sqlCommand.CommandText += " WHERE " + p_strPkName + "=" + p_nPkValue;
            }

            SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();

            if (sqlDataReader.Read())
            {
                object o = Util.getValueFromSqlDataReader(sqlDataReader, 0);
                if (!DBNull.Value.Equals(o))
                {
                    nNextValue = (int) Util.getValueFromSqlDataReader(sqlDataReader, 0) + 1;
                }
            }

            sqlDataReader.Close();

            return nNextValue;
        }

        public static bool ExecuteNonQuery(SqlCommand p_sqlCommand)
        {
            bool fSuccess = false;

            try
            {
                p_sqlCommand.ExecuteNonQuery();
                fSuccess = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "database error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                fSuccess = false;
            }
            return fSuccess;
        }

        private static bool selectHasRecords(SqlCommand p_sqlCommand)
        {
            bool fHasRecords = false;

            try
            {
                SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    fHasRecords = true;
                }
                sqlDataReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "database error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw e;
            }
            return fHasRecords;
        }

        public static string createSqlString(object p_o)
        {
            return createSqlString(p_o, false, false, true);
        }

        public static string createSqlString(object p_o, bool p_fIsString)
        {
            return createSqlString(p_o, p_fIsString, false, true);
        }

        public static string createSqlString(object p_o, bool p_fIsString, bool p_fNullAsEmptyString)
        {
            return createSqlString(p_o, p_fIsString, false, true);
        }

        /*
        * Creates a SQL string by adding a simple quote at the beginning and at the end of the string.
        * 
        */
        public static string createSqlString(object p_o, bool p_fIsString, bool p_fNullAsEmptyString, bool p_fUseSingleQuotes)
        {
            if (DBNull.Value.Equals(p_o))
            {
                if (p_fIsString && p_fNullAsEmptyString)
                {
                    if (p_fUseSingleQuotes)
                    {
                        return "''";
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "null";
                }
            }
            else
            {
                if (p_fIsString)
                {
                    if (p_fUseSingleQuotes)
                    {
                        return "'" + processQuotes((string) p_o) + "'";
                    }
                    else
                    {
                        return (string) p_o;
                    }
                }
                else
                {
                    return p_o.ToString();
                }
            }
        }

        /*
         * Replace every single quote by two single quotes
         */
        public static string processQuotes(string p_s)
        {
            string s = p_s.Replace("'", "''");
            return s;
        }

        private static bool FkCheck(SqlCommand p_sqlCommand, bool p_bShouldExist, 
            string p_strTableName, string p_strFieldName, int p_value)
        {
            bool bExists;

            p_sqlCommand.CommandText = "SELECT " + p_strFieldName + " FROM " + p_strTableName + "  WHERE " + p_strFieldName + "=" + p_value.ToString();
            bExists = selectHasRecords(p_sqlCommand);
            if (bExists)
            {
                if (!p_bShouldExist)
                {
                    MessageBox.Show("Field " + p_strTableName + "." + p_strFieldName + ": value " + p_value.ToString() + " is referenced, cannot be deleted", "speeddating database error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                if (p_bShouldExist)
                {
                    MessageBox.Show("Field " + p_strTableName + "." + p_strFieldName + ": value " + p_value.ToString()  + " is missing but is referenced", "speeddating database error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }
            
        private static bool FkCheck(SqlCommand p_sqlCommand, bool p_bShouldExist, 
            string p_strTableName, string p_strFieldName, object p_value)
        {
            if (!DBNull.Value.Equals(p_value))
            {
                return FkCheck(p_sqlCommand, p_bShouldExist, p_strTableName, p_strFieldName, (int) p_value);
            }
            return true;
        }
            
        private static bool FkCheck(SqlCommand p_sqlCommand, bool p_bShouldExist, string p_strTableAndFieldName, object p_value)
        {
            return FkCheck(p_sqlCommand, p_bShouldExist, p_strTableAndFieldName, p_strTableAndFieldName, p_value);
        }

        public static bool IsCharField(string p_strType)
        {
            if (p_strType.Equals("nchar") 
                || p_strType.Equals("nvarchar") 
                || p_strType.Equals("char") 
                || p_strType.Equals("varchar") 
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetColumnType(SqlCommand p_sqlCommand,
            SqlDataReader p_sqlDataReader,
            int nTTAB, int nColumn )
        {
            string strColumnType;

            p_sqlCommand.CommandText = "SELECT dcty_id from tcol,dcty WHERE tcol.ttab=" 
                + nTTAB + " AND tcol.tcol_colno=" + nColumn + " AND tcol.dcty=dcty.dcty";

            p_sqlDataReader = p_sqlCommand.ExecuteReader();
            p_sqlDataReader.Read();
            strColumnType = p_sqlDataReader.GetString(0);
            p_sqlDataReader.Close();

            return strColumnType;
        }
        
        public static int getDefTablePk(SqlCommand p_sqlCommand, string p_strTableName,
            string p_strId)
        {
            int pk = -1;
            bool bHasData;

            p_sqlCommand.CommandText = "SELECT " + p_strTableName + " FROM " + p_strTableName
                + " WHERE " + p_strTableName + "_id='" + p_strId + "'";

            SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();
            bHasData = sqlDataReader.Read();
            if (bHasData)
            {
                pk = sqlDataReader.GetInt32(0);
            }
            sqlDataReader.Close();
            if (!bHasData)
            {
                throw new Exception("Missing id " + p_strId + " in def table " + p_strTableName);
            }

            return pk;
        }
        
        public static int getAnyPkFromTable(SqlCommand p_sqlCommand, string p_strTableName,
            string p_strFieldName)
        {
            int pk = -1;
            bool bHasData;

            p_sqlCommand.CommandText = "SELECT " + p_strFieldName + " FROM " + p_strTableName;

            SqlDataReader sqlDataReader = p_sqlCommand.ExecuteReader();
            bHasData = sqlDataReader.Read();
            if (bHasData)
            {
                pk = sqlDataReader.GetInt32(0);
            }
            sqlDataReader.Close();

            if (!bHasData)
            {
                throw new Exception("No entry in table " + p_strTableName);
            }
            return pk;
        }

        //
        // TACT
        //
        public static bool TACTDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "DELETE FROM tact WHERE tact=" + (int) p_row["tact", Util.getDataRowVersion(p_row)];
            return ExecuteNonQuery(p_sqlCommand);
        }

        public static bool TACTInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TACTCheckInsert(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "INSERT INTO tact (tact, tper, dact, tact_date)"
                    + " VALUES (" 
                    + (int) p_row["tact"] + ","
                    + (int) p_row["tper"] + ","
                    + (int) p_row["dact"] + ","
                    + createSqlString(p_row["tact_date"], true) + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        public static bool TACTCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, true, "tper", "tper", p_row["tper"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "dact", "dact", p_row["dact"]))
                return false;

            return true;
        }

        public static bool TACTUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tact SET "
                + "tper=" + p_row["tper"]
                + ", dact=" + p_row["dact"]
                + ", tact_date=" + createSqlString(p_row["tact_date"], true)
                + " WHERE tact=" + (int) p_row["tact"];
            return ExecuteNonQuery(p_sqlCommand);
        }
    }
}