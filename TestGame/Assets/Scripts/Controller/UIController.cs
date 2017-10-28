using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIController : MonoBehaviour{
  //за чьим здоровьем следим
  private Player curPlayer;

  private EnemyPool enemyPool;
  [SerializeField] private HUDHealth hudHealth;

  [SerializeField] private GameOverText gameOverText;

  [SerializeField] private HUDScore hudScore;

  [SerializeField] private SettingPopupMenu settingPopupMenu;
  // Use this for initialization


  void Start() {
    curPlayer = Controller.GetCurPlayer();
    if (curPlayer != null) {
      curPlayer.UpdateHealthEvent += UpdateHealth;
      curPlayer.HealthEndedEvent += ShowText;
      hudHealth.GetComponent<Image>().sprite = hudHealth.HealthTexture2D[curPlayer.SettingObject.Healht];
    }
    enemyPool = GameObject.FindGameObjectWithTag("Controller").GetComponent<EnemyPool>();
    if (enemyPool != null) {
      enemyPool.DieEvent += UpdateScore;
    }
  }


  private void OnDestroy() {
    if (curPlayer != null) {
      curPlayer.UpdateHealthEvent -= UpdateHealth;
      curPlayer.HealthEndedEvent -= ShowText;
    }
    if (enemyPool != null) {
      enemyPool.DieEvent -= UpdateScore;
    }
  }

  private void UpdateHealth() {
    //ставить защиту от дурака на все подряд нет смысла
    hudHealth.GetComponent<Image>().sprite = hudHealth.HealthTexture2D[curPlayer.SettingObject.Healht];
  }

  private void ShowText() {
    StartCoroutine(ShowTextEffect());
  }

  private IEnumerator ShowTextEffect() {
    gameOverText.gameObject.SetActive(true);
    //так как единоразово используется, то выносить в общий класс затратнее
    PauseOn();
    yield return new WaitForSecondsRealtime(3.0f);
    //выходит в главное меню
    PauseOff();
    SceneManager.LoadScene("MainScene");
  }

  private void UpdateScore() {
    hudScore.UpdateScore();
  }

  public void OpenSettingMenuClick() {
    PauseOn();
    settingPopupMenu.gameObject.SetActive(true);
  }

  public static void PauseOn() {
    Time.timeScale = 0;
  }

  public static void PauseOff() {
    Time.timeScale = 1;
  }
}