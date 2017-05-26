using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strut : MonoBehaviour {

	private GroundStructure groundStructureScript;

	public float workTime = 2.0f;
	public int cost = 3;

	public float heightIncrease = 0.01f;

	void Start ()
	{
		groundStructureScript = GetComponent<GroundStructure> ();
	}

	public void Build (WorkerNPC npc)
	{
		Debug.Log("Building the strut");
		StartCoroutine(BuildStrut(npc));
	}

	IEnumerator BuildStrut(WorkerNPC npc){
		yield return new WaitForSeconds(workTime);
		Debug.Log("Built the strut");
		transform.position = new Vector3 (transform.position.x, transform.position.y + heightIncrease, transform.position.z);
		if (!groundStructureScript.groundPlatform.struts.Contains(this)) {
			groundStructureScript.groundPlatform.struts.Add(this);
		}
		npc.hasResource = false;
		npc.FetchResource();
	}
}
