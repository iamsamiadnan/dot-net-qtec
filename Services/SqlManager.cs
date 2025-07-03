using System;
using Microsoft.Data.SqlClient;

namespace dot_net_qtec.Services;


public class SqlManager
{
    private readonly SqlConnection _sqlConnection;

    public SqlManager(string connectionString)
    {
        _sqlConnection = new SqlConnection(connectionString);
    }

    public void ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null, bool isStoredProcedure = false) // insert, update, delete
    {
        using SqlCommand cmd = CreateCommand(query, parameters, isStoredProcedure);

        if (_sqlConnection.State != System.Data.ConnectionState.Open)
            _sqlConnection.Open();

        cmd.ExecuteNonQuery();
    }

    public List<T> ExecuteReader<T>(string query, Func<SqlDataReader, T> mapFunc, Dictionary<string, object>? parameters = null)
    {
        using SqlCommand cmd = CreateCommand(query, parameters);
        using SqlDataReader reader = cmd.ExecuteReader();

        List<T> rows = new List<T>();

        while (reader.Read())
            rows.Add(mapFunc(reader));

        return rows;
    }

    public void ExecuteTransaction(List<SqlCommand> sqlCommands)
    {
        using var transaction = _sqlConnection.BeginTransaction();

        try
        {
            foreach (var cmd in sqlCommands)
            {
                using var _ = cmd;
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public SqlCommand CreateCommand(string query, Dictionary<string, object>? parameters, bool isStoredProcedure = false)
    {
        SqlCommand cmd = new SqlCommand(query, _sqlConnection);

        if (isStoredProcedure) cmd.CommandType = System.Data.CommandType.StoredProcedure;

        if (parameters != null)
        {
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
            }
        }



        if (_sqlConnection.State != System.Data.ConnectionState.Open)
            _sqlConnection.Open();

        return cmd;
    }

    public static T? GetValue<T>(SqlDataReader reader, string columnName)
    {
        var value = reader[columnName];
        return value == DBNull.Value ? default : (T?)Convert.ChangeType(value, typeof(T));
    }
}
