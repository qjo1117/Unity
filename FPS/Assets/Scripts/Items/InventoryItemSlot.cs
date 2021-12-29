using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : InventoryBaseSlot, IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler, IDropHandler
{
	// 기존 상속된 변수들
	//public Image _icon;
	//public Text _text;
	//public Item _item;

	public GameObject		_objImage;
	public GameObject		_objCount;

	// 드래그 앤 드롭 변수들
	[SerializeField] private Canvas		_canvas;
	private RectTransform				_rectImage;
	private CanvasGroup					_canvasGroup;
	private Vector3						_memoryPos;


	private void Start()
	{
		_rectImage = GetComponent<RectTransform>();
		_canvasGroup = GetComponent<CanvasGroup>();
		if (_canvasGroup == null)
		{
			_canvasGroup = gameObject.AddComponent<CanvasGroup>();
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
		}
		_canvas = transform.root.GetComponentInChildren<InventoryController>().GetComponent<Canvas>();
	}
	
	public void SetItem(Item item)
	{
		_item = item;

		_rectImage = GetComponent<RectTransform>();
		_canvasGroup = GetComponent<CanvasGroup>();
		if(_canvasGroup == null) {
			_canvasGroup = gameObject.AddComponent<CanvasGroup>();
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
		}

		_text = transform.GetChild(0).GetComponent<Text>();
		_image = transform.GetChild(1).GetComponent<Image>();

	}

	public void SetValue()
	{
		if(_item == null) {
			return;
		}

		_image.sprite = Item.GetItemImage(_item.type);
		_text.text = $"{_item.count}";
	}

	public void __OnButtonClick()
	{
		if(Input.GetMouseButtonDown(1) == true) {
			_item.count -= 1;
			SetValue();
		}

	}

	public void OnPointerDown(PointerEventData eventData)
	{

	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		_memoryPos = _rectImage.anchoredPosition;
		_canvasGroup.blocksRaycasts = false;
		_canvasGroup.alpha = 0.6f;
	}

	public void OnEndDrag(PointerEventData eventData)
	{

		if (eventData.position.x > 300) {
			_item.count -= 1;
			if (_item.count <= 0) {
				Debug.Log($"{_item.name} Item Use");
				Managers.Resource.DelPrefab(gameObject);
			}
			SetValue();
		}
		_rectImage.anchoredPosition = _memoryPos;
		_canvasGroup.blocksRaycasts = true;
		_canvasGroup.alpha = 1.0f;

		
	}

	public void OnDrag(PointerEventData eventData)
	{
		_rectImage.anchoredPosition += eventData.delta / _canvas.scaleFactor;
	}

	public void OnDrop(PointerEventData eventData)
	{


	}
}
