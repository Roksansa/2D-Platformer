using System;
using System.Collections;
using UnityEngine;

public sealed class Player : MonoBehaviour {
  //будем держать ссылку на джостик
  [SerializeField] private MyJoystick joystick;

  private Rigidbody2D cacheRigidbody2D;

  public Rigidbody2D CacheRigidbody2D {
    get { return cacheRigidbody2D; }
  }

  //чтобы плавно двигать камеру вслед за персонажем, будем хранить позицию сдвига
  private Vector2 shiftByVector;

  public Vector2 ShiftByVector {
    get { return shiftByVector; }
  }

  //произошло ли обновление "физики" (поменял ли персонаж свое положение)
  public bool isUpdate;

  public bool IsUpdate {
    get { return isUpdate; }
    set { isUpdate = value; }
  }

  //события связанные с изменением здоровья у игрока
  public delegate void OnEventHandler();

  public event OnEventHandler UpdateHealthEvent = delegate { };
  public event OnEventHandler HealthEndedEvent = delegate { };

  //экземпляр класса с общими настройками (размер/скорость/здоровье)
  private CommonSettingObject settingObject;

  public CommonSettingObject SettingObject {
    get { return settingObject; }
  }


  private float widthMaxPosition;
  private float heightMaxPosition;


  //неузъявим ли сейчас персонаж (запущена ли сопрограмма)
  //при потере жизни он начинает мигать и становиться неуязвим на две секунды
  private bool isInvulnerable = false;

  // Use this for initialization
  private void Awake() {
    settingObject = new CommonSettingObject {
      Healht = 3,
      Speed = EnvironmentSingleton.Instance.Width*0.1f,
      Size = EnvironmentSingleton.Instance.Width*0.05f
    };
    //чтобы не вызывать каждый раз getComponent при обращении
    cacheRigidbody2D = GetComponent<Rigidbody2D>();
    shiftByVector = cacheRigidbody2D.position;
    //устанавливаем размер, и начальное положение
    //используем только 1 раз, поэтому можно не кэшировать
    GetComponent<SpriteRenderer>().size = new Vector2(settingObject.Size, settingObject.Size);
    cacheRigidbody2D.MovePosition(new Vector2(0, 0));
    GetComponent<BoxCollider2D>().size = new Vector2(settingObject.Size*0.95f, settingObject.Size*0.95f);
    //установим, что при спавне объект смотрит направо
    shiftByVector = Vector2.right*settingObject.Speed*Time.fixedDeltaTime;
    widthMaxPosition = EnvironmentSingleton.Instance.WidthMax;
    heightMaxPosition = EnvironmentSingleton.Instance.HeightMax;
  }

  // Update is called once per frame
  void FixedUpdate() {
    Move();
  }

  private void Move() {
    if (joystick.InputDirectionVector2 == joystick.ZeroInputVector2) return;
    float speed = settingObject.Speed;
    Vector2 vec = joystick.InputDirectionVector2.normalized;
    shiftByVector = Vector2.ClampMagnitude(new Vector2(vec.x*speed, vec.y*speed), speed)*Time.deltaTime;
    Vector2 curPosition = cacheRigidbody2D.position;
    float partSize = settingObject.Size/2;
    if (shiftByVector.x + curPosition.x - partSize < -widthMaxPosition ||
        shiftByVector.x + curPosition.x + partSize > widthMaxPosition) {
      shiftByVector.x = 0;
    }
    if (shiftByVector.y + curPosition.y - partSize < -heightMaxPosition ||
        shiftByVector.y + curPosition.y + partSize > heightMaxPosition) {
      shiftByVector.y = 0;
    }
    //есть уперлись в угол
    if (joystick.ZeroInputVector2 == shiftByVector) return;
    isUpdate = true;
    double cos = (shiftByVector.x)/(Math.Sqrt(shiftByVector.x*shiftByVector.x + shiftByVector.y*shiftByVector.y));
    float angle = (float) Math.Acos(cos)*57.2958f;
    if (shiftByVector.y < 0) {
      angle = -angle;
    }
    cacheRigidbody2D.rotation = angle;
    cacheRigidbody2D.MovePosition(cacheRigidbody2D.position + shiftByVector);
  }


  private void OnCollisionEnter2D(Collision2D other) {
    if (other.gameObject.CompareTag("Enemy") && !other.gameObject.GetComponent<Enemy>().IsShell) {
      cacheRigidbody2D.velocity = new Vector2(0, 0);
      cacheRigidbody2D.angularVelocity = 0;
      if (!isInvulnerable) {
        //теряем жизнь и мигаем если остались живы
        bool islive = UpdateHealth(1);
        if (islive) {
          StartCoroutine(BlinkingEffect());
        }
      }
    }
  }

  private IEnumerator BlinkingEffect() {
    isInvulnerable = true;
    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
    Color maxAlfa = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a);
    Color minAlfa = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a/10);
    float time = 0;
    bool max = true;
    while (time < 2.0f) {
      time += 0.1f;
      if (max) {
        sprite.color = minAlfa;
        max = false;
      }
      else {
        sprite.color = maxAlfa;
        max = true;
      }
      yield return new WaitForSeconds(0.1f);
    }
    if (!max) {
      sprite.color = maxAlfa;
    }
    isInvulnerable = false;
  }

  private void OnCollisionExit2D(Collision2D other) {
    cacheRigidbody2D.velocity = new Vector2(0, 0);
    cacheRigidbody2D.angularVelocity = 0;
  }

  private bool UpdateHealth(int i) {
    settingObject.Healht -= i;
    UpdateHealthEvent();
    if (settingObject.Healht == 0) {
      HealthEndedEvent();
      return false;
    }
    return true;
  }
}