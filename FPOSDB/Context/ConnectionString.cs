using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FPOSDB.Context
{
    public static class ConnectionString
    {
        public static string ServerInstance;
        public static string DBName;

        public static void CreateString(string serverInstance, string dbName)
        {
            ServerInstance = serverInstance;
            DBName = dbName;
        }
        public static string GetString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.ConnectTimeout = 1;
            builder.DataSource = ServerInstance;
            builder.IntegratedSecurity = true;
            //builder["Trusted_Connection"] = true;
            builder.InitialCatalog = DBName;
            //builder.UserInstance = true;
            return builder.ConnectionString;
        }
        public static string GetString(string serverInstance, string dbName)
        {
            CreateString(serverInstance, dbName);
            return $"Data Source={ServerInstance};Initial Catalog={DBName};Integrated Security=True;Connection Timeout=1";
        }


    }
}
