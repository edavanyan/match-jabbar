using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand : ICloneable
{
    void execute();
}

public interface ICommand<T> : ICloneable where T : MulticastDelegate
{
    void execute(T onComplete);
}
