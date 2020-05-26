using System;
using System.Collections.Generic;
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
            return $"Data Source={ServerInstance};Initial Catalog={DBName};Integrated Security=True;Connection Timeout=1";
        }
        public static string GetString(string serverInstance, string dbName)
        {
            CreateString(serverInstance, dbName);
            return $"Data Source={ServerInstance};Initial Catalog={DBName};Integrated Security=True;Connection Timeout=1";
        }


    }
}
