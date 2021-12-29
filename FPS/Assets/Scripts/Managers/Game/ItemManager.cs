using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager
{
    public List<ItemInfo> _items;

    public enum Item
	{
        Box = 0,
        Drink,
        Bullet_5,
        Bullet_7,
        AK_47,
        Kar98,
        Double,
	}

    public void AddItemList(ItemInfo info)
	{
        _items.Add(info);
	}

    public void Init()
	{
        _items.Clear();
        // 아직 Json이 익숙하지않아서 수기로 코드를 넣었습니다.
        AddItemList(new ItemInfo("구급상자", 0, 5));
        AddItemList(new ItemInfo("에너지드링크", 0, 10));
        AddItemList(new ItemInfo("5mm", 0, 30));
        AddItemList(new ItemInfo("7mm", 0, 30));

        AddItemList(new GunInfo("AK-47", 0, 1).SetGunType(Define.GunType.AR));
        AddItemList(new GunInfo("Kar98", 0, 1).SetGunType(Define.GunType.SR));
        AddItemList(new GunInfo("DoubleBarrer", 0, 1).SetGunType(Define.GunType.SHUTGUN));
    }

}
