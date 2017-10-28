using System;
using System.Collections;
using UnityEngine;


public sealed class BattleShell : MonoBehaviour{
  private bool isUse;

  //от какого объекта будут лететь пули
  private Player curTarget;

  private Transform curDirectionPlayer;
  private Rigidbody2D cacheRigidbody2D;

  //использован ли снаряд
  private bool isApplied;
  private float speed;
  //направление движения для пули, задается единоворазово в функции fire
  private Vector2 direction;

  public bool IsUse {
    get { return isUse; }
  }

  private void Awake() {
    gameObject.SetActive(false);
    cacheRigidbody2D = GetComponent<Rigidbody2D>();
    isUse = false;
    speed = EnvironmentSingleton.Instance.WidthMax*0.2f;
    Vector2 sizeShell = new Vector2(EnvironmentSingleton.Instance.WidthMax*0.05f,
      EnvironmentSingleton.Instance.WidthMax*0.01f);
    Vector2 sizeCollaiderShell = new Vector2(EnvironmentSingleton.Instance.WidthMax*0.05f,
      EnvironmentSingleton.Instance.WidthMax*0.007f);
    GetComponent<SpriteRenderer>().size = sizeShell;
    GetComponent<BoxCollider2D>().size = sizeCollaiderShell;
  }

  private void FixedUpdate() {
    if (isUse) {
      cacheRigidbody2D.MovePosition(cacheRigidbody2D.position + direction*Time.deltaTime);
    }
  }

  public void Fire() {
    if (curTarget == null) {
      //привязали
      curTarget = Controller.GetCurPlayer();
      curDirectionPlayer = curTarget.transform.GetChild(0);
    }
    //упрощенное решение
    //заняли позицию в точке 
    gameObject.SetActive(true);   
    Vector2 newPos = new Vector2(curDirectionPlayer.position.x, curDirectionPlayer.position.y);
    cacheRigidbody2D.position = newPos;
    //последнее направление движения (значит объект смотрит в эту сторону)
    direction = curTarget.ShiftByVector.normalized;
    direction = Vector2.ClampMagnitude(new Vector2(direction.x*speed, direction.y*speed), speed);
    cacheRigidbody2D.rotation = curTarget.CacheRigidbody2D.rotation;
    isUse = true;
    isApplied = false;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Enemy") && !isApplied) {
      Enemy curEmeny = other.GetComponent<Enemy>();
      if (!curEmeny.IsShell) {
        isApplied = true;
        curEmeny.IsShell = true;
        StartCoroutine(OnOffset());
      }
    }
  }

  private IEnumerator OnOffset() {
    //пока что фпс настолько высокий, что просто не успевает сменить позицию прежде, чем отключается объект.
    cacheRigidbody2D.position = new Vector2(0, 1000);
    yield return new WaitForFixedUpdate();
    isUse = false;
    gameObject.SetActive(false);
  }


  private void OnTriggerExit2D(Collider2D other) {
    if (other.CompareTag("Wall")) {
      isUse = false;
      gameObject.SetActive(false);
    }
  }
}