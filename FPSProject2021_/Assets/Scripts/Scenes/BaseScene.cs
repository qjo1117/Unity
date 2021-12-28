using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
	private void Awake()
	{
        gameObject.SetActive(true);
        Init();
	}


	public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

	protected virtual void Init()
	{
		Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
		if(obj == null)
		{
			Managers.Resource.NewPrefab("EventSystem").name = "@EventSystem";
		}
	}

	public abstract void Clear();
}
