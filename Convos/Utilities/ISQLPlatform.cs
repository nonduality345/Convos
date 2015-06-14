using Convos.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Convos.Utilities
{
    /// <summary>
    /// An interface for sql helper functions
    /// </summary>
    public interface ISQLPlatform
    {
        /// <summary>
        /// Adds a parameter into the parameters collection
        /// </summary>
        /// <param name="command">The command for which the parameters are being sent</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="value">The parameter value</param>
        void AddParameterWithValue(DbCommand command, string parameterName, Object value);

        /// <summary>
        /// Creates the sql command with the supplied connection
        /// </summary>
        /// <param name="commandText">The command text to execute</param>
        /// <param name="connection">The connection on which to execute the command</param>
        /// <param name="commandType">The type of command</param>
        /// <returns>A DB Command object</returns>
        DbCommand CreateCommand(string commandText, DbConnection connection, CommandType commandType);

        /// <summary>
        /// Creates a sql connection object
        /// </summary>
        /// <param name="connectionString">The key of the connection string in the config file</param>
        /// <returns>A DB connection object</returns>
        DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// This function executes a sql stored procedure and returns a data reader of the results
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="message">A message returned back from the database</param>
        /// <param name="parameters">A dictionary of parameters for the stored procedure</param>
        /// <param name="procName">The name of the stored procedure</param>
        /// <param name="result">The result of the stored procedure</param>
        /// <returns>A data reader of the resulting recordset</returns>
        DataSet ExecuteStoredProcWithResults(string connectionString, out string message, Dictionary<string, object> parameters, string procName, out ResultCode result);

        /// <summary>
        /// This function executes a sql stored procedure that does not return a resulting recordset
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="message">A message returned back from the database</param>
        /// <param name="parameters">A dictionary of parameters for the stored procedure</param>
        /// <param name="procName">The name of the stored procedure</param>
        /// <param name="result">The result of the stored procedure</param>
        void ExecuteStoredProc(string connectionString, out string message, Dictionary<string, object> parameters, string procName, out ResultCode result);
    }
}