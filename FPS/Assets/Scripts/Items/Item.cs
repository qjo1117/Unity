using UnityEngine;
using UnityEngine.UI;



public class Item
{
    public string name;
    public Sprite icon;
    public int count;

    public Define.ItemType type;

    static public Sprite GetItemImage(Define.ItemType type)
	{
        switch(type)
		{
            case Define.ItemType.Grenate:
                return Managers.Resource.Load<Sprite>("Image/Grenate");
            case Define.ItemType.Gun:
                return Managers.Resource.Load<Sprite>("Image/Gun");
            case Define.ItemType.Health:
                return Managers.Resource.Load<Sprite>("Image/Health");
            case Define.ItemType.Mana:
                return Managers.Resource.Load<Sprite>("Image/Mana");
        }

        return Managers.Resource.Load<Sprite>("Image/None");
	}

    static public GameObject GetPrefab(Define.ItemType type)
	{
        switch (type)
        {
            case Define.ItemType.Grenate:
                return Managers.Resource.NewPrefab("Item/Grenate");
            case Define.ItemType.Gun:
                return Managers.Resource.NewPrefab("Item/Gun");
            case Define.ItemType.Health:
                return Managers.Resource.NewPrefab("Item/Health");
            case Define.ItemType.Mana:
                return Managers.Resource.NewPrefab("Item/Mana");
        }

        return Managers.Resource.NewPrefab("Item/None");
    }

    public virtual void Apply() { }
}

public class ItemInfo
{
    public string name;     // 이름
    public int count;       // 현재 들고 있는 갯수
    public int maxCount;    // 최대로 들 수 있는 갯수
    public Sprite sprite;

    public ItemInfo(string name, int count, int maxCount)
	{
        this.name = name;
        this.count = count;
        this.maxCount = maxCount;
	}
}

public class GunInfo : ItemInfo
{
    private Define.GunType type;            // AR,SR,SHUNGUN 구별

    private int isCanShutType;              // 현재 사용할 수 잇는 상태를 여기에 저장한다.
    private Define.GunShutType shutType;    // 현재 단발, 점사, 연사모드인지 체크한다.

    public float verticalPop;               // 가능할지는 모르겠지만 에임 떨림 구현
    public float horizontalPop;             // 가능할지는 모르겠지만 에임 떨림 구현

    public Define.GunShutType ShutType 
    { 
        get { return shutType; } 
        set
        {
            // 비트플래그로 체크하고 창작 가능여부를 확인한다.
            int check = ((int)value & isCanShutType);

            // 사용불가능
            if(check == 0) {
                return;
			}

            shutType = (Define.GunShutType)(1 << check);
        } 
    }

    public GunInfo(string name, int count, int maxCount) : base(name, count, maxCount) 
    {
        type = Define.GunType.AR;
    }

    public GunInfo SetGunType(Define.GunType type)
	{
        switch(type) {
            case Define.GunType.AR:
                SetAR();
                break;
            case Define.GunType.SR:
                SetSR();
                break;
            case Define.GunType.SHUTGUN:
                SetShutGun();
                break;
            default:
                Debug.Log("Item GunInfo Error");
                break;
        }
        return this;
	}
    
    void SetAR()
	{
        type = Define.GunType.AR;
        verticalPop = 0.1f;
        horizontalPop = 0.1f;
        isCanShutType = (int)Define.GunShutType.First | (int)Define.GunShutType.Threeple | (int)Define.GunShutType.Continue;
        shutType = Define.GunShutType.First;
    }

    void SetSR()
	{
        type = Define.GunType.SR;
        verticalPop = 0.3f;
        horizontalPop = 0.3f;
        isCanShutType = (int)Define.GunShutType.First;
        shutType = Define.GunShutType.First;
    }

    void SetShutGun()
	{
        type = Define.GunType.SHUTGUN;
        verticalPop = 0.5f;
        horizontalPop = 0.5f;
        isCanShutType = (int)Define.GunShutType.First;
        shutType = Define.GunShutType.First;
    }
}