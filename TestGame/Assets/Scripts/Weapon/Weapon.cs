using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Weapon : MonoBehaviour{
  [SerializeField] private FireButton fireButton;

  //стреляем ли мы сейчас (запущена ли сопрограмма)
  private bool isFire = false;

  private void Awake() {
    fireButton.FireButtonEvent += FireOn;
    fireButton.FireEndButtonEvent += FireOff;
  }

  private void OnDestroy() {
    fireButton.FireButtonEvent -= FireOn;
    fireButton.FireEndButtonEvent -= FireOff;
  }

  private void FireOn() {
    if (fireButton.IsPress) {
      return;
    }
    fireButton.IsPress = true;
    if (!isFire) {
      Debug.Log("I TUUUUUT!!!");
      StartCoroutine(Fire());
    }
  }

  private void FireOff() {
    fireButton.IsPress = false;
  }

  private IEnumerator Fire() {
    isFire = true;
    while (fireButton.IsPress) {
      Controller.BattleShellPool.Fire();
      yield return new WaitForSeconds(0.5f);
    }
    isFire = false;
  }
}