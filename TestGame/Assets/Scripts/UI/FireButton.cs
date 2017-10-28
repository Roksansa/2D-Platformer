using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public sealed class FireButton : MonoBehaviour,  IDragHandler, IPointerUpHandler, IPointerDownHandler{
	private bool isPress;

	public bool IsPress {
		get { return isPress; }
		set { isPress = value; }
	}

	public delegate void OnEventHandler();
	public event OnEventHandler FireButtonEvent = delegate {  };
	public event OnEventHandler FireEndButtonEvent = delegate {  };
	
	public void OnPointerDown(PointerEventData eventData) {
		Debug.Log("I tuuuuuuuuut");
		FireButtonEvent();
	}

	public void OnPointerUp(PointerEventData eventData) {
		FireEndButtonEvent();
	}

	public void OnDrag(PointerEventData eventData) {
		if (!eventData.pointerEnter) {
			OnPointerUp(eventData);
		}
	}
	
	
}
