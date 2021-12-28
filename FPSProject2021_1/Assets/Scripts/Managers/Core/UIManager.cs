using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _order = 10;

    Stack<UI_Popup> _stackPopup = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    public GameObject Root
	{
        get {
            GameObject root = GameObject.Find("@UI_Root");
            if(root == null) {
                root = new GameObject { name = "@UI_Root" };
			}
            return root;
		}
	}

    public void SetCanvas(GameObject go, bool sort = true)
	{
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if(sort == true) {
            canvas.sortingOrder = _order;
            _order += 1;
		}
        else {
            canvas.sortingOrder = 0;
		}
	}

    public T MakeWorldSpace<T>(Transform parent = null, string name = null) where T : UI_Base
	{
        if(string.IsNullOrEmpty(name)) {
            name = typeof(T).Name;
		}

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
        if(parent != null) {
            go.transform.parent = parent;
		}

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
	}

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
        if(string.IsNullOrEmpty(name)) {
            name = typeof(T).Name;
		}

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
        if(parent != null) {
            go.transform.SetParent(parent);
		}

        return Util.GetOrAddComponent<T>(go);
	}

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
        if(string.IsNullOrEmpty(name)) {
            name = typeof(T).Name;
		}

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
	}

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
	{
        if(string.IsNullOrEmpty(name)) {
            name = typeof(T).Name;
		}

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _stackPopup.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
	}

    public void ClosePopupUI(UI_Popup popup)
	{
        if(_stackPopup.Count == 0) {
            return;
		}

        if(_stackPopup.Peek() != popup) {
            Debug.Log("Close Popup Failed!");
            return;
		}

        ClosePopupUI();
	}

    public void ClosePopupUI()
	{
        if(_stackPopup.Count == 0) {
            return;
		}

        UI_Popup popup = _stackPopup.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order -= 1;
	}
    public void CloseAllPopupUI()
    {
        while (_stackPopup.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
