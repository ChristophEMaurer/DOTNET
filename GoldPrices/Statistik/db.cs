using System.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace fsd
{
    public class Db
    {
        public const int COL_LEN_DEF_ID = 50;
        public const int COL_LEN_DEF_DESCRIPTION = 300;

        public const int COL_LEN_TPER_NAME = 25;
        public const int COL_LEN_TPER_LASTNAME = 25;
        public const int COL_LEN_TPER_EMAIL = 50;
        public const int COL_LEN_TPER_PHONE = 50;
        public const int COL_LEN_TPER_CONTACT_EMAIL = 50;
        public const int COL_LEN_TPER_CONTACT_PHONE = 50;

        public const int COL_LEN_TLOC_NAME = 25;
        public const int COL_LEN_TLOC_STREET = 25;
        public const int COL_LEN_TLOC_ZIPCODE = 10;
        public const int COL_LEN_TLOC_CITY = 25;


        private static string s_strConnectionString;

        public static bool s_bDebug = false;
        public const string s_strDbNullString = "<NULL>";
        public const int NUMBER_COLUMN_WIDTH = 40;

        public const string s_strDEFTABLE_EMPTY = "none";
        public const string s_strDEFTABLE_DGEN_DEFAULT = "m";
        public const string s_strDEFTABLE_DMAI_DEFAULT = "n";
        public const string s_strDEFTABLE_DREF_DEFAULT = "none";
        public const string s_strDEFTABLE_TPER_DSTA_DEFAULT = "teil_ange";
        public const string s_strDEFTABLE_TAPP_DSTA_DEFAULT = "teil_ange";
        public const string s_strDEFTABLE_TEVT_DSTA_DEFAULT = "evt_gepl";
        public const string s_strDEFTABLE_DACT_DEFAULT = "none";
        public const string s_strConnectionUser = "Server=cmaurer;Database=speeddating;user id=sxstest;password=sxstest";
        public const string s_strConnectionTrusted = "Server=cmaurer1;Database=speeddating;Trusted_Connection=Yes";

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
        // TAPP
        //
        public static bool TAPPDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "DELETE FROM tapp WHERE tapp=" + (int) p_row["tapp", Util.getDataRowVersion(p_row)];
            return ExecuteNonQuery(p_sqlCommand);
        }

        public static bool TAPPInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TAPPCheckInsert(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "INSERT INTO tapp (tapp, tevt, tper, dsta, tapp_description)"
                    + " VALUES (" 
                    + (int) p_row["tapp"] + ","
                    + (int) p_row["tevt"] + ","
                    + (int) p_row["tper"] + ","
                    + (int) p_row["dsta"] + ","
                    + createSqlString(p_row["tapp_description"], true) + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        public static bool TAPPCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, true, "tevt", "tevt", p_row["tevt"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "tper", "tper", p_row["tper"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "dsta", "dsta", p_row["dsta"]))
                return false;

            return true;
        }

        public static bool TAPPUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tapp SET "
                + "tevt=" + p_row["tevt"]
                + ", tper=" + p_row["tper"]
                + ", dsta=" + p_row["dsta"]
                + ", tapp_description=" + createSqlString(p_row["tapp_description"], true)
                + " WHERE tapp=" + (int) p_row["tapp"];
            return ExecuteNonQuery(p_sqlCommand);
        }

        //
        // TEVT
        //
        public static bool TEVTDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TEVTCheckDelete(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "DELETE FROM tevt WHERE tevt=" + (int) p_row["tevt", Util.getDataRowVersion(p_row)];
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        public static bool TEVTInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TEVTCheckInsert(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "INSERT INTO tevt (tevt, tevt_date, tevt_age, tevt_time, tloc, dsta)"
                    + " VALUES (" 
                    + (int) p_row["tevt"] + ","
                    + createSqlString(p_row["tevt_date"], true) + ","
                    + createSqlString(p_row["tevt_age"], true) + ","
                    + createSqlString(p_row["tevt_time"], true) + ","
                    + (int) p_row["tloc"] + ","
                    + (int) p_row["dsta"] + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        private static bool TEVTCheckDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, false, "tapp", "tevt", p_row["tevt", Util.getDataRowVersion(p_row)]))
                return false;

            return true;
        }

        private static bool TEVTCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, true, "tloc", "tloc", p_row["tloc"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "dsta", "dsta", p_row["dsta"]))
                return false;

            return true;
        }

        public static bool TEVTUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tevt SET "
                + "tevt_date=" + createSqlString(p_row["tevt_date"], true)
                + ", tevt_age=" + createSqlString(p_row["tevt_age"], true)
                + ", tevt_time=" + createSqlString(p_row["tevt_time"], true)
                + ", tloc=" + p_row["tloc"]
                + ", dsta=" + p_row["dsta"]
                + " WHERE tevt=" + (int) p_row["tevt"];
            return ExecuteNonQuery(p_sqlCommand);
        }
        
        //
        // DEFTABLE
        //
        public static bool DEFTABLEDelete(SqlCommand p_sqlCommand, string p_strDefTableName, int p_nDEFTABLE)
        {
            if (p_strDefTableName.Equals("dgen") 
                || p_strDefTableName.Equals("dmai") 
                || p_strDefTableName.Equals("dref"))
            {
                if (!FkCheck(p_sqlCommand, false, "tper", p_strDefTableName, p_nDEFTABLE))
                    return false;
            }
            else if (p_strDefTableName.Equals("dsta"))
            {
                if (!FkCheck(p_sqlCommand, false, "tapp", p_strDefTableName, p_nDEFTABLE))
                    return false;
                if (!FkCheck(p_sqlCommand, false, "tevt", p_strDefTableName, p_nDEFTABLE))
                    return false;
                if (!FkCheck(p_sqlCommand, false, "tper", p_strDefTableName, p_nDEFTABLE))
                    return false;
            }
            else if (p_strDefTableName.Equals("dact"))
            {
                if (!FkCheck(p_sqlCommand, false, "tact", p_strDefTableName, p_nDEFTABLE))
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
                    + p_strDefTableName + "," + p_strDefTableName + "_id," + p_strDefTableName + "_description)"
                    + " VALUES (" 
                    + (int) p_row["pk"] + ","
                    + createSqlString(p_row["id"], true) + ","
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
                + p_strDefTableName + "_id='"  + (string) p_row["id"]
                + "'," + p_strDefTableName + "_description="  + createSqlString(p_row["description"], true)
                + " WHERE " + p_strDefTableName + "=" + (int) p_row["pk"];
            return ExecuteNonQuery(p_sqlCommand);
        }

        //
        // TLOC
        //
        public static bool TLOCDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TLOCCheckDelete(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "DELETE FROM tloc WHERE tloc=" + (int) p_row["tloc", Util.getDataRowVersion(p_row)];
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        public static bool TLOCInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TLOCCheckInsert(p_sqlCommand, p_row))
            {
                p_sqlCommand.CommandText = "INSERT INTO tloc (tloc, tloc_name, tloc_street, tloc_zipcode, tloc_city)"
                    + " VALUES (" 
                    + (int) p_row["tloc"] + ","
                    + createSqlString(p_row["tloc_name"], true) + ","
                    + createSqlString(p_row["tloc_street"], true) + ","
                    + createSqlString(p_row["tloc_zipcode"], true) + ","
                    + createSqlString(p_row["tloc_city"], true) + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        private static bool TLOCCheckDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, false, "tevt", "tloc", p_row["tloc", Util.getDataRowVersion(p_row)]))
                return false;

            return true;
        }
        
        private static bool TLOCCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            return true;
        }

        public static bool TLOCUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tloc SET "
                + "tloc_name=" + createSqlString(p_row["tloc_name"], true)
                + ", tloc_street=" + createSqlString(p_row["tloc_street"], true)
                + ", tloc_zipcode=" + createSqlString(p_row["tloc_zipcode"], true)
                + ", tloc_city=" + createSqlString(p_row["tloc_city"], true)
                + " WHERE tloc=" + (int) p_row["tloc"];
            return ExecuteNonQuery(p_sqlCommand);
        }

        //
        // TPER
        //
        public static bool TPERDelete(SqlConnection p_sqlConnection, SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TPERCheckDelete(p_sqlCommand, p_row))
            {
                if (DialogResult.Yes != MessageBox.Show("Person '" 
                    + p_row["tper_lastname", Util.getDataRowVersion(p_row)] + "' löschen?",
                    "Bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return false;
                }

                int nTPER = (int) p_row["tper", Util.getDataRowVersion(p_row)];

                p_sqlCommand.CommandText = "DELETE FROM tper WHERE tper=" + nTPER;
                return ExecuteNonQuery(p_sqlCommand);
            }

            return false;
        }

        public static bool TPERInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (TPERCheckInsert(p_sqlCommand, p_row))
            {
                string ts = createSqlString(DateTime.Now.ToString(), true);

                p_sqlCommand.CommandText = "INSERT INTO tper (tper, tper_lastname, tper_name, tper_age, tper_email, tper_phone"
                    + ", tper_contact_email, tper_contact_phone, dgen, dmai, dref, dsta, tper_created, tper_modified)"
                    + " VALUES (" 
                    + (int) p_row["tper"] + ","
                    + createSqlString(p_row["tper_lastname"], true) + ","
                    + createSqlString(p_row["tper_name"], true) + ","
                    + (int) p_row["tper_age"] + ","
                    + createSqlString(p_row["tper_email"], true) + ","
                    + createSqlString(p_row["tper_phone"], true) + ","
                    + createSqlString(p_row["tper_contact_email"], true) + ","
                    + createSqlString(p_row["tper_contact_phone"], true) + ","
                    + (int) p_row["dgen"] + ","
                    + (int) p_row["dmai"] + ","
                    + (int) p_row["dref"] + ","
                    + (int) p_row["dsta"] + ","
                    + ts + ","
                    + ts + ")";
                return ExecuteNonQuery(p_sqlCommand);
            }
            return false;
        }

        private static bool TPERCheckInsert(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, true, "dgen", "dgen", p_row["dgen"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "dmai", "dmai", p_row["dmai"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "dref", "dref", p_row["dref"]))
                return false;
            if (!FkCheck(p_sqlCommand, true, "dsta", "dsta", p_row["dref"]))
                return false;

            return true;
        }

        private static bool TPERCheckDelete(SqlCommand p_sqlCommand, DataRow p_row)
        {
            if (!FkCheck(p_sqlCommand, false, "tapp", "tper", p_row["tper", Util.getDataRowVersion(p_row)]))
                return false;

            return true;
        }

        public static bool TPERUpdate(SqlCommand p_sqlCommand, DataRow p_row)
        {
            p_sqlCommand.CommandText = "UPDATE tper SET "
                + "tper_lastname=" + createSqlString(p_row["tper_lastname"], true)
                + ", tper_name=" + createSqlString(p_row["tper_name"], true) 
                + ", tper_age=" + p_row["tper_age"] 
                + ", tper_email=" + createSqlString(p_row["tper_email"], true) 
                + ", tper_phone=" + createSqlString(p_row["tper_phone"], true) 
                + ", tper_contact_email=" + createSqlString(p_row["tper_contact_email"], true) 
                + ", tper_contact_phone=" + createSqlString(p_row["tper_contact_phone"], true)
                + ", dgen=" + p_row["dgen"]
                + ", dmai=" + p_row["dmai"]
                + ", dref=" + p_row["dref"]
                + ", dsta=" + p_row["dsta"]
                + ", tper_modified=" + createSqlString(DateTime.Now.ToString(), true)
                + " WHERE tper=" + p_row["tper"];

            return ExecuteNonQuery(p_sqlCommand);
        }

        private static bool ExecuteNonQuery(SqlCommand p_sqlCommand)
        {
            bool fSuccess = false;

            try
            {
                p_sqlCommand.ExecuteNonQuery();
                fSuccess = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "speeddating database error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show(e.ToString(), "speeddating database error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw e;
            }
            return fHasRecords;
        }

        private static string createSqlString(object p_o)
        {
            return createSqlString(p_o, false);
        }

        /*
         * Creates a SQL string by adding a simple quote at the beginning and at the end of the string.
         * 
         */
        private static string createSqlString(object p_o, bool p_fIsString)
        {
            if (DBNull.Value.Equals(p_o))
            {
                return "null";
            }
            else
            {
                if (p_fIsString)
                    return "'" + (string) p_o + "'";
                else
                    return p_o.ToString();
            }
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