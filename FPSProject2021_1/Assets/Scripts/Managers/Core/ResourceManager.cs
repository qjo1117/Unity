using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T: Object
	{
		if(typeof(T) == typeof(GameObject)) {
			string name = path;
			int index = name.LastIndexOf('/');
			if(index >= 0) {
				name = name.Substring(index + 1);
			}

			GameObject go = Managers.Pool.GetOriginal(name);
			if(go != null) {
				return go as T;
			}
		}

		return Resources.Load<T>(path);
	}


	public GameObject Instantiate(string path, Transform parent = null)
	{
		GameObject origin = Load<GameObject>($"Prefabs/{path}");

		if(origin == null) {
			Debug.Log($"Failed to Load Prefab : {path}");
			return null;
		}

		if(origin.GetComponent<Poolable>() != null) {
			return Managers.Pool.Pop(origin, parent).gameObject;
		}

		GameObject go = Object.Instantiate(origin, parent);
		go.name = origin.name;
		return go;
	}

	public void Destroy(GameObject go)
	{
		if(go == null) {
			return;
		}

		Poolable poolable = go.GetComponent<Poolable>();
		if(poolable != null) {
			Managers.Pool.Push(poolable);
			return;
		}

		Object.Destroy(go);
	}
}


