using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] static private List<InventoryBaseSlot>    _listItemSlot = new List<InventoryBaseSlot>();

    [SerializeField] static private bool                       _isInventory = false;
    [SerializeField] static public GameObject                  _inventory = null;

    void Start()
    {

        GameObject slot = Managers.Resource.NewPrefab("ItemSlot");
        slot.SetActive(false);
        Managers.Pool.CreatePool(slot, 30);
        Destroy(slot);

    }

    static public void AddItem(Item item)
	{
        GameObject obj = Managers.Resource.NewPrefab("ItemSlot", GetInventory());
        InventoryItemSlot slot = obj.GetComponent<InventoryItemSlot>();
        if(slot == null) {
            slot = obj.AddComponent<InventoryItemSlot>();
		}
        _listItemSlot.Add(slot);
        slot.gameObject.SetActive(true);
        slot.SetItem(item);
        slot.SetValue();

    }

    static public Transform GetInventory()
	{
        if (_inventory == null) {
            _inventory = Managers.Game._inventory.GetComponentInChildren<InventoryUI>().gameObject;
            _inventory.SetActive(true);
        }
        return _inventory.transform;
    }

    void DeleteItem(Item item)
	{
	}

    void Update()
    {
        
    }
}
