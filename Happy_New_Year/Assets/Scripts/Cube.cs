using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cube : MonoBehaviour
{
    void Start()
    {
        transform.DOMove(Vector3.up, 5.0f);
        transform.DOScale(Vector3.one * 3, 5.0f);
        transform.DORotate(Vector3.forward * 180.0f, 1.0f);

        Material mat = GetComponent<MeshRenderer>().material;
        mat.DOColor(Color.cyan, 5.0f);
    }

    void Update()
    {
        
    }
}
