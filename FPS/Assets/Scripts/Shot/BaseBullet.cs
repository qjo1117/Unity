using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    public Vector3      _velocity;
    public Rigidbody    _rigid;
    public GameObject   _decal;

    public float        _durationTime = 10.0f;

    public int          _damage = 30;

    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
    }

	private void FixedUpdate()
	{
        CheckGround();
        if(transform.position.y < -100) {
            Managers.Resource.DelPrefab(gameObject);
        }
    }

	void CheckGround()
	{

        RaycastHit hit;
        //bool cast = Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, LayerMask.GetMask("Ground"));

        bool cast = Physics.SphereCast(transform.position, 0.1f, transform.forward, out hit, 1.0f);

        Debug.DrawRay(transform.position, transform.forward);

        if(cast == true) {
            _decal = Managers.Resource.NewPrefab("BulletDecal", GunManager.gun._decalParent.transform);
            _decal.transform.position = hit.point;
            _decal.transform.rotation = Quaternion.EulerAngles(hit.normal.x, hit.normal.y + 90.0f, hit.normal.z);
            
            Managers.Resource.DelPrefab(gameObject);
		}
	}


    public void Velocity(Vector3 target)
	{
        _rigid = GetComponent<Rigidbody>();
        _rigid.velocity = CalculateVelocity(target, transform.position, 1.0f);
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        Vector3 dist = target - origin;
        Vector3 distXZ = dist;
        distXZ.y = 0.0f;

        float sY = dist.y;
        float sXZ = distXZ.magnitude;

        float vXZ = sXZ / time;
        float vY = sY / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distXZ.normalized;
        result *= vXZ;
        result.y = vY;
        return result;
    }
}
