using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField] private int _exp;
    [SerializeField] private int _gold;

    public int Exp 
	{ 
		get { return _exp; } 
		set { 
			_exp = value;
			// LevelUp Check

			int level = Level;
			while(true) {
				// 최대 레벨 확인
				Data.Stat stat;
				if(Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false) {
					break;
				}

				// 레벨 부족
				if(_exp < stat.totalExp) {
					break;
				}

				level += 1;
				_exp -= stat.totalExp;
			}

			if(level != Level) {
				Level = level;
				SetStat(level);
			}
		} 
	}
    public int Gold { get { return _gold; } set { _gold = value; } }


	private void Start()
	{
		_level = 1;
		_defence = 5;
		_moveSpeed = 5.0f;

		_gold = 0;

		SetStat(_level);
	}

	public void SetStat(int level)
	{
		Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
		Data.Stat stat = dict[level];

		_hp = stat.maxHp;
		_maxHp = stat.maxHp;
		_attack = stat.attack;
	} 
}
