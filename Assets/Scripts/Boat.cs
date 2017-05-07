using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

	public int hp = 5;
	private float elapsedTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (hp <= 0) {
//			Sink();
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler (0, 0, 20), elapsedTime);
			transform.position = new Vector3(transform.position.x, transform.position.y-0.1f, transform.position.z);
			elapsedTime += Time.deltaTime;
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag("Arrow")){
			Debug.Log("I've been hit!!!!!");
			hp--;
		}
	}

//	void Sink(){
//		transform.rotation = Quaternion.Slerp(Quaternion.Euler (0, 0, 0), Quaternion.Euler (0, 0, -80), 1.0f);
//	}

}
