﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Docks : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			Debug.Log("Player entered docks....");
		}
	}
}