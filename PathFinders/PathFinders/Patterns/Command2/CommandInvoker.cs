using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Command2
{
    internal class CommandInvoker
    {
        private readonly Stack<ICommand> undoStack = new();
        private readonly Stack<ICommand> redoStack = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute(); 
            undoStack.Push(command);
            redoStack.Clear();
        }

        public void UndoLastCommand()
        {
            if(undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
            }
        }

        public void RedoLastCommand()
        {
            var command = redoStack.Pop();
            command.Redo();
            undoStack.Push(command);
        }
    }
}
