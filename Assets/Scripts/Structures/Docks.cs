using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Docks : MonoBehaviour {

	public King kingScript = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

//	void OnTriggerEnter2D (Collider2D col)
//	{
//		if (col.CompareTag ("Player")) {
//			Player playerScript = col.gameObject.GetComponent<Player>();
//			playerScript.atDocks = true;
//			playerScript.currentKingScript = kingScript;
//		}
//	}
//
//	void OnTriggerExit2D (Collider2D col)
//	{
//		if (col.CompareTag ("Player")) {
//			Player playerScript = col.gameObject.GetComponent<Player>();
//			playerScript.atDocks = false;
//			playerScript.currentKingScript = null;
//		}
//	}
}
