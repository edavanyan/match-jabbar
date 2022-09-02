using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IService {

}

public abstract class Service<T> : MonoBehaviour, IService
{
    private readonly Dictionary<Type, T> _typeObjectMapper = new Dictionary<Type, T>();

    protected void MapToType(Type type, T obj) {
        _typeObjectMapper.Add(type, obj);
    }

    protected T GetServiceOfType(Type typeOfConfig) {
        return _typeObjectMapper[typeOfConfig];
    }
}
