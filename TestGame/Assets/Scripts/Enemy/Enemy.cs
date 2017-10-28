using System;
using System.Collections;
using System.Text;
using UnityEngine;

public sealed class Enemy : MonoBehaviour{
  private CommonSettingObject settingObject;

  public CommonSettingObject SettingObject {
    get { return settingObject; }
    set { settingObject = value; }
  }

  //события генерируемые при смерти врага от пули
  //оповещаем пул сначала, а пул уже работает с UIконтролером
  public delegate void OnEventHandler();

  public event OnEventHandler DieEvent = delegate { };

  private Rigidbody2D cacheRigidbody2D;

  //используется ли экземпляр
  private bool isUse;
  //время на столкновения с другим противников и отскакивания от него
  private float timepast;
  //вектор сдвига в секунду
  private Vector2 shiftByVector;

  //специфическая настройка радиуса агрессии
  private float AggroRadius;
  //время заморозки
  private float timeFreeze = 3.0f;
  //заморожен ли
  private bool isFreeze;
  //столкновение врагов. реализация отскакивания
  private bool isStop;

  //началась ли анимация смерти
  private bool isDie;

  public bool IsDie {
    get { return isDie; }
  }

  //смерть от стрелы
  private bool isShell;

  public bool IsShell {
    get { return isShell; }
    set { isShell = value; }
  }

  //был ли объект в пределах поля
  private bool isTerrain;

  private void Awake() {
    gameObject.SetActive(false);
    cacheRigidbody2D = GetComponent<Rigidbody2D>();
    AggroRadius = EnvironmentSingleton.Instance.Width*0.2f;
    isUse = false;
    isTerrain = false;
    isDie = false;
    isFreeze = false;
  }

  // Update is called once per frame
  void FixedUpdate() {
    //реализовать AI/пока просто бегают пусть
    if (isUse && !isFreeze) {
      if (Vector2.Distance(cacheRigidbody2D.position, Controller.GetCurPlayer().CacheRigidbody2D.position) <=
          AggroRadius) {
        float speed = settingObject.Speed;
        shiftByVector = Vector2.ClampMagnitude(new Vector2(
            (Controller.GetCurPlayer().CacheRigidbody2D.position.x - cacheRigidbody2D.position.x)*speed,
            (Controller.GetCurPlayer().CacheRigidbody2D.position.y - cacheRigidbody2D.position.y)*speed),
          speed);
        double cos = (shiftByVector.x)/(Math.Sqrt(shiftByVector.x*shiftByVector.x + shiftByVector.y*shiftByVector.y));
        float angle = (float) Math.Acos(cos)*57.2958f;
        if (shiftByVector.y < 0) {
          angle = -angle;
        }
        cacheRigidbody2D.rotation = angle;
      }
      if (!isStop) {
        Move();
      }
      else {
        if (timepast > 0.4f) {
          timepast = 0;
          isStop = false;
          cacheRigidbody2D.velocity = Vector2.zero;
        }
        else {
          timepast += Time.deltaTime;
        }
      }
    }
  }

  private void Move() {
    cacheRigidbody2D.MovePosition(cacheRigidbody2D.position + shiftByVector*Time.deltaTime);
  }

  public bool IsUse {
    get { return isUse; }
    set { isUse = value; }
  }

  public void Create() {
    GetComponent<BoxCollider2D>().enabled = true;
    gameObject.SetActive(true);
    int randomShift = Controller.random.Next(50, 100);
    Vector2 randomVector =
      new Vector2(
        Controller.random.Next(-EnvironmentSingleton.Instance.Width + 150, EnvironmentSingleton.Instance.Width - 150),
        Controller.random.Next(-EnvironmentSingleton.Instance.Height + 150,
          EnvironmentSingleton.Instance.Height - 150));
    //примитивный разброс (выбор стороны для спавна)
    int x = Controller.random.Next(0, 2);
    int y = Controller.random.Next(0, 2);
    if (x == 0 && y == 0) {
      randomVector.x = -EnvironmentSingleton.Instance.WidthMax - randomShift;
      cacheRigidbody2D.rotation = Controller.random.Next(-45, 45);
    }
    if (x == 0 && y == 1) {
      randomVector.x = EnvironmentSingleton.Instance.WidthMax + randomShift;
      cacheRigidbody2D.rotation = Controller.random.Next(-180, -135);
      if (randomShift%2 == 0) {
        cacheRigidbody2D.rotation = Controller.random.Next(135, 180);
      }
    }
    if (x == 1 && y == 0) {
      randomVector.y = -EnvironmentSingleton.Instance.HeightMax - randomShift;
      cacheRigidbody2D.rotation = Controller.random.Next(45, 135);
    }
    if (x == 1 && y == 1) {
      randomVector.y = EnvironmentSingleton.Instance.HeightMax + randomShift;
      cacheRigidbody2D.rotation = Controller.random.Next(-135, -45);
    }

    //в каком направлении будут двигаться до проявления агрессии
    float speed = settingObject.Speed;
    shiftByVector = Vector2.ClampMagnitude(new Vector2(
        (float) Math.Cos(cacheRigidbody2D.rotation/57.2f)*speed,
        (float) Math.Sin(cacheRigidbody2D.rotation/57.2f)*speed),
      speed);
    cacheRigidbody2D.position = randomVector;
    GetComponent<SpriteRenderer>().size = new Vector2(settingObject.Size, settingObject.Size);
    GetComponent<BoxCollider2D>().size = new Vector2(settingObject.Size*0.95f, settingObject.Size*0.95f);
    isUse = true;
    isDie = false;
    isFreeze = false;
    isTerrain = false;
    isStop = false;
    isShell = false;
  }

  public void Die() {
    isDie = true;
    isFreeze = true;
    cacheRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    cacheRigidbody2D.velocity = new Vector2(0, 0);
    cacheRigidbody2D.angularVelocity = 0;
    StartCoroutine(DieWithEffect());
  }

  public void DieWithoutEffect() {
    isDie = true;
    gameObject.SetActive(false);
    isUse = false;
    isFreeze = false;
    cacheRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
  }

  private IEnumerator DieWithEffect() {
    float time = 0;
    int i = 0;
    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    GetComponent<BoxCollider2D>().enabled = false;
    Vector2 vector2 = new Vector2();
    while (time < 1) {
      i++;
      vector2.Set(settingObject.Size*(1 - time), settingObject.Size*(1 - time));
      spriteRenderer.size = vector2;
      time += Time.deltaTime;
      yield return new WaitForFixedUpdate();
    }
    //выкинули за пределы
    cacheRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    cacheRigidbody2D.position = new Vector2(0, 1000);
    //подождем кадр, чтобы артефакты не мешали при создании нового объекта
    yield return new WaitForFixedUpdate();
    gameObject.SetActive(false);
    isUse = false;
    isFreeze = false;
  }


  private void OnCollisionEnter2D(Collision2D other) {
    if (other.gameObject.CompareTag("Enemy") && !isFreeze) {
      if (isTerrain) {
        float speed = settingObject.Speed;
        cacheRigidbody2D.AddForce(other.relativeVelocity, ForceMode2D.Impulse);
        isStop = true;
        //сменить направление немного
        shiftByVector = new Vector2(-shiftByVector.x, -shiftByVector.y);
        cacheRigidbody2D.rotation = cacheRigidbody2D.rotation + 180;
        cacheRigidbody2D.velocity = cacheRigidbody2D.velocity.normalized*speed;
        cacheRigidbody2D.angularVelocity = 0;
      }
      return;
    }
    if (other.gameObject.CompareTag("Player") && !isDie) {
      if (!isFreeze) {
        isFreeze = true;
        StartCoroutine(OnFreeze());
      }
    }
  }

  private void OnCollisionStay2D(Collision2D other) {
    if (other.gameObject.CompareTag("Player") && isDie) {
      cacheRigidbody2D.velocity = new Vector2(0, 0);
      cacheRigidbody2D.angularVelocity = 0;
    }
  }

  private IEnumerator OnFreeze() {
    cacheRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    yield return new WaitForSeconds(timeFreeze);
    cacheRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    isFreeze = false;
  }

  private void OnCollisionExit2D(Collision2D other) {
    if (other.gameObject.CompareTag("Player")) {
      cacheRigidbody2D.velocity = new Vector2(0, 0);
      cacheRigidbody2D.angularVelocity = 0;
    }
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Weapon") && !isDie) {
      DieEvent();
      Die();
    }
  }

  private void OnTriggerExit2D(Collider2D other) {
    if (other.CompareTag("Wall") && !isTerrain) {
      //враг появился на поле
      isTerrain = true;
      return;
    }
    if (other.CompareTag("Wall") && isTerrain && !isDie && Vector2.Distance(cacheRigidbody2D.position,
          Controller.GetCurPlayer().CacheRigidbody2D.position) > AggroRadius) {
      //покинул зону поля, уничтожаем
      DieWithoutEffect();
    }
  }
}