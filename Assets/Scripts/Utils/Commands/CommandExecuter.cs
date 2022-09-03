using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExecuter : EventListener
{
        private Queue<ICommand> _commandList = new Queue<ICommand>();
        private bool _busy = false;

        public CommandExecuter()
        {
                GameController.Instance.Events.RegisterObserver(this);
        }

        public void RegisterCommand(ICommand command)
        {
                _commandList.Enqueue(command);
                if (!_busy)
                {
                        ExecuteCommands();
                }
        }

        private void ExecuteCommands()
        {
                if (_commandList.TryDequeue(out var command))
                {
                        _busy = true;
                        command.execute();
                }
        }

        [EventHandler]
        private void OnCommandExecutionCompleteEvent(CommandExecutionCompleteEvent completeEvent)
        {
                _busy = false;
                ExecuteCommands();
        }
}
