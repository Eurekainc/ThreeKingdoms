using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

	public King kingScript;

	public Platform platformScript;

	public int peasantsToSpawn = 3;
	public GameObject peasantPrefab;

	public float timeToRequestFood = 20.0f;
	private float elapsedTime = 0.0f;

	// Use this for initialization
	void Start () {
		platformScript = GetComponent<Platform>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (peasantsToSpawn > 0 && platformScript.level > 0) {
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= timeToRequestFood) {
				RequestFood();
			}
		}
	}

	public void Activate ()
	{
		switch (platformScript.level){
			case 0:
				kingScript.houses++;
				platformScript.level++;
				// reset house cost so that builder brings necessary resources
				platformScript.cost[0] = peasantsToSpawn;// need 3 food to create 3 peasants
				platformScript.cost[1] = 0;
				platformScript.cost[2] = 0;
				platformScript.cost[3] = 0;
				break;
			case 1:
				Debug.Log("Next Upgrade -> Castle");
				CreatePeasants ();
				platformScript.level++;
				platformScript.cost[0] = 5;// need 3 food to create 3 peasants
				platformScript.cost[1] = 5;
				platformScript.cost[2] = 15;
				platformScript.cost[3] = 2;
				break;
			case 2:
				Debug.Log("Activate castle model");
				break;
			default:
				Debug.Log("Fall through House.cs in Activate");
				break;
		}
	}

	public void CreatePeasants ()
	{
		for (int i = 0; i < peasantsToSpawn; i++) {
			GameObject peasant = (GameObject)Instantiate (peasantPrefab, transform.position, Quaternion.identity);
			kingScript.npcs.Add (peasant);
			kingScript.npcScripts.Add (peasant.GetComponent<NPC> ());
			kingScript.UpdatePeasantCount ();
			peasantsToSpawn = 0;
		}
	}

	void RequestFood(){
		Debug.Log("REQUEST FOOD");
		kingScript.FeedHouse(platformScript);
	}



}
