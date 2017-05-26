using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Channel : MonoBehaviour {

	public float workTime = 2.0f;
	public int cost = 3;
	public float heightIncrease = 0.3f;

	private FallingObject fallingScript;
	public Ground groundPlatform;
	private GroundStructure groundStructureScript;

	void Start(){
		fallingScript = GetComponent<FallingObject>();
		groundStructureScript = GetComponent<GroundStructure>();
	}

	public void Landed ()
	{
		Debug.Log ("... landed remove struts from build list");

		// first check if this platform has struts
		if (fallingScript.groundPlatform.struts.Count > 0) {
			// channel stops the building of all other structures on this platform and focuses all attention on to the platform
			fallingScript.groundPlatform.structures.Clear ();
			fallingScript.groundPlatform.structures.Add (groundStructureScript);
			fallingScript.groundPlatform.ResetStructureBuilding ();
		}
	}

	public void Build (WorkerNPC npc)
	{
		Debug.Log("Building the channel");
		StartCoroutine(BuildChannel(npc));
	}

	IEnumerator BuildChannel(WorkerNPC npc){
		yield return new WaitForSeconds(workTime);
		Debug.Log("Raise the channel");
		transform.position = new Vector3 (fallingScript.groundPlatform.struts[0].transform.position.x, transform.position.y + heightIncrease, transform.position.z);
		Build(npc);
	}

}
