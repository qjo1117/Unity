using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundGenerator : MonoBehaviour
{
    Queue<GameObject> _grounds = new Queue<GameObject>();
    int currentY = 0;
    int y = 0;


    int count = 0;

    bool isUse = false;

    void Start()
    {
        GameObject go = Managers.Resource.NewPrefab("SampleBackGround", transform);
        Managers.Pool.CreatePool(go);
        Managers.Resource.DelPrefab(go);
    }

    void Update()
    {
        y = Managers.Game.Player.BlockCount;
        if(y == currentY) {
            return;
        }

        currentY = y;
        if (currentY % 5 == 4) { 
            SpawnBackGround();
        }
    }

    GameObject SpawnBackGround()
	{
        GameObject go;
        Debug.Log($"Ground Count : {_grounds.Count}");

        if (_grounds.Count < 5) {
            go = Managers.Resource.NewPrefab("SampleBackGround", transform);

        }
        else {
            go = _grounds.Dequeue();
        }
        _grounds.Enqueue(go);

        count += 3;

        float result = 2.3f * count;
        go.transform.position = new Vector3(transform.position.x, Managers.Game.Player.transform.position.y + 7.5f, transform.position.z);
        return go;
	}
}
