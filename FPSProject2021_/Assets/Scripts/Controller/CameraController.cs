using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Define.CameraMode  _mode           = Define.CameraMode.QuarterView;
    [SerializeField] private Vector3            _quaterDelta    = new Vector3(0.0f, 6.0f, -5.0f);
    [SerializeField] private GameObject         _player         = null;
    [SerializeField] private Vector3            _firstDelta     = new Vector3(0.0f, 0.0f, 0.0f);

    public Transform _cameraArm;
    public Transform _characterBody;

    public LayerMask _layer;

    void Start()
    {
        Managers.Input.KeyAction -= OnInputSelectView;
        Managers.Input.KeyAction += OnInputSelectView;

        _layer = LayerMask.GetMask("Ground");

        transform.position = _player.transform.position + _quaterDelta;
    }

    void Update()
    {
        SelectMode();
    }

    void SelectMode()
	{
        LookAround();

        switch (_mode)
		{
            case Define.CameraMode.QuarterView:
                QuarterViewMode();
                break;
            case Define.CameraMode.FirstView:
                FirstViewMode();
                break;
        }
    }

    float _rotateX = 0.0f;
    void OnInputMouseRotation(Define.MouseEvent evt)
	{
		float mouseX = Input.GetAxis("Mouse X") * 100.0f * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * 100.0f * Time.deltaTime;

		// 마우스가 움직이면 따라 움직임
		_rotateX -= mouseY;
		_rotateX = Mathf.Clamp(_rotateX, -90.0f, 90.0f);
		// 위에를 보게는 해준지만 플레이어가 위를 안본다.
		//transform.localRotation = Quaternion.Euler(_rotateX, 0.0f, 0.0f);
		_player.transform.Rotate(Vector3.up * mouseX);
	}

    void FirstViewMode()
	{
        if (_mode != Define.CameraMode.FirstView)
        {
            return;
        }

        transform.position = _player.transform.position + _firstDelta;
    }

    void QuarterViewMode()
	{
        Vector3 delta = _cameraArm.up * _quaterDelta.y + _cameraArm.forward * _quaterDelta.z;

        RaycastHit hit;
        if (Physics.Raycast(_player.transform.position, delta, out hit, _quaterDelta.magnitude, _layer))
        {
            float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
            transform.position = _player.transform.position + (delta.normalized * dist);
        }
        else
		{
            transform.position = _player.transform.position + delta;
		}
	}

    void OnInputSelectView()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetQuaterView(_quaterDelta);
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetFirstView(_firstDelta);
        }
    }

    public void SetQuaterView(Vector3 delta)
	{
        _mode = Define.CameraMode.QuarterView;
        _quaterDelta = delta;
	}

    public void SetFirstView(Vector3 delta)
    {
        _mode = Define.CameraMode.FirstView;
        _firstDelta = delta;
    }

    void LookAround()
    {
        // 일반적인 상황에 모두 적용시켜준다.
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        // 범위 제한
        x = x < 180.0f ? Mathf.Clamp(x, -1.0f, 70.0f) : Mathf.Clamp(x, 355.0f, 361f);

        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
        _characterBody.rotation = Quaternion.Euler(camAngle.x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
