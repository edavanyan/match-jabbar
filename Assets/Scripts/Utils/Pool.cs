using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour, IPoolable
{   
    private readonly List<T> _freeItemList = new List<T>();
    private readonly T _prototypePrefab;

    public Pool(T prototypePrefab) 
    {
        _prototypePrefab = prototypePrefab;
    }

    public T NewItem() {
        if (_freeItemList.Count > 0) {
            var item = _freeItemList[0];
            _freeItemList.RemoveAt(0);

            item.New();
            return item;
        }

        return GameObject.Instantiate<T>(_prototypePrefab);
    }

    public void DestoryItem(T item) {
        _freeItemList.Add(item);
        item.Free();
    }
}
