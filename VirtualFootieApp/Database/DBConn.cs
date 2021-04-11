using System;
using System.Data;
using System.Data.SqlClient;

namespace VirtualFootieApp.Database
{
    public class DBConn
    {
        IDbConnection _connection;
        readonly string _connectionString = "Data Source=.;Initial Catalog=futbin;Integrated Security=True";

        public IDbConnection Connection
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._connectionString))
                {
                    throw new Exception("connection string missing");
                }

                this._connection = new SqlConnection(_connectionString);
                return this._connection;
            }
        }
    }
}
