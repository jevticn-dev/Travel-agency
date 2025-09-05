using PathFinders.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Multiton
{
    public class DatabaseManager
    {
        private static readonly ConcurrentDictionary<string, IDatabaseService> _instances = new ConcurrentDictionary<string, IDatabaseService>();

        private DatabaseManager() { }

        public static IDatabaseService GetInstance(string connectionString)
        {
            return _instances.GetOrAdd(connectionString, key =>
            {
                string dbType = GetDatabaseTypeFromConnection(key);
                IDatabaseService service;

                switch (dbType)
                {
                    case "mysql":
                        service = new PathFinders.Patterns.Factory.MySqlServiceFactory(key).CreateDatabaseService();
                        break;
                    case "sqlite":
                        service = new PathFinders.Patterns.Factory.SQLiteServiceFactory(key).CreateDatabaseService();
                        break;
                    default:
                        throw new ArgumentException("Unsupported database type.");
                }
                return service;
            });
        }

        private static string GetDatabaseTypeFromConnection(string connectionString)
        {
            string normalized = connectionString.ToLower();
            if (normalized.Contains("server=") || normalized.Contains("uid=") || normalized.Contains("port="))
            {
                return "mysql";
            }
            if (normalized.Contains("data source"))
            {
                return "sqlite";
            }
            return "unknown";
        }
    }
}