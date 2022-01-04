using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2 move = new Vector2(); // 이동량
    [SerializeField] private BaseStat stat = new BaseStat();

    int checkLayer = 1 << (int)Define.Layer.Block;

    SpriteRenderer spriteRenderer = null;

    private int blockCount = 0;
    public int BlockCount => blockCount;
    int currentCoin = 0;
    public int CurrentCoin { get { return currentCoin; } set { currentCoin = value; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

	private void Init()
	{
        blockCount = 0;
        currentCoin = 0;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Managers.Resource.Load<Sprite>("Sprites/Characters", stat.selNum);
    }

	void CheckIsBlock()
	{
        bool cast = Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 2.0f, checkLayer, QueryTriggerInteraction.UseGlobal);
        blockCount += 1;
        if (cast == true) {
            // 블록에 위치할 경우에는 코인과 블록의 층을 세어준다.

            currentCoin += stat.increaseCoin;
        }
        else {
            Debug.Log("GameOver");
            // UI로 찍어줘야함
            Debug.Log($"Block : {blockCount}");
            Debug.Log($"currentCoin : {currentCoin}");

            stat.coin += currentCoin;
        }
    }

    public void __OnChangeButton()
	{
        spriteRenderer.flipX = !spriteRenderer.flipX;
        move.x = -move.x;

        transform.position = new Vector3(transform.position.x + move.x, transform.position.y + move.y, transform.position.z);

        CheckIsBlock();
        Managers.Game.Blocks.NewCycleBlock();
    }
    public void __OnFowardButton()
	{
        transform.position = new Vector3(transform.position.x + move.x, transform.position.y + move.y, transform.position.z);
        Debug.Log("ForwardButton");

        CheckIsBlock();
        Managers.Game.Blocks.NewCycleBlock();
    }

	private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + -Vector3.up);
        
	}

}
