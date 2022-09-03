using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExecutionCompleteEvent : Event
{
    public SwipeCommand Command { get; set; }
}
