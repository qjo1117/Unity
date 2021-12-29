using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Explosion_2 : MonoBehaviour
{
    [SerializeField] private Transform _characterBody;      // 캐릭터 모델
    [SerializeField] private Transform _cameraArm;          // 캐릭터의 시점

    [SerializeField] private Transform           _playerCamera = null;
    [SerializeField] public Vector3              _playerCameraOffset;

    private CapsuleCollider     _capsule;
    private float               _castRadius = 0.5f;

    [SerializeField] private LayerMask _layer;

    private Vector3 _velocity;
    private Vector3 _moveDir;
    private Vector3 _moveInputValue;
    private Vector3 _gravity;

    [SerializeField]
    public class CheckState
    {
        [SerializeField] public bool isMoving = false;
        [SerializeField] public bool isBackMoving = false;
        [SerializeField] public bool isNoneContol = false;
        [SerializeField] public bool isGround = false;
        [SerializeField] public bool isCanFoward = false;
    }

    [SerializeField] private float          _speed = 20.0f;

    [SerializeField] private CheckState     _states = new CheckState();

    private CheckState State => _states;

    private Vector3 CapsuleTopCenterPoint
        => new Vector3(transform.position.x, transform.position.y + _capsule.height - _capsule.radius, transform.position.z);
    private Vector3 CapsuleBottomCenterPoint
        => new Vector3(transform.position.x, transform.position.y + _capsule.radius, transform.position.z);

    // -------------------------------------------------------------------

    [SerializeField] private bool       _isInventory = false;
    [SerializeField] private bool       _isObtainItem = false;
    private GameObject _inventory;
    private GameObject _nearObjectUI;

	private void Start()
	{
        // 키보드 입력
        //Managers.Input.AddMouseAction(LookAround);

        Managers.Input.AddKeyAction(OnInputMove);
        Managers.Input.AddKeyAction(OnInputObtainItem);
        Managers.Input.AddKeyAction(OnInputInventory);

        // 카메라
        if(_playerCamera == null)
		{
            _playerCamera = Camera.main.transform;
		}
        _playerCamera.position = _cameraArm.position + _playerCameraOffset;

        // 캡슐추가
        TryGetComponent<CapsuleCollider>(out _capsule);
        _layer = LayerMask.GetMask("Ground");

        // 인벤토리
        _inventory = Managers.Resource.NewPrefab("Inventory");
        _inventory.SetActive(false);
        _nearObjectUI = Managers.Resource.NewPrefab("InteractionItem");
        _nearObjectUI.SetActive(false);


        // 기본 셋팅
        transform.position = new Vector3(0.0f, 2.0f, 0.0f);
    }

	private void FixedUpdate()
	{
        CheckGround();
        CheckFoward();

        CalculateVelocity();
    }

	private void Update()
	{
        CheckItem();
    }    

    void CheckItem()
	{
        LayerMask layer = LayerMask.GetMask("Item");
        RaycastHit hit;
        bool cast = Physics.SphereCast(transform.position, 0.5f, _characterBody.forward, out hit, 1.0f, layer, QueryTriggerInteraction.UseGlobal);

        if(cast == true)
		{
            _nearObjectUI.SetActive(true);
            _isObtainItem = true;
        }
        else
		{
            _nearObjectUI.SetActive(false);
            _isObtainItem = false;
        }
	}

    void CalculateVelocity()
	{
        if(State.isMoving == false)
		{
            _moveDir = Vector3.zero;
        }

        if(State.isGround == false)
		{
            _gravity = Vector3.zero;
        }
        else
		{
            _gravity = new Vector3(0.0f, -0.981f * 0.981f, 0.0f);
        }

        transform.position += (_moveDir + _gravity);
        _moveDir *= 0.7f;
    }

    void CheckGround()
	{
        float distance = float.MaxValue;

        RaycastHit hit;
        bool cast = Physics.SphereCast(CapsuleBottomCenterPoint, _castRadius - 0.1f, Vector3.down, out hit, 0.5f, _layer);

        State.isGround = true;
        if(cast == true)
		{
            distance = Mathf.Max(hit.distance - 0.1f, 0.0f);
            State.isGround = (distance <= 0.0001f);
        }
    }

    void CheckFoward()
	{
        Debug.DrawRay(transform.position, _moveDir);

        RaycastHit hit;
        bool cast = Physics.CapsuleCast(CapsuleBottomCenterPoint, CapsuleTopCenterPoint, _castRadius, _characterBody.forward, out hit, 1.0f, _layer);

        State.isCanFoward = true;
        if(cast == true)
		{
            float forwardObstacleAngle = Vector3.Angle(hit.normal, Vector3.up);
            State.isCanFoward = (forwardObstacleAngle <= 50.0f);

            if(State.isCanFoward == false)
			{
                _moveDir = new Vector3(0.0f, _moveDir.y, 0.0f);
			} 
        }

    }

    void OnInputObtainItem()
	{
        if(Input.GetKeyDown(KeyCode.E) == false)
		{
            return;
		}


        if(_isObtainItem == true)
		{
            Debug.Log("획득");
		}
        
	}

    void OnInputInventory()
	{
        if(Input.GetKeyDown(KeyCode.I) == false)
		{
            return;
		}

        // 열리면 닫히고 닫히면 열어준다.
        _isInventory = _isInventory == false ? true : false;


        if(_isInventory == true)
		{
            _inventory.SetActive(true);
		}
		else
		{
            _inventory.SetActive(false);
		}
    }

    void OnInputStop()
	{

	}

    void OnInputMove()
	{
        _moveInputValue = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        State.isMoving = _moveInputValue.magnitude > 0.001f ? true : false;
        if(State.isMoving == false)
		{
            return;
		}

        // vector그대로 안가져온 이유는 좌우 움직임만 쓰고 싶기 때문에
        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0.0f, _cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0.0f, _cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * _moveInputValue.y + lookRight * _moveInputValue.x;

        // 현재 플레이어가 빽무빙을 치고 있을 경우
        if (moveDir.z < 0)
        {
            State.isBackMoving = true;
            _characterBody.forward = new Vector3(moveDir.x, moveDir.y, Mathf.Abs(moveDir.z));
        }
        else
        {
            _characterBody.forward = moveDir;
        }

        _moveDir = moveDir * Time.deltaTime * _speed;

	}


    void LookAround(Define.MouseEvent evt)
    {
        // 일반적인 상황에 모두 적용시켜준다.

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        // 범위 제한
        x = x < 180.0f ? Mathf.Clamp(x, -1.0f, 70.0f) : Mathf.Clamp(x, 355.0f, 361f);

        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
        _characterBody.rotation = Quaternion.Euler(0.0f, camAngle.y + mouseDelta.x, camAngle.z);
    }
}



/*
 * 3인칭 : TPSCharacterController를 참조해서 만듬.
 * 
 */