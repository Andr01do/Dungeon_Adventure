using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Header("Settings")]

    public int maxPoolSize = 10;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        GameObject objectToSpawn;

        if (poolDictionary[key].Count > 0)
        {
            objectToSpawn = poolDictionary[key].Dequeue();

            if (objectToSpawn == null)
            {
                return CreateNewObject(prefab, position, rotation);
            }
        }
        else
        {

            objectToSpawn = CreateNewObject(prefab, position, rotation);
        }


        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;


        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        return objectToSpawn;
    }

    private GameObject CreateNewObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject newObj = Instantiate(prefab, position, rotation);

        newObj.name = prefab.name;
        return newObj;
    }

    public void ReturnToPool(GameObject obj)
    {
        string key = obj.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }
        if (poolDictionary[key].Count >= maxPoolSize)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[key].Enqueue(obj);
    }
}