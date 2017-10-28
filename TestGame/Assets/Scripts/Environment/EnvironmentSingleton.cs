using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//класс синглтон для настройки среды (хранит значения высоты/ширины экрана)
public class EnvironmentSingleton{
  //инициализация
  private static readonly EnvironmentSingleton instance = new EnvironmentSingleton();
  static EnvironmentSingleton() { }

  private EnvironmentSingleton() {
    widthMax = GameObject.Find("Floor").GetComponent<SpriteRenderer>().size.x/2;
    heightMax = GameObject.Find("Floor").GetComponent<SpriteRenderer>().size.y/2;
    //устанавливаем параметры экрана каждый раз при загрузке сцены
    mainCamera = GameObject.Find("Main Camera").GetComponent("Camera") as Camera;
    float curAspect = mainCamera.aspect;
    //если разные разрешения, то меняем разрешение камеры
    if (Math.Abs(curAspect - aspectDefault) > 0.01) {
      float a = aspectDefault/curAspect;
      orthographicSize = (int)(mainCamera.orthographicSize*a);
    }
    Presetting();
    //юнитековких координат влезает в экран в высоту
    height = (int)mainCamera.orthographicSize*2;
    width = widthDefault;
  }

  //поля

  private const int widthDefault = 1366;
  private const float aspectDefault = 1.77f;
  private float orthographicSize = 384;
  private Camera mainCamera;


  private int width;
  public int Width { get { return width; }}
  private int height;
  public int Height { get { return height; } }

  //размеры игрового поля
  //границы для камеры/игрока, на которое может сдвинуться
  private float widthMax;
  private float heightMax;


  public float WidthMax {
    get { return widthMax; }
  }

  public float HeightMax {
    get { return heightMax; }
  }
  public static EnvironmentSingleton Instance {
    get { return instance; }
  }

  public void Presetting() {
    if (mainCamera == null) {
      mainCamera = GameObject.Find("Main Camera").GetComponent("Camera") as Camera;
    }
    mainCamera.orthographicSize = orthographicSize;
  }
}