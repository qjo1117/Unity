using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : BaseScene
{


	protected override void Init()
	{
		base.Init();
		SceneType = Define.Scene.Game;

		//Managers.Resource.Instantiate("Player", transform);
		Managers.Game._inventory = Managers.Resource.NewPrefab("InventoryUI", transform).GetComponent<InventoryController>();
		Managers.Game._inventory.Init();

		GunManager.gun.Start();

		ItemInteractable.SpawnItem(Define.ItemType.Gun, "BlueGun", new Vector3(0.0f, 5.0f, -5.0f), 1, transform);
		ItemInteractable.SpawnItem(Define.ItemType.Health, "Kit", new Vector3(5.0f, 5.0f, 0.0f), 1, transform);
		ItemInteractable.SpawnItem(Define.ItemType.Mana, "Drink", new Vector3(5.0f, 5.0f, 5.0f), 1, transform);
		ItemInteractable.SpawnItem(Define.ItemType.Grenate, "Explosion", new Vector3(-5.0f, 5.0f, 0.0f), 1,transform);
	}

	void Start()
    {

	}

    void Update()
    {
        
    }

	public override void Clear()
	{

	}
}
