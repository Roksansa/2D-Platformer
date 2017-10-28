using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDScore : MonoBehaviour{

	private string score = "Score: ";
	private Text uiText;

	private void Awake() {
		uiText = GetComponent<Text>();
		uiText.text = string.Concat(score, Controller.KilledEnemies.ToString());
	}

	public void UpdateScore() {
		uiText.text = string.Concat(score,  Controller.KilledEnemies.ToString());
	}
}
