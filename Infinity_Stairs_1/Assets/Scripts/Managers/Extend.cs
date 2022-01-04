using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extend
{
	public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
	{
		T component;
		if(obj.TryGetComponent<T>(out component) == false) {
			component = obj.AddComponent<T>();
		}
		return component;
	}

}
