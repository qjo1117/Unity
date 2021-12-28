using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
	#region Pool

	class Pool
	{
		public GameObject _origin;
		public Transform  _root;
		Stack<Poolable> _stackPool = new Stack<Poolable>();

		public void Init(GameObject origin, int count = 5)
		{
			_origin = origin;
			_root = new GameObject().transform;
			_root.name = $"{origin.name}_Root";

			for (int i = 0; i < count; ++i) {
				Push(Create());
			}
		}


		Poolable Create()
		{
			GameObject go = Object.Instantiate<GameObject>(_origin);
			go.name = _origin.name;
			return go.GetOrAddComponent<Poolable>();
		}

		public void Push(Poolable poolable)
		{
			if(poolable) {
				return;
			}

			poolable.transform.parent = _root;
			poolable.gameObject.SetActive(false);
			poolable.IsUsing = false;

			_stackPool.Push(poolable);
		}

		public Poolable Pop(Transform parent)
		{
			Poolable poolable;

			if(_stackPool.Count > 0){
				poolable = _stackPool.Pop();
			}
			else {
				// 왜 넣지않고 만들기만 하면은 바로 사용하기 위해서
				poolable = Create();
			}

			if(parent == null) {
				poolable.transform.parent = Managers.Scene.CurrentScene.transform;
			}

			poolable.transform.parent = parent;
			poolable.IsUsing = true;

			return poolable;
		}

	}

	#endregion

	Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
	Transform _root;
	

	public void Init()
	{
		if(_root == null) {
			_root = new GameObject { name = "@Pool_Root" }.transform;
			Object.DontDestroyOnLoad(_root);
		}
	}

	public void CreatePool(GameObject origin, int count = 5)
	{
		Pool pool = new Pool();
		pool.Init(origin, count);
		pool._root.parent = _root;

		_pool.Add(origin.name, pool);
	}

	public void Push(Poolable poolable)
	{
		string name = poolable.gameObject.name;

		// 만약 없는 녀석이면
		if(_pool.ContainsKey(name) == false) {
			GameObject.Destroy(poolable.gameObject);
			return;
		}

		_pool[name].Push(poolable);
	}


	public Poolable Pop(GameObject origin, Transform parent = null)
	{
		if(_pool.ContainsKey(origin.name) == false) {
			CreatePool(origin);
		}

		return _pool[origin.name].Pop(parent);
	}

	public GameObject GetOriginal(string name)
	{
		if(_pool.ContainsKey(name) == false) {
			return null;
		}
		return _pool[name]._origin;
	}

	public void Clear()
	{
		foreach(Transform child in _root) {
			GameObject.Destroy(child.gameObject);
		}

		_pool.Clear();
	}
}
