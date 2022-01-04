using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
	#region Pool
	class Pool
	{
		public GameObject original { get; private set; }

		public Transform root { get; set; }

		Stack<Poolable> _poolStack = new Stack<Poolable>();

		public void Init(GameObject origin,int count = 5)
		{
			original = origin;
			root = new GameObject().transform;
			root.name = $"{origin.name}_Root";

			for (int i = 0; i < count; ++i)
			{
				Push(Create());
			}
		}

		Poolable Create()
		{
			GameObject go = Object.Instantiate<GameObject>(original);
			go.name = original.name;

			Poolable pool = go.GetComponent<Poolable>();
			if(pool == null)
			{
				pool = go.AddComponent<Poolable>();
			}
			return pool;
		}

		public void Push(Poolable poolable)
		{
			if(poolable == null)
			{
				return;
			}

			poolable.transform.parent = root;
			poolable.gameObject.SetActive(false);
			poolable.isUsing = false;

			_poolStack.Push(poolable);
		}

		public Poolable Pop(Transform parent)
		{
			Poolable poolable;
			if(_poolStack.Count > 0)
			{
				poolable = _poolStack.Pop();
			}
			else
			{
				poolable = Create();
			}

			if(parent == null)
			{
				parent = root;
			}

			poolable.gameObject.SetActive(true);
			poolable.transform.parent = parent;
			poolable.isUsing = true;

			return poolable;
		}
	}
	#endregion

	private Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
	private Transform _root;

	public void Init()
	{
		if(_root == null)
		{
			_root = new GameObject { name = "@Pool_Root" }.transform;
			Object.DontDestroyOnLoad(_root);
		}
	}

	public void Push(Poolable poolable)
	{
		string name = poolable.gameObject.name;
		if(_pool.ContainsKey(name) == false)
		{
			GameObject.Destroy(poolable.gameObject);
			return;
		}
		_pool[name].Push(poolable);
	}

	public void CreatePool(GameObject origin, int count = 5)
	{
		if(_pool.ContainsKey(origin.name) == true)
		{
			return;
		}

		Pool pool = new Pool();
		pool.Init(origin, count);
		pool.root.parent = _root;

		_pool.Add(origin.name, pool);
	}

	public Poolable Pop(GameObject origin, Transform parent = null)
	{
		if(_pool.ContainsKey(origin.name) == false)
		{
			CreatePool(origin);
		}
		return _pool[origin.name].Pop(parent);
	}

	public GameObject GetOriginal(string name)
	{
		if(_pool.ContainsKey(name) == false)
		{
			return null;
		}
		return _pool[name].original;
	}

	public void Clear()
	{
		foreach(Transform child in _root)
		{
			GameObject.Destroy(child.gameObject);
		}
		_pool.Clear();
	}
}
