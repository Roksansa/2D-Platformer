using UnityEngine;


public class EnemyPool :MonoBehaviour{
  private int size;

  public int Size {
    get { return size; }
  }
  private Enemy[] enemyArray;
  [SerializeField] private GameObject enemy;
  [SerializeField] private Transform parentEnemyPool;
  private int incCapacity;
  //хранит общие настройки, объекты пула имеют лишь ссылки на этот объект
  private CommonSettingObject settingObject;
  //максимальный размер пула
  private int maxSize;


  
  //события генерируемые при смерти врага от пули для оповещения UIконтроллера
  public delegate void OnEventHandler();
  public event OnEventHandler DieEvent = delegate {  };
  
  public void Create() {
    for (int i = 0; i < enemyArray.Length; i++) {
      if (!enemyArray[i].IsUse) {
        enemyArray[i].Create();
        return;
      }
    }
    Enemy curEnemy = AddEmenyInPool();
    if (curEnemy != null) {
      curEnemy.Create();
    }
  }

  private void OnDestroy() {
    for (int i = 0; i < enemyArray.Length; i++) {
      enemyArray[i].DieEvent -= OnDeadEnemy;
    }
  }

  public void EnemyPoolCreate(int size1, int size2) {
    settingObject = new CommonSettingObject {
      Healht = 1,
      Speed = EnvironmentSingleton.Instance.Width*0.1f,
      Size = EnvironmentSingleton.Instance.Width*0.05f
    };

    size = size1;
    maxSize = size2;
    incCapacity = size1;
    size = size < maxSize ? size : maxSize;
    enemyArray = new Enemy[size];
    GameObject gameObjectNew;
    for (int i = 0; i < enemyArray.Length; i++) {
      //создаем объект и записываем в пул только привязанный компонент со скриптом(ведь всегда можно получить объект)
      gameObjectNew = Instantiate(enemy);
      gameObjectNew.transform.SetParent(parentEnemyPool);
      enemyArray[i] = gameObjectNew.GetComponent<Enemy>();
      enemyArray[i].SettingObject = settingObject;
      //подписываемся на событие
      enemyArray[i].DieEvent += OnDeadEnemy;
    }
  }

  private Enemy AddEmenyInPool () {
    if (enemyArray.Length + incCapacity > maxSize) return null;
    Enemy[] curEnemyArray = new Enemy[enemyArray.Length+incCapacity];
    int i;
    for (i = 0; i < enemyArray.Length; i++) {
      curEnemyArray[i] = enemyArray[i];
    }
    GameObject gameObjectNew;
    for (i = enemyArray.Length; i < curEnemyArray.Length; i++) {
      gameObjectNew = Instantiate(enemy);
      gameObjectNew.transform.SetParent(parentEnemyPool);
      curEnemyArray[i] = gameObjectNew.GetComponent<Enemy>();
      curEnemyArray[i].SettingObject = settingObject;
      curEnemyArray[i].DieEvent += OnDeadEnemy;
    }
    i = enemyArray.Length;
    enemyArray = curEnemyArray;
    return enemyArray[i];
  }

  public void OnDeadEnemy() {
    //посылаем событие uicontroller
    Controller.KilledEnemies++;
    DieEvent();
  }
}
