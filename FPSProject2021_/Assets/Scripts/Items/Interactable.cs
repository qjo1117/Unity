using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float        _radius = 2.0f;
    public bool         _isInteracted = false;
    public float        _distance = 3.0f;

    public Item         _item;

    public GameObject   _player = null;
    public LayerMask    _layer;
    static public List<Interactable> _checkList = new List<Interactable>();

    // 준비가 끝나면 true
    public bool _isUse = false;

    void Start()
    {
        Init();
    }

    void Update()
    {
        CheckInteract();
    }

    public virtual void Init()
	{
        _layer = -1;
        _item = new Item { name = "", count = 0, icon = null, type = Define.ItemType.None };
        _checkList.Add(this);
    }

    // 이걸 사용하도록 하자
    public virtual void OnCollisionInteractable(RaycastHit hit)
    {

    }

    public virtual void OnCollisionNonInteractable()
    {

    }

    public void CheckInteract()
	{
        RaycastHit hit;
        bool cast = Physics.SphereCast(transform.position, 0.0f, -Vector3.one, out hit, _distance, LayerMask.GetMask("Player"));

        if(cast == true) {
            _player = hit.collider.gameObject;
            _isInteracted = true;
            OnCollisionInteractable(hit);
        }
        else {
            _isInteracted = false;
            OnCollisionNonInteractable();
        }
	}



	private void OnDrawGizmosSelected()
	{
        Gizmos.color = _isInteracted == false ? Color.yellow : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _distance);
	}
}
