using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

	public King kingScript;

	public Platform platformScript;

	public int peasantsToSpawn = 3;
	public GameObject peasantPrefab;

	// Use this for initialization
	void Start () {
		platformScript = GetComponent<Platform>();
		kingScript = platformScript.kingScript;
	}

	public void Activate ()
	{
		switch (platformScript.level){
			case 0:
				kingScript.houses++;
				platformScript.level++;
				// cost to upgrade to keep
				platformScript.cost[0] = 10;// need 3 food to create 3 peasants
				platformScript.cost[1] = 15;
				platformScript.cost[2] = 5;
				platformScript.cost[3] = 0;

				// simplification once a house is built it just spawns peasants
				CreatePeasants ();

				// add the house to the "Houses Under Construction list"
//				kingScript.housingPlatformsUnderConstruction.Add(platformScript);// builders should now prioritize this as if it were a house to be built

				break;
			case 1:
				Debug.Log("Next Upgrade -> Castle");
				CreatePeasants ();
				platformScript.level++;
			// cost to upgrade to Castle
				platformScript.cost[0] = 20;
				platformScript.cost[1] = 15;
				platformScript.cost[2] = 30;
				platformScript.cost[3] = 5;
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
			NPC peasantScript = peasant.GetComponent<NPC> ();
			peasantScript.kingScript = kingScript;
			peasantScript.occupation = i;// 0 = woodcutter, 1 = quarryman, 2 = farmer
			kingScript.npcScripts.Add (peasantScript);
//			peasantScript.Init();
		}
		StartCoroutine(RecruitPeasantsInSystem());// put this in a coroutine to give the peasants a chance to run Awake() and Start(), before being accessed by the Kingscript
		peasantsToSpawn = 0;
	}

	IEnumerator RecruitPeasantsInSystem(){
		yield return new WaitForSeconds(0.1f);
		kingScript.UpdatePeasantCount ();// this also updates the task list for all NPCs in King.cs
	}



}
