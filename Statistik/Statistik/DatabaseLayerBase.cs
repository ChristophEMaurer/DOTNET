using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using System.Globalization;


/*
 Data Source=TORCL;User Id=system;Password=cmaurer;
 Data Source=XE;User Id=system;Password=cmaurer;
 ORA-12154: TNS:could not resolve the connect identifier specified
 */

namespace CMaurer.Common
{
    /// <summary>
    /// The database base class that allows access to different types of databases
    /// </summary>
    public abstract class DatabaseLayerBase
    {
        private BusinessLayerBase _businessLayerBase;
        private DatabaseImplementation _databaseImplementation;
        private IDbConnection _connection;
        private IDbConnection _connection2;
        private int _openStack;
        private string _connectionString;

        private DatabaseLayerBase() 
        { 
        }

        public DatabaseLayerBase(BusinessLayerBase businessLayerBase, DatabaseType databaseType, string connectionString) 
        {
            _businessLayerBase = businessLayerBase;
            _connectionString = connectionString;

            switch (databaseType)
            {
                case DatabaseType.OracleXE:
                    _databaseImplementation = new DatabaseImplementationOracle();
                    break;
            }
        }

        public DatabaseType DatabaseType 
        { 
            get { return _databaseImplementation.DatabaseType; } 
        }

        public string DateTime2DBDateTimeString(object dateTime) 
        {
            return _databaseImplementation.DateTime2DBDateTimeString(dateTime);
        }

        public string DateTime2DBTimeString(object dateTime)
        {
            return _databaseImplementation.DateTime2DBTimeString(dateTime);
        }

        public string NullableDateTime2DBDateString(DateTime? dateTime)
        {
            return _databaseImplementation.NullableDateTime2DBDateString(dateTime);
        }

        public string NullableDateTime2DBDateTimeString(DateTime? dateTime)
        {
            return _databaseImplementation.NullableDateTime2DBDateTimeString(dateTime);
        }

        public string Object2DBDateString(object value)
        {
            return _databaseImplementation.Object2DBDateString(value);
        }

        public string TimestampNowSyntax()
        {
            return _databaseImplementation.TimestampNowSyntax();
        }

        public string MakeConcat(string value1, string value2)
        {
            return _databaseImplementation.MakeConcat(value1, value2);
        }

        public string MakeConcat(string value1, string value2, string value3)
        {
            return _databaseImplementation.MakeConcat(value1, value2, value3);
        }

        public string HandleTopLimitStuff(string sql, string numberRecords)
        {
            return _databaseImplementation.HandleTopLimitStuff(sql, numberRecords);
        }
        public void HandleTopLimitStuff(StringBuilder sb, string numberRecords)
        {
            _databaseImplementation.HandleTopLimitStuff(sb, numberRecords);
        }


        /// <summary>
        /// Convert a database column of type sqlserver-int (c#-Int32) to an int.
        /// Every database except Oracle : int id = (int)row["ID"]
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int ConvertToInt32(object value)
        {
            return _databaseImplementation.ConvertToInt32(value);
        }
        public long ConvertToInt64(object value)
        {
            return _databaseImplementation.ConvertToInt64(value);
        }

        public string MakeLeft(string sql, int characters)
        {
            return _databaseImplementation.MakeLeft(sql, characters);
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            protected set { _connectionString = value; }
        }

        public DbProviderFactory DataFactory
        {
            set { _databaseImplementation.DataFactory = value; }
            get { return _databaseImplementation.DataFactory; }
        }

        protected IDbConnection Connection
        {
            get { return _connection; }
        }

        protected IDbConnection Connection2
        {
            get { return _connection2; }
        }

        public string DateString2DBDateString(string date)
        {
            DateTime dt = Tools.InputTextDate2DateTime(date);
            return DateTime2DBDateTimeString(dt);
        }
        public string DateString2DBDateStringEnd(string date)
        {
            DateTime dt = Tools.InputTextDate2DateTimeEnd(date);
            return DateTime2DBDateTimeString(dt);
        }
        public string TimeString2DBTimeString(string time)
        {
            DateTime dt = Tools.InputTextTime2DateTime(time);
            return DateTime2DBTimeString(dt);
        }

        public string Int2NullableForeignKey(object value)
        {
            string returnValue;

            if (value == DBNull.Value)
            {
                returnValue = "null";
            }
            else
            {
                returnValue = value.ToString();
            }

            return returnValue;
        }
        public string CreateLikeExpression(string value)
        {
            return _databaseImplementation.CreateLikeExpression(value);
        }

        protected IDbCommand CreateCommand()
        {
            return DataFactory.CreateCommand();
        }

        public string CleanSqlStatement(string sql)
        {
            return _databaseImplementation.CleanSqlStatement(sql);
        }

        protected bool Open(string connectionString)
        {
            bool success = false;

            if (_connection == null)
            {
                _connection = DataFactory.CreateConnection();
                _connection.ConnectionString = connectionString;
                try
                {
                    _connection.Open();
                    success = (_connection.State == ConnectionState.Open);
                }
                catch (Exception exception)
                {
                    Write2ErrorLog(exception);
                }
            }

            return success;
        }

        protected bool Open()
        {
            bool success = true;

            if (_connection == null)
            {
                _connection = DataFactory.CreateConnection();
                try
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    _openStack = 1;
                    success = (_connection.State == ConnectionState.Open);

                    //"Attempted to read or write protected memory. This is often an indication that other memory is corrupt."}
                }
                catch (Exception e)
                {
                    _openStack = 0;
                    success = false;
                    Write2ErrorLog(e);
                }
            }
            else
            {
                _openStack++;
            }
            return success;
        }

        protected bool Open2()
        {
            bool success = true;

            if (_connection2 == null)
            {
                _connection2 = DataFactory.CreateConnection();
                try
                {
                    _connection2.ConnectionString = _connectionString;
                    _connection2.Open();
                    success = (_connection2.State == ConnectionState.Open);
                }
                catch (Exception e)
                {
                    success = false;
                    Write2ErrorLog(e);
                }
            }
            return success;
        }

        protected void Close()
        {
            _openStack--;

            if (_openStack == 0)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Dispose();
                _connection = null;
            }
        }

        protected void Close2()
        {
            if (_connection2.State == ConnectionState.Open)
            {
                _connection2.Close();
            }
            _connection2.Dispose();
            _connection2 = null;
        }

        protected virtual void Write2ErrorLog(Exception exception)
        {
            _businessLayerBase.DisplayError(exception, null);
        }

        protected virtual void Write2ErrorLog(Exception exception, string message)
        {
            _businessLayerBase.DisplayError(exception, message);
        }

        protected virtual void Write2ErrorLog(string message)
        {
            _businessLayerBase.DisplayError(null, message);
        }

        public void MapSqlParameter2Command(IDbCommand command, ArrayList sqlParameters)
        {
            if (sqlParameters != null)
            {
                foreach (IDataParameter sqlParameter in sqlParameters)
                {
                    //
                    // do NOT log the value, or you will write all user passwords to the log file
                    //
                    command.Parameters.Add(sqlParameter);
                }
            }
        }

        public long ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        public long ExecuteScalar(string sql, ArrayList sqlParameters)
        {
            long result = 0;

            if (this.Open())
            {
                try
                {
                    sql = CleanSqlStatement(sql);
                    IDbCommand commmand = _connection.CreateCommand();
                    commmand.CommandText = sql;
                    if (sqlParameters != null)
                    {
                        MapSqlParameter2Command(commmand, sqlParameters);
                    }


                    object count = commmand.ExecuteScalar();
                    result = Convert.ToInt32(count, CultureInfo.InvariantCulture);

                    commmand.Dispose();
                    commmand = null;
                }
                catch (Exception exception)
                {
                    Write2ErrorLog(exception);
                }
                finally
                {
                    this.Close();
                }
            }
            else
            {
                Write2ErrorLog("Could not open connection");
            }

            return result;
        }

        public int ExecuteScalarInteger(string sql, ArrayList sqlParameters)
        {
            int result = 0;

            if (this.Open())
            {
                try
                {
                    sql = CleanSqlStatement(sql);
                    IDbCommand commmand = _connection.CreateCommand();
                    commmand.CommandText = sql;
                    if (sqlParameters != null)
                    {
                        MapSqlParameter2Command(commmand, sqlParameters);
                    }


                    object count = commmand.ExecuteScalar();
                    result = Convert.ToInt32(count, CultureInfo.InvariantCulture);
                    commmand.Dispose();
                    commmand = null;
                }
                catch (Exception e)
                {
                    Write2ErrorLog(e);
                }
                finally
                {
                    this.Close();
                }
            }
            else
            {
                Write2ErrorLog("Could not open connection!");
            }

            return result;
        }

        protected int GetLastGeneratedId()
        {
            int ID_NewRecord = 0;
            string sql = "SELECT @@IDENTITY";

            try
            {
                IDbCommand command = _connection.CreateCommand();
                command.CommandText = sql;
                object value = command.ExecuteScalar();
                ID_NewRecord = int.Parse(value.ToString(), CultureInfo.InvariantCulture);
                command.Dispose();
                command = null;
            }
            catch (Exception exception)
            {
                Write2ErrorLog(exception);
            }
            return ID_NewRecord;
        }
        /// <summary>
        /// Execute an SQL Statement. Any error produces a message box.
        /// Return error or success
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="aSqlParameter"></param>
        /// <param name="bSuccess">Is set to false on error</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, ArrayList sqlParameters)
        {
            int effectedRows = 0;

            if (this.Open())
            {
                try
                {
                    sql = CleanSqlStatement(sql);
                    IDbCommand command = _connection.CreateCommand();
                    command.CommandText = sql;
                    if (sqlParameters != null)
                    {
                        MapSqlParameter2Command(command, sqlParameters);
                    }
                    effectedRows = command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                catch (Exception e)
                {
                    Write2ErrorLog(e);
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                Write2ErrorLog("Could not open connection!");
            }

            return effectedRows;
        }

        /// <summary>
        /// Holt einen Datensatz. Es darf nur einer herauskommen, sonst stimmt etwas nicht.
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="aSqlParameter"></param>
        /// <param name="sTable"></param>
        /// <returns>Wenn es genau einen Datensazt gibt, kommt dieser heraus,
        /// <br/>wenn mehr als einer herauskommt, wird eine Exception geworfen.
        /// <br/>Wenn keiner herauskommt, wird null zurückgegeben.
        /// </returns>
        public DataRow GetRecord(string sql, ArrayList sqlParameters, string tableName)
        {
            return GetRecord(sql, sqlParameters, tableName, false);
        }

        /// <summary>
        /// Holt einen Datensatz. Wenn mehr als einer herauskommt und man returnFirst angibt, bekommt man den ersten, 
        /// ansonsten wird eine Exception geworfen.
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="aSqlParameter"></param>
        /// <param name="sTable"></param>
        /// <returns>Wenn es genau einen Datensazt gibt, kommt dieser heraus,
        /// <br/>wenn mehr als einer herauskommt, und das nicht gewünscht wird, wird eine Exception geworfen.
        /// <br/>Wenn keiner herauskommt, wird null zurückgegeben.
        /// </returns>
        public DataRow GetRecord(string sql, ArrayList sqlParameters, string tableName, bool returnFirst)
        {
            int count = 0;
            DataRow dataRow = null;

            if (this.Open())
            {
                try
                {
                    sql = CleanSqlStatement(sql);
                    IDbCommand command = _connection.CreateCommand();
                    command.CommandText = sql;
                    MapSqlParameter2Command(command, sqlParameters);
                    IDbDataAdapter dataAdapter = DataFactory.CreateDataAdapter();
                    dataAdapter.SelectCommand = command;
                    DataSet dataSet = new DataSet(tableName);
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    dataAdapter.Fill(dataSet);

                    count = dataSet.Tables[0].Rows.Count;
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        dataRow = dataSet.Tables[0].Rows[0];
                    }

                    dataSet.Dispose();
                    dataSet = null;
                    dataAdapter = null;
                }
                catch (Exception exception)
                {
                    Write2ErrorLog(exception);
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                Write2ErrorLog("Could not open connection!");
            }

            if (!returnFirst && count > 1)
            {
                // Mehr als ein Datensatz wurde gefunden. Das ist ganz schlecht. Im Nachhinein
                // kann man da nur eine Exception werfen.
                throw new MultipleRecordsException("Found more than one record\r" + sql);
            }

            return dataRow;
        }

        public DataView GetDataView(string sql, string tableName)
        {
            return GetDataView(sql, null, tableName);
        }

        public DataView GetDataView(string sql, ArrayList sqlParameters, string tableName)
        {
            DataView dataView = null;

            if (this.Open())
            {
                try
                {
                    sql = this.CleanSqlStatement(sql);
                    IDbCommand command = _connection.CreateCommand();
                    command.CommandText = sql;

                    if (sqlParameters != null)
                    {
                        MapSqlParameter2Command(command, sqlParameters);
                    }

                    IDbDataAdapter dataAdapter = DataFactory.CreateDataAdapter();
                    dataAdapter.SelectCommand = command;
                    DataSet dataSet = new DataSet(tableName);
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    dataAdapter.Fill(dataSet);

                    dataView = new DataView(dataSet.Tables[0]);

                    dataSet.Dispose();
                    dataSet = null;
                    dataAdapter = null;
                }
                catch (Exception exception)
                {
                    Write2ErrorLog(exception + ": " + sql);
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                Write2ErrorLog("Could not open connection!");
            }
            return dataView;
        }

        public int InsertRecord(string sql, ArrayList parameters, string tableName)
        {
            int ID_NewRecord = -1;
            string cleanSql = CleanSqlStatement(sql);
            if (this.Open())								// ausdrücklich für GetLastGeneratedId hier geöffnet!
            {
                if (this.ExecuteNonQuery(cleanSql, parameters) > 0)
                {
                    ID_NewRecord = this.GetLastGeneratedId();
                }
                else
                {
                    string message = "New record could not be inserted into table <@Table> !";
                    message = message.Replace("@Table", tableName.ToString());
                    Write2ErrorLog(message);
                }
                this.Close();
            }
            else
            {
                Write2ErrorLog("Could not open connection!");
            }

            return ID_NewRecord;
        }

        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }

        protected bool DeleteRecord(string sql, ArrayList sqlParameters, int id, string tableName)
        {
            bool success = true;
            if (this.ExecuteNonQuery(sql, sqlParameters) == 0)
            {
                success = false;
                string message = "Record <@ID> of table <@Table> could not be deleted!";
                message = message.Replace("@ID", id.ToString(CultureInfo.InvariantCulture));
                message = message.Replace("@Table", tableName);
                Write2ErrorLog(message);
            }

            return success;
        }
        #region Parameter
        public IDbDataParameter SqlParameterInt(string parameterName, int value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            return _databaseImplementation.SqlParameterInt(parameterName, value);
        }

        public IDbDataParameter SqlParameter(string parameterName, int value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            return _databaseImplementation.SqlParameter(parameterName, value);
        }

        public IDbDataParameter SqlParameter(string parameterName, string value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            return _databaseImplementation.SqlParameter(parameterName, value);
        }

        public IDbDataParameter SqlParameter(string parameterName, object value)
        {
            return _databaseImplementation.SqlParameter(parameterName, value);
        }

        protected IDbDataParameter SqlParameter(string parameterName, byte[] arValue)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            return _databaseImplementation.SqlParameter(parameterName, arValue);
        }

        public IDbDataParameter SqlParameterInt(string parameterName, object value)
        {
            // OleDB-Parameters beziehen sich auf die Position, nicht auf den Namen!!!
            // Reihenfolge ist wichtig!!!
            //Errorerrorerror
            return _databaseImplementation.SqlParameterInt(parameterName, value);
        }
 
        #endregion

        public virtual bool TestDatabaseConnection()
        {
            bool success = Open();

            Close();

            return success;
        }

        public DataView TestDatabaseConnection(string testDatabaseConnectionString, string sql)
        {
            DataView dataView = null;

            IDbConnection connection = null;
            IDbCommand command = null;
            DataSet dataSet = null;
            IDbDataAdapter dataAdapter = null;
            try
            {
                connection = DataFactory.CreateConnection();
                if (testDatabaseConnectionString != null)
                {
                    connection.ConnectionString = testDatabaseConnectionString;
                }
                else
                {
                    connection.ConnectionString = _connectionString;
                }
                connection.Open();
                command = connection.CreateCommand();
                command.CommandText = sql;
                dataAdapter = DataFactory.CreateDataAdapter();
                dataAdapter.SelectCommand = command;
                dataSet = new DataSet("Test");
                dataSet.Locale = CultureInfo.InvariantCulture;
                dataAdapter.Fill(dataSet);

                dataView = new DataView(dataSet.Tables[0]);
            }
            catch
            {
            }
            finally
            {
                if (dataSet != null)
                {
                    dataSet.Dispose();
                }
                dataAdapter = null;
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }

            return dataView;
        }
        public bool OpenForImport()
        {
            return Open();
        }
        public void CloseForImport()
        {
            Close();
        }
        public DataRow GetConfig(string key)
        {
            string sb =
                @"
                SELECT 
                    [Value]
                FROM 
                    Config
                WHERE
                    [Key]=@Key
                ";

            ArrayList arSqlParameter = new ArrayList();

            arSqlParameter.Add(SqlParameter("@Key", key));

            return this.GetRecord(sb, arSqlParameter, "Config");
        }
    }
}
