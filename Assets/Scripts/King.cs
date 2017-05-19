using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class King : MonoBehaviour {

	public GlobalGameScript globalGameScript;

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
//	public int foodThreashold = 5;
//	public int woodThreashold = 10;
//	public int stoneThreashold = 5;
//	public int metalThreashold = 2;
//
//	public int peasantThreashold = 5;
//
//	public int maxPopulationPerPlatform = 5;// if there are more peasants per platform than this then more platforms are built




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
	public int initialFood = 0;
	public int initialWood = 0;
	public int initialStone = 0;
	public int initialMetal = 0;
	public int[] availableResources = new int[4];

	// builder's queue to wait for resources
	private bool checkingResourceArrived = false;
	public List<NPC> resourceQueue = new List<NPC>();

	// resource stores
	public Transform foodResourceStore;
	public Transform woodResourceStore;
	public Transform stoneResourceStore;

	public GameObject foodResourcePrefab;
	private Bounds foodResourceBounds;// need the bounds for positioning at the docks
	public int foodResourceColumns = 3;// The number of columns to stack at the docks
	public GameObject woodResourcePrefab;
	private Bounds woodResourceBounds;// need the bounds for positioning at the docks
	public int woodResourceColumns = 1;// The number of columns to stack at the docks
	public GameObject stoneResourcePrefab;
	private Bounds stoneResourceBounds;// need the bounds for positioning at the docks
	public int stoneResourceColumns = 3;// The number of columns to stack at the docks

	// Resource indicators
	public GameObject foodIcon;
	public GameObject woodIcon;
	public GameObject stoneIcon;

	// Resource Lists - used to keep track of which objects are taken from GlobalGameScript.cs and assigned to this island
	// these lists are used in the methods: RemoveAndPositionResource() & CreateAndPositionResource()
	private List<GameObject> foods = new List<GameObject>();
	private List<GameObject> woods = new List<GameObject>();
	public List<GameObject> stones = new List<GameObject>();// made public for debugging purposes
	private List<GameObject> metals = new List<GameObject>();

	// ferry pickup point where player picks up NPCs
	public Transform ferryPickUp;
	public List<NPC> npcsWaitingForFerry = new List<NPC>();

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


	// TRACK ALL CONSTRUCTION Platforms
	// use this to determine how much resource is required per island
	public List<Platform> platformsUnderConstruction = new List<Platform>();


	// Track task lists for epasants and builders
	public List<Platform> peasantTasks = new List<Platform>();
	public List<Platform> builderTasks = new List<Platform>();

	// NPC stats, speed attack points, health points etc.
	public float slowSpeed = 1.0f;
	public float mediumSpeed = 3.0f;
	public float fastSpeed = 10.0f;

	// Archers adn attack logic
	public bool enemySighted = false;
	public Transform enemyTransform;

	// Detect which resources to load
	// this is also in Player .cs
	public bool canLoadFood = false;
	public bool canLoadWood = false;
	public bool canLoadStone = false;

	// Use this for initialization
	void Awake ()
	{

		if (globalGameScript == null) {
			globalGameScript = GameObject.Find("GlobalGameScript").GetComponent<GlobalGameScript>();
		}

		for (int i = 0; i < platforms.Count; i++) {
			platformScripts.Add (platforms [i].GetComponent<Platform> ());
		}

		for (int i = 0; i < platformScripts.Count; i++) {
			if (platformScripts [i].farming) {
				farmingPlatforms.Add(platformScripts [i]);
				farmingPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
				platformsUnderConstruction.Add(platformScripts [i]);
			}else if(platformScripts [i].logging){
				loggingPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].quarry){
				quarryPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].mine){
				minePlatforms.Add(platformScripts[i]);
				miningPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
				platformsUnderConstruction.Add(platformScripts [i]);
			}
			else if(platformScripts [i].housing){
				housingPlatforms.Add(platformScripts[i]);
				housingPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
				platformsUnderConstruction.Add(platformScripts [i]);
			}
			else if(platformScripts [i].workshop){
				workshopPlatforms.Add(platformScripts[i]);
				workshopPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
				platformsUnderConstruction.Add(platformScripts [i]);
			}
			else if(platformScripts [i].archery){
				archeryPlatforms.Add(platformScripts[i]);
				archeryPlatformsUnderConstruction.Add(platformScripts [i]);// Setup all constructable platforms as "Under Construction"
				platformsUnderConstruction.Add(platformScripts [i]);
			}
		}


		for (int i = 0; i < npcs.Count; i++) {
			npcScripts.Add(npcs[i].GetComponent<NPC>());
			// TODO: make sure to update the King script when NPCs move between islands
			npcScripts[i].kingScript = this;// assign the King script to each NPC
		}

		if (npcScripts.Count > 0){
			king = npcScripts[0];// assign the first NPC to be the king
		}

	}

	void Start(){
		StartCoroutine(FirstTaskAssignments());
		FindRequiredResources ();// set the docks to display which resources are required
		CreateInitialResources();
	}

	void CreateInitialResources ()
	{
		
		if (initialFood > 0) {
			for (int i = 0; i < initialFood; i++) {
				CreateAndPositionResource (0);
				availableResources[0]++;
			}
		}
		if (initialWood > 0) {
			for (int i = 0; i < initialWood; i++) {
				CreateAndPositionResource (1);
				availableResources[1]++;
			}
		}
		if (initialStone > 0) {
			for (int i = 0; i < initialStone; i++) {
				CreateAndPositionResource (2);
				availableResources[2]++;
			}
		}
//		if (initialMetal > 0) {
//			for (int i = 0; i < availableResources [3]; i++) {
//				CreateAndPositionResource (3);
//				availableResources[3]++;
//			}
//		}
		
	}

	IEnumerator FirstTaskAssignments(){
		yield return new WaitForSeconds(setUpDelay);
		SetNPCTasks ();
	}

	public void SetNPCTasks ()
	{
//		Debug.Log("SET NPC TASKKKKK " + gameObject.name);
		// Update all NPC tasks
		for (int i = 0; i < npcScripts.Count; i++) {
//			Debug.Log("Set NPC TASK: " + npcScripts[i].gameObject.name);
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

	// called after modifying the king script's NPCs list
	public void UpdateNPCs ()
	{
		npcScripts.Clear();
		for (int i = 0; i < npcs.Count; i++) {
			npcScripts.Add(npcs[i].GetComponent<NPC>());
			// TODO: make sure to update the King script when NPCs move between islands
			npcScripts[i].kingScript = this;// assign the King script to each NPC
		}
		UpdatePeasantCount ();
	}

	void ResetNPCLists ()
	{
		peasants.Clear();// = new List<NPC>();
		builders.Clear();// = new List<NPC>();
		archers.Clear();// = new List<NPC>();
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

	void FindRequiredResources ()
	{

//		Debug.Log("Find REquired resources...");

		bool needFood = false;
		bool needWood = false;
		bool needStone = false;

		int totalFoodRequired = 0;
		int totalWoodRequired = 0;
		int totalStoneRequired = 0;


		for (int i = 0; i < platformsUnderConstruction.Count; i++) {
			if (platformsUnderConstruction [i].cost.Length > 0) {
				for (int j = 0; j < platformsUnderConstruction [i].cost.Length; j++) {
					if (j == 0) {
						totalFoodRequired += platformsUnderConstruction [i].cost [j];
					}
					if (j == 1) {
						totalWoodRequired += platformsUnderConstruction [i].cost [j];
					}
					if (j == 2) {
						totalStoneRequired += platformsUnderConstruction [i].cost [j];
					}
				}
			}
		}

		if (totalFoodRequired > availableResources[0]) {
			foodIcon.SetActive (true);
		} else {
			foodIcon.SetActive (false);
		}

		if (totalWoodRequired > availableResources[1]) {
			woodIcon.SetActive (true);
		} else {
			woodIcon.SetActive (false);
		}

		if (totalStoneRequired > availableResources[2]) {
			stoneIcon.SetActive (true);
		} else {
			stoneIcon.SetActive (false);
		}

	}

	// POSITION the resource at the docks
	// This is responsible for instantiating the resource object and positioning it properly at the docks
	public void CreateAndPositionResource (int resourceType)
	{
		int resourceColumns = 1;
		GameObject prefabToInst = null;
		Transform docksLocation = foodResourceStore;// just put placeholder value here
		Bounds resBounds = foodResourceBounds;// just put placeholder value here
		switch (resourceType) {
		case 0:
			resourceColumns = foodResourceColumns;
			prefabToInst = foodResourcePrefab;
			docksLocation = foodResourceStore;
			resBounds = foodResourceBounds;
			break;
		case 1:
			resourceColumns = woodResourceColumns;
			prefabToInst = woodResourcePrefab;
			docksLocation = woodResourceStore;
			resBounds = woodResourceBounds;
			break;
		case 2:
			resourceColumns = stoneResourceColumns;
			prefabToInst = stoneResourcePrefab;
			docksLocation = stoneResourceStore;
			resBounds = stoneResourceBounds;
			break;
		default:
			Debug.Log ("Fall through King.cs PositionResource()");
			break;
		}
		int resourcePositionIndex = availableResources [resourceType] - 1;
//		Debug.Log ("available resources: " + resourcePositionIndex);
		int yPosIndex = Mathf.FloorToInt (resourcePositionIndex / resourceColumns);
		int xPosIndex = resourcePositionIndex - (resourceColumns * yPosIndex);

		if (resourceType == 0) {
			GameObject res = GetFirstInactiveGameObjectFromList (globalGameScript.foods);
			if (res == null) {
				res = (GameObject)Instantiate (prefabToInst, new Vector3 (0,0,0), Quaternion.identity);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
				globalGameScript.foods.Add(res);
			} else {
				res.SetActive(true);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
			}
			foods.Add(res);// add to local King.cs list
		} else if (resourceType == 1) {
			GameObject res = GetFirstInactiveGameObjectFromList (globalGameScript.woods);
			if (res == null) {
				res = (GameObject)Instantiate (prefabToInst, new Vector3 (0,0,0), Quaternion.identity);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
				globalGameScript.woods.Add(res);
			} else {
				res.SetActive(true);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
			}
			woods.Add(res);// add to local King.cs list
		} else if (resourceType == 2) {
			GameObject res = GetFirstInactiveGameObjectFromList (globalGameScript.stones);
			if (res == null) {
				res = (GameObject)Instantiate (prefabToInst, new Vector3 (0,0,0), Quaternion.identity);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
//				Debug.Log("resBounds.size.x: " + resBounds.size.x);
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
				globalGameScript.stones.Add(res);
			} else {
				res.SetActive(true);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
//				Debug.Log("resBounds.size.x: " + resBounds.size.x);
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
			}
			stones.Add(res);// add to local King.cs list
		}else if (resourceType == 3){
			GameObject res = GetFirstInactiveGameObjectFromList (globalGameScript.metals);
			if (res == null) {
				res = (GameObject)Instantiate (prefabToInst, new Vector3 (0,0,0), Quaternion.identity);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
				globalGameScript.metals.Add(res);
			} else {
				res.SetActive(true);
				resBounds = res.GetComponent<BoxCollider2D>().bounds;
				res.transform.position = new Vector3 (docksLocation.position.x + resBounds.size.x * xPosIndex, docksLocation.position.y + resBounds.size.y * yPosIndex, docksLocation.position.z);
			}
			metals.Add(res);// add to local King.cs list
		}


	}

	public void RemoveAndPositionResource (int resourceType)
	{
//		Debug.Log("Remove resource from island");
		if (resourceType == 0) {
			if (foods.Count > 0) {
				GameObject res = foods[foods.Count - 1];
				foods.Remove(res);
				res.SetActive(false);
			}
		} else if (resourceType == 1) {
			if (woods.Count > 0) {
				GameObject res = woods[woods.Count - 1];
				woods.Remove(res);
				res.SetActive(false);
			}
		} else if (resourceType == 2) {
//			Debug.Log("Remove resource type 2");
			if (stones.Count > 0) {
//				Debug.Log("remove stone");
				GameObject res = stones[stones.Count - 1];
				stones.Remove(res);
//				Debug.Log("stone removed");
				res.SetActive(false);
			}
		}else if (resourceType == 3){
			if (metals.Count > 0) {
				GameObject res = metals[metals.Count - 1];
				metals.Remove(res);
				res.SetActive(false);
			}
		}
	}

	// Player over resource - RESOURCE SELECT
	public void SelectResource (int resourceType)
	{
		if (resourceType == 0) {
			canLoadFood = true;
			foodIcon.transform.position = new Vector3(foodIcon.transform.position.x, foodIcon.transform.position.y + 1, foodIcon.transform.position.z);
		}else if (resourceType == 1){
			canLoadWood = true;
			woodIcon.transform.position = new Vector3(woodIcon.transform.position.x, woodIcon.transform.position.y + 1, foodIcon.transform.position.z);
		}else if (resourceType == 2){
			canLoadStone = true;
			stoneIcon.transform.position = new Vector3(stoneIcon.transform.position.x, stoneIcon.transform.position.y + 1, stoneIcon.transform.position.z);
		}
	}
	public void DeSelectResource (int resourceType)
	{
		if (resourceType == 0) {
			canLoadFood = false;
			foodIcon.transform.position = new Vector3(foodIcon.transform.position.x, foodIcon.transform.position.y - 1, foodIcon.transform.position.z);
		}else if (resourceType == 1){
			canLoadWood = false;
			woodIcon.transform.position = new Vector3(woodIcon.transform.position.x, woodIcon.transform.position.y - 1, foodIcon.transform.position.z);
		}else if (resourceType == 2){
			canLoadStone = false;
			stoneIcon.transform.position = new Vector3(stoneIcon.transform.position.x, stoneIcon.transform.position.y - 1, stoneIcon.transform.position.z);
		}
	}

	// INFORM BUILDER THAT RESOURCE HAS ARRIVED --- used in NPC.cs
	public void CheckResourceArrived ()
	{
		// only run this again after the second is up... otherwise it might get out of control
		if (!checkingResourceArrived) {
//			Debug.Log ("Check resource arrived");
			FindRequiredResources();// Check if we have all the resources we require and switch the indicator either on or off
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

	public GameObject GetFirstInactiveGameObjectFromList (List<GameObject> list)
	{
		GameObject returnedGO = null;
		for (int i = 0; i < list.Count; i++) {
			if (!list [i].active) {
				returnedGO = list[i];
				break;
			}
		}

		return returnedGO;

	 }



}
