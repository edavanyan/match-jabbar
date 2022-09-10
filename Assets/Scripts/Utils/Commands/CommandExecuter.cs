using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExecuter : EventListener
{
        private readonly Queue<ICommand> _commandList = new Queue<ICommand>();
        public bool Busy { get; private set; }

        public CommandExecuter()
        {
                GameController.Instance.Events.RegisterObserver(this);
        }

        public void RegisterCommand(ICommand command)
        {
                _commandList.Enqueue(command);
                if (!Busy)
                {
                        ExecuteCommands();
                }
        }

        private void ExecuteCommands()
        {
                if (_commandList.TryDequeue(out var command))
                {
                        Busy = true;   
                        command.execute();
                }
        }

        [EventHandler]
        private void OnCommandExecutionCompleteEvent(CommandExecutionCompleteEvent completeEvent)
        {
                Busy = false;
                ExecuteCommands();
        }
}
