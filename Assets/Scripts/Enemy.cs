using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		Debug.Log("TRIGGER enter ENEMY");
		if (col.CompareTag("Arrow")){
			Debug.Log("I've been hit!!!!!");
		}
	}

}
