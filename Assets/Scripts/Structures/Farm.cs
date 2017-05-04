using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour {

	public King kingScript;

	public Platform platformScript;

	// Use this for initialization
	void Start () {
		platformScript = GetComponent<Platform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Activate ()
	{
		switch (platformScript.level){
			case 0:
				kingScript.farms++;
				platformScript.level++;
				break;
			default:
				Debug.Log("Fall through Farm.cs in Activate");
				break;
		}
	}

}
