using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using System.Globalization;

namespace CMaurer.Common
{
    public abstract class DatabaseImplementation
    {
        private DbProviderFactory _dataFactory;

        public abstract DatabaseType DatabaseType { get; }
        public abstract string DateTime2DBDateTimeString(object dateTime);
        public abstract string DateTime2DBTimeString(object dateTime);
        public abstract string NullableDateTime2DBDateString(DateTime? dateTime);
        public abstract string NullableString2String(object text);
        public abstract string NullableDateTime2DBDateTimeString(DateTime? dateTime);
        public abstract string Object2DBDateString(object value);
        public abstract string TimestampNowSyntax();
        public abstract string MakeConcat(string value1, string value2);
        public abstract string MakeConcat(string value1, string value2, string value3);

        protected DatabaseImplementation()
        {
        }

        public DbProviderFactory DataFactory
        {
            set { _dataFactory = value; }
            get { return _dataFactory; }
        }

        public virtual int ConvertToInt32(object value)
        {
            int intValue = Convert.ToInt32(value);
            return intValue;
        }
        public virtual long ConvertToInt64(object value)
        {
            long intValue = Convert.ToInt64(value);
            return intValue;
        }

        public virtual string MakeLeft(string sql, int characters)
        {
            return string.Format(CultureInfo.InvariantCulture, "left({0}, {1})", sql, characters);
        }

        public virtual string CreateLikeExpression(string value)
        {
            return value;
        }

        public virtual string CleanSqlStatement(string sql)
        {
            string text = Regex.Replace(sql, @"(\s\s+)", @" ");
            text = text.Trim();
            return text;
        }
        public abstract string HandleTopLimitStuff(string sql, string numberRecords);
        public abstract void HandleTopLimitStuff(StringBuilder sb, string numberRecords);

        public IDbDataParameter SqlParameterInt(string parameterName, int value)
        {
            return SqlParameterInt(DataFactory, parameterName, value);
        }

        public static IDbDataParameter SqlParameterInt(DbProviderFactory dataFactory, string parameterName, int value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter sqlParameter = dataFactory.CreateParameter();
            sqlParameter.ParameterName = parameterName;
            sqlParameter.DbType = DbType.Int32;
            sqlParameter.Value = value;

            return sqlParameter;
        }

        public virtual IDbDataParameter SqlParameter(string parameterName, int value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter sqlParameter = DataFactory.CreateParameter();
            sqlParameter.ParameterName = parameterName;
            sqlParameter.DbType = DbType.Int32;
            sqlParameter.Value = value;
            return sqlParameter;
        }

        public virtual IDbDataParameter SqlParameter(string parameterName, string value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter sqlParameter = DataFactory.CreateParameter();
            sqlParameter.ParameterName = parameterName;
            sqlParameter.DbType = DbType.String;
            sqlParameter.Value = value;
            return sqlParameter;
        }

        public virtual IDbDataParameter SqlParameter(string parameterName, object value)
        {
            IDbDataParameter sqlParameter = DataFactory.CreateParameter();
            sqlParameter.ParameterName = parameterName;

            if (value is Int32)
            {
                sqlParameter.DbType = DbType.Int32;
            }
            else if (value is decimal)
            {
                sqlParameter.DbType = DbType.Int32;
            }
            else if (value is string)
            {
                sqlParameter.DbType = DbType.String;
            }
            else if (value is byte[])
            {
                // ChirurgenDokumente.Blob
                sqlParameter.DbType = DbType.Binary;
            }
            else
            {
                // bei DBNull.Value
                sqlParameter.DbType = DbType.Object;
            }

            sqlParameter.Value = value;
            return sqlParameter;
        }

        protected virtual IDbDataParameter SqlParameter(string parameterName, byte[] arValue)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter sqlParameter = DataFactory.CreateParameter();
            sqlParameter.ParameterName = parameterName;
            sqlParameter.DbType = DbType.Binary;
            sqlParameter.Value = arValue;
            return sqlParameter;
        }

        public virtual IDbDataParameter SqlParameterInt(string parameterName, object value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            IDbDataParameter oSQLParameter = DataFactory.CreateParameter();
            oSQLParameter.ParameterName = parameterName;
            oSQLParameter.DbType = DbType.Int32;
            if (value == DBNull.Value)
            {
                oSQLParameter.Value = DBNull.Value;
            }
            else
            {
                oSQLParameter.Value = ConvertToInt32(value);
            }
            return oSQLParameter;
        }
        public virtual void CompactDatabase(string strTargetDatabaseConnectionString)
        {
        }
    }
}

