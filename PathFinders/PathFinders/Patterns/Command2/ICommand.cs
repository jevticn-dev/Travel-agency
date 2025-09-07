using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Command2
{
    internal interface ICommand
    {
        public void Execute();
        public void Undo();
        public void Redo();

    }
}
