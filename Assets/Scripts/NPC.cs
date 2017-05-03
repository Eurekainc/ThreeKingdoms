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
	// 2 -> archer
	// 3 -> King
	public int occupation = 0;

	// Moving between platforms state
	public Platform destinationPlatformScript = null;// can be private
	public bool movingToPlatform = false;

	// Carrying resources
	public bool carryingResource = false;
	public int carriedResourceIndex;

	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
//		if (kingScript != null) {
//			Init();
//		}
	}

	public void FindTask(){
		if (occupation == 0) {
			FindTaskPeasant ();
		} else if (occupation == 1) {
			FindTaskBuilder ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// degrading occupation.... might not use this
//		if (occupation > 0) {
//			if (!active) {
//				degradeTime += Time.deltaTime;
//			} else {
//				degradeTime = 0.0f;
//			}
//
//			if (degradeTime >= timeToDegrade) {
//				occupation = 0;
//				Debug.Log ("Revert back to a peasant");
//				kingScript.UpdatePeasantCount ();
//			}
//
//		}

		// TODO: change this to fire CheckTask on trigger enter or something...
		if (movingToPlatform) {
			if (!agent.pathPending) {
				if (agent.remainingDistance <= agent.stoppingDistance) {
					if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
						Debug.Log ("ARRIVED>>>>>");
			             movingToPlatform = false;
			             CheckTask();
			         }
			     }
			 }
		}

	}


	void FindTaskPeasant ()
	{

		// only do this if not carrying a resource... i.e. if either en route to a task or not doing anything... otherwise might drop important resource
		if (!carryingResource) {
			if (kingScript.peasantTasks.Count > 0) {
				Platform taskPlatform = kingScript.peasantTasks [0];
				kingScript.peasantTasks.RemoveAt (0);// take the task and remove it from the list
				active = true;
				destinationPlatformScript = taskPlatform;
				GoToWorkPlatformPeasant ();
			} else {
				Debug.Log("Idle about... maybe become a builder or archer???");
			}
		}

	}

	void FindTaskBuilder ()
	{

		// only take action if not carrying a resource
		if (!carryingResource) {
			if (kingScript.builderTasks.Count > 0) {
				Platform taskPlatform = kingScript.builderTasks [0];
				active = true;
				destinationPlatformScript = taskPlatform;
				if (kingScript.platformBuilder.ContainsKey (destinationPlatformScript)) {
					kingScript.platformBuilder [destinationPlatformScript]++;
				} else {
					kingScript.platformBuilder.Add (destinationPlatformScript, 1);
				}
				GoToDocksBuilder ();
			}
		}

	}










	void GoToWorkPlatformPeasant()
	{
		agent.SetDestination (destinationPlatformScript.transform.position);
		movingToPlatform = true;
	}
	void GoToDocksPeasant()
	{
		agent.SetDestination (kingScript.docks.transform.position);
		movingToPlatform = true;
	}

	void GoToWorkPlatformBuilder()
	{
		if (occupation == 1) {
			Debug.Log("Going to platform..... " + destinationPlatformScript.gameObject.name);
		}
		agent.SetDestination (destinationPlatformScript.transform.position);
		movingToPlatform = true;
	}
	void GoToDocksBuilder()
	{
		agent.SetDestination (kingScript.docks.transform.position);
		movingToPlatform = true;
	}



	public void GoToPlatform (Transform platform, Platform platformScript)
	{
		if (occupation == 1) {
			Debug.Log("Going to platform..... " + platformScript.gameObject.name);
		}
		active = true;
		agent.SetDestination (platform.position);

		movingToPlatform = true;
		destinationPlatformScript = platformScript;
	}

	public void SendBuilderToDocks(Transform platform, Platform platformScript){
		active = true;
		// builder always goes to docks to fetch resource
		agent.SetDestination (kingScript.docks.transform.position);
		movingToPlatform = true;
		destinationPlatformScript = platformScript;
	}


	void CheckTask ()
	{
//		Debug.Log("Check task..... " + kingScript.days);
		// if its a peasant
		if (occupation == 0) {
			// if a peasant isn't carrying a resource then they need to go to work to produce resources
			if (!carryingResource) {
				if (destinationPlatformScript.farming) {
					Debug.Log ("Start farming");
					StartCoroutine (Farm ());
				} else if (destinationPlatformScript.logging) {
					Debug.Log ("Start logging");
					StartCoroutine (GatherWood ());
				} else if (destinationPlatformScript.quarry) {
					Debug.Log ("Start quarrying");
					StartCoroutine (GatherStone ());
				} else if (destinationPlatformScript.mine) {
					Debug.Log ("Start mining");
				} else if (destinationPlatformScript.workshop) {
					Debug.Log ("Train to become a builder");
				} else if (destinationPlatformScript.archery) {
					Debug.Log ("Train to become an archer");
				}
			} 
			// if peasant is carrying a resource then they're offloading it at the docks...
			else {
				// TODO: divide the docks up into different regions so that NPC drops off package at the correct spot for player to pickup
				Debug.Log ("Arrived at docks, offload package.....");
				StartCoroutine (OffloadAtDocks ());
			}
		}

		// if its a builder (this is called once arriving at docks or build site depending on whether they're carrying a resource
		else if (occupation == 1) {
			// if a builder isn't carrying a resource then they will pick up appropriate resource from docks, then go to their build task

			if (!carryingResource) {
				
				// join the queue to get a resource
				if (!kingScript.resourceQueue.Contains (this)) {
					kingScript.resourceQueue.Add (this);
				}
				kingScript.CheckResourceArrived ();// activate builder at front of queue to check and see if his resource has arrived, if not then go to the back of the queue

			}
			// builder is carrying resource so they're here to build
			// or possibly they were trying to build another structure which was completed before builder arrived with resource.... in that case return teh resource
			else {
				if (destinationPlatformScript.farming) {
					Debug.Log ("Build a farm");
					StartCoroutine (BuildSectionOfStructure ());
				} else if (destinationPlatformScript.quarry) {
					Debug.Log ("Build quarry");
					StartCoroutine (BuildSectionOfStructure ());
				} else if (destinationPlatformScript.mine) {
					Debug.Log ("Build mine");
					StartCoroutine (BuildSectionOfStructure ());
				} else if (destinationPlatformScript.housing) {
					Debug.Log ("Build or feed house");
					StartCoroutine (BuildSectionOfStructure ());
				} else if (destinationPlatformScript.workshop) {
					Debug.Log ("Train to become a builder");
					StartCoroutine (BuildSectionOfStructure ());
				} else if (destinationPlatformScript.archery) {
					Debug.Log ("Train to become an archer");
					StartCoroutine (BuildSectionOfStructure ());
				}
			}
		}
	}


	// Tasks
	// TODO: inactivate peasant work platform area if the work area is damaged by attacks or is depleted, then set to inactive and find a new task
	IEnumerator Farm(){
		yield return new WaitForSeconds(kingScript.farmTime);
		Debug.Log("Farming Completed.... go to docks");
		carriedResourceIndex = 0;
		carryingResource = true;
		GoToDocksPeasant();
	}
	IEnumerator GatherWood(){
		yield return new WaitForSeconds(kingScript.loggingTime);
		Debug.Log("Logging Completed.... go to docks");
		carriedResourceIndex = 1;
		carryingResource = true;
		GoToDocksPeasant();
	}
	IEnumerator GatherStone(){
		yield return new WaitForSeconds(kingScript.quarryingTime);
		Debug.Log("Quarrying Completed.... go to docks");
		carriedResourceIndex = 2;
		carryingResource = true;
		GoToDocksPeasant();
	}
	IEnumerator Mining(){
		yield return new WaitForSeconds(kingScript.miningTime);
		Debug.Log("Mining Completed.... go to docks");
		carriedResourceIndex = 3;
		carryingResource = true;
		GoToDocksPeasant();
	}

	IEnumerator OffloadAtDocks(){
		yield return new WaitForSeconds(kingScript.offloadTime);
		Debug.Log("Finished offloading at docks... become inactive");
		kingScript.availableResources[carriedResourceIndex]++;
		carryingResource = false;

		kingScript.CheckResourceArrived ();// activate builder at front of queue to check and see if his resource has arrived, if not then go to the back of the queue
//		GoToWorkPlatformPeasant();

		kingScript.SetTaskList ();// get King script to reassess the situation and assign new tasks, kingScript calls FindTask on all NPCs
//		FindTaskPeasant();// check that there aren't more important tasks now that builders have had time to build farms, archery towers etc...
	}



	// Building Tasks
	// this is called from King.cs when a peasant drops off resource, then flags the first builder in the queue to get the resource
	public void GetResource ()
	{
		kingScript.resourceQueue.Remove (this);
		bool gotResource = false;
		for (int i = 0; i < destinationPlatformScript.cost.Length; i++) {
			if (destinationPlatformScript.cost [i] > 0 && kingScript.availableResources [i] > 0) {
				Debug.Log ("_ _ _ We'll take resource index: " + i);
				kingScript.availableResources [i]--;
				carriedResourceIndex = i;
				gotResource = true;
				break;
			}
		}
		if (gotResource) {
//			kingScript.resourceQueue.Remove (this);
			carryingResource = true;
//			GoToPlatform (destinationPlatformScript.transform, destinationPlatformScript);
			GoToWorkPlatformBuilder();
		} else {
			Debug.Log ("This isn't the right resource, go to bakc of the queue");
			kingScript.resourceQueue.Add (this);

			// if there are other builders in line then tell the next builder that a resource has arrived
			if (kingScript.resourceQueue.Count > 1) {
				kingScript.CheckResourceArrived ();
			}
//			active = false;
		}
	}

	IEnumerator BuildSectionOfStructure ()
	{
		yield return new WaitForSeconds (kingScript.buildTime);
		Debug.Log ("part of structure built (or fed) ");
		// subtract the carried resource form the cost
		destinationPlatformScript.cost [carriedResourceIndex]--;
		carryingResource = false;

		int itemsStillRequired = 0;
		for (int i = 0; i < destinationPlatformScript.cost.Length; i++) {
			itemsStillRequired += destinationPlatformScript.cost [i];
		}

		if (itemsStillRequired <= 0) {
			Debug.Log ("structure is built... Find a new build task");
			ActivateStructure (destinationPlatformScript);
			active = false;
			// decrement the platformBuilder counter
			// TODO: possibly inform other builders here that the platform has been built and they should go back to docks to return resources
			if (kingScript.platformBuilder.ContainsKey (destinationPlatformScript)) {
				kingScript.platformBuilder [destinationPlatformScript]--;
			}
			kingScript.builderTasks.Remove(destinationPlatformScript);// take the task and remove it from the list
//			FindTaskBuilder ();
		} else {
			SendBuilderToDocks(destinationPlatformScript.transform, destinationPlatformScript);
		}
	}

	void ActivateStructure (Platform completedStructure)
	{
		// the Platform script takes care of removing builder and under construction items from the King.cs lists
		completedStructure.ActivateStructure();
	}

//	IEnumerator BuildFarm ()
//	{
//		yield return new WaitForSeconds (kingScript.buildTime);
//		Debug.Log ("part of farm built");
//		// subtract the carried resource form the cost
//		destinationPlatformScript.cost [carriedResourceIndex]--;
//		carryingResource = false;
//
//		int itemsStillRequired = 0;
//		for (int i = 0; i < destinationPlatformScript.cost.Length; i++) {
//			itemsStillRequired += destinationPlatformScript.cost [i];
//		}
//
//		if (itemsStillRequired <= 0) {
//			Debug.Log ("Farm is built... wait for more build instructions... maybe idle about");
//			active = false;
//		} else {
//			SendBuilderToDocks(destinationPlatformScript.transform, destinationPlatformScript);
//		}
//	}
//	IEnumerator BuildQuarry ()
//	{
//		yield return new WaitForSeconds (kingScript.buildTime);
//		Debug.Log ("part of quarry built");
//		// subtract the carried resource form the cost
//		destinationPlatformScript.cost [carriedResourceIndex]--;
//		carryingResource = false;
//
//		int itemsStillRequired = 0;
//		for (int i = 0; i < destinationPlatformScript.cost.Length; i++) {
//			itemsStillRequired += destinationPlatformScript.cost [i];
//		}
//
//		if (itemsStillRequired <= 0) {
//			Debug.Log ("Quarry is built... wait for more build instructions... maybe idle about");
//			active = false;
//		} else {
//			SendBuilderToDocks(destinationPlatformScript.transform, destinationPlatformScript);
//		}
//	}

}
