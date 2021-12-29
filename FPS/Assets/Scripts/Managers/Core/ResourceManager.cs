using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{

    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            // ObjectPoolManager에 있는 녀석인지 확인한다.
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
            {
                return go as T;
            }
        }

        return Resources.Load<T>(path);
    }

    // Resource에서 저장하는 것이 아니라 불러오는 것이다.
    // 거의 Resource폴더를 접근해서 Prefab,Sound,Particle등을 참조하기 쉽게 만들어둔 PathManager느낌
    public GameObject NewPrefab(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to Load prefab : {path}");
            return null;
        }

        // Poolable을 가지고 있으면 PoolManager에 있는 녀석이다.
        if (original.GetComponent<Poolable>() != null)
        {
            return Managers.Pool.Pop(original, parent).gameObject;
        }

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void DelPrefab(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        // 만약에 풀링이 필요한 아이라면 -> 풀링 매니저한테 맡겨진다.
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }

    public void Destroy(GameObject go, float time)
    {
        if (go == null)
        {
            return;
        }

        Object.Destroy(go);
    }
}
