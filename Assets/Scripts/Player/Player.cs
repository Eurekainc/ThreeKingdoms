using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public LayerMask resourceLayer;
	public LayerMask npcLayer;

	public King currentKingScript = null;

	public Transform resourceSensor;

	private ResourceMarker resourceMarker = null;

	public bool atDocks = false;

	public int maxInventory = 10;
	public int currentInventory = 0;
	public int[] inventoryResources = new int[4];
	public List<NPC> passengers = new List<NPC>();

	// NPCs in range to be picked up
	public List<NPC> npcsInRange = new List<NPC>();

	public NPC calledNPC = null;
	public GameObject ring;
	private bool callingNPC = false;
	public float ringTime = 2.0f;
	private float ringElapsedTime = 0.0f;

	// Detect which resources to load
	// this also appears in King.cs
	public bool canLoadFood = false;
	public bool canLoadWood = false;
	public bool canLoadStone = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		// RESOURCE SENSING

		if (Input.GetKeyDown ("down")) {
			if (currentInventory < maxInventory) {
				Debug.Log ("Pick up NPC or resource");
				// picking up resource takes priority over NPC, because itherwise may interrupt NPC from doing work
				if (canLoadFood && currentKingScript.availableResources[0] > 0){
					Debug.Log("Pick up FOOD");
					inventoryResources[0]++;
					currentKingScript.availableResources[0]--;
					currentKingScript.RemoveAndPositionResource (0);
					currentInventory++;
				}
				else if (canLoadWood && currentKingScript.availableResources[1] > 0){
					Debug.Log("Pick up WOOD");
					inventoryResources[1]++;
					currentKingScript.availableResources[1]--;
					currentKingScript.RemoveAndPositionResource (1);
					currentInventory++;
				}else if (canLoadStone && currentKingScript.availableResources[2] > 0){
					Debug.Log("Pick up STONE");
					inventoryResources[2]++;
					currentKingScript.availableResources[2]--;
					currentKingScript.RemoveAndPositionResource (2);
					currentInventory++;
				}
				else if (npcsInRange.Count > 0) {
					passengers.Add (npcsInRange[npcsInRange.Count - 1]);
					npcsInRange[npcsInRange.Count - 1].PickUp ();
					currentInventory++;
				}

			} else {
				Debug.Log ("Inventory full");
			}
		}

		if (Input.GetKeyDown ("up")) {
			if (canLoadFood && inventoryResources[0] > 0){
				Debug.Log("Drop off FOOD");
				inventoryResources[0]--;
				currentKingScript.availableResources[0]++;
				currentKingScript.CreateAndPositionResource (0);
				currentKingScript.CheckResourceArrived ();// activate builder at front of queue to check and see if his resource has arrived, if not then go to the back of the queue
			}
			else if (canLoadWood && inventoryResources[1] > 0){
				Debug.Log("Drop off WOOD");
				inventoryResources[1]--;
				currentKingScript.availableResources[1]++;
				currentKingScript.CreateAndPositionResource (1);
				currentKingScript.CheckResourceArrived ();// activate builder at front of queue to check and see if his resource has arrived, if not then go to the back of the queue
			}else if (canLoadStone && inventoryResources[2] > 0){
				Debug.Log("Drop off STONE");
				inventoryResources[2]--;
				currentKingScript.availableResources[2]++;
				currentKingScript.CreateAndPositionResource (2);
				currentKingScript.CheckResourceArrived ();// activate builder at front of queue to check and see if his resource has arrived, if not then go to the back of the queue
			}
			else if (passengers.Count > 0 && currentKingScript != null) {
				NPC droppedOffNPC = passengers[passengers.Count - 1];
				droppedOffNPC.gameObject.SetActive(true);
				droppedOffNPC.DropOff(currentKingScript.foodResourceStore.position, currentKingScript);// Drop NPC off at specific position on teh docks... food point for now
				passengers.RemoveAt (passengers.Count - 1);
				currentInventory--;
			} else {
				Debug.Log ("Haven't got any NPCs in the inventory... or there's no currentKingScript");
			}
		}

		transform.Translate(new Vector3(0.3f * input.x, 0, 0));

	}

//	void CallNPC (float fracTime)
//	{
//		Debug.Log ("Call NPC");
//		if (!ring.active) {
//			ring.SetActive (true);
//		}
//		ring.transform.localScale = Vector3.Lerp(ring.transform.localScale, new Vector3(20, 20, 20), fracTime);
//	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("NPC")) {
			NPC npcScript = col.gameObject.GetComponent<NPC> ();
			npcsInRange.Add (npcScript);
//			if (npcScript.waitingForFerry) {
//				Debug.Log("can pick up NPC...");
//			}
		}
		if (col.CompareTag ("Docks")) {
			GameObject docksObj = col.gameObject;
			currentKingScript = docksObj.GetComponent<Platform>().kingScript;
		}

	}


	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("NPC")) {
			NPC npcScript = col.gameObject.GetComponent<NPC> ();
			npcsInRange.Remove(npcScript);
//			if (npcScript.waitingForFerry) {
//				Debug.Log("can pick up NPC...");
//			}
		}

		// Beware, if islands are too close together and 1 docks is entered before another is exited, it might clear the new docks object when it actuially wants to clear the old docks kingScript
		if (col.CompareTag ("Docks")) {
			currentKingScript = null;
		}
	}



}
