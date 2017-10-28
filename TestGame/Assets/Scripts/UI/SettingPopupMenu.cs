using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPopupMenu : MonoBehaviour{
  private void Awake() {
    gameObject.SetActive(false);
  }

  public void NewGameClick() {
    UIController.PauseOff();
    SceneManager.LoadScene("Scene");
  }

  public void ExitMainMenuClick() {
    UIController.PauseOff();
    SceneManager.LoadScene("MainScene");
  }

  public void ContinueClick() {
    UIController.PauseOff();
    gameObject.SetActive(false);
  }
}