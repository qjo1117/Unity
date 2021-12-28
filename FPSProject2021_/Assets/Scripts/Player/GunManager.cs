using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Bullet
{
	public string name;
	public int number;
	public float force;

	public Bullet(string name, int number, float force)
	{
		this.number = number;
		this.name = name;
		this.force = force;
	}
}

public enum BULLET_TYPE
{
	BULLET_9,
	BULLET_5,
}

public class GunManager
{
	public static GunManager	gun = new GunManager();
	public GameObject			_obj = new GameObject { name = "@GunManager" };

	public GameObject			_bulletParent = new GameObject { name = "Bullet_Root" };
	public GameObject			_decalParent = new GameObject { name = "Decal_Root" };

	private Bullet[]			_bullet = new Bullet[typeof(BULLET_TYPE).GetEnumNames().Length];

	public void Start()
	{
		_bulletParent.transform.parent = _obj.transform;
		_decalParent.transform.parent = _obj.transform;

		int count = 0;
		// Enum에 정의된 오브젝트들을 만들어주고 Pool에 등록시켜준다.
		string[] enumStr = typeof(BULLET_TYPE).GetEnumNames();
		foreach(string str in enumStr) {
			RegisterBullet(str);

			Bullet data = new Bullet(str,count,0.0f);
			_bullet[count] = data;
			count += 1;
		}

		_bullet[(int)BULLET_TYPE.BULLET_5].force = 5.0f;
		_bullet[(int)BULLET_TYPE.BULLET_9].force = 9.0f;

		// Decal 셋팅
		GameObject obj = Managers.Resource.NewPrefab("BulletDecal", _decalParent.transform);
		Managers.Pool.CreatePool(obj, 40);
		GameObject.Destroy(obj);

	}

	public Bullet GetBulletData(int index) 
	{
		return _bullet[index];
	}

	private void RegisterBullet(string name)
	{
		// ObjectPool안에 BulletObject를 넣어준다.
		GameObject bulletObject = Managers.Resource.NewPrefab(name, _bulletParent.transform);
		Managers.Pool.CreatePool(bulletObject, 20);
		GameObject.Destroy(bulletObject);
	}

	public void Shot(GameObject player, Bullet bullet)
	{
		Vector3 startPos = player.transform.position;
		Vector3 endPos = startPos + player.transform.forward * bullet.force;

		GameObject obj = Managers.Resource.NewPrefab(bullet.name, _bulletParent.transform);
		obj.transform.position = player.transform.position;
		obj.transform.forward = player.transform.forward;
		obj.GetComponent<BaseBullet>().Velocity(endPos);
	}



}
