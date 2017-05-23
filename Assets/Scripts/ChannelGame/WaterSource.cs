using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class WaterSource : MonoBehaviour {

	public ChannelsMaster masterScript;

	public LayerMask groundLayer;

	public Bounds myBounds;

	public bool openChannel = false;
	public bool closeChannel = false;
	public bool running = false;
	public float flowRate = 1.0f;

	public List<Ground> fillingPlatforms = new List<Ground>();

	// Use this for initialization
	void Start ()
	{
		if (masterScript == null) {
			masterScript = GameObject.Find("ChannelsMaster").GetComponent<ChannelsMaster>();
		}
		myBounds = GetComponent<BoxCollider2D>().bounds;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (openChannel) {
//			masterScript.OpenChannel(this);
			WaterfallHit ();
			openChannel = false;
		}
		if (closeChannel) {
//			masterScript.CloseChannel(this);
			closeChannel = false;
		}

	}

	void WaterfallHit(){
		RaycastHit2D hit = Physics2D.Raycast(new Vector3(myBounds.max.x, myBounds.min.y, transform.position.z), -Vector2.up, Mathf.Infinity, groundLayer);
		if (hit.collider != null) {
            Debug.Log("Hit the ground object: " + hit.collider.gameObject.name);
			Ground groundScript = hit.collider.gameObject.GetComponent<Ground>();
			groundScript.waterSource = this;
			fillingPlatforms.Add(groundScript);
			UpdateFilledPlatforms();
        }
	}

	void ResetFilledPlatforms(){
		for (int i = 0; i < fillingPlatforms.Count; i++) {
			fillingPlatforms[i].currentFlowRateIn -= (flowRate/fillingPlatforms.Count);
		}
	}
	void UpdateFilledPlatforms ()
	{
		for (int i = 0; i < fillingPlatforms.Count; i++) {
			fillingPlatforms[i].InFlow(flowRate/fillingPlatforms.Count);
		}
	}

	public void AddFillingPlatform (Ground fillingPlatform)
	{
		Debug.Log ("Adding a filling platform");
		if (!fillingPlatforms.Contains (fillingPlatform)) {
			ResetFilledPlatforms();// first reset the currently filled platforms, by removing the previously calculated flowrate in
			fillingPlatforms.Add(fillingPlatform);// add a new platform... then
			UpdateFilledPlatforms();// add the newly calculated flowrate in.
		}
	}

}
