using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PathFinders.Backup
{
    internal class BackupScheduler
    {
        private readonly List<IBackupObserver> observers = new List<IBackupObserver>();
        private readonly System.Timers.Timer timer;

        public BackupScheduler(int intervalHours)
        {
            this.timer = new System.Timers.Timer(intervalHours * 60 * 60 * 1000);
            timer.Elapsed += (sender, e) => NotifyObservers();
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void Attach(IBackupObserver observer)
        {
            observers.Add(observer);
            Console.WriteLine("Observer attached.");
        }

        public void Detach(IBackupObserver observer)
        {
            observers.Remove(observer);
            Console.WriteLine("Observer detached.");
        }

        private void NotifyObservers()
        {
            Console.WriteLine("Notifying observers to perform backup...");
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }

        public void Stop()
        {
            if(this.timer != null)
            {
                this.timer.Enabled = false;
                this.timer.Dispose();
                Console.WriteLine("Backup scheduler stopped!");
            }
        }
    }
}
