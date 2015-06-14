using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Convos.Enums;

namespace Convos.Utilities
{
    /// <summary>
    /// A helper class that handles interfacing with a SQL Server database
    /// </summary>
    public class SQLServerPlatform : ISQLPlatform
    {
        #region " Members "
        /// <summary>
        /// The output parameter name to get the result code
        /// </summary>
        private const string OUT_RESULT_CODE = "ResultCode";
        /// <summary>
        /// The output parameter name to get the error message
        /// </summary>
        private const string OUT_ERROR_MESSAGE = "ErrorMsg";
        #endregion " Members "

        #region " Constructors and Public Methods "
        /// <summary>
        /// Adds a parameter into the parameters collection
        /// </summary>
        /// <param name="command">The command for which the parameters are being sent</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="value">The parameter value</param>
        public void AddParameterWithValue(DbCommand command, string parameterName, object value)
        {
            if (!command.GetType().IsAssignableFrom(typeof(SqlCommand)))
            {
                throw new ArgumentException("Must provide an instance of a SqlCommand");
            }
            if (value == null)
            {
                ((SqlCommand)command).Parameters.AddWithValue(parameterName, DBNull.Value);
            }
            else
            {
                ((SqlCommand)command).Parameters.AddWithValue(parameterName, value);
            }
        }

        /// <summary>
        /// Creates the sql command with the supplied connection
        /// </summary>
        /// <param name="commandText">The command text to execute</param>
        /// <param name="connection">The connection on which to execute the command</param>
        /// <param name="commandType">The type of command</param>
        /// <returns>A DB Command object</returns>
        public DbCommand CreateCommand(string commandText, DbConnection connection, CommandType commandType)
        {
            if (!connection.GetType().IsAssignableFrom(typeof(SqlConnection)))
            {
                throw new ArgumentException("Must provide an instance of a SqlConnection");
            }
            SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)connection);
            cmd.CommandType = commandType;
            return cmd;
        }

        /// <summary>
        /// Creates a sql connection object
        /// </summary>
        /// <param name="connectionString">The key of the connection string in the config file</param>
        /// <returns>A DB connection object</returns>
        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// This function executes a sql stored procedure and returns a data reader of the results
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="message">A message returned back from the database</param>
        /// <param name="parameters">A dictionary of parameters for the stored procedure</param>
        /// <param name="procName">The name of the stored procedure</param>
        /// <param name="result">The result of the stored procedure</param>
        /// <returns>A data reader of the resulting recordset</returns>
        public DataSet ExecuteStoredProcWithResults(string connectionString, out string message, Dictionary<string, object> parameters, string procName, out ResultCode result)
        {
            var ds = new DataSet();
            using (DbConnection conn = CreateConnection(connectionString))
            {
                conn.Open();
                using (DbCommand cmd = CreateCommand(procName, conn, CommandType.StoredProcedure))
                {
                    foreach (var kvp in parameters)
                    {
                        AddParameterWithValue(cmd, kvp.Key, kvp.Value);
                    }

                    AddErrorParams(cmd);
                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = (SqlCommand)cmd;
                        adapter.Fill(ds);
                    }
                    message = GetErrorMessage(cmd);
                    result = GetResultCode(cmd);
                }
            }
            return ds;
        }

        /// <summary>
        /// This function executes a sql stored procedure that does not return a resulting recordset
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="message">A message returned back from the database</param>
        /// <param name="parameters">A dictionary of parameters for the stored procedure</param>
        /// <param name="procName">The name of the stored procedure</param>
        /// <param name="result">The result of the stored procedure</param>
        public void ExecuteStoredProc(string connectionString, out string message, Dictionary<string, object> parameters, string procName, out ResultCode result)
        {
            using (DbConnection conn = CreateConnection(connectionString))
            {
                conn.Open();
                using (DbCommand cmd = CreateCommand(procName, conn, CommandType.StoredProcedure))
                {
                    foreach (var kvp in parameters)
                    {
                        AddParameterWithValue(cmd, kvp.Key, kvp.Value);
                    }

                    AddErrorParams(cmd);
                    cmd.ExecuteNonQuery();
                    message = GetErrorMessage(cmd);
                    result = GetResultCode(cmd);
                }
            }
        }
        #endregion " Constructors and Public Methods "

        #region " Private Methods "
        /// <summary>
        /// This method adds the result code and error message parameters into the command
        /// </summary>
        /// <param name="cmd">The command object to add to</param>
        private void AddErrorParams(DbCommand cmd)
        {
            DbParameter param = cmd.CreateParameter();
            param.ParameterName = OUT_ERROR_MESSAGE;
            param.DbType = DbType.String;
            param.Size = 255;
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);

            param = cmd.CreateParameter();
            param.ParameterName = OUT_RESULT_CODE;
            param.DbType = DbType.Int16;
            param.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(param);
        }

        /// <summary>
        /// This method retrieves the error message from the command
        /// </summary>
        /// <param name="cmd">The command object from which to get the error message</param>
        /// <returns>The error message as a string</returns>
        private string GetErrorMessage(DbCommand cmd)
        {
            object val = cmd.Parameters[OUT_ERROR_MESSAGE].Value;
            if (val == null || val == DBNull.Value)
            {
                return "";
            }
            return (string)val;
        }

        /// <summary>
        /// This method retrieves the result code from the command
        /// </summary>
        /// <param name="cmd">The command object from which to get the result code</param>
        /// <returns>The result code</returns>
        private ResultCode GetResultCode(DbCommand cmd)
        {
            object val = cmd.Parameters[OUT_RESULT_CODE].Value;
            if (val == null || val == DBNull.Value)
            {
                return ResultCode.UNKNOWN;
            }
            return (ResultCode)Convert.ToInt32(val);
        }
        #endregion " Private Methods "
    }
}