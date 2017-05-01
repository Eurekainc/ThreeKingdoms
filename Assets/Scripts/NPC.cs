using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour {

	public King kingScript;

	private NavMeshAgent agent;
	public bool active = false;

	// countdown until unit reverts back to a peasant
	public float timeToDegrade = 50.0f;// say 5 days
	private float degradeTime = 0.0f;

	// occupations
	// 0-> peasant
	// 1 -> builder
	// 2-> archer
	public int occupation = 0;

	// Moving between platforms state
	private Platform destinationPlatformScript = null;
	public bool movingToPlatform = false;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (occupation > 0) {
			if (!active) {
				degradeTime += Time.deltaTime;
			} else {
				degradeTime = 0.0f;
			}

			if (degradeTime >= timeToDegrade) {
				occupation = 0;
				Debug.Log ("Revert back to a peasant");
				kingScript.UpdatePeasantCount ();
			}

		}

		// TODO: change this to fire CheckTask on trigger enter or something...
		if (movingToPlatform) {
			if (!agent.pathPending)
			 {
				if (agent.remainingDistance <= agent.stoppingDistance)
			     {
					if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
			         {
			             Debug.Log("ARRIVED>>>>>");
			             movingToPlatform = false;
			             CheckTask();
			         }
			     }
			 }
		}

	}

	public void GoToPlatform (Transform platform, Platform platformScript)
	{
		active = true;
		agent.SetDestination (platform.position);

		Debug.Log("agent.pathPending: " + agent.pathPending);
		Debug.Log("agent.remainingDistance: " + agent.remainingDistance);

		movingToPlatform = true;
		destinationPlatformScript = platformScript;
	}

	void CheckTask(){
		if (destinationPlatformScript.farming) {
			Debug.Log ("Start farming");
			StartCoroutine(Farm());
		} else if (destinationPlatformScript.logging) {
			Debug.Log ("Start logging");
		} else if (destinationPlatformScript.quarry) {
			Debug.Log ("Start quarrying");
		} else if (destinationPlatformScript.mine) {
			Debug.Log ("Start mining");
		} else if (destinationPlatformScript.housing) {
			Debug.Log ("Build house");
		} else if (destinationPlatformScript.workshop) {
			Debug.Log ("Train to become a builder");
		} else if (destinationPlatformScript.archery) {
			Debug.Log ("Train to become an archer");
		}
		else if (destinationPlatformScript.docks) {
			// TODO: divide the docks up into different regions so that NPC drops off package at the correct spot for player to pickup
			Debug.Log ("Arrived at docks, offload package.....");
			StartCoroutine(ArriveAtDocks());
		}
	}


	// Tasks
	IEnumerator Farm(){
		yield return new WaitForSeconds(kingScript.farmTime);
		Debug.Log("Farming Completed.... go to docks");
		GoToPlatform (kingScript.docks.transform, kingScript.docks);
	}

	IEnumerator ArriveAtDocks(){
		yield return new WaitForSeconds(kingScript.offloadTime);
		Debug.Log("Finished offloading at docks... become inactive");
		active = false;
	}

}
