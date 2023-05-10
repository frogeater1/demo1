using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> _poolList = new();

    private void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        foreach (GameObject poolPrefab in poolPrefabs)
        {
            Transform parent = new GameObject(poolPrefab.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(poolPrefab, parent),
                e => e.SetActive(true),
                e => e.SetActive(false),
                Destroy
            );
            
            _poolList.Add(newPool);
        }
    }
}