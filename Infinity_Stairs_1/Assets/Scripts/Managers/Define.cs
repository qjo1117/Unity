using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
	public enum Layer
	{
		Block = 8,
	}

	public enum Scene
	{
		Unknown,
		Lobby,
		Game,
		SelectCharacter,
		
	}


	public enum MouseEvent
	{
		None,
		Press,
		Click,
	}
}