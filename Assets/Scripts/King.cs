using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class King : MonoBehaviour {

	public float cycleTime = 10.0f;
	private float currentTime = 0.0f;
	public int days = 0;

	public float setUpDelay = 1.0f;// pause in seconds before work tasks are assigned

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
//	[HideInInspector]
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
//	[HideInInspector]
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


	// Threashold values for assigning peasant and builder work task lists
	public int foodThreashold = 5;
	public int woodThreashold = 10;
	public int stoneThreashold = 5;
	public int metalThreashold = 2;

	public int peasantThreashold = 5;

	public int maxPopulationPerPlatform = 5;// if there are more peasants per platform than this then more platforms are built




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
	private bool checkingResourceArrived = false;
	public List<NPC> resourceQueue = new List<NPC>();

	// resource stores
	public Transform foodResourceStore;
	public Transform woodResourceStore;
	public Transform stoneResourceStore;

	public GameObject foodResourcePrefab;
	public GameObject woodResourcePrefab;
	public GameObject stoneResourcePrefab;


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


	// TRACK CONSTRUCTION
	public List<Platform> platformsUnderConstruction = new List<Platform>();


	// Track task lists for epasants and builders
	public List<Platform> peasantTasks = new List<Platform>();
	public List<Platform> builderTasks = new List<Platform>();


	// Archers adn attack logic
	public bool enemySighted = false;
	public Transform enemyTransform;

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

	}

	void Start(){
		StartCoroutine(FirstTaskAssignments());
	}

	IEnumerator FirstTaskAssignments(){
		yield return new WaitForSeconds(setUpDelay);
		SetNPCTasks ();
	}

	public void SetNPCTasks ()
	{
		// Update all NPC tasks
		for (int i = 0; i < npcScripts.Count; i++) {
			npcScripts[i].FindTask();
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
		currentTime += Time.deltaTime;
		if (currentTime >= cycleTime) {
			currentTime = 0.0f;
			days++;
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
			}else if (npcScripts [i].occupation == 3){
				builders.Add(npcScripts[i]);
			}
			else if (npcScripts [i].occupation == 4){
				archers.Add(npcScripts[i]);
			}
		}
		SetNPCTasks ();
	}

	// Defense & ARCHERS
	public void AlertArchers (Collider2D enemyCollider)
	{
		enemySighted = true;
		enemyTransform = enemyCollider.transform;
		for (int i = 0; i < archers.Count; i++) {
			archers[i].Patrol();
		}
	}

	// INFORM BUILDER THAT RESOURCE HAS ARRIVED --- used in NPC.cs
	public void CheckResourceArrived ()
	{
		// only run this again after the second is up... otherwise it might get out of control
		if (!checkingResourceArrived) {
//			Debug.Log ("Check resource arrived");
			checkingResourceArrived = true;
			StartCoroutine (AllowBuildersToTakeResource ());
		}
	}
	IEnumerator AllowBuildersToTakeResource ()
	{
		yield return new WaitForSeconds (resourcePickupWaitTime);
		if (resourceQueue.Count > 0) {
			resourceQueue [0].GetResource ();
		}
		checkingResourceArrived = false;
	}




	// NPC Notifications
	public void NotifyWaitingPeasants ()
	{
		if (builtFarmingPlatforms.Count > 0) {
			for (int i = 0; i < npcScripts.Count; i++) {
				if (npcScripts [i].waitingForFarmingPlatform) {
					npcScripts [i].FindTask();
				}
			}
		}	
	}



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

	 public Platform FindPlatformWithLowestPopulation(List<Platform> platforms, Dictionary<Platform, int> populationDict){
		int[] popPerPlatform = new int[platforms.Count];
		for (int i = 0; i < platforms.Count; i++) {
			if (populationDict.ContainsKey (platforms [i])) {
				popPerPlatform [i] = populationDict [platforms [i]];
			} else {
				popPerPlatform [i] = 0;
			}
		}

		int lowestPlatformPopulation = Mathf.Min(popPerPlatform);
		int targetPlatformIndex = System.Array.IndexOf(popPerPlatform, lowestPlatformPopulation);
		Platform targetPlatform = platforms[targetPlatformIndex];

		return targetPlatform;

	 }

	 public void AddNPCToPopulation (Platform key, Dictionary<Platform, int> dict)
	{
		if (dict.ContainsKey (key)) {
			dict[key]++;
		}else{
			dict.Add(key, 1);
		}
	 }
	public void RemoveNPCFromPopulation (Platform key, Dictionary<Platform, int> dict)
	{
		if (dict.ContainsKey (key)) {
			dict[key]--;
		}
	 }



}
