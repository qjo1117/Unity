using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CoinGenerator
{
    [SerializeField] private Sprite[] coinSprites = new Sprite[4];
    Queue<Coin> coins = new Queue<Coin>();

    float currentTime = 0.0f;
    float maxTime = 0.0f;

    public void Init()
    {
        for (int i = 0; i < 4; ++i) {
            coinSprites[i] = Managers.Resource.Load<Sprite>("Sprites/Item_Coin", i);
        }
        maxTime = 1.0f;

        GameObject go = Managers.Resource.NewPrefab("Coin");
        Managers.Pool.CreatePool(go);
        Managers.Resource.DelPrefab(go);
    }

    public void UpdateAnimation()
	{
        currentTime += Time.deltaTime;
        if(currentTime < maxTime) {
            return;
		}
        currentTime -= maxTime;

        foreach(Coin coin in coins) {
            coin.GetComponent<SpriteRenderer>().sprite = coinSprites[coin.currentIndex % 4];
            coin.currentIndex += 1;
        }
	}

    public void SpawnCoin(Vector3 position)
	{
        GameObject go = Managers.Resource.NewPrefab("Coin", Managers.Game.Blocks.transform);
        go.transform.position = position + Vector3.up;
        coins.Enqueue(go.GetComponent<Coin>());
    }

    public void DestroyCoin()
	{
        Managers.Resource.DelPrefab(coins.Dequeue().gameObject);
    }

}
