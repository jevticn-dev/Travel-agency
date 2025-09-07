using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                var builder = new System.Data.SQLite.SQLiteConnectionStringBuilder(connectionString);
                string dbFileName = builder.DataSource; //iz connection stringa se uzima ime baze

                string scriptDirectory = Path.Combine(baseDirectory, @"..\..\..\Scripts");
                string fullBasePath = Path.Combine(scriptDirectory, dbFileName);

                string time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupName = $"{dbFileName}_backup_{time}.db";
                string backupFile = Path.Combine(backupFolder, backupName);

                File.Copy(fullBasePath, backupFile, true);

                //Console.WriteLine("SQLite backup created: " + backupFile);

            }
            catch(Exception ex)
            {
                Console.WriteLine("SQLite backup failed! " + ex.Message);
            }
        }
    }
}
