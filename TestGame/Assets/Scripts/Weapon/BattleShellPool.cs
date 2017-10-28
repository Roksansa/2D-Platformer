using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BattleShellPool : MonoBehaviour {
  private int size;

  public int Size {
    get { return size; }
  }
  private BattleShell[] battleShellArray;
  [SerializeField] private GameObject battleShell;
  [SerializeField] private Transform parentBattleShell;

  public void Fire() {
    for (int i = 0; i < battleShellArray.Length; i++) {
      if (!battleShellArray[i].IsUse) {
        battleShellArray[i].Fire();
        return;
      }
    }
  }

  //необходимо выделить всего 26 пуль, которых будет достаточно для игры
  public void BattleShellCreate(int size1=26) {
    size = size1;
    battleShellArray = new BattleShell[size];
    GameObject gameObjectNew;
    for (int i = 0; i < battleShellArray.Length; i++) {
      //создаем объект и записываем в пул только привязанный компонент со скриптом(ведь всегда можно получить объект)
      gameObjectNew = Instantiate(battleShell);
      gameObjectNew.transform.SetParent(parentBattleShell);
      battleShellArray[i] = gameObjectNew.GetComponent<BattleShell>();
    }
  }
}
