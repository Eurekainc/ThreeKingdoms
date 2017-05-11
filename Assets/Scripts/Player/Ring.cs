using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {

	private NPC hitNPC = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("NPC")) {
			Debug.Log ("HIT NPC.....");
			if (hitNPC == null) {
				hitNPC = col.gameObject.GetComponent<NPC>();
				hitNPC.GoToFerry();
			}
		}
	}

}
