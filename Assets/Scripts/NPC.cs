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
	private Platform destinationPlatformScript = null;
	public bool movingToPlatform = false;

	// Carrying resources
	public bool carryingResource = false;
	public int carriedResourceIndex;

	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
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
			// order of importance
			// Farm -> Forrest -> Quarry -> Mine

			int peasantsWorkingFarms = 0;
			int[] availableFarms = new int[kingScript.builtFarmingPlatforms.Count];// an array containing the number of peasants working the platform, for each available platform
			for (int i = 0; i < kingScript.builtFarmingPlatforms.Count; i++) {
				if (kingScript.platformPeasant.ContainsKey (kingScript.builtFarmingPlatforms [i])) {
					peasantsWorkingFarms += kingScript.platformPeasant [kingScript.builtFarmingPlatforms [i]];
					availableFarms [i] = kingScript.platformPeasant [kingScript.builtFarmingPlatforms [i]];
				} else {
					availableFarms [i] = 0;
				}
			}
			if (kingScript.builtFarmingPlatforms.Count <= 0) {
				peasantsWorkingFarms = int.MaxValue;
			}

			int peasantsWorkingForrest = 0;
			int[] availableForrests = new int[kingScript.loggingPlatforms.Count];// an array containing the number of peasants working the platform, for each available platform
			for (int i = 0; i < kingScript.loggingPlatforms.Count; i++) {
				if (kingScript.platformPeasant.ContainsKey (kingScript.loggingPlatforms [i])) {
					peasantsWorkingForrest += kingScript.platformPeasant [kingScript.loggingPlatforms [i]];
					availableForrests [i] = kingScript.platformPeasant [kingScript.loggingPlatforms [i]];
				} else {
					availableForrests [i] = 0;
				}
			}
			if (kingScript.loggingPlatforms.Count <= 0) {
				peasantsWorkingForrest = int.MaxValue;
			}

			int peasantsWorkingQuarries = 0;
			int[] availableQuarries = new int[kingScript.quarryPlatforms.Count];// an array containing the number of peasants working the platform, for each available platform
			for (int i = 0; i < kingScript.quarryPlatforms.Count; i++) {
				if (kingScript.platformPeasant.ContainsKey (kingScript.quarryPlatforms [i])) {
					peasantsWorkingQuarries += kingScript.platformPeasant [kingScript.quarryPlatforms [i]];
					availableQuarries [i] = kingScript.platformPeasant [kingScript.quarryPlatforms [i]];
				} else {
					availableQuarries [i] = 0;
				}
			}
			if (kingScript.quarryPlatforms.Count <= 0) {
				peasantsWorkingQuarries = int.MaxValue;
			}

			int peasantsWorkingMines = 0;
			int[] availableMines = new int[kingScript.builtMiningPlatforms.Count];// an array containing the number of peasants working the platform, for each available platform
			for (int i = 0; i < kingScript.builtMiningPlatforms.Count; i++) {
				if (kingScript.platformPeasant.ContainsKey (kingScript.builtMiningPlatforms [i])) {
					peasantsWorkingMines += kingScript.platformPeasant [kingScript.builtMiningPlatforms [i]];
					availableMines [i] = kingScript.platformPeasant [kingScript.builtMiningPlatforms [i]];
				} else {
					availableMines [i] = 0;
				}
			}
			if (kingScript.builtMiningPlatforms.Count <= 0) {
				peasantsWorkingMines = int.MaxValue;
			}

			int[] peasantPlatforms = new int[4];
			peasantPlatforms [0] = peasantsWorkingFarms;
			peasantPlatforms [1] = peasantsWorkingForrest;
			peasantPlatforms [2] = peasantsWorkingQuarries;
			peasantPlatforms [3] = peasantsWorkingMines;

			int minValue = Mathf.Min (peasantPlatforms);
			int platformTypeIndex = 0;
			if (minValue < int.MaxValue) {
				platformTypeIndex = System.Array.IndexOf (peasantPlatforms, minValue);
			}

			// NOTE: there is no real prioritisation here
			// forrest and quarrying will be assigned first because initially no farms or mines have been built, so are not an option

			// update platformPeasant dictionary and then assign task
			switch (platformTypeIndex) {
				case 0:
					Debug.Log ("------- Work a farm ------");
					// work the farm with the least other peasants
					int farmIndex = System.Array.IndexOf (availableFarms, Mathf.Min (availableFarms));
					// if this farm isn't in the dictionary (i.e. if there are no other peasants working there), then add it to the dictionary
					if (kingScript.platformPeasant.ContainsKey (kingScript.builtFarmingPlatforms [farmIndex])) {
						kingScript.platformPeasant [kingScript.builtFarmingPlatforms [farmIndex]] += 1;
					} else {
						kingScript.platformPeasant.Add(kingScript.builtFarmingPlatforms [farmIndex], 1);
					}
					// set active, destination adn tell peasasnt to go there
					active = true;
					destinationPlatformScript = kingScript.builtFarmingPlatforms [farmIndex];
					GoToWorkPlatformPeasant();
					break;
				case 1:
					Debug.Log("Work a forrest");
					int forrestIndex = System.Array.IndexOf(availableForrests, Mathf.Min(availableForrests));
					if (kingScript.platformPeasant.ContainsKey (kingScript.loggingPlatforms [forrestIndex])) {
						kingScript.platformPeasant[kingScript.loggingPlatforms[forrestIndex]] += 1;
					} else {
						kingScript.platformPeasant.Add(kingScript.loggingPlatforms [forrestIndex], 1);
					}
					active = true;
					destinationPlatformScript = kingScript.loggingPlatforms [forrestIndex];
					GoToWorkPlatformPeasant();
					break;
				case 2:
					Debug.Log("Work a Quarry");
					int quarryIndex = System.Array.IndexOf(availableQuarries, Mathf.Min(availableQuarries));
					if (kingScript.platformPeasant.ContainsKey (kingScript.quarryPlatforms [quarryIndex])) {
						kingScript.platformPeasant[kingScript.quarryPlatforms[quarryIndex]] += 1;
					} else {
						kingScript.platformPeasant.Add(kingScript.quarryPlatforms [quarryIndex], 1);
					}
					active = true;
					destinationPlatformScript = kingScript.quarryPlatforms [quarryIndex];
					GoToWorkPlatformPeasant();
					break;
				case 3:
					Debug.Log("Work a mine");
					int mineIndex = System.Array.IndexOf(availableMines, Mathf.Min(availableMines));
					if (kingScript.platformPeasant.ContainsKey (kingScript.builtMiningPlatforms [mineIndex])) {
						kingScript.platformPeasant[kingScript.builtMiningPlatforms[mineIndex]] += 1;
					} else {
						kingScript.platformPeasant.Add(kingScript.builtMiningPlatforms [mineIndex], 1);
					}
					active = true;
					destinationPlatformScript = kingScript.builtMiningPlatforms [mineIndex];
					GoToWorkPlatformPeasant();
					break;
				default:
				Debug.Log("Fall through FindTask in NPC.cs");
				break;
			}
	}

	void FindTaskBuilder ()
	{
		// order of importance
		// Farm -> Houses -> Mines -> Archery (--> special items: boats, bridges, dragon towers etc...)

		// FARMS
		int buildersWorkingFarms = 0;
		int[] availableFarms = new int[kingScript.farmingPlatformsUnderConstruction.Count];// an array containing the number of peasants working the platform, for each available platform
		for (int i = 0; i < kingScript.farmingPlatformsUnderConstruction.Count; i++) {
			if (kingScript.platformBuilder.ContainsKey (kingScript.farmingPlatformsUnderConstruction [i])) {
				buildersWorkingFarms += kingScript.platformBuilder [kingScript.farmingPlatformsUnderConstruction [i]];
				availableFarms [i] = kingScript.platformBuilder [kingScript.farmingPlatformsUnderConstruction [i]];
			} else {
				availableFarms [i] = 0;
			}
		}
		if (kingScript.farmingPlatformsUnderConstruction.Count <= 0) {
			buildersWorkingFarms = int.MaxValue;
		}

		// HOUSES
		int buildersWorkingHouses = 0;
		int[] availableHouses = new int[kingScript.housingPlatformsUnderConstruction.Count];// an array containing the number of peasants working the platform, for each available platform
		for (int i = 0; i < kingScript.housingPlatformsUnderConstruction.Count; i++) {
			if (kingScript.platformBuilder.ContainsKey (kingScript.housingPlatformsUnderConstruction [i])) {
				buildersWorkingHouses += kingScript.platformBuilder [kingScript.housingPlatformsUnderConstruction [i]];
				availableHouses [i] = kingScript.platformBuilder [kingScript.housingPlatformsUnderConstruction [i]];
			} else {
				availableHouses [i] = 0;
			}
		}
		if (kingScript.housingPlatformsUnderConstruction.Count <= 0) {
			buildersWorkingHouses = int.MaxValue;
		}

		// TODO
		// FEED HOUSES

		// WORKSHOPS
		int buildersWorkingWorkshops = 0;
		int[] availableWorkshops = new int[kingScript.workshopPlatformsUnderConstruction.Count];// an array containing the number of peasants working the platform, for each available platform
		for (int i = 0; i < kingScript.workshopPlatformsUnderConstruction.Count; i++) {
			if (kingScript.platformBuilder.ContainsKey (kingScript.workshopPlatformsUnderConstruction [i])) {
				buildersWorkingWorkshops += kingScript.platformBuilder [kingScript.workshopPlatformsUnderConstruction [i]];
				availableWorkshops [i] = kingScript.platformBuilder [kingScript.workshopPlatformsUnderConstruction [i]];
			} else {
				availableWorkshops [i] = 0;
			}
		}
		if (kingScript.workshopPlatformsUnderConstruction.Count <= 0) {
			buildersWorkingWorkshops = int.MaxValue;
		}

		// MINES
		int buildersWorkingMines = 0;
		int[] availableMines = new int[kingScript.miningPlatformsUnderConstruction.Count];// an array containing the number of peasants working the platform, for each available platform
		for (int i = 0; i < kingScript.miningPlatformsUnderConstruction.Count; i++) {
			if (kingScript.platformBuilder.ContainsKey (kingScript.miningPlatformsUnderConstruction [i])) {
				buildersWorkingMines += kingScript.platformBuilder [kingScript.miningPlatformsUnderConstruction [i]];
				availableMines [i] = kingScript.platformBuilder [kingScript.miningPlatformsUnderConstruction [i]];
			} else {
				availableMines [i] = 0;
			}
		}
		if (kingScript.miningPlatformsUnderConstruction.Count <= 0) {
			buildersWorkingMines = int.MaxValue;
		}

		// Archery Range
		int buildersWorkingArchery = 0;
		int[] availableArcheryRanges = new int[kingScript.archeryPlatformsUnderConstruction.Count];// an array containing the number of peasants working the platform, for each available platform
		for (int i = 0; i < kingScript.archeryPlatformsUnderConstruction.Count; i++) {
			if (kingScript.platformBuilder.ContainsKey (kingScript.archeryPlatformsUnderConstruction [i])) {
				buildersWorkingArchery += kingScript.platformBuilder [kingScript.archeryPlatformsUnderConstruction [i]];
				availableArcheryRanges [i] = kingScript.platformBuilder [kingScript.archeryPlatformsUnderConstruction [i]];
			} else {
				availableArcheryRanges [i] = 0;
			}
		}
		if (kingScript.archeryPlatformsUnderConstruction.Count <= 0) {
			buildersWorkingMines = int.MaxValue;
		}



		int[] builderPlatforms = new int[5];
		builderPlatforms [0] = buildersWorkingFarms;
		builderPlatforms [1] = buildersWorkingHouses;
		builderPlatforms [2] = buildersWorkingWorkshops;
		builderPlatforms [3] = buildersWorkingMines;
		builderPlatforms [4] = buildersWorkingArchery;

		int minValue = Mathf.Min (builderPlatforms);
		int platformTypeIndex = 0;
		if (minValue < int.MaxValue) {
			platformTypeIndex = System.Array.IndexOf (builderPlatforms, minValue);
		}

		if (kingScript.builtFarmingPlatforms.Count == 0 && kingScript.farmingPlatformsUnderConstruction.Count > 0) {
			Debug.Log("Build the first farm");
			kingScript.platformBuilder.Add (kingScript.farmingPlatformsUnderConstruction [0], 1);
			active = true;
			destinationPlatformScript = kingScript.farmingPlatformsUnderConstruction [0];
			GoToDocksBuilder ();
		} else if (kingScript.builtHousingPlatforms.Count == 0 && kingScript.housingPlatformsUnderConstruction.Count > 0) {
			Debug.Log("Build the first house");
			kingScript.platformBuilder.Add (kingScript.housingPlatformsUnderConstruction [0], 1);
			active = true;
			destinationPlatformScript = kingScript.housingPlatformsUnderConstruction [0];
			GoToDocksBuilder ();
		}else if (kingScript.builtWorkshopPlatforms.Count == 0 && kingScript.workshopPlatformsUnderConstruction.Count > 0) {
			Debug.Log("Build the first workshop");
			kingScript.platformBuilder.Add (kingScript.workshopPlatformsUnderConstruction [0], 1);
			active = true;
			destinationPlatformScript = kingScript.workshopPlatformsUnderConstruction [0];
			GoToDocksBuilder ();
		} else if (kingScript.builtMiningPlatforms.Count == 0 && kingScript.miningPlatformsUnderConstruction.Count > 0) {
			Debug.Log("Build the first mine");
			kingScript.platformBuilder.Add (kingScript.miningPlatformsUnderConstruction [0], 1);
			active = true;
			destinationPlatformScript = kingScript.miningPlatformsUnderConstruction [0];
			GoToDocksBuilder ();
		} else if (kingScript.builtArcheryPlatforms.Count == 0 && kingScript.archeryPlatformsUnderConstruction.Count > 0) {
			Debug.Log("Build the first archery range");
			kingScript.platformBuilder.Add (kingScript.archeryPlatformsUnderConstruction [0], 1);
			active = true;
			destinationPlatformScript = kingScript.archeryPlatformsUnderConstruction [0];
			GoToDocksBuilder ();
		} 
		// if there is at least 1 builder on each of the important platforms, try to evenly distribute builders between platforms
		else {

			// update platformBuilder dictionary and then assign task
			switch (platformTypeIndex) {
			case 0:
				Debug.Log ("Build a farm");
					// work the farm with the least other peasants
				int farmIndex = System.Array.IndexOf (availableFarms, Mathf.Min (availableFarms));
					// if this farm isn't in the dictionary (i.e. if there are no other peasants working there), then add it to the dictionary
				if (kingScript.platformBuilder.ContainsKey (kingScript.farmingPlatformsUnderConstruction [farmIndex])) {
					kingScript.platformBuilder [kingScript.farmingPlatformsUnderConstruction [farmIndex]] += 1;
				} else {
					kingScript.platformBuilder.Add (kingScript.farmingPlatformsUnderConstruction [farmIndex], 1);
				}
					// set active, destination adn tell peasasnt to go there
				active = true;
				destinationPlatformScript = kingScript.farmingPlatformsUnderConstruction [farmIndex];
				GoToDocksBuilder ();
				break;
			case 1:
				Debug.Log ("Build a house");
				int houseIndex = System.Array.IndexOf (availableHouses, Mathf.Min (availableHouses));
				if (kingScript.platformBuilder.ContainsKey (kingScript.housingPlatformsUnderConstruction [houseIndex])) {
					kingScript.platformBuilder [kingScript.housingPlatformsUnderConstruction [houseIndex]] += 1;
				} else {
					kingScript.platformBuilder.Add (kingScript.housingPlatformsUnderConstruction [houseIndex], 1);
				}
				active = true;
				destinationPlatformScript = kingScript.housingPlatformsUnderConstruction [houseIndex];
				GoToDocksBuilder ();
				break;
			case 2:
				Debug.Log ("Build a Workshop");
				int workshopIndex = System.Array.IndexOf (availableWorkshops, Mathf.Min (availableWorkshops));
				if (kingScript.platformBuilder.ContainsKey (kingScript.workshopPlatformsUnderConstruction [workshopIndex])) {
					kingScript.platformBuilder [kingScript.workshopPlatformsUnderConstruction [workshopIndex]] += 1;
				} else {
					kingScript.platformBuilder.Add (kingScript.workshopPlatformsUnderConstruction [workshopIndex], 1);
				}
				active = true;
				destinationPlatformScript = kingScript.workshopPlatformsUnderConstruction [workshopIndex];
				GoToDocksBuilder ();
				break;
			case 3:
				Debug.Log ("Build a Mine");
				int mineIndex = System.Array.IndexOf (availableMines, Mathf.Min (availableMines));
				if (kingScript.platformBuilder.ContainsKey (kingScript.miningPlatformsUnderConstruction [mineIndex])) {
					kingScript.platformBuilder [kingScript.miningPlatformsUnderConstruction [mineIndex]] += 1;
				} else {
					kingScript.platformBuilder.Add (kingScript.miningPlatformsUnderConstruction [mineIndex], 1);
				}
				active = true;
				destinationPlatformScript = kingScript.miningPlatformsUnderConstruction [mineIndex];
				GoToDocksBuilder ();
				break;
			case 4:
				Debug.Log ("Build an archery range");
				int archeryIndex = System.Array.IndexOf (availableArcheryRanges, Mathf.Min (availableArcheryRanges));
				if (kingScript.platformBuilder.ContainsKey (kingScript.archeryPlatformsUnderConstruction [archeryIndex])) {
					kingScript.platformBuilder [kingScript.archeryPlatformsUnderConstruction [archeryIndex]] += 1;
				} else {
					kingScript.platformBuilder.Add (kingScript.archeryPlatformsUnderConstruction [archeryIndex], 1);
				}
				active = true;
				destinationPlatformScript = kingScript.archeryPlatformsUnderConstruction [archeryIndex];
				GoToDocksBuilder ();
				break;
			default:
				Debug.Log ("Fall through FindTask in NPC.cs");
				break;
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
		Debug.Log("Check task..... " + kingScript.days);
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

			// TODO: put a timer and queue in so that multiple builders can't steal resources from each other or snatch resources before player has a chance to take them
			if (!carryingResource) {

				// join the queue to get a resource
				kingScript.resourceQueue.Add(this);
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
		FindTaskPeasant();// check that there aren't more important tasks now that builders have had time to build farms, archery towers etc...
	}



	// Building Tasks
	// this is called from King.cs when a peasant drops off resource, then flags the first builder in the queue to get the resource
	public void GetResource ()
	{
		kingScript.resourceQueue.Remove (this);
		bool gotResource = false;
		for (int i = 0; i < destinationPlatformScript.cost.Length; i++) {
			if (destinationPlatformScript.cost [i] > 0 && kingScript.availableResources [i] > 0) {
				Debug.Log ("We'll take resource index: " + i);
				kingScript.availableResources [i]--;
				carriedResourceIndex = i;
				gotResource = true;
				break;
			}
		}
		if (gotResource) {
			kingScript.resourceQueue.Remove (this);
			carryingResource = true;
			Debug.Log ("Got resources to build a structure");
			GoToPlatform (destinationPlatformScript.transform, destinationPlatformScript);
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
			ActivateStructure(destinationPlatformScript);
			active = false;
			// decrement the platformBuilder counter
			// TODO: possibly inform other builders here that the platform has been built and they should go back to docks to return resources
			kingScript.platformBuilder[destinationPlatformScript]--;
			FindTaskBuilder ();
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
