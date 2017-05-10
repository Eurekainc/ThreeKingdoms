using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public LayerMask resourceLayer;

	public Transform resourceSensor;

	private ResourceMarker resourceMarker = null;

	public int maxInventory = 10;
	public int currentInventory = 0;
	public int[] inventoryResources = new int[4];

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
				} else {
					Debug.Log("Haven't got any of this in the inventory... " + resourceMarker.resourceIndex);
				}
			}
		}
		if (resourceObj == null && resourceMarker != null) {
			resourceMarker.selected = false;
			resourceMarker = null;
		}

		transform.Translate(new Vector3(0.8f * input.x, 0, 0));

	}



}
