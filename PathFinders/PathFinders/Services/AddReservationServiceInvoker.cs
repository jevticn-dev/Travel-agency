using PathFinders.Patterns.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public class AddReservationServiceInvoker
    {
        private ICommand _command;

        public void SetCommand(ICommand command)
        {
            _command = command;
        }

        public void ExecuteCommand()
        {
            _command.Execute();
        }
    }
}
