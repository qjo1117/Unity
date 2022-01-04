using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
	public int currentIndex = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.name.Contains("Player")) {
			PlayerController player = other.GetComponent<PlayerController>();
			player.CurrentCoin += 100;
			Managers.Game.Blocks.coinGenarator.DestroyCoin();
		}
	}

	private void FixedUpdate()
	{
	}
}
