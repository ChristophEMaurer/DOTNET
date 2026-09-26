using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

//C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Data.OracleClient.dll
//using System.Data.OracleClient;

namespace CMaurer.Common
{
	/// <summary>
	/// </summary>
    public class DatabaseImplementationOracle : DatabaseImplementation
	{
        public DatabaseImplementationOracle()
		{
            DataFactory = DbProviderFactories.GetFactory("System.Data.OracleClient");
        }

        public override int ConvertToInt32(object o)
        {
            return Convert.ToInt32(o);
        }

        public override string MakeConcat(string s1, string s2)
        {
            return string.Format("concat({0}, {1})", s1, s2);
        }

        public override string MakeConcat(string s1, string s2, string s3)
        {
            return string.Format("concat({0}, concat({1}, {2}))", s1, s2, s3);
        }

        public override string TimestampNowSyntax()
        {
            return "SYSDATE";
        }

        public override string NullableDateTime2DBDateString(DateTime? dt)
        {
            string s;

            if (dt.HasValue)
            {
                s = string.Format("to_date('{0:00}.{1:00}.{2:0000}', 'dd.mm.yyyy'", dt.Value.Day, dt.Value.Month, dt.Value.Year);
            }
            else
            {
                s = "null";
            }

            return s;
        }

        public override string Object2DBDateString(object o)
        {
            string s;

            if (o == null)
            {
                DateTime? dt = null;
                s = NullableDateTime2DBDateString(dt);
            }
            else if (o == DBNull.Value)
            {
                DateTime? dt = null;
                s = NullableDateTime2DBDateString(dt);
            }
            else
            {
                DateTime? dt = (DateTime?)o;
                s = NullableDateTime2DBDateString(dt);
            }

            return s;
        }

        public override string NullableString2String(object text)
        {
            string sReturn;

            if (text == DBNull.Value)
            {
                sReturn = "";
            }
            else
            {
                sReturn = (string) text;
            }

            return sReturn;
        }

        public override string DateTime2DBDateTimeString(object oDateTime)
        {
            string sReturn;

            if (oDateTime == DBNull.Value)
            {
                sReturn = "null";
            }
            else
            {
                DateTime dt = (DateTime)oDateTime;

                sReturn = NullableDateTime2DBDateTimeString(dt);
            }

            return sReturn;
        }

        public override string NullableDateTime2DBDateTimeString(DateTime? oDateTime)
        {
            string sReturn;

            if (oDateTime.HasValue)
            {
                sReturn = string.Format("to_date('{0:00}.{1:00}.{2:0000}', 'dd.mm.yyyy HH24:MI:SS'",
                    oDateTime.Value.Day, oDateTime.Value.Month, oDateTime.Value.Year,
                    oDateTime.Value.Hour, oDateTime.Value.Minute, oDateTime.Value.Second
                    );
            }
            else
            {
                sReturn = "null";
            }

            return sReturn;
        }

        public override string DateTime2DBTimeString(object oDateTime)
        {
            // nicht getestet
            string sReturn;

            if (oDateTime == DBNull.Value)
            {
                sReturn = "null";
            }
            else
            {
                DateTime dt = (DateTime)oDateTime;

                sReturn = string.Format("to_date('01.01.1800 {0:00}:{1:00}:{2:00}', 'dd.mm.yyyy HH24:MI:SS'", 
                    dt.Hour, dt.Minute, dt.Second
                    );
            }

            return sReturn;
        }

        // reservierte Identifier müssen mit ??? gequoted werden
        // ParameterDirection benötigen ein : statt einem @
        public override string CleanSqlStatement(string sSQL)
        {
            string s = sSQL.Replace('@', ':');

            // [hallo] > hallo
            s = Regex.Replace(s, @"\[", @"");
            s = Regex.Replace(s, @"\]", @"");

            return base.CleanSqlStatement(s);
        }

        public override IDbDataParameter SqlParameter(string sParameterName, string sValue)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter oSQLParameter = DataFactory.CreateParameter();
            oSQLParameter.ParameterName = sParameterName.Replace('@', ':');
            oSQLParameter.DbType = DbType.String;
            oSQLParameter.Value = sValue;

            return oSQLParameter;
        }

        public override IDbDataParameter SqlParameter( string sParameterName, int iValue)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter oSQLParameter = DataFactory.CreateParameter();
            oSQLParameter.ParameterName = sParameterName.Replace('@', ':');
            oSQLParameter.DbType = DbType.Int32;
            oSQLParameter.Value = iValue;
            return oSQLParameter;
        }

        public override IDbDataParameter SqlParameter(string sParameterName, object value)
        {
            IDbDataParameter oSQLParameter = DataFactory.CreateParameter();
            oSQLParameter.ParameterName = sParameterName.Replace('@', ':');

            if (value is Int32)
            {
                oSQLParameter.DbType = DbType.Int32;
            }
            else if (value is string)
            {
                oSQLParameter.DbType = DbType.String;
            }
            else if (value is byte[])
            {
                // ChirurgenDokumente.Blob
                oSQLParameter.DbType = DbType.Binary;
            }
            else
            {
                // bei DBNull.Value
                oSQLParameter.DbType = DbType.Object;
            }

            oSQLParameter.Value = value;
            return oSQLParameter;
        }

        protected override IDbDataParameter SqlParameter(string sParameterName, byte[] arValue)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter oSQLParameter = DataFactory.CreateParameter();
            oSQLParameter.ParameterName = sParameterName.Replace('@', ':');
            oSQLParameter.DbType = DbType.Binary;
            oSQLParameter.Value = arValue;
            return oSQLParameter;
        }

        public override IDbDataParameter SqlParameterInt(string sParameterName, object o)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter oSQLParameter = DataFactory.CreateParameter();
            oSQLParameter.ParameterName = sParameterName.Replace('@', ':');
            oSQLParameter.DbType = DbType.Int32;
            if (o == DBNull.Value)
            {
                oSQLParameter.Value = DBNull.Value;
            }
            else
            {
                oSQLParameter.Value = ConvertToInt32(o);
            }
            return oSQLParameter;
        }

        public override string HandleTopLimitStuff(string sql, string numRecords)
        {
            string s = sql.Replace("@@LIMIT@@", "LIMIT " + numRecords);
            s = s.Replace("@@TOP@@", "");

            return s;
        }

        public override void HandleTopLimitStuff(StringBuilder sb, string numRecords)
        {
            sb.Replace("@@LIMIT@@", "LIMIT " + numRecords);
            sb.Replace("@@TOP@@", "");
        }

        public override DatabaseType DatabaseType 
        {
            get { return DatabaseType.OracleXE; } 
        }
    }
}

