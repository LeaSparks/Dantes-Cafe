using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private List<GameObject> _pool;
    Transform _parent;
    GameObject _objectType;

    public ObjectPool(GameObject objectType, int amount, GameObject parent)
    {
        _pool = new List<GameObject>();
        _parent = parent.transform;
        _objectType = objectType;

        GameObject tmp;

        for (int i = 0; i < amount; i++)
        {
            AddToPool();
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                return _pool[i];
            }
        }
        return AddToPool();
    }

    public GameObject GetActivePooledObject()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                _pool[i].SetActive(true);
                return _pool[i];
            }
        }
        return AddToPool(true);
    }

    private GameObject AddToPool(bool activate = false)
    {
        GameObject tmp = GameObject.Instantiate(_objectType, _parent);
        tmp.SetActive(activate);
        _pool.Add(tmp);

        return tmp;
    }

}
