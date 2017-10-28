using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class HUDHealth : MonoBehaviour{
	private Sprite[] healthTexture2D;

	public Sprite[] HealthTexture2D {
		get { return healthTexture2D; }
		set { healthTexture2D = value; }
	}

	private void Awake() {
		healthTexture2D = Resources.LoadAll<Sprite>("HeartSprite");
	}
}
