using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SwipeCommand : ICommand, IPoolable
{
    private Tile _tile1;
    private Tile _tile2;
    private TweenCallback _onComplete;

    public SwipeCommand()
    {
        
    }
    
    public SwipeCommand(Tile tile1, Tile tile2)
    {
        Set(tile1, tile2);
    }

    public void Set(Tile tile1, Tile tile2, TweenCallback onComplete = null)
    {
        _tile1 = tile1;
        _tile2 = tile2;
        _onComplete = onComplete;
    }

    public void execute()
    {
        var element = _tile1.Element;
        _tile1.SetElement(_tile2.Element);
        _tile2.SetElement(element, false, _onComplete);
    }

    public void New()
    {
        
    }

    public void Free()
    {
        
    }

    public object Clone()
    {
        return new SwipeCommand(_tile1, _tile2);
    }
}
