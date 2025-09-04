using PathFinders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Factory
{
    public abstract class DatabaseFactory
    {
        public abstract IDatabaseService CreateDatabaseService();
    }
}
