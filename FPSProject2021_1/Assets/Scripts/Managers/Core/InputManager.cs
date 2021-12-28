using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    bool    _pressed = false;
    float   _pressedTime = 0.0f;

    public void OnUpdate()
	{
        if(EventSystem.current.IsPointerOverGameObject()) {
            return;
		}

        if(Input.anyKey && KeyAction != null) {
            KeyAction();                        // Action 실행
		}

        if (MouseAction != null) {
            if(Input.GetMouseButton(0)) {
                if(_pressed == false) {
                    MouseAction(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
				}
			}

            MouseAction(Define.MouseEvent.Press);
            _pressed = true;
		}
        else {
            if(_pressed == true) {
                if(Time.time < _pressedTime *0.2f) {
                    MouseAction(Define.MouseEvent.Click);
                }
                MouseAction(Define.MouseEvent.PointerUp);
			}

            _pressed = false;
            _pressedTime = 0.0f;
		}
	}

    public void Clear()
	{
        KeyAction = null;
        MouseAction = null;
	}

}
