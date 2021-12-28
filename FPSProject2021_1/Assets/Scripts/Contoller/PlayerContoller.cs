using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField] PlayerStat _stat;

    int _mask = 0;

    CapsuleCollider     _capsule;
    Rigidbody           _rigid;

    float   _capsuleRadius = 10.0f;

    bool    _isGround = false;
    bool    _isJumping = false;
    bool    _isForward = false;
    bool    _isOnSteepSlope = false;

    // 정해진 값들
    float   _gravity = 9.8f;
    float   _maxSlopeAngle = 30.0f;
    float   _checkGroundDist = 0.0f;

    // 현재 사용할 녀석들
    float   _currentGravity = 0.0f;
    float   _fixedDeltaTime = 0.0f;
    Vector3 _moveDir;

    float   _currentGroundDist = 0.0f;
    Vector3 _currentGroundNormal;
    float   _currentSlopeAngle = 0.0f;
    float   _currentforwardSlopeAngle = 0.0f;

    private void InitCapsuleCollider()
    {
        TryGetComponent(out _capsule);
        if (_capsule == null)
        {
            _capsule = gameObject.AddComponent<CapsuleCollider>();

            // 렌더러를 모두 탐색하여 높이 결정
            float maxHeight = -1f;

            // 1. SMR 확인
            var smrArr = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smrArr.Length > 0)
            {
                foreach (var smr in smrArr)
                {
                    foreach (var vertex in smr.sharedMesh.vertices)
                    {
                        if (maxHeight < vertex.y)
                        {
                            maxHeight = vertex.y;
                        }
                    }
                }
            }
            // 2. MR 확인
            else
            {
                var mfArr = GetComponentsInChildren<MeshFilter>();
                if (mfArr.Length > 0)
                {
                    foreach (var mf in mfArr)
                    {
                        foreach (var vertex in mf.mesh.vertices)
                        {
                            if (maxHeight < vertex.y)
                                maxHeight = vertex.y;
                        }
                    }
                }
            }

            // 3. 캡슐 콜라이더 값 설정
            if (maxHeight <= 0)
            {
                maxHeight = 1f;
            }

            float center = maxHeight * 0.5f;

            _capsule.height = maxHeight;
            _capsule.center = Vector3.up * center;
            _capsule.radius = 0.2f;
        }

        _capsuleRadius = _capsule.radius * 0.9f;
        //_capsuleRadiusDiff = _capsule.radius - _capsuleRadius + 0.05f;
    }
    private void InitRigidbody()
    {
        TryGetComponent(out _rigid);
        if (_rigid == null)
        {
            _rigid = gameObject.AddComponent<Rigidbody>();
        }

        // 회전은 자식 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
        _rigid.constraints = RigidbodyConstraints.FreezeRotation;
        _rigid.interpolation = RigidbodyInterpolation.Interpolate;
        _rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rigid.useGravity = false; // 중력 직접 제어
    }

    private void UpdateGravity()
    {
        if (_isGround == true)
        {
            _currentGravity = 0.0f;

            _isJumping = false;
        }
        else
        {
            _currentGravity += _fixedDeltaTime * _gravity;
        }
    }


    void Start()
    {
        InitRigidbody();
        InitCapsuleCollider();

        _stat = gameObject.GetOrAddComponent<PlayerStat>();


        Managers.Input.KeyAction -= OnKeyEvent;
        Managers.Input.KeyAction += OnKeyEvent;

    }

    void Update()
    {
        UpdateGravity();
    }

    private void FixedUpdate()
	{

	}

    void OnKeyEvent()
	{
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.position += new Vector3(h, 0.0f, v);
	}

    void CheckGround()
	{
        _currentGroundDist = float.MaxValue;
        _currentGroundNormal = Vector3.up;
        _currentSlopeAngle = 0.0f;
        _currentforwardSlopeAngle = 0.0f;


        Vector3 castBottomPoint = new Vector3(transform.position.x, transform.position.y + _capsule.radius, transform.position.z);
        RaycastHit hit;

        _isGround = false;
        if (Physics.SphereCast(castBottomPoint, _capsuleRadius, Vector3.down, 
            out hit, _checkGroundDist, _mask)) {
            _currentGroundNormal = hit.normal;

            _currentSlopeAngle = Vector3.Angle(_currentGroundNormal, Vector3.up);
            _currentforwardSlopeAngle = Vector3.Angle(_currentGroundNormal, _moveDir) - 90.0f;

            _isOnSteepSlope = _currentSlopeAngle >= _maxSlopeAngle;



        }

    }

    void CheckFoward()
	{
        Vector3 castBottomPoint = new Vector3(transform.position.x, transform.position.y + _capsule.radius, transform.position.z);
        Vector3 castTopPoint = new Vector3(transform.position.x, transform.position.y + _capsule.height - _capsule.radius, transform.position.z);

        RaycastHit hit;


        if (Physics.CapsuleCast(castBottomPoint, castTopPoint, _capsuleRadius,
            _moveDir + Vector3.down * 0.1f, out hit, 0.1f, -1)) {
            float fowardAngle = Vector3.Angle(hit.normal, Vector3.up);
            _isForward = fowardAngle <= _maxSlopeAngle;
		}

    }
}
