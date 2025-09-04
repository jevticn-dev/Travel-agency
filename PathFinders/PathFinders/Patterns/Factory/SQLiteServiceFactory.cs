using PathFinders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Factory
{
    public class SQLiteServiceFactory : DatabaseFactory
    {
        private readonly string _connectionString;

        public SQLiteServiceFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override IDatabaseService CreateDatabaseService()
        {
            return new SQLiteService(_connectionString);
        }
    }
}
