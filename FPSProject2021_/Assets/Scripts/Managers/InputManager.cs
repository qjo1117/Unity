using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
	public Action KeyAction = null;
	public Action<Define.MouseEvent> MouseAction = null;
	public Action NotKeyAction = null;		// 따로 기능 추가

	bool _pressed = false;

	#region Mouse Helper Function
	public void AddKeyAction(Action func)
	{
		KeyAction -= func;
		KeyAction += func;
	}
	public void AddMouseAction(Action<Define.MouseEvent> func)
	{
		MouseAction -= func;
		MouseAction += func;
	}
	public void AddNotKeyAction(Action func)
	{
		NotKeyAction -= func;
		NotKeyAction += func;
	}
	#endregion

	public void OnUpdate()
	{
		if(Input.anyKey == false && NotKeyAction != null) {
			NotKeyAction();
		}

		if(Input.anyKey == true && KeyAction != null) {
			KeyAction();
		}

		// UI와 마우스클릭에 대한 상호작용 막기
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		if (MouseAction != null) {
			if(Input.GetMouseButton(0)) {
				MouseAction(Define.MouseEvent.Press);
				_pressed = true;
			}
			else {
				if (_pressed)
				{
					MouseAction(Define.MouseEvent.Click);
				}
				else
				{
					MouseAction(Define.MouseEvent.None);
				}
				_pressed = false;
			}
		}
	}
}
