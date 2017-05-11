using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

//		// TODO: find a reliable way to only detect one resource type at a time
		Collider2D resourceObj = Physics2D.OverlapCircle (resourceSensor.position, 1.0f, resourceLayer);
		if (resourceObj != null) {
			if (resourceMarker == null) {
				resourceMarker = resourceObj.gameObject.GetComponent<ResourceMarker> ();
				resourceMarker.selected = true;
			}

			if (Input.GetKeyDown ("down")) {
				if (currentInventory < maxInventory) {
					currentInventory++;
					Debug.Log ("Pick up object / resource");
					int resourceIndex = resourceMarker.RemoveResource ();
					inventoryResources [resourceIndex]++;
				} else {
					Debug.Log ("Inventory full");
				}
			}
			if (Input.GetKeyDown ("up")) {
				if (inventoryResources [resourceMarker.resourceIndex] > 0) {
					Debug.Log ("Drop off object / resource");
					int resourceIndex = resourceMarker.AddResource ();
					inventoryResources [resourceIndex]--;
					if (currentInventory > 0) {
						currentInventory--;
					}
				} else {
					Debug.Log ("Haven't got any of this in the inventory... " + resourceMarker.resourceIndex);
				}
			}
		}
		if (resourceObj == null && resourceMarker != null) {
			resourceMarker.selected = false;
			resourceMarker = null;
		}

		// Calling NPC, maybe jumping if on land???
		if (Input.GetButtonDown ("Fire3") && !callingNPC) {
			callingNPC = true;
		}

		if (callingNPC) {
			CallNPC (ringElapsedTime / ringTime);
			ringElapsedTime += Time.deltaTime;

			if (ringElapsedTime >= ringTime) {
				ringElapsedTime = 0.0f;
				callingNPC = false;
				ring.transform.localScale = new Vector3 (1, 1, 1);
				ring.SetActive (false);
			}
		}

		// Picking up NPC
//		if (currentKingScript != null) {
//			Collider2D waitingNPC = Physics2D.OverlapCircle (currentKingScript.ferryPickUp.position, 1.0f, npcLayer);
//			if (waitingNPC != null) {
//				if (Input.GetKeyDown ("down")) {
//					if (currentInventory < maxInventory) {
//						Debug.Log ("Pick up NPC");
//						NPC pickedUpNPC = waitingNPC.gameObject.GetComponent<NPC> ();
//						pickedUpNPC.PickUp();
//						passengers.Add (pickedUpNPC);
//						currentInventory++;
//
//					} else {
//						Debug.Log ("Inventory full");
//					}
//				}
//
//				if (Input.GetKeyDown ("up")) {
//					if (passengers.Count > 0) {
//						Debug.Log ("Drop off NPC");
//						NPC droppedOffNPC = passengers[passengers.Count - 1];
//						droppedOffNPC.gameObject.SetActive(true);
//						droppedOffNPC.DropOff(currentKingScript.ferryPickUp.position, currentKingScript);
//						passengers.RemoveAt (passengers.Count - 1);
//						currentInventory--;
//					} else {
//						Debug.Log ("Haven't got any NPCs in the inventory... ");
//					}
//				}
//			}
//		}
		if (Input.GetKeyDown ("down")) {
			if (currentInventory < maxInventory) {
				Debug.Log ("Pick up NPC");
				if (npcsInRange.Count > 0) {
					passengers.Add (npcsInRange[npcsInRange.Count - 1]);
					npcsInRange[npcsInRange.Count - 1].PickUp ();
					npcsInRange.RemoveAt(npcsInRange.Count - 1);
					currentInventory++;
				}

			} else {
				Debug.Log ("Inventory full");
			}
		}

		if (Input.GetKeyDown ("up")) {
			if (passengers.Count > 0) {
				Debug.Log ("Drop off NPC");
				NPC droppedOffNPC = passengers[passengers.Count - 1];
				droppedOffNPC.gameObject.SetActive(true);
				droppedOffNPC.DropOff(currentKingScript.ferryPickUp.position, currentKingScript);
				passengers.RemoveAt (passengers.Count - 1);
				currentInventory--;
			} else {
				Debug.Log ("Haven't got any NPCs in the inventory... ");
			}
		}

		transform.Translate(new Vector3(0.8f * input.x, 0, 0));

	}

	void CallNPC (float fracTime)
	{
		Debug.Log ("Call NPC");
		if (!ring.active) {
			ring.SetActive (true);
		}
		ring.transform.localScale = Vector3.Lerp(ring.transform.localScale, new Vector3(20, 20, 20), fracTime);
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("NPC")) {
			NPC npcScript = col.gameObject.GetComponent<NPC> ();
			npcsInRange.Add (npcScript);
//			if (npcScript.waitingForFerry) {
//				Debug.Log("can pick up NPC...");
//			}
		}
		if (col.CompareTag ("Island")) {
			currentKingScript = col.gameObject.GetComponent<King>();
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

		// maybe don't clear king script in case exiting a trigger after entering another trigger
//		if (col.CompareTag ("Island")) {
//			currentKingScript = null;
//		}
	}



}
