using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
	enum GameObjects
    {
        HPBar,
	}


    Stat _stat;
	public override void Init()
	{
        Bind<GameObject>(typeof(GameObjects));
        _stat = transform.parent.gameObject.GetComponent<Stat>();
    }

	void Update()
    {
        // UI를 들고 있는 녀석의 Transform을 가져온다.
        Transform parent = gameObject.transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size).y;

        transform.LookAt(Camera.main.transform);
        transform.rotation = Camera.main.transform.rotation;

        float ratio = _stat.Hp / (float)_stat.MaxHp;
        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
	{
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
	}
}
