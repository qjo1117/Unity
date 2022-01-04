using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
	Transform			root = null;
	Queue<GameObject>	_blocks = new Queue<GameObject>();        // 블록들을 저장한다.
	public CoinGenerator coinGenarator = new CoinGenerator();

	float maxHeight = -4.5f;
	const float moveY = 1.0f;     // 올라갈 수 있는 량

	int currentX = 0;                   // 현재 가로는 얼마만큼 진행했는가?
	const float moveX = 2.0f;

	[SerializeField] private Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);

	int maxX = 2;

	private void Start()
	{
		coinGenarator.Init();
	}

	private void Update()
	{
		coinGenarator.UpdateAnimation();
	}

	public BlockGenerator Init(Transform _root)
	{
		// 풀에 10정도 만들어둔다.
		GameObject objPool = Managers.Resource.NewPrefab("Block", root);
		Managers.Pool.CreatePool(objPool, 10);
		Managers.Resource.DelPrefab(objPool);
		root = _root;

		// 한번 어떻게 찍히는지 싸워보자
		for (int i = 0; i < 10; ++i) {
			NewBlock();
		}

		return this.GetComponent<BlockGenerator>();
	}

	// 생성과 동시에 등록을 해준다.
	public GameObject NewBlock()
	{
		GameObject block = Managers.Resource.NewPrefab("Block", root);
		block.transform.position = RandomCoord();
		_blocks.Enqueue(block);
		return block;
	}

	public void NewCycleBlock()
	{
		GameObject block = _blocks.Dequeue();
		block.transform.position = RandomCoord();
		_blocks.Enqueue(block);

		if(Random.Range(0,3) == 1) {
			coinGenarator.SpawnCoin(block.transform.position);
		}
	}

	// 랜덤하게 좌표를 설정해주는 기능을 한다.
	Vector3 RandomCoord()
	{
		int random = Random.Range(0, 2);
		Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);

		pos.x = (currentX * moveX);
		pos.y = maxHeight;

		if (random == 0) {
			currentX -= 1;
			if(-maxX > currentX) {
				currentX = -maxX + 1;
			}
		}
		else {
			currentX += 1;
			if (maxX < currentX) {
				currentX = maxX - 1;
			}
		}
		maxHeight += moveY;

		return pos;
	}

}
