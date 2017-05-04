using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archery : MonoBehaviour {

	public King kingScript;

	public Platform platformScript;

	public int archersToSpawn = 3;
	public GameObject npcPrefab;

	// Use this for initialization
	void Start () {
		platformScript = GetComponent<Platform>();
	}

	public void Activate ()
	{
		switch (platformScript.level){
			case 0:
				kingScript.workshops++;
				platformScript.level++;
				// cost to upgrade to keep
				platformScript.cost[0] = 10;// need 3 food to create 3 peasants
				platformScript.cost[1] = 15;
				platformScript.cost[2] = 5;
				platformScript.cost[3] = 0;

				// simplification once a house is built it just spawns peasants
				CreateArchers ();

				break;
			default:
				Debug.Log("Fall through House.cs in Activate");
				break;
		}
	}

	public void CreateArchers ()
	{
		for (int i = 0; i < archersToSpawn; i++) {
			GameObject newNPC = (GameObject)Instantiate (npcPrefab, transform.position, Quaternion.identity);
			kingScript.npcs.Add (newNPC);
			NPC newNPCScript = newNPC.GetComponent<NPC> ();
			newNPCScript.kingScript = kingScript;
			newNPCScript.occupation = 4;// 0 = woodcutter, 1 = quarryman, 2 = farmer, 3 = builder, 4 = archer, 5 = King
			kingScript.npcScripts.Add (newNPCScript);
		}
		StartCoroutine(RecruitPeasantsInSystem());// put this in a coroutine to give the peasants a chance to run Awake() and Start(), before being accessed by the Kingscript
		archersToSpawn = 0;
	}

	IEnumerator RecruitPeasantsInSystem(){
		yield return new WaitForSeconds(0.1f);
		kingScript.UpdatePeasantCount ();// this also updates the task list for all NPCs in King.cs
	}
}
