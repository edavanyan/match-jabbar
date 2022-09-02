using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
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

            item.gameObject.SetActive(true);
            return item;
        }

        return GameObject.Instantiate<T>(_prototypePrefab);
    }

    public void DestoryItem(T item) {
        _freeItemList.Add(item);
        item.transform.SetParent(null);
        item.gameObject.SetActive(false);
    }
}
