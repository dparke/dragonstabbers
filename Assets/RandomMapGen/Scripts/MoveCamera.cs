using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

	public float speed = 4f;
	public GameObject target;

	void FixedUpdate(){
		if (target != null)
    {
			var pos = target.transform.position;
			pos.z = Camera.main.transform.position.z;
			Camera.main.transform.position = pos;
		}
	}
}