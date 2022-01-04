using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
	private BlockGenerator blockSpawn = null;
	public BlockGenerator Blocks { get { return blockSpawn; } }

	private PlayerController player = null;
	public PlayerController Player {  get { return player; } }

	public void Init()
	{
		blockSpawn = new GameObject { name = "@Block_Root" }.GetOrAddComponent<BlockGenerator>();
		blockSpawn.Init(blockSpawn.transform);


		player = GameObject.FindObjectOfType<PlayerController>();
		if(player == null) {
			player = Managers.Resource.NewPrefab("Player").GetOrAddComponent<PlayerController>();
		}
	}

}
