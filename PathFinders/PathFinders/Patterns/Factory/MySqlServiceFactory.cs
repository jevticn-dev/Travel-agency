using PathFinders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Factory
{
    public class MySqlServiceFactory : DatabaseFactory
    {
        private readonly string _connectionString;

        public MySqlServiceFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override IDatabaseService CreateDatabaseService()
        {
            return new MySqlService(_connectionString);
        }
    }
}
