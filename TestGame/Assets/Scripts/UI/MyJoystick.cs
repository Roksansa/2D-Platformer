using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MyJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
  private Image backgroundImage; //Изображение основного Image
  [SerializeField] private Image joystickImg; //Изображение дочернего Image

  private Vector2 inputDirectionVector2;
  private Vector2 zeroInputVector2;

  public Vector2 InputDirectionVector2 {
    get { return inputDirectionVector2; }
  }

  public Vector2 ZeroInputVector2 {
    get { return zeroInputVector2; }
  }

  private void Start() {
    backgroundImage = GetComponent<Image>(); //Получаем данные компонента VirtualJoystickContainer в
    // вектор сдвига
    zeroInputVector2 = Vector2.zero;
    inputDirectionVector2 = zeroInputVector2;
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 pos = zeroInputVector2;
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundImage.rectTransform,
      eventData.position, eventData.pressEventCamera, out pos)) {
      //от -0.5-0.5
      pos.x = (pos.x/backgroundImage.rectTransform.sizeDelta.x);
      pos.y = (pos.y/backgroundImage.rectTransform.sizeDelta.y);
      inputDirectionVector2 = new Vector2(pos.x, pos.y);
      //так как значение дробные, то квадрат не провзойдет 0.5.
      inputDirectionVector2 = (inputDirectionVector2.SqrMagnitude() > 0.5)
        ? Vector2.ClampMagnitude(inputDirectionVector2, 0.7f)
        : inputDirectionVector2;
      //перетягиваем
      joystickImg.rectTransform.anchoredPosition =
        new Vector3(inputDirectionVector2.x*(backgroundImage.rectTransform.sizeDelta.x*2/5),
          inputDirectionVector2.y*(backgroundImage.rectTransform.sizeDelta.y*2/5));
    }
  }

  public void OnPointerDown(PointerEventData eventData) {
    OnDrag(eventData);
  }

  //сравнительно не часто вызывается, поэтому оставим Vector3.zero
  public void OnPointerUp(PointerEventData eventData) {
    inputDirectionVector2 = zeroInputVector2;
    joystickImg.rectTransform.anchoredPosition = Vector3.zero;
  }
}