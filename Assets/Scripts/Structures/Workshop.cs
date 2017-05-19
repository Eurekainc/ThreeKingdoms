using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workshop : MonoBehaviour {

	public King kingScript;

	public Platform platformScript;

	public int buildersToSpawn = 1;
	public GameObject npcPrefab;

	// Use this for initialization
	void Start () {
		platformScript = GetComponent<Platform>();
	}

	public void Activate ()
	{

		kingScript.houses++;
		platformScript.level++;
		CreateBuilders ();

//		switch (platformScript.level){
//			case 0:
//				kingScript.workshops++;
//				platformScript.level++;
//				// cost to upgrade to keep
//				platformScript.cost[0] = 10;// need 3 food to create 3 peasants
//				platformScript.cost[1] = 15;
//				platformScript.cost[2] = 5;
//				platformScript.cost[3] = 0;
//
//				// simplification once a house is built it just spawns peasants
//				CreateBuilders ();
//
//				break;
//			default:
//				Debug.Log("Fall through House.cs in Activate");
//				break;
//		}
	}

	public void CreateBuilders ()
	{
		for (int i = 0; i < buildersToSpawn; i++) {
			GameObject newNPC = (GameObject)Instantiate (npcPrefab, transform.position, Quaternion.identity);
			kingScript.npcs.Add (newNPC);
			NPC newNPCScript = newNPC.GetComponent<NPC> ();
			newNPCScript.kingScript = kingScript;
			newNPCScript.occupation = 3;// 0 = woodcutter, 1 = quarryman, 2 = farmer, 3 = builder
			kingScript.npcScripts.Add (newNPCScript);
			platformScript.residentNPCs.Add(newNPCScript);
		}
		StartCoroutine(RecruitPeasantsInSystem());// put this in a coroutine to give the peasants a chance to run Awake() and Start(), before being accessed by the Kingscript
		buildersToSpawn = 0;
	}

	IEnumerator RecruitPeasantsInSystem(){
		yield return new WaitForSeconds(0.1f);
		kingScript.UpdatePeasantCount ();// this also updates the task list for all NPCs in King.cs
	}
}
