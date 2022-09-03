using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

public class EventService : Service<Event>
{

    private List<EventListener> _eventListeners = new List<EventListener>();
    private object[] _parameters = new object[1];

    public void Awake()
    {
        MapToType(typeof(CommandExecutionCompleteEvent), new CommandExecutionCompleteEvent());
    }

    public T Get<T>() where T : Event
    {
        return (T)GetServiceOfType(typeof(T));
    }

    public void RegisterObserver(EventListener observer)
    {
        _eventListeners.Add(observer);
    }

    public void FireEvent(Type eventType)
    {
        foreach (var eventListener in _eventListeners)
        {
            var methodInfo = eventListener.GetType().GetDeclaredMethods();
            foreach (var info in methodInfo)
            {
                if (info.GetCustomAttribute<EventHandler>() != null)
                {
                    if (info.GetParameters()[0].ParameterType == eventType)
                    {
                        _parameters[0] = GetServiceOfType(eventType);
                        info.Invoke(eventListener, _parameters);
                        break;
                    }
                }
            }
        }
    }
}
