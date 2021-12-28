using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
	public GameObject _useUI = null;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		RaycastHit hit;
		// 총이 잘 떨어지길래 그냥 레이캐스트를 써서 띄워주자
		bool isGround = Physics.Raycast(transform.position, -Vector3.up, out hit, 0.5f, LayerMask.GetMask("Ground"));
		if (isGround) {
			transform.position = hit.point + new Vector3(0.0f, 2.0f, 0.0f);
			GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	public override void Init()
	{
		base.Init();

		_layer = LayerMask.GetMask("Player");
		_useUI = GameObject.Find("InteractionItem");
		if (_useUI == null) {
			Debug.LogError("Item Use UI Connect");
		}

		Managers.Input.AddKeyAction(OnInputInteractable);

		
	}

	static public void SpawnItem(Define.ItemType type, string name, Vector3 pos, int count = 0, Transform parent = null)
	{
		GameObject item = Item.GetPrefab(type);
		ItemInteractable iteminteractable = item.GetComponent<ItemInteractable>();
		iteminteractable.Init();

		item.transform.position = pos;
		item.transform.parent = parent;
		iteminteractable._isUse = true;
		iteminteractable._item = new Item { type = type, name = name, count = count };

	}

	public override void OnCollisionInteractable(RaycastHit hit)
	{
		_useUI.SetActive(true);


	}

	public override void OnCollisionNonInteractable()
	{
		bool check = false;
		foreach (Interactable obj in _checkList) {
			if (obj._isInteracted == true) {
				check = true;
			}
		}

		if (check == false) {
			_useUI.SetActive(false);
		}


	}

	public void OnInputInteractable()
	{
		if(_isInteracted == false) {
			return;
		}
		if(Input.GetKeyDown(KeyCode.E) == false) {
			return;
		}

		InventoryUI.AddItem(_item);
		_isInteracted = false;
		//_player.GetComponent<PlayerController>()._handleGun.name = "BlueGun";
		gameObject.SetActive(false);
		_useUI.SetActive(false);
	}
}
