using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Backup
{
    internal class Backup
    {

        public BackupScheduler InitializeBackup(string connectionString, string backupPath)
        {

            IBackupStrategy backupStrategy;
            string databaseName;

            if (connectionString.ToLower().Contains("server=") && connectionString.ToLower().Contains("database="))
            {
                var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString);
                databaseName = builder.Database;
                backupStrategy = new MySQLBackupStrategy(connectionString, backupPath);
            }
            else if (connectionString.ToLower().Contains("data source=") && connectionString.ToLower().Contains("version="))
            {
                var builder = new System.Data.SQLite.SQLiteConnectionStringBuilder(connectionString);
                databaseName = Path.GetFileName(builder.DataSource);
                backupStrategy = new SQLiteBackupStrategy(connectionString, backupPath);
            }
            else
            {
                Console.WriteLine("Unsupported database type.");
                return null;
            }

            IBackupObserver backupObserver = new BackupObserver(backupStrategy); //posmatrac koji kreira backup

            BackupScheduler scheduler = new BackupScheduler(24);
            scheduler.Attach(backupObserver);

            return scheduler;
        }
    }
}
