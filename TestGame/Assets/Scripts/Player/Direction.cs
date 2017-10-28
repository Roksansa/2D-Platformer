using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour {
	private void Awake() {
		GetComponent<SpriteRenderer>().size = new Vector2(EnvironmentSingleton.Instance.WidthMax*0.03f, EnvironmentSingleton.Instance.WidthMax*0.03f);
		Vector3 vec = GetComponent<Transform>().localPosition;
		GetComponent<Transform>().localPosition = new Vector3(EnvironmentSingleton.Instance.WidthMax*0.03f, vec.y,vec.z);
	}
}
