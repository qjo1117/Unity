using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject                   _focus = null;
    public InventoryController          _inven = null;

    public GameObject                   _lancher = null;
    public float                        _bulletForce_7 = 10.0f;
    public float                        _bulletForce_5 = 5.0f;

    [SerializeField] private GameObject _characterModel;

    private Bullet                      _bullet;

    public int                          _hp = 100;
    [SerializeField] float              _grenateForce = 10.0f;

    [SerializeField] private Animator   _anim = null;
    public GameObject                   _grenade;

    void Start()
    {
        _inven = FindObjectOfType<InventoryController>();

        Managers.Input.AddMouseAction(OnInputMouseShot);

        _bullet = GunManager.gun.GetBulletData(0);

        
    }

	private void FixedUpdate()
	{
    }

	void Update()
    {
        if(transform.position.y < -50.0f) {
            transform.position = Vector3.zero + Vector3.up * 5.0f;
		}

        if(Input.GetKeyDown(KeyCode.Q)) {
            GrenateAiming();
        }
        if(Input.GetKeyUp(KeyCode.Q)) {
            GrenateShut(); 
        }
    }

    void OnInputAim(Define.MouseEvent evt)
	{
        // 클릭이벤트여야만 가능
        if(evt != Define.MouseEvent.Click) {
            return;
		}
        // 오른쪽 버튼 눌럿을때만 가능
        if(Input.GetMouseButtonDown(1) == true) {
            return;
		}

        // TO DO Anim으로 하는 방법
        // CameraController때문에 시점 고정이 되어 있음
	}

    void GrenateAiming()
	{
        // 왼쪽 버튼을 클릭할때만 가능
        if (Input.GetKeyDown(KeyCode.Q) != true) {
            return;
        }

        DrawProjection preview = GetComponent<DrawProjection>();
        LineRenderer linerenderer = GetComponent<LineRenderer>();
        if (preview == null)
        {
            preview = gameObject.AddComponent<DrawProjection>();
            linerenderer = gameObject.AddComponent<LineRenderer>();
        }
        preview._force = _grenateForce;
        preview.enabled = true;
        linerenderer.enabled = true;


    }

    void GrenateShut()
	{
        DrawProjection preview = GetComponent<DrawProjection>();
        LineRenderer linerenderer = GetComponent<LineRenderer>();
        if (preview == null)
        {
            preview = gameObject.AddComponent<DrawProjection>();
            linerenderer = gameObject.AddComponent<LineRenderer>();
        }
        preview._force = 0.0f;
        preview.enabled = false;
        linerenderer.enabled = false;

        // 생성
        GameObject obj = Managers.Resource.NewPrefab("Grenade", GunManager.gun._bulletParent.transform);
        obj.SetActive(true);
        obj.transform.position = transform.position + _lancher.transform.up * 2.0f;

        obj.GetComponent<Grenade>().Velocity(_characterModel.transform);
    }

    void OnInputMouseGrenadeShut(Define.MouseEvent evt)
	{
        // 클릭하고 있다가 마우스를 때면 쏘게 만들어준다.

	}

    void OnInputMouseShot(Define.MouseEvent evt)
	{
        // 클릭 이벤트일 때만 사용
        if(evt != Define.MouseEvent.Click) {
            return;
		}
        // 왼쪽 버튼을 클릭할때만 가능
        if(Input.GetMouseButtonDown(0) == true) {
            return;
		}

        GunManager.gun.Shot(_lancher, _bullet);
        _anim.SetTrigger("triOneShut");

    }
}
