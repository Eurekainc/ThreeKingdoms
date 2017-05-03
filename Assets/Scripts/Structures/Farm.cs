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
				// reset farm cost so that builder brings necessary resources---> upgrade
//				platformScript.cost[0] = 0;// need 3 food to create 3 peasants
//				platformScript.cost[1] = 0;
//				platformScript.cost[2] = 0;
//				platformScript.cost[3] = 0;
				break;
			default:
				Debug.Log("Fall through Farm.cs in Activate");
				break;
		}
	}

}
