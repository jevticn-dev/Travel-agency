using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace PathFinders.Backup
{
    internal class MySQLBackupStrategy : IBackupStrategy
    {
        private readonly string connectionString;
        private readonly string backupFolder;

        public MySQLBackupStrategy(string connectionString, string backupFolder)
        {
            this.connectionString = connectionString;
            this.backupFolder = backupFolder;
        }

        public void CreateBackup()
        {
            try
            {
                if (!Directory.Exists(this.backupFolder))
                {
                    Directory.CreateDirectory(this.backupFolder);
                }


                var builder = new MySqlConnectionStringBuilder(connectionString);
                string dbFileName = builder.Database;

                string time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupName = $"{dbFileName}_backup_{time}.sql";
                string backupFile = Path.Combine(backupFolder, backupName);


                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = connection;
                            connection.Open();
                            mb.ExportToFile(backupFile);
                            connection.Close();
                        }
                    }
                }

                Console.WriteLine("MySQL backup created: " + backupFile);

            }
            catch (Exception ex)
            {
                Console.WriteLine("MySQL backup failed! " + ex.Message);
            }
        }
    }
}
