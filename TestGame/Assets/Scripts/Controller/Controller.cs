using System.Collections;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(EnemyPool))]
[RequireComponent(typeof(BattleShellPool))]
public sealed class Controller : MonoBehaviour{
  private EnemyPool EnemyPool;
  public static BattleShellPool BattleShellPool;
  public static Random random;
  public static int KilledEnemies;
  private static Player CurPlayer;

  private bool work;
  // Use this for initialization
  private void Awake() {
    random = new Random();
    KilledEnemies = 0;
    EnemyPool = GetComponent<EnemyPool>();
    BattleShellPool = GetComponent<BattleShellPool>();
    work = true;
  }

  void Start() {
    EnemyPool.EnemyPoolCreate(5, 100);
    StartCoroutine(SpawnEnemy());
    BattleShellPool.BattleShellCreate();
  }

  private IEnumerator SpawnEnemy() {
    //пока идет игра
    while (work) {
      EnemyPool.Create();
      yield return new WaitForSeconds(3f);
    }
  }

   public static Player GetCurPlayer() {
     if (CurPlayer == null) {
       CurPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
     }
     return CurPlayer;
   }
}