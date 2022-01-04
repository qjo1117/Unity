using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{


	protected override void Init()
	{
		base.Init();
		SceneType = Define.Scene.Game;

		Managers.Game.Init();
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
