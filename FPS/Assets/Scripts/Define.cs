using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
	public enum CameraMode
	{
		QuarterView,
		FirstView,
	}

	public enum MouseEvent
	{
		None,
		Press,
		Click,
	}

	public enum Scene
	{
		Unknown,
		Login,
		Lobby,
		Game,
	}

	public enum PlayerAnimState
	{
		IDLE,
		RUN,
		SHOOT,
		BACKRUN,
	}

	public enum ItemType
	{
		None,
		Gun,
		Grenate,
		Health,
		Mana,
	}

	public enum GunType
	{
		AR,
		SR,
		SHUTGUN,
	}

	public enum GunShutType
	{
		// TO DO : 네이밍 고치자.
		First		= (1 << 0),		// 단발
		Threeple	= (1 << 1),		// 점사
		Continue	= (1 << 2),		// 연발
	}
}