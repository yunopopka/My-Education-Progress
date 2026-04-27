using UnityEngine;
using System.Collections.Generic;

public class _EffectPool : MonoBehaviour
{
    public GameObject EffectPrefab;
    public int poolSize = 5;

    private List<GameObject> pool = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(EffectPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledEffect()
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
