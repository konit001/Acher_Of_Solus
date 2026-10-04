using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }

    #region Singleton 
    public static ObjectPooling Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // สร้างที่นี่แทน Start() เผื่อมีคนเรียก SpawnFromPool ก่อน Start ของ Object นี้จะรัน
            poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion 

    public List<Pool> pools;
    public Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    [Tooltip("ขนาด pool ที่สร้างให้อัตโนมัติ เมื่อมีคนขอ prefab ที่ไม่ได้ลงทะเบียนไว้ใน List")]
    public int defaultPoolSize = 10;

    void Start()
    {
        foreach (Pool pool in pools)
        {
            CreatePool(pool.prefab, pool.size);
        }
    }

    // แยกออกมาเพื่อให้ทั้งตอน Start และตอนสร้าง pool อัตโนมัติใช้โค้ดชุดเดียวกัน
    private Queue<GameObject> CreatePool(GameObject prefab, int size)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(prefab, objectPool);
        return objectPool;
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        // prefab ของสกิลใหม่ ๆ มักไม่ได้ถูกใส่ใน List ของ Inspector — สร้าง pool ให้เองดีกว่าเงียบหาย
        if (!poolDictionary.TryGetValue(prefab, out Queue<GameObject> objectPool))
        {
            objectPool = CreatePool(prefab, defaultPoolSize);
        }

        GameObject objToSpawn = objectPool.Dequeue();

        // ถ้าตัวหัวคิวยังบินอยู่ แปลว่า pool หมด — สร้างเพิ่มแทนที่จะแย่งกระสุนที่ยังทำงานอยู่มาใช้
        if (objToSpawn.activeInHierarchy)
        {
            objectPool.Enqueue(objToSpawn);
            objToSpawn = Instantiate(prefab);
        }

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        objectPool.Enqueue(objToSpawn);

        return objToSpawn;
    }
}

public interface IPooledObject
{
    void OnObjectSpawn();
}