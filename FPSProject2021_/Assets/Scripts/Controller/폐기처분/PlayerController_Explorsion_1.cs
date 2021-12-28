using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Explorsion_1 : MonoBehaviour
{
	[SerializeField] private float _speed = 20.0f;
	[SerializeField] private bool _ground = false;
	[SerializeField] private float _jump = 10.0f;

	[SerializeField] private Transform _characterBody;      // 캐릭터 모델
	[SerializeField] private Transform _cameraArm;          // 캐릭터의 시점

	private Transform _playerCamera = null;
	public Vector3 _playerCameraOffset;

	[SerializeField] private Animator _animator;
	public bool _bMove = false;
	public bool _bBackMove = false;
	public bool _bFastMove = false;
	public bool _bJump = false;

	const float _gravity = 9.8f;

	void Start()
	{
		Managers.Input.AddKeyAction(OnInputMove);
		Managers.Input.AddMouseAction(LookAround);
		Managers.Input.AddNotKeyAction(OnNotKey);
		Managers.Input.AddMouseAction(OnClickGunShut);
		Managers.Input.AddKeyAction(OnInputJump);

		if (_playerCamera == null)
		{
			_playerCamera = Camera.main.gameObject.transform;
			_playerCamera.localPosition = _playerCameraOffset;
		}

		if (false == _characterBody.gameObject.TryGetComponent<Animator>(out _animator))
		{
			Debug.LogError($"Not Have Animator {gameObject.name}");
		}
	}

	void Update()
	{

	}

	private void FixedUpdate()
	{
		//HitGroundCheck();
	}

	void HitGroundCheck()
	{
		Vector3 velocity = new Vector3(0.0f, -_gravity, 0.0f) * Time.deltaTime;
		LayerMask layer = LayerMask.GetMask("Ground");

		//Collider[] collider = Physics.OverlapBox(transform.position, new Vector3(0.1f, 0.2f, 0.1f), Quaternion.identity, layer);
		//if (collider != null)
		//{
		//    transform.position = collider[0].transform.position;
		//    Debug.DrawLine(transform.position, new Vector3(0.1f, 0.2f, 0.1f));
		//}

		transform.Translate(velocity);
	}

	void OnNotKey()
	{
		if (_bMove == true)
		{
			_bMove = false;
			_animator.SetBool("bMove", _bMove);
		}
		if (_bBackMove == true)
		{
			_bBackMove = false;
			_animator.SetBool("bBackMove", _bBackMove);
		}
		if (_bFastMove == true)
		{
			_bFastMove = false;
			_animator.SetBool("bFastMove", _bFastMove);
		}
		if (_bJump == true)
		{
			_bJump = false;
		}                                   // TO DO Animation
	}

	void OnClickGunShut(Define.MouseEvent evt)
	{
		if (evt != Define.MouseEvent.Click)
		{
			return;
		}

		Debug.Log("찰싹");

	}

	void OnInputJump()
	{
		float jump = Input.GetAxis("Jump");

		// 점프 여부를 체크한다.
		_bJump = (jump > 0.0f) ? true : false;
		if (_bJump == false)
		{
			return;
		}

		// TO DO Animatio Jump

		transform.Translate(Vector3.up * (_jump * Time.deltaTime));
	}

	void OnInputMove()
	{
		Vector2 mouseInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		_bMove = (mouseInput.magnitude > 0.0f) ? true : false;         // 움직이고 있는지 확인.
		_bBackMove = (mouseInput.y < 0.0f) ? true : false;             // 뒤로 움직이는 지 확

		if (_bMove == false && _bBackMove == false)
		{
			return;
		}

		// TO DO Animation 빠르게 움직이는 동작 
		if (Input.GetKey(KeyCode.LeftShift))
		{
			_bFastMove = true;
			_animator.SetBool("bFastMove", _bBackMove);
		}

		_animator.SetBool("bMove", _bMove);
		_animator.SetBool("bBackMove", _bBackMove);

		// vector그대로 안가져온 이유는 좌우 움직임만 쓰고 싶기 때문에
		Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0.0f, _cameraArm.forward.z).normalized;
		Vector3 lookRight = new Vector3(_cameraArm.right.x, 0.0f, _cameraArm.right.z).normalized;
		Vector3 moveDir = lookForward * mouseInput.y + lookRight * mouseInput.x;

		// 현재 플레이어가 빽무빙을 치고 있을 경우
		if (moveDir.z < 0)
		{
			_characterBody.forward = new Vector3(moveDir.x, moveDir.y, Mathf.Abs(moveDir.z));
		}
		else
		{
			_characterBody.forward = moveDir;
		}


		// 앞에 언덕이 있는지 여부를 체크한다.
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f, LayerMask.GetMask("Ground")))
		{
			float angle = Vector3.Angle(hit.normal, Vector3.up);

			Debug.DrawRay(transform.position, Vector3.down);

			if (angle > 30.0f)
			{
				return;
			}
		}

		// 움직임이 실패하면 움직임을 막는다.
		if (CanMoveCheck(moveDir) == false)
		{
			return;
		}

		transform.position += moveDir * Time.deltaTime * 5.0f;
	}

	bool CanMoveCheck(Vector3 moveDir)
	{
		Vector3 posForward = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
		Vector3 posRight = posForward + (Vector3.right * 0.5f);      //transform.position + Vector3.right;
		Vector3 posLeft = posForward - (Vector3.right * 0.5f);       //transform.position - Vector3.right;
		LayerMask layer = LayerMask.GetMask("Ground");

		// 현재 앞에 그라운드가 존재하는지 여부를 체크한다.
		//Debug.DrawRay(transform.position, moveDir);       // | Debug용도
		//Debug.DrawRay(posRight, moveDir);                 // | Debug용도
		//Debug.DrawRay(posLeft, moveDir);                  // | Debug용도

		bool bForward = CanMove_RayRast(posForward, moveDir, layer);
		bool bRight = CanMove_RayRast(posRight, moveDir, layer);
		bool bLeft = CanMove_RayRast(posLeft, moveDir, layer);
		if (bForward == false || bRight == false || bLeft == false)
		{
			return false;
		}
		return true;
	}

	bool CanMove_RayRast(Vector3 start, Vector3 end, LayerMask laymask)
	{
		RaycastHit hit;
		if (Physics.Raycast(start, end, out hit, 1, laymask))
		{
			return false;
		}
		return true;
	}

	void LookAround(Define.MouseEvent evt)
	{
		// 일반적인 상황에 모두 적용시켜준다.
		if (evt != Define.MouseEvent.None)
		{
			return;
		}

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