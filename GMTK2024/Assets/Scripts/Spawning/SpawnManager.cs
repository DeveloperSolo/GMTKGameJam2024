using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private List<GameObject> instances = new List<GameObject>();

    public GameObject SpawnInstance()
    {
        GameObject instance = FindAvailableInstance();
        if(instance != null)
        {
            instance.SetActive(true);
            return instance;
        }
        return InstantiateInstance();
    }

    public void ResetAll()
    {
        for (int i = 0; i < instances.Count; ++i)
        {
            if (instances[i].activeSelf)
            {
                instances[i].SetActive(false);
            }
        }
    }

    private GameObject InstantiateInstance()
    {
        GameObject instance = Instantiate(prefab);
        instance.transform.SetParent(transform);
        return instance;
    }

    private GameObject FindAvailableInstance()
    {
        for(int i = 0; i < instances.Count; ++i)
        {
            if (instances[i].activeSelf)
            {
                continue;
            }
            return instances[i];
        }
        return null;
    }
}