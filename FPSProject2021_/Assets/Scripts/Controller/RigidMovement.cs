using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{

    [SerializeField] private Transform  _cameraArm;
    [SerializeField] private Transform  _characterBody;

    public Animator   _anim;

    // 양심에 찔려서 타이핑이라도 일일히 다하면서 수정했습니다.
    #region Define
    public class Components
	{
        public Rigidbody        rigid;
        public CapsuleCollider  capsule;
    }

    public class States
	{
        public bool isMoving;
        public bool isBackMoving;
        public bool isRunning;
        public bool isGround;
        public bool isJumping;
        public bool isJumpTriggered;
        public bool isNonControl;
        public bool isCanForward;
        public bool isOnSteepSlope;

        public bool isShoot;
	}

    [Serializable]
    public class CheckOption
    {
        [Tooltip("지면으로 체크할 레이어 설정")]
        public LayerMask groundLayerMask = -1;

        [Range(0.01f, 0.5f), Tooltip("전방 감지 거리")]
        public float forwardCheckDistance = 0.1f;

        [Range(0.1f, 10.0f), Tooltip("지면 감지 거리")]
        public float groundCheckDistance = 2.0f;

        [Range(0.0f, 0.1f), Tooltip("지면 인식 허용 거리")]
        public float groundCheckThreshold = 0.01f;
    }

    [Serializable]
    public class MoveOption
	{
        [Range(1.0f, 10.0f), Tooltip("이동 속도")]
        public float speed = 5.0f;

        [Range(1.0f, 3.0f), Tooltip("달리기 증가량")]
        public float runningCoef = 2.0f;

        [Range(1f, 10f), Tooltip("점프 강도")]
        public float jumpForce = 4.2f;

        [Range(1.0f, 70.0f), Tooltip("등반 가능한 경사각")]
        public float maxSlopeAngle = 50.0f;

        [Range(0.0f, 4.0f), Tooltip("경사로 이동속도 변화율(가속 / 감속)")]
        public float slopeAccel = 2.0f;

        [Range(-9.81f, 0.0f), Tooltip("중력")]
        public float gravity = -9.81f;
	}

    [Serializable]
    public class Values
    {
        public Vector3 moveDir;
        public Vector3 groundNormal;
        public Vector3 groundCross;
        public Vector3 horizontalVelocity;

        //[Space]
        //public float jumpCooldown;
        //public int jumpCount;
        //public float outOfControllDuration;

        [Space]
        public float groundDistance;
        public float groundSlopeAngle;         // 현재 바닥의 경사각
        public float groundVerticalSlopeAngle; // 수직으로 재측정한 경사각
        public float forwardSlopeAngle; // 캐릭터가 바라보는 방향의 경사각
        public float slopeAccel;        // 경사로 인한 가속/감속 비율

        [Space]
        public float gravity; // 직접 제어하는 중력값
    }

    [SerializeField] private Components     _components = new Components();
    [SerializeField] private CheckOption    _checkOptions = new CheckOption();
    [SerializeField] private MoveOption     _moveOptions = new MoveOption();
    [SerializeField] private States         _currentStates = new States();
    [SerializeField] private Values         _currentValues = new Values();

    public Components  Com => _components;
    public CheckOption COption => _checkOptions;
    public MoveOption  MOption => _moveOptions;
    public States      State => _currentStates;
    public Values      Current => _currentValues;


    private float       _capsuleRadiusDiff = 1.2f;
    private float       _fixedDeltaTime = 0.1f;

    private float       _castRadius; // Sphere, Capsule 레이캐스트 반지름

    private Vector3 CapsuleTopCenterPoint
        => new Vector3(transform.position.x, transform.position.y + Com.capsule.height - Com.capsule.radius, transform.position.z);

    private Vector3 CapsuleBottomCenterPoint
        => new Vector3(transform.position.x, transform.position.y + Com.capsule.radius, transform.position.z);
    #endregion

    void Start()
    {
        InitRigidbody();
        InitCapsuleCollider();

        RegisterInputFunction();
    }

	private void FixedUpdate()
	{
        _fixedDeltaTime = Time.fixedDeltaTime;

        CheckForward();
        CheckGround();

        UpdateGravity();

        CalculateMovements();
        ApplyMovementsToRigidbody();
    }

	void Update()
    {
        AnimatioStateUpdate();
    }

    void AnimatioStateUpdate()
	{
        _anim.SetBool("bMove", State.isMoving);
        _anim.SetBool("bBackMove", State.isBackMoving);
	}

	#region 초기화 함수
	private void InitRigidbody()
    {
        TryGetComponent(out Com.rigid);
        if (Com.rigid == null) {
            Com.rigid = gameObject.AddComponent<Rigidbody>();
        }

        // 회전은 자식 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
        Com.rigid.constraints = RigidbodyConstraints.FreezeRotation;
        Com.rigid.interpolation = RigidbodyInterpolation.Interpolate;
        Com.rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Com.rigid.useGravity = false; // 중력 직접 제어
    }

    private void InitCapsuleCollider()
    {
        TryGetComponent(out Com.capsule);
        if (Com.capsule == null) {
            Com.capsule = gameObject.AddComponent<CapsuleCollider>();

            // 렌더러를 모두 탐색하여 높이 결정
            float maxHeight = -1f;

            // 1. SMR 확인
            var smrArr = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smrArr.Length > 0) {
                foreach (var smr in smrArr) {
                    foreach (var vertex in smr.sharedMesh.vertices) {
                        if (maxHeight < vertex.y) {
                            maxHeight = vertex.y;
                        }
                    }
                }
            }
            // 2. MR 확인
            else
            {
                var mfArr = GetComponentsInChildren<MeshFilter>();
                if (mfArr.Length > 0) {
                    foreach (var mf in mfArr) {
                        foreach (var vertex in mf.mesh.vertices) {
                            if (maxHeight < vertex.y)
                                maxHeight = vertex.y;
                        }
                    }
                }
            }

            // 3. 캡슐 콜라이더 값 설정
            if (maxHeight <= 0) {
                maxHeight = 1f;
            }

            float center = maxHeight * 0.5f;

            Com.capsule.height = maxHeight;
            Com.capsule.center = Vector3.up * center;
            Com.capsule.radius = 0.2f;
        }

        _castRadius = Com.capsule.radius * 0.9f;
        _capsuleRadiusDiff = Com.capsule.radius - _castRadius + 0.05f;
    }
    #endregion

    #region 입력
    void RegisterInputFunction()
	{
        Managers.Input.AddKeyAction(OnInputMoving);
        Managers.Input.AddKeyAction(OnInputJumping);

        Managers.Input.AddNotKeyAction(OnInputStop);
	}

    void OnInputMoving()
	{
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        if(moveInput.magnitude <= 0.01f) {
            return;
		}

        // vector그대로 안가져온 이유는 좌우 움직임만 쓰고 싶기 때문에
        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0.0f, _cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0.0f, _cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        State.isBackMoving = false;
        // 현재 플레이어가 빽무빙을 치고 있을 경우
        if (moveDir.z < 0)
        {
            State.isBackMoving = true;
        }

        Current.moveDir = moveDir;
        State.isMoving = true;
        State.isRunning = Input.GetKey(KeyCode.LeftShift);
    }           // 방향키, 쉬프트 검사

    void OnInputJumping()
	{
        if(Input.GetKeyDown(KeyCode.Space) == false) {
            return;
		}

        // 지면에서만 점프가능
        if(State.isGround == false) {
            return;
		}

        // 접근 불가능 경사로에서는 점프 불가능
        if(State.isOnSteepSlope == true) {
            return;
		}

        State.isJumpTriggered = true;
	}

    void OnInputStop()
	{
        State.isMoving = false;
        State.isRunning = false;
        State.isBackMoving = false;
	}

	#endregion

	#region 검사
	/*----------------------------------------------------------------------------
     * 1. 상태검사
     * 물리적 상태를 검사하기 위한 방법으로 충돌 검사를 할 수 있는 함수를 생각해보기
     * - 가장 간단한 방법은 자신의 위치에서 수직으로 레이를 쏴서 검사하는 방법이 있다.
     * 문제점 : 각 끝점에 충돌 체크하는 영역이 부족하므로 확실한 충돌 체크를 보장할 수 가 없다.
     * - 네 모퉁이로 쏘는 방법은 어떨까?
     * 문제점 : 평지, 큐브, 균일한 경사면에서는 괜찮지만
     *         언덕, 산간지형, 불규칙한 지형에서는 정확하게 결과를 얻는다는 보장을 할 수 없다.
     * 방법 : RayCast로는 부족하다는 것을 인지했기 때문에 다른 대체제를 찾아본다.
     * 1. Trigger, Collider 함수를 이용한다.
     * 문제점 : 성능이 거지같다.
     * 2. SweepTest 를 사용한다.
     * 문제점 : 맞닿아있는 콜라이더는 감지하지 못한다. 
     *         만약 커버를 치기 위해 리지드바디 콜라이더 오브젝트를 만든다면 성능이 좋다고 보장하기 힘들다.
     * 3. Cast함수를 사용한다. (Sphere, Box)
     * 나쁘지 않으므로 Cast함수를 채택한다.
     * 
     * 1. 전방 장애물 검사
     * 전방에 장애물이 있는 지 체크하는 검사방법을 채택한다.
     * 캐릭터의 캡슐 콜라이더와 같거나 작은 크기의 캡슐캐스트를 현재 위치에서 다음 위치 방향으로 검사한다.
    ----------------------------------------------------------------------------*/
	private void CheckForward()
	{
        RaycastHit hit;
        bool cast = Physics.CapsuleCast(CapsuleBottomCenterPoint, CapsuleTopCenterPoint, _castRadius,
            Current.moveDir + Vector3.down * 0.1f, out hit, COption.forwardCheckDistance, -1, QueryTriggerInteraction.Ignore);

        State.isCanForward = true;
        if (cast == true)
		{
            // hit의 경사면을 가져오고 최대 경사면을 넘어가면 못 움직이게 만들어준다.
            float forwardAngle = Vector3.Angle(hit.normal, Vector3.up);
            State.isCanForward = forwardAngle <= MOption.maxSlopeAngle;     // 현재 경사면이 Option보다 작으면 움직일 수 있다.
		}
	}

    /*----------------------------------------------------------------------------
     * 2. 지면 검사
     * 하단방향 지면검사를 통해 다음의 정보들을 추출한다.     마지막 계산때 이용해 먹기 위해
     * - 캐릭터와 지면사이의 거리(높이)
     * - 캐릭터가 현재 지면에 위치해 있는지 여부
     * - 지면의 경사각(기울기)
     * - 캐릭터가 이동할 방향을 기준으로 하는 경사각
     *      (캐릭터가 이동할 방향의 실제 경사각을 구한다.)
     * - 현재 캐릭터가 오를 수 없는 경사면에 위치해 있는지 여부
     *      (이 정보를 이용해 추후 이동 구현시 오를 수 없는 경사면에 있을 경우 미끄러지게 한다.)
     * - 경사면의 회전축 벡터
     *      (캐릭터가 경사면을 따라 이동할 수 있도록, 월드 이동벡터를 회전시키기 위한 기준이 된다.)
    *----------------------------------------------------------------------------*/
    private void CheckGround()
	{
        Current.groundDistance = float.MaxValue;
        Current.groundNormal = Vector3.up;
        Current.groundSlopeAngle = 0.0f;        // 지면 경사각
        Current.forwardSlopeAngle = 0.0f;       // 방향 경사각

        // 캡슐콜라이더이기 때문에 하단은 원으로 구성되어 있어서 구캐스트를 이용한다.
        RaycastHit hit;
        bool cast = Physics.SphereCast(CapsuleBottomCenterPoint, _castRadius, Vector3.down,
            out hit, COption.groundCheckDistance, COption.groundLayerMask, QueryTriggerInteraction.Ignore);

        State.isGround = false;
        if(cast == true)
		{
            // 지면 노멀벡터를 얻는다.
            Current.groundNormal = hit.normal;

            // 현재 위치한 지면의 경사각 구하기 (캐릭터 이동방향 고려)
            Current.groundSlopeAngle = Vector3.Angle(Current.groundNormal, Vector3.up);
            Current.forwardSlopeAngle = Vector3.Angle(Current.groundNormal, Current.moveDir) - 90.0f;

            // 현재 그라운드 각도가 범위밖에 벗어났는지 확인.
            State.isOnSteepSlope = Current.groundSlopeAngle >= MOption.maxSlopeAngle;

            Current.groundDistance = Mathf.Max(hit.distance - _capsuleRadiusDiff - COption.groundCheckThreshold, -10.0f);

            // 현재 거리가 0이고 급경사가 없으면 땅에 착지한 것으로 간주한다.
            State.isGround = (Current.groundDistance <= 0.0001f) && (State.isOnSteepSlope == false);
        }

        // 월드 이동벡터 회전축
        Current.groundCross = Vector3.Cross(Current.groundNormal, Vector3.up);
	}
	#endregion

	#region 중력
	/*----------------------------------------------------------------------------
     * 1. 중력
     * 중력이 작동하는 경우
     *      캐릭터가 공중에 떠 있는 경우
     *      캐릭터가 오를 수 없는 경사면에 위치한 경우
     * 중력이 작용하지 않을 경우
     *      캐릭터가 지면에 위치한 경우
     * 
     * 중력의 직접제어
     * 이유 : Rigidbody의 useGravity를 사용할 경우, 중력은 내부적으로 velocity값의 변화를 통해 적용된다.
     * Rigidbody.Move() 사용할 경우 그저 useGravity를 사용하고 중력을 신경쓰지 않아도 된다.
     * 하지만 지금은 Rigidbody.Move()를 사용하지않고 Velocity를 직접 제어하므로 경사면에서 수평 벡터를 회전시켜 결국 y속도에도 영향을 주어야한다.
     * 그래서 useGravity를 사용자 정의 velocity안에 넣어준다.
    * ----------------------------------------------------------------------------*/
	private void UpdateGravity()
	{
        if(State.isGround == true) {
            Current.gravity = 0.0f;

            State.isJumping = false;
		}
        else {
            Current.gravity += _fixedDeltaTime * MOption.gravity;
		}
	}
	#endregion

	#region 최종 적용 함수
	private void CalculateMovements()
	{
        // 현재 급경사이고 GroundDistane가 망햇을 경우
        if (State.isOnSteepSlope == true && Current.groundDistance < 0.1f) {

            // 땅의 각도를 구해서 반대편으로 힘을 준다.
            Current.horizontalVelocity =
                Quaternion.AngleAxis(90.0f - Current.groundSlopeAngle, Current.groundCross) * (Vector3.up * Current.gravity);

            Com.rigid.velocity = Current.horizontalVelocity;
            return;
		}

        // 1. 점프
        if(State.isJumpTriggered == true) {
            Current.gravity = MOption.jumpForce;

            State.isJumpTriggered = false;
            State.isJumping = true;
		}

        // 2. XZ 이동속도 계산
        // 공중에서 전방이 막힌 경우 제한 (지상에서는 벽에 붙어서 이동할 수 있도록 허용)
        if(State.isCanForward == false && State.isGround == false || State.isJumping == true && State.isGround == true) {
            Current.horizontalVelocity = Vector3.zero;
		}
        else {      // 이동이 가능한 경우

            float speed = State.isMoving == false ? 0.0f :
                          State.isRunning == false ? MOption.speed :
                                                     MOption.speed * MOption.runningCoef;

            Current.horizontalVelocity = Current.moveDir * speed;
		}

        // 3. XZ 벡터 회전
        // 지상이거나 지면에 가까운 높이
        if(State.isGround == true || Current.groundDistance < COption.groundCheckDistance && State.isJumping == false) {
            if(State.isMoving == true && State.isCanForward == true) {
                // 경사로 인한 가속 / 감속
                if(MOption.slopeAccel > 0.0f) {
                    // 경사인지 확인
                    bool isPlus = Current.forwardSlopeAngle >= 0.0f;
                    float absFsAngle = isPlus == true ? Current.forwardSlopeAngle : -Current.forwardSlopeAngle;
                    float accel = MOption.slopeAccel * absFsAngle * 0.1111f + 1.0f;        // 엑셀값을 구해준다.

                    Current.slopeAccel = isPlus == false ? accel : 1.0f / accel;            // 경사이면 감속 / 아니면 가속

                    Current.horizontalVelocity *= Current.slopeAccel;
                }

                // 벡터 회전 (경사로)
                Current.horizontalVelocity =
                    Quaternion.AngleAxis(-Current.groundSlopeAngle, Current.groundCross) * Current.horizontalVelocity;
            }
		}
	}

    private void ApplyMovementsToRigidbody()
    {
        //if (State.isOutOfControl)
        //{
        //    Com.rigid.velocity = new Vector3(Com.rigid.velocity.x, Current.gravity, Com.rigid.velocity.z);
        //    return;
        //}

        Com.rigid.velocity = Current.horizontalVelocity + Vector3.up * (Current.gravity);
    }
    #endregion
}
