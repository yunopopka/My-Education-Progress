using UnityEngine;
using System.Collections.Generic;

public class _BulletPool : MonoBehaviour
{
    public GameObject BulletPrefab;
    public int poolSize = 10;

    private List<GameObject> pool = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(BulletPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledBullet()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }
}
