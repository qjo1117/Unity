using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
	private static Managers _instance = null;
	public static Managers Instance { get { Init(); return _instance; } }

	ResourceManager _resource = new ResourceManager();
	InputManager _input = new InputManager();
	PoolManager _pool = new PoolManager();
	SceneManagerEx _scene = new SceneManagerEx();
	GameManager _game = new GameManager();

	public static ResourceManager Resource {  get { return Instance._resource; } }
	public static InputManager Input {  get { return Instance._input; } }
	public static PoolManager Pool {  get { return Instance._pool; } }
	public static SceneManagerEx Scene {  get { return Instance._scene; } }
	public static GameManager Game { get { return Instance._game; } }

	private void Start()
	{
		Init();
	}

	static void Init()
	{
		if(_instance == null)
		{
			GameObject go = GameObject.Find("@Managers");
			if(go == null)
			{
				go = new GameObject { name = "@Managers" };
				go.AddComponent<Managers>();
			}

			// 삭제 방지
			DontDestroyOnLoad(go);
			_instance = go.GetComponent<Managers>();
		}

		_instance._pool.Init();

	}

	private void Update()
	{
		_input.OnUpdate();	
	}


	public static void Clear()
	{
		_instance._pool.Clear();
		_instance._scene.Clear();
	}
}
