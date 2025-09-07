using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.SQLite;

namespace PathFinders.Backup
{
    internal class SQLiteBackupStrategy : IBackupStrategy
    {
        private readonly string connectionString;
        private readonly string backupFolder;

        public SQLiteBackupStrategy(string connectionString, string backupFolder)
        {
            this.connectionString = connectionString;
            this.backupFolder = backupFolder;
        }

        public void CreateBackup()
        {
            try {
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                var builder = new System.Data.SQLite.SQLiteConnectionStringBuilder(connectionString);
                string dbFileName = builder.DataSource; //iz connection stringa se uzima ime baze

                string time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupName = $"{dbFileName}_backup_{time}.db";
                string backupFile = Path.Combine(backupFolder, backupName);

                using (var source = new SQLiteConnection(connectionString))
                {
                    source.Open();

                    using (var destination = new SQLiteConnection($"Data Source={backupFile};Version=3;"))
                    {
                        destination.Open();
                        source.BackupDatabase(destination, "main", "main", -1, null, 0);
                    }
                }

                Console.WriteLine("SQLite backup created: " + backupFile);

            }
            catch(Exception ex)
            {
                Console.WriteLine("SQLite backup failed! " + ex.Message);
            }
        }
    }
}
