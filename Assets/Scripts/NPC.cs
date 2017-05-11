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
	// 0-> woodcutter
	// 1 -> quarryman
	// 2 -> farmer
	// 3 -> Builder
	// 4 -> Archer
	// 5 -> King

	public int occupation = 0;

	// some triggers for waiting NPCs
	public bool waitingForFarmingPlatform = false;

	// Moving between platforms state
	public Platform destinationPlatformScript = null;// can be private
	public bool movingToPlatform = false;

	// Carrying resources / Ferry
	public bool goingToFerry = false;
	public bool waitingForFerry = false;
	public float ferryWaitTime = 2.0f;
	public bool carryingResource = false;
	public int carriedResourceIndex;

	// Archers
	public GameObject projectilePrefab;
	public LayerMask enemyLayer;
	private CircleCollider2D collider;
	private bool patrolling = false;
	private bool attacking = false;
	public float attackPositionDelay = 1.0f;// slight delay before moving to new position, so that its not a continuous navmesh calculation
	private Vector3 attackPosition;
	private bool firing = false;
	public float reloadTime = 1.0f;

	// King
	private bool parading = false;

	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
		collider = GetComponent<CircleCollider2D> ();
//		if (kingScript != null) {
//			Init();
//		}

	}

	public void FindTask(){
		if (occupation == 0) {
//			FindTaskPeasant ();
			FindForrest();
		}else if (occupation == 1) {
			FindQuarry();
		}else if (occupation == 2) {
			FindFarm();
		}else if (occupation == 3) {
//			FindTaskBuilder ();
			FindBuildTask();
		}else if (occupation == 4) {
			Patrol();
		} else if (occupation == 5) {
			Parade();
		}
	}

	void Update ()
	{

		// TODO: change this to fire CheckTask on trigger enter or something...
		if (movingToPlatform) {
			if (!agent.pathPending) {
				if (agent.remainingDistance <= agent.stoppingDistance) {
					if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
						Debug.Log ("ARRIVED>>>>>");
						movingToPlatform = false;
						CheckTask ();
					}
				}
			}
		}

		// Archers scanning for enemies
		// can continuously check if enemy is attacking, but for performance and gameplay, might want to handicap this, put it in a coroutine or something
		if (occupation == 4) {
			Collider2D enemyInRange = Physics2D.OverlapCircle (transform.position, collider.radius, enemyLayer);
			if (enemyInRange != null) {
				if (!kingScript.enemySighted) {
					kingScript.AlertArchers (enemyInRange);
				}
				if (!firing) {
					firing = true;
					StartCoroutine(FireProjectile());
				}
			}
		}

	}

	void FindFarm ()
	{
		int builtFarms = kingScript.builtFarmingPlatforms.Count;
		if (builtFarms > 0) {
			destinationPlatformScript = kingScript.FindPlatformWithLowestPopulation (kingScript.builtFarmingPlatforms, kingScript.platformPeasant);
			kingScript.AddNPCToPopulation (destinationPlatformScript, kingScript.platformPeasant);
			active = true;
			GoToWorkPlatformPeasant ();
		} else {
			waitingForFarmingPlatform = true;// this is used when a farm is built, then can call this particular NPC to go farming
		}
	}
	void FindForrest ()
	{

		destinationPlatformScript = kingScript.FindPlatformWithLowestPopulation(kingScript.loggingPlatforms, kingScript.platformPeasant);
		kingScript.AddNPCToPopulation(destinationPlatformScript, kingScript.platformPeasant);
		active = true;
		GoToWorkPlatformPeasant ();

	}

	void FindQuarry ()
	{

		destinationPlatformScript = kingScript.FindPlatformWithLowestPopulation(kingScript.quarryPlatforms, kingScript.platformPeasant);
		kingScript.AddNPCToPopulation(destinationPlatformScript, kingScript.platformPeasant);
		active = true;
		GoToWorkPlatformPeasant ();

	}


	// called at NPC start and again everytime a builder completes a build
	void FindBuildTask ()
	{

		bool haveTask = false;

		int farmingPlatformsToBuild = kingScript.farmingPlatformsUnderConstruction.Count;
		int builtFarms = kingScript.builtFarmingPlatforms.Count;

		int housingPlatformsToBuild = kingScript.housingPlatformsUnderConstruction.Count;
		int builtHouses = kingScript.builtHousingPlatforms.Count;

		int workshopPlatformsToBuild = kingScript.workshopPlatformsUnderConstruction.Count;
		int builtWorkshops = kingScript.builtWorkshopPlatforms.Count;

		int archeryPlatformsToBuild = kingScript.archeryPlatformsUnderConstruction.Count;
		int builtArcheryRanges = kingScript.builtArcheryPlatforms.Count;

		if (farmingPlatformsToBuild > 0 && builtFarms <= 0) {
			destinationPlatformScript = kingScript.FindPlatformWithLowestPopulation (kingScript.farmingPlatformsUnderConstruction, kingScript.platformBuilder);
			kingScript.AddNPCToPopulation (destinationPlatformScript, kingScript.platformBuilder);
			active = true;
			haveTask = true;
			GoToDocksBuilder ();
		}

		// if there are houses to build, at least 1 farm and no built houses and not already building
		if (housingPlatformsToBuild > 0 && builtFarms > 0 && builtHouses <= 0 && !haveTask){
			destinationPlatformScript = kingScript.FindPlatformWithLowestPopulation (kingScript.housingPlatformsUnderConstruction, kingScript.platformBuilder);
			kingScript.AddNPCToPopulation (destinationPlatformScript, kingScript.platformBuilder);
			active = true;
			haveTask = true;
			GoToDocksBuilder ();
		}

		// if there are workshops to build, at least 1 farm & 1 house and no built workshops and not already building
		if (workshopPlatformsToBuild > 0 && builtFarms > 0 && builtHouses > 0 && builtWorkshops <= 0 && !haveTask){
			destinationPlatformScript = kingScript.FindPlatformWithLowestPopulation (kingScript.workshopPlatformsUnderConstruction, kingScript.platformBuilder);
			kingScript.AddNPCToPopulation (destinationPlatformScript, kingScript.platformBuilder);
			active = true;
			haveTask = true;
			GoToDocksBuilder ();
		}

		// if there are archery ranges to build, at least 1 farm & 1 house & 1 workshop and no built archery ranges and not already building
		if (archeryPlatformsToBuild > 0 && builtFarms > 0 && builtHouses > 0 && builtWorkshops > 0 && builtArcheryRanges <= 0 && !haveTask){
			destinationPlatformScript = kingScript.FindPlatformWithLowestPopulation (kingScript.archeryPlatformsUnderConstruction, kingScript.platformBuilder);
			kingScript.AddNPCToPopulation (destinationPlatformScript, kingScript.platformBuilder);
			active = true;
			haveTask = true;
			GoToDocksBuilder ();
		}

	}

	// Archers

	// this is public because its called from King.cs when an enemy is spotted ( King.cs AlertArchers() )
	public void Patrol ()
	{
		Debug.Log("Patrol...");
		// choose a random platform and move there, once reached choose another platform... until needed for attack
		int randomPlatformIndex = Random.Range (0, kingScript.platformScripts.Count);
		destinationPlatformScript = kingScript.platformScripts [randomPlatformIndex];
		patrolling = true;

		if (kingScript.enemySighted) {
			Debug.Log("Enemy sighted move to attack...");
			StartCoroutine(AttackPosition());
		} else {
			GoToPatrolPlatformArcher ();
		};
	}

	IEnumerator AttackPosition(){
		yield return new WaitForSeconds(attackPositionDelay);
		NavMeshHit hit;

		if (NavMesh.SamplePosition(kingScript.enemyTransform.position, out hit, 10.0f, NavMesh.AllAreas)) {
			attackPosition = hit.position;
		}

		Debug.Log("attackPosition: " + attackPosition);

		GoToAttackPositionArcher ();
	}
	IEnumerator FireProjectile ()
	{
		yield return new WaitForSeconds(reloadTime);
		Debug.Log("Fire!!!!");
		GameObject projectile = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
		Projectile projectileScript = projectile.GetComponent<Projectile>();
		projectileScript.endPoint = kingScript.enemyTransform.position;
		projectileScript.shoot = true;
		firing = false;
	}

//	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)// for 2 control points
	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)// for 1 control point
	{
	  float u = 1-t;
	  float tt = t*t;
	  float uu = u*u;
	  float uuu = uu * u;
	  float ttt = tt * t;
	 
//	  Vector3 p = uuu * p0; //first term
//	  p += 3 * uu * t * p1; //second term
//	  p += 3 * u * tt * p2; //third term
//	  p += ttt * p3; //fourth term

		// for a single control point
		Vector3 p = uu * p0; //first term
		p += 2 * u * t * p1; //second term
		p += tt * p2; //third term
	 
	  return p;
	}

	// King
	void Parade(){
		// choose a random platform and move there, once reached choose another platform... until needed for something else
		int randomPlatformIndex = Random.Range(0, kingScript.platformScripts.Count);
		destinationPlatformScript = kingScript.platformScripts[randomPlatformIndex];
		parading = true;
		GoToParadePlatformKing();
	}



	// MOVEMENT
	// NPCs moving to their respective platforms...
	// Farmer, Woodcutter, Quarryman
	void GoToWorkPlatformPeasant()
	{
		agent.SetDestination (destinationPlatformScript.transform.position);
		movingToPlatform = true;
	}
	void GoToDocksPeasant (int resourceIndex)
	{
		Debug.Log ("resourceIndex: " + resourceIndex);
		if (resourceIndex == 0) {
			Debug.Log("Go to food store");
			agent.SetDestination (kingScript.foodResourceStore.position);
//			agent.SetDestination (kingScript.docks.transform.position);
		}else if (resourceIndex == 1) {
			Debug.Log("Go to wood store");
			agent.SetDestination (kingScript.woodResourceStore.position);
//			agent.SetDestination (kingScript.docks.transform.position);
		}else if (resourceIndex == 2) {
			Debug.Log("Go to stone store");
			agent.SetDestination (kingScript.stoneResourceStore.position);
//			agent.SetDestination (kingScript.docks.transform.position);
		}else {
			agent.SetDestination (kingScript.docks.transform.position);
		} 
		movingToPlatform = true;
	}

	// Builder
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

	// Archer
	void GoToPatrolPlatformArcher()
	{
		agent.SetDestination (destinationPlatformScript.transform.position);
		movingToPlatform = true;
	}
	void GoToAttackPositionArcher(){
		agent.SetDestination (attackPosition);
		movingToPlatform = true;
	}

	// King
	void GoToParadePlatformKing()
	{
		agent.SetDestination (destinationPlatformScript.transform.position);
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

		if (goingToFerry) {
			Debug.Log("Arrived at ferry");
			kingScript.npcsWaitingForFerry.Add(this);
			goingToFerry = false;
			waitingForFerry = true;
			StartCoroutine(WaitAtFerry());
		} else {

			if (occupation == 0) {
				if (carryingResource) { 
					StartCoroutine (OffloadAtDocks ());
				} else { 
					StartCoroutine (GatherWood ());
				}
				;
			} else if (occupation == 1) {
				if (carryingResource) { 
					StartCoroutine (OffloadAtDocks ());
				} else { 
					StartCoroutine (GatherStone ());
				}
				;
			} else if (occupation == 2) {
				if (carryingResource) { 
					StartCoroutine (OffloadAtDocks ());
				} else { 
					StartCoroutine (Farm ());
				}
				;
			}

		// if its a builder (this is called once arriving at docks or build site depending on whether they're carrying a resource
		else if (occupation == 3) {
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
						StartCoroutine (BuildSectionOfStructure ());
					} else if (destinationPlatformScript.quarry) {
						StartCoroutine (BuildSectionOfStructure ());
					} else if (destinationPlatformScript.mine) {
						StartCoroutine (BuildSectionOfStructure ());
					} else if (destinationPlatformScript.housing) {
						StartCoroutine (BuildSectionOfStructure ());
					} else if (destinationPlatformScript.workshop) {
						StartCoroutine (BuildSectionOfStructure ());
					} else if (destinationPlatformScript.archery) {
						StartCoroutine (BuildSectionOfStructure ());
					}
				}
			} else if (occupation == 4) {
				Patrol ();// check for enemies in Patrol function
			} else if (occupation == 5) {
				// if nobody is attacking, then parade, else... move to castle???
				Parade ();
			}
		}
	}

	// this tells NPC to go to be picked up by the boat
	public void GoToFerry ()
	{
		Debug.Log("Going to ferry");
		agent.SetDestination (kingScript.ferryPickUp.position);
		goingToFerry = true;
		movingToPlatform = true;
	}

	IEnumerator WaitAtFerry(){
		yield return new WaitForSeconds(ferryWaitTime);
		kingScript.npcsWaitingForFerry.Remove(this);
		waitingForFerry = false;
		movingToPlatform = false;
		CheckTask();
	}

	public void PickUp(){
		kingScript.npcs.Remove(gameObject);
		kingScript.UpdateNPCs ();
		gameObject.SetActive(false);
	}
	public void DropOff(Vector3 newPosition, King newKingScript){
		agent.Warp(newPosition);
		kingScript = newKingScript;
		kingScript.npcs.Add(gameObject);
		kingScript.UpdateNPCs ();

	}


	// Tasks
	// TODO: inactivate peasant work platform area if the work area is damaged by attacks or is depleted, then set to inactive and find a new task
	IEnumerator Farm(){
		yield return new WaitForSeconds(kingScript.farmTime);
		carriedResourceIndex = 0;
		carryingResource = true;
		GoToDocksPeasant(carriedResourceIndex);
	}
	IEnumerator GatherWood(){
		yield return new WaitForSeconds(kingScript.loggingTime);
		carriedResourceIndex = 1;
		carryingResource = true;
		GoToDocksPeasant(carriedResourceIndex);
	}
	IEnumerator GatherStone(){
		yield return new WaitForSeconds(kingScript.quarryingTime);
		carriedResourceIndex = 2;
		carryingResource = true;
		GoToDocksPeasant(carriedResourceIndex);
	}
	IEnumerator Mining(){
		yield return new WaitForSeconds(kingScript.miningTime);
		carriedResourceIndex = 3;
		carryingResource = true;
		GoToDocksPeasant(carriedResourceIndex);
	}

	IEnumerator OffloadAtDocks ()
	{
		yield return new WaitForSeconds (kingScript.offloadTime);

		kingScript.availableResources [carriedResourceIndex]++;
		carryingResource = false;

		// instantiating resource object
//		if (carriedResourceIndex == 0) {
//			GameObject foodResource = (GameObject)Instantiate(kingScript.foodResourcePrefab, kingScript.foodResourceStore.position, Quaternion.identity);
//		}else if (carriedResourceIndex == 1){
//			GameObject woodResource = (GameObject)Instantiate(kingScript.woodResourcePrefab, kingScript.woodResourceStore.position, Quaternion.identity);
//		}else if (carriedResourceIndex == 2){
//			GameObject stoneResource = (GameObject)Instantiate(kingScript.stoneResourcePrefab, kingScript.stoneResourceStore.position, Quaternion.identity);
//		}

		kingScript.CheckResourceArrived ();// activate builder at front of queue to check and see if his resource has arrived, if not then go to the back of the queue
		GoToWorkPlatformPeasant();
	}



	// Building Tasks
	// this is called from King.cs when a peasant drops off resource, then flags the first builder in the queue to get the resource
	public void GetResource ()
	{
		kingScript.resourceQueue.Remove (this);
		bool gotResource = false;
		for (int i = 0; i < destinationPlatformScript.cost.Length; i++) {
			if (destinationPlatformScript.cost [i] > 0 && kingScript.availableResources [i] > 0) {
				kingScript.availableResources [i]--;
				carriedResourceIndex = i;
				gotResource = true;
				break;
			}
		}
		if (gotResource) {
			carryingResource = true;
			GoToWorkPlatformBuilder();
		} else {
			kingScript.resourceQueue.Add (this);

			// if there are other builders in line then tell the next builder that a resource has arrived
			if (kingScript.resourceQueue.Count > 1) {
				kingScript.CheckResourceArrived ();
			}
		}
	}

	IEnumerator BuildSectionOfStructure ()
	{
		yield return new WaitForSeconds (kingScript.buildTime);
		// subtract the carried resource form the cost
		destinationPlatformScript.cost [carriedResourceIndex]--;
		carryingResource = false;

		int itemsStillRequired = 0;
		for (int i = 0; i < destinationPlatformScript.cost.Length; i++) {
			itemsStillRequired += destinationPlatformScript.cost [i];
		}

		if (itemsStillRequired <= 0) {
			ActivateStructure (destinationPlatformScript);
			// decrement the platformBuilder counter
			// TODO: possibly inform other builders here that the platform has been built and they should go back to docks to return resources
			if (kingScript.platformBuilder.ContainsKey (destinationPlatformScript)) {
				kingScript.platformBuilder [destinationPlatformScript]--;
			}
			kingScript.NotifyWaitingPeasants ();// notify any peasants that may have been waiting for this structure
			active = false;
			FindBuildTask ();
		} else {
			SendBuilderToDocks(destinationPlatformScript.transform, destinationPlatformScript);
		}
	}

	void ActivateStructure (Platform completedStructure)
	{
		// the Platform script takes care of removing builder and under construction items from the King.cs lists
		completedStructure.ActivateStructure();
	}

}
