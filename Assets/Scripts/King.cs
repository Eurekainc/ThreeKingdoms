using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class King : MonoBehaviour {

	public float cycleTime = 10.0f;
	private float currentTime = 0.0f;
	public int days = 0;

	// POPULATION THREASHHOLDS
	public int minPopulation = 5;// 5 or more population means we can start building a castle
	public int minMilitaryPopulation = 10;// 10 or more population means we can start training archers
	public int minArchersForBoat = 10;// number of archers required to start building boats

	// platforms
	public List<GameObject> platforms = new List<GameObject>();
	[HideInInspector]
	public List<Platform> platformScripts = new List<Platform>();

	[HideInInspector]
	public List<Platform> farmingPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> farmingPlatformsUnderConstruction = new List<Platform>();// busy being constructed by builder
	[HideInInspector]
	public List<Platform> builtFarmingPlatforms = new List<Platform>();// ready to be farmed

	[HideInInspector]
	public List<Platform> loggingPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> quarryPlatforms = new List<Platform>();

	[HideInInspector]
	public List<Platform> minePlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> miningPlatformsUnderConstruction = new List<Platform>();// busy being constructed by builder
	[HideInInspector]
	public List<Platform> builtMiningPlatforms = new List<Platform>();// ready to be mined

	[HideInInspector]
	public List<Platform> housingPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> housingPlatformsUnderConstruction = new List<Platform>();// busy being constructed by builder
	[HideInInspector]
	public List<Platform> builtHousingPlatforms = new List<Platform>();// ready to be fed

	[HideInInspector]
	public List<Platform> workshopPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> workshopPlatformsUnderConstruction = new List<Platform>();// busy being constructed by builder
	[HideInInspector]
	public List<Platform> builtWorkshopPlatforms = new List<Platform>();// ready to train builders

	[HideInInspector]
	public List<Platform> archeryPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> archeryPlatformsUnderConstruction = new List<Platform>();// busy being constructed by builder
	[HideInInspector]
	public List<Platform> builtArcheryPlatforms = new List<Platform>();// ready to be train archers

	// dictionaries to log how many peasants or builders are busy with each platform
	public Dictionary<Platform, int> platformPeasant = new Dictionary<Platform, int>();
	public Dictionary<Platform, int> platformBuilder = new Dictionary<Platform, int>();


	public Platform docks;

	// All NPCs
	public List<GameObject> npcs = new List<GameObject>();
	public List<NPC> npcScripts = new List<NPC>();

	public NPC king;
	public List<NPC> peasants = new List<NPC>();
	public List<NPC> builders = new List<NPC>();
	public List<NPC> archers = new List<NPC>();

	// build / work / combat times/delays
	public float offloadTime = 1.0f;

	public float farmTime = 2.0f;
	public float loggingTime = 2.0f;
	public float quarryingTime = 2.0f;
	public float miningTime = 2.0f;
	public float buildTime = 2.0f;
	public float reloadTime = 2.0f;


	public float resourcePickupWaitTime = 1.0f;// this is the delay that builders have before picking up a resource... allows player time to take the resource first
	// resources
//	public int food = 0;// 0
//	public int wood = 0;// 1
//	public int stone = 0;// 2
//	public int metal = 0;// 3
	// 0 -> food
	// 1 -> wood
	// 2 -> stone
	// 3 -> metal
	public int[] availableResources = new int[4];

	// builder's queue to wait for resources
	public List<NPC> resourceQueue = new List<NPC>();

	// structures
	public int farms = 0;
	public int quarries = 0;
	public int forrests = 0;
	public int houses = 0;
	public int workshops = 0;
	public int keeps = 0;
	public int castles = 0;
	public int archeryRange = 0;
	public int archeryLookout = 0;
	public int archeryTower = 0;
	public int boats = 0;

	// ITEM COSTS
	public int[] farmCost = new int[4];
	//public int[] forrestCost = new int[4];// forrest is free
	public int[] quarryCost = new int[4];
	public int[] houseCost = new int[4];
	public int[] workshopCost = new int[4];
	public int[] keepCost = new int[4];
	public int[] castleCost = new int[4];
	public int[] archeryRangeCost = new int[4];
	public int[] archeryLookoutCost = new int[4];
	public int[] archeryTowerCost = new int[4];

	public int[] boatCost = new int[4];

	public int[] peasantCost = new int[4];
	public int[] builderCost = new int[4];
	public int[] archerCost = new int[4];

	// TRACK CONSTRUCTION
	public List<Platform> platformsUnderConstruction = new List<Platform>();

	// Use this for initialization
	void Awake ()
	{
		for (int i = 0; i < platforms.Count; i++) {
			platformScripts.Add (platforms [i].GetComponent<Platform> ());
		}

		for (int i = 0; i < platformScripts.Count; i++) {
			if (platformScripts [i].farming) {
				farmingPlatforms.Add(platformScripts [i]);
				farmingPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
			}else if(platformScripts [i].logging){
				loggingPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].quarry){
				quarryPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].mine){
				minePlatforms.Add(platformScripts[i]);
				miningPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
			}
			else if(platformScripts [i].housing){
				housingPlatforms.Add(platformScripts[i]);
				housingPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
			}
			else if(platformScripts [i].workshop){
				workshopPlatforms.Add(platformScripts[i]);
				workshopPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
			}
			else if(platformScripts [i].archery){
				archeryPlatforms.Add(platformScripts[i]);
				archeryPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
			}
		}


		for (int i = 0; i < npcs.Count; i++) {
			npcScripts.Add(npcs[i].GetComponent<NPC>());
			// TODO: make sure to update the King script when NPCs move between islands
			npcScripts[i].kingScript = this;// assign the King script to each NPC
		}

		king = npcScripts[0];// assign the first NPC to be the king

		// the init function
//		DecideAction();
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentTime += Time.deltaTime;
		if (currentTime >= cycleTime) {
			currentTime = 0.0f;
			days++;
//			Debug.Log("New day..... " + days);
//			DecideAction();
		}
	}

	void ResetNPCLists(){
		peasants = new List<NPC>();
		builders = new List<NPC>();
		archers = new List<NPC>();
	}

	public void UpdatePeasantCount ()
	{
		ResetNPCLists();
		for (int i = 0; i < npcScripts.Count; i++) {
			if (npcScripts [i].occupation == 0) {
				peasants.Add(npcScripts[i]);
			}else if (npcScripts [i].occupation == 1){
				builders.Add(npcScripts[i]);
			}
			else if (npcScripts [i].occupation == 2){
				archers.Add(npcScripts[i]);
			}
		}
	}

	// INFORM BUILDER THAT RESOURCE HAS ARRIVED --- used in NPC.cs
	public void CheckResourceArrived(){
		StartCoroutine(AllowBuildersToTakeResource());
	}
	IEnumerator AllowBuildersToTakeResource ()
	{
		yield return new WaitForSeconds (resourcePickupWaitTime);
		if (resourceQueue.Count > 0) {
			resourceQueue [0].GetResource ();
		}
	}


	void SendNPCToPlatform(NPC npc, Transform platform, Platform platformScript){
		npc.GoToPlatform(platform, platformScript);
	}
	void AssignBuilderTask (NPC npc, Transform platform, Platform platformScript)
	{
		npc.SendBuilderToDocks(platform, platformScript);
	}


	// Determining the goals for the day
	void DecideAction ()
	{

		// NPCs
		int[] population = new int[4];
		population [0] = 1;//king
		population [1] = peasants.Count;
		population [2] = builders.Count;
		population [3] = archers.Count;

		int populationSum = SumIntArray (population);

		// STRUCTURES
		int[] structureCount = new int[11];
		structureCount [0] = farms;
		structureCount [1] = workshops;
		structureCount [2] = quarries;
		structureCount [3] = forrests;
		structureCount [4] = houses;
		structureCount [5] = keeps;
		structureCount [6] = castles;
		structureCount [7] = archeryRange;
		structureCount [8] = archeryLookout;
		structureCount [9] = archeryTower;
		structureCount [10] = boats;

		int structureSum = SumIntArray (structureCount);

		// RESOURCES
//		int[] resourceCount = new int[4];
//		resourceCount [0] = food;
//		resourceCount [1] = wood;
//		resourceCount [2] = stone;
//		resourceCount [3] = metal;

		int resourceSum = SumIntArray (availableResources);



		// BASIC ALGORITHM
		// --------------------------
		// population is low --> build more houses
		// population is above a certain threashhold --> build a castle
		// population is above a higher threashhold --> build archery range
		// we're being attacked --> build archery range
		// archery population is above a certain threashhold --> build boats


		// some ideas
		/// maybe if there are a lot of resources and no construction happening then either build somehting or increase army...

		// Initial actions
		// #1 increase population
		if (populationSum < minPopulation) {
			Debug.Log ("Build a house");
			BuildHouse ();
		} else if (populationSum >= minPopulation) {
			BuildCastle ();
		} else if (populationSum >= minMilitaryPopulation) {
			BuildDefense ();
		} else if (population [3] >= minArchersForBoat) {
			BuildBoat ();
		}


		// Maintain any existing building projects
		if (platformsUnderConstruction.Count > 0) {
			for (int i = 0; i < platformsUnderConstruction.Count; i++) {
				if (platformsUnderConstruction [i].housing) 
				{
					BuildHouse();
				}
				else if (platformsUnderConstruction [i].farming)
				{
					BuildFarm();
				}
			}
		}

	}

	// HIGH LEVEL DIRECTIVES
	// ------------------------
	void BuildHouse ()
	{
		// check resource levels

		// - enough -> send builder to start building
		// 			- check for available builders
		// 			- available builder --> send to build
		// 			- no available builders --> if workshop then check for available peasants, if available peasants, then send for training to become builders

		// - not enough -> send peasant to gather resources



		// is there already a house under construction?? if not then assign the first housing platform in the list
		Platform houseUnderConstruction = null;
		for (int i = 0; i < platformsUnderConstruction.Count; i++) {
			if (housingPlatforms.Contains (platformsUnderConstruction [i])) {
				Debug.Log ("A house is already under construction");
				houseUnderConstruction = platformsUnderConstruction [i];
			}
		}

		if (houseUnderConstruction == null) {
			houseUnderConstruction = housingPlatforms[0];
		}

		bool haveResources = false;
		for (int i = 0; i < availableResources.Length; i++) {
			if (availableResources [i] > 0 && houseUnderConstruction.cost [i] > 0) {
				haveResources = true;// have at least some of the necessary resource to build a house
			}
		}

		// yes we have resources to build a house, so assign builder to build a house
		if (haveResources) 
		{
			AssignBuildHouse ();
		} 
		// no resources to build a house so remedy this
		else {
			// if wood or stone is required then can directly send a peasant to gather resource
			// if food or metal is required then need to build either a mine or a farm
			bool gatherWood = false;
			bool gatherStone = false;
			bool buildFarm = false;
			bool buildMine = false;

			for (int i = 0; i < houseUnderConstruction.cost.Length; i++) {
				if (houseUnderConstruction.cost [i] > 0 && availableResources [i] <= 0) {
					Debug.Log ("We need" + houseUnderConstruction.cost [i] + " of resource index: " + i);
					if (i == 0) {
						buildFarm = true;
					} else if (i == 1) {
						gatherWood = true;
					} else if (i == 2) {
						gatherStone = true;
					} else if (i == 3) {
						buildMine = true;
					}
				}
			}
			if (gatherWood) {
				AssignLogging ();
			}
			;
			if (gatherStone) {
				AssignQuarrying ();
			}
			;
			if (buildFarm) {
				AssignBuildFarm ();
			}
			;
			if (buildMine) {
				AssignBuildMine();
			}
		}
	}

	void BuildFarm ()
	{
		Platform farmUnderConstruction = null;
		for (int i = 0; i < platformsUnderConstruction.Count; i++) {
			if (farmingPlatforms.Contains (platformsUnderConstruction [i])) {
				Debug.Log ("A farm is already under construction");
				farmUnderConstruction = platformsUnderConstruction [i];
			}
		}

		if (farmUnderConstruction == null) {
			farmUnderConstruction = farmingPlatforms[0];
		}

		bool haveResources = false;
		for (int i = 0; i < availableResources.Length; i++) {
			if (availableResources [i] > 0 && farmUnderConstruction.cost [i] > 0) {
				haveResources = true;// have at least some of the necessary resource to build a house
			}
		}

		// yes we have resources to build a house, so assign builder to build a house
		if (haveResources) 
		{
			AssignBuildFarm ();
		} 
		// no resources to build a farm so remedy this
		else {
			// if wood or stone is required then can directly send a peasant to gather resource
			// if food or metal is required then need to build either a mine or a farm
			bool gatherWood = false;
			bool gatherStone = false;
			bool buildFarm = false;
			bool buildMine = false;

			for (int i = 0; i < farmUnderConstruction.cost.Length; i++) {
				if (farmUnderConstruction.cost [i] > 0 && availableResources [i] <= 0) {
					Debug.Log ("We need" + farmUnderConstruction.cost [i] + " of resource index: " + i);
					if (i == 0) {
						buildFarm = true;
					} else if (i == 1) {
						gatherWood = true;
					} else if (i == 2) {
						gatherStone = true;
					} else if (i == 3) {
						buildMine = true;
					}
				}
			}
			if (gatherWood) {
				AssignLogging ();
			}
			;
			if (gatherStone) {
				AssignQuarrying ();
			}
			;
			if (buildFarm) {
				AssignBuildFarm ();
			}
			;
			if (buildMine) {
				AssignBuildMine();
			}
		}
	}

	void BuildCastle(){
		Debug.Log("Build a castle");
	}
	void BuildDefense(){
		
	}
	void BuildBoat(){
		
	}

	public void FeedHouse (Platform platformToFeed)
	{

		// if no food then produce more food
		if (availableResources [0] <= 0) {
			if (farms <= 0) {
				// build a farm
				BuildFarm ();
			} else if (farms > 0 && farmingPlatforms.Count > farms) {
				// build more farms
				Debug.Log("Build more farms");
				BuildFarm ();
				Debug.Log("- - - - - - - - - - assign more farmers");
				AssignFarming();
			}
		} else {
			AssignBuildPeasant(platformToFeed);// builders take food to houses to make more peasants
		}

	}


	// NPC LEVEL TASKS
	// ------------------------

	// ASSIGN TASKS
	void AssignFarming ()
	{
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 0) {
				SendNPCToPlatform(npcScripts [i], farmingPlatforms[0].transform, farmingPlatforms[0]);
			}
		}
	}
	// as each platform is exhausted it mreoves itself form the KingScript's list...
	void AssignLogging ()
	{
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 0) {
				SendNPCToPlatform(npcScripts [i], loggingPlatforms[0].transform, loggingPlatforms[0]);
			}
		}
	}
	void AssignQuarrying ()
	{
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 0) {
				SendNPCToPlatform(npcScripts [i], quarryPlatforms[0].transform, quarryPlatforms[0]);
			}
		}
	}

	// builders all go to the docks first and then carry out their task
	void AssignBuildFarm(){
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 1) {
				AssignBuilderTask(npcScripts [i], farmingPlatforms[0].transform, farmingPlatforms[0]);
			}
		}
	}
	void AssignBuildHouse(){
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 1) {
				AssignBuilderTask(npcScripts [i], housingPlatforms[0].transform, housingPlatforms[0]);
			}
		}
	}
	void AssignBuildWorkshop(){
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 1) {
				AssignBuilderTask(npcScripts [i], workshopPlatforms[0].transform, workshopPlatforms[0]);
			}
		}
	}
	void AssignBuildMine(){
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 1) {
				AssignBuilderTask(npcScripts [i], minePlatforms[0].transform, minePlatforms[0]);
			}
		}
	}
	void AssignBuildArcheryRange(){
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 1) {
				AssignBuilderTask(npcScripts [i], archeryPlatforms[0].transform, archeryPlatforms[0]);
			}
		}
	}
	void AssignBuildBoat(){
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 1) {
				AssignBuilderTask(npcScripts [i], docks.transform, docks);
			}
		}
	}

	// builder takes food to house to create a peasant
	void AssignBuildPeasant(Platform platformToFeed){
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 1) {
				AssignBuilderTask(npcScripts [i], platformToFeed.transform, platformToFeed);
			}
		}
	}




	// REVISED TODO-LISTING



	// UTILITY FUNCTIONS
		public int SumIntArray(int[] toBeSummed)
	 {
	     int sum = 0;
	     foreach (int item in toBeSummed)
	     {
	         sum += item;
	     }
	     return sum;
	 }



}
