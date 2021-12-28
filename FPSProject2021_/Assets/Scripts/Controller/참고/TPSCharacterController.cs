using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*----------------------------------------------------------
 * 참고한 곳
   https://www.youtube.com/watch?v=P4qyRyQdySw
----------------------------------------------------------*/

public class TPSCharacterController : MonoBehaviour
{
    [SerializeField] private Transform _characterBody;
    [SerializeField] private Transform _cameraArm;

    private Animator animator;

    void Start()
    {
        
        animator = _characterBody.GetComponent<Animator>();
    }

    void Update()
    {
        LookAround();
        Move();
    }

	private void Move()
	{
        Vector2 mouseInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = mouseInput.magnitude != 0;
        //animator.SetBool("isMove", isMove);

        if(isMove == true)
		{
            // vector그대로 안가져온 이유는 좌우 움직임만 쓰고 싶기 때문에
            Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0.0f, _cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(_cameraArm.right.x, 0.0f, _cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * mouseInput.y + lookRight * mouseInput.x;

            _characterBody.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * 5.0f;
		}
        
        Debug.DrawRay(_cameraArm.position, new Vector3(_cameraArm.forward.x, 0.0f, _cameraArm.forward.z).normalized, Color.red);
	}

	private void LookAround()
	{
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;

        // 범위 제한
        x = x < 180.0f ? Mathf.Clamp(x, -1.0f, 70.0f) : Mathf.Clamp(x, 355.0f, 361f);

        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

	}
}
