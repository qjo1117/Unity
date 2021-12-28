using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawProjection : MonoBehaviour
{
    LineRenderer _lineRenderer;

    public GameObject _shutPoint;
    public int _pointCount;

    // 외부에서 가져와야함
    public float _force = 10.0f;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if(_lineRenderer == null) {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }

    void Update()
    {
        Projection();
    }

    void Projection()
	{
        _lineRenderer.positionCount = _pointCount;

        List<Vector3> points = new List<Vector3>();
        Vector3 startPos = _shutPoint.transform.position;
        Vector3 startVelocity = _shutPoint.transform.up * _force;

        for (float time = 0.0f; time < _pointCount; time += 0.2f) {
            Vector3 tempPos = startPos + (startVelocity * time);
            tempPos.y = startPos.y + (startVelocity.y * time) + (Physics.gravity.y / 2.0f * time * time);
            points.Add(tempPos);

            if(Physics.OverlapSphere(tempPos, 2, 0).Length > 0) {
                _lineRenderer.positionCount = points.Count;
                break;
			}
        }
        _lineRenderer.SetPositions(points.ToArray());
    }
}
