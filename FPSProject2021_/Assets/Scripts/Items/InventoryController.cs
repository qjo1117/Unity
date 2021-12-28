using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public bool             _isInventory = false;
    public InventoryUI      _inventory = null;

    

	void Start()
    {

    }

    void Update()
    {
        
    }

    // 아이템을 얻게 되면
    public void AddItem(ItemInfo item)
	{

	}

	public void Init()
	{
        gameObject.SetActive(false);
        Managers.Input.AddKeyAction(OnInputInventory);
    }

	void OnInputInventory()
	{
        if(Input.GetKeyDown(KeyCode.I) == false) {
            return;
		}

        _isInventory = !_isInventory;
        gameObject.SetActive(_isInventory);
    }
}
