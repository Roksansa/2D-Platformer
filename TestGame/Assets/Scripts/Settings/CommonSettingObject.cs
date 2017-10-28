using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonSettingObject{
	protected float size;

	protected float speed;

	protected int healht;

	public float Size {
		get { return size; }
		set { size = value; }
	}

	public float Speed {
		get { return speed; }
		set { speed = value; }
	}

	public int Healht {
		get { return healht; }
		set { healht = value;
			if (healht < 0) {
				healht = 0;
			}
		}
	}
	

}
