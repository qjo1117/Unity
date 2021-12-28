using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalLife : MonoBehaviour
{
    

    void Start()
    {
        StartCoroutine(DestroyDuration(10.0f));
    }

    void Update()
    {
        
    }

    IEnumerator DestroyDuration(float time)
	{
        yield return new WaitForSeconds(time);
        Managers.Resource.DelPrefab(gameObject);
	}
}
