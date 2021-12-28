using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public PlayerController _player = null;
    public List<GameObject> _mosnters = new List<GameObject>();

    private ItemManager _items = new ItemManager();
    public ItemManager ItemM { get { return _items; } }

    public InventoryController _inventory = null;


    void Start()
    {
        ItemM.Init();
    }

    void Update()
    {
        
    }
}
