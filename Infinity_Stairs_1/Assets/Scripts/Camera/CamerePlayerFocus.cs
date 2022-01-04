using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CamerePlayerFocus : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Range(0.0f, 5.0f)]
    [SerializeField] private float speed = 0.1f;
    Vector3 checkPos = new Vector3();

    [SerializeField] static private bool isMove = false;
    int count = 0;

    void Start()
    {
        if(target == null) {
            target = Managers.Resource.NewPrefab("Player", Managers.Scene.transform).transform;
		}
    }

    public void TargetMove()
	{
        isMove = true;
        count = 0;
	}

    void Update()
    {
        if (isMove == true) {
            transform.DOMoveY(target.position.y, speed);
            isMove = false;
        }



    }

}
