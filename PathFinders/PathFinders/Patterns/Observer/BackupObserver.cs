using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Backup
{
    internal class BackupObserver : IBackupObserver
    {
        private readonly IBackupStrategy backupStrategy;

        public BackupObserver(IBackupStrategy backupStrategy)
        {
            this.backupStrategy = backupStrategy;
        }

        public void Update()
        {
            Console.WriteLine("Observer received notification. Starting back up...");
            backupStrategy.CreateBackup();
        }

    }
}

