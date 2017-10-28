using UnityEngine;

public sealed class CameraMovement : MonoBehaviour{
  //границы
  //Должна следовать за персонажем если тот приближается к любому из краев экрана на дистанцию меньше четверти длины или ширины экрана в зависимости от стороны. При этом, если персонаж приближается к краю игрового поля, то камера за ним не следует, то есть камера не должна захватывать объекты находящиеся за пределами игрового поля.
  private float widthBorder;
  private float heightBorder;
  private Transform transformPlayer;
  private Player player;
  private Transform cameraPosition;

  private float widthMax;
  private float heightMax;
  //переменная для необходимости выполнять update

  // Use this for initialization
  private void Awake() {
    EnvironmentSingleton.Instance.Presetting();
    widthBorder = EnvironmentSingleton.Instance.Width*1/4;
    heightBorder = EnvironmentSingleton.Instance.Height*1/4;
    widthMax = EnvironmentSingleton.Instance.WidthMax;
    heightMax = EnvironmentSingleton.Instance.HeightMax;
    cameraPosition = transform;
  }

  void Start() {
    player = Controller.GetCurPlayer();
    transformPlayer = player.gameObject.GetComponent<Transform>();
  }

  private void LateUpdate() {
    //если произошло обновление физики и произошли изменения, то вызываем move()
    if (!player.IsUpdate) return;
    move();
  }

  //оптимизировать код
  private void move() {
    Vector3 vectorCameraPosition = cameraPosition.position;
    float curWidthForBorderX1 = vectorCameraPosition.x - widthBorder;
    float curWidthForBorderX2 = vectorCameraPosition.x + widthBorder;
    float curHeightForBorderY1 = vectorCameraPosition.y - heightBorder;
    float curHeightForBorderY2 = vectorCameraPosition.y + heightBorder;
    Vector3 vectorPlayerPosition = transformPlayer.position;
    byte shiftX = 0;
    byte shiftY = 0;
    if (vectorPlayerPosition.x < curWidthForBorderX1 && player.ShiftByVector.x < 0
        || vectorPlayerPosition.x > curWidthForBorderX2 && player.ShiftByVector.x > 0) {
      shiftX = 1;
    }
    if (vectorPlayerPosition.y < curHeightForBorderY1 && player.ShiftByVector.y < 0
        || vectorPlayerPosition.y > curHeightForBorderY2 && player.ShiftByVector.y > 0) {
      shiftY = 1;
    }
    //сдвигать камеру не надо.
    if (shiftX == 0 && shiftY == 0) return;
    //сдвигаем камеру
    //получаем насколько необходимо передвинуть камеру
    Vector3 pos = new Vector3(shiftX*player.ShiftByVector.x, shiftY*player.ShiftByVector.y, 0f);
    //крайние границы, которые получим если передвинем камеру
    vectorCameraPosition =
      new Vector2(player.ShiftByVector.x + vectorCameraPosition.x, player.ShiftByVector.y + vectorCameraPosition.y);
    //проверяем границы. если дальше, то обновление камеры 0 по этой оси.
    curWidthForBorderX1 = vectorCameraPosition.x - EnvironmentSingleton.Instance.Width/2;
    curWidthForBorderX2 = vectorCameraPosition.x + EnvironmentSingleton.Instance.Width/2;
    curHeightForBorderY1 = vectorCameraPosition.y - EnvironmentSingleton.Instance.Height/2;
    curHeightForBorderY2 = vectorCameraPosition.y + EnvironmentSingleton.Instance.Height/2;
    if (curWidthForBorderX1 < -widthMax || curWidthForBorderX2 > widthMax) {
      pos.x = 0;
    }
    if (curHeightForBorderY1 < -heightMax || curHeightForBorderY2 > heightMax) {
      pos.y = 0;
    }
    cameraPosition.Translate(pos);
    player.IsUpdate = false;
  }
}