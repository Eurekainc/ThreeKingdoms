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

	// test
	public Ground groundOrigin;
	private bool addOriginToFill = false;
	public List<Ground> orderedFillingPlatforms = new List<Ground>();//test
	public List<Ground> levelsToFillLeft = new List<Ground>();
	public List<Ground> levelsToFillRight = new List<Ground>();

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
//            Debug.Log("Hit the ground object: " + hit.collider.gameObject.name);
			Ground groundScript = hit.collider.gameObject.GetComponent<Ground>();
//			groundScript.waterSource = this;
//			fillingPlatforms.Add(groundScript);
			groundOrigin = groundScript;// test
			SetFillingOrder();
        }
	}

	public void SetFillingOrder(){
		SetOrderedFillingPlatformOrigin();// test
		SetOrderedFillingPlatformsRight();// test
		SetOrderedFillingPlatformsLeft();// test
		UpdateFilledPlatforms();
	}

	void SetOrderedFillingPlatformOrigin ()
	{
		
		int index = groundOrigin.groundIndex;
		float lowestLevel = groundOrigin.waterLevel.transform.position.y + groundOrigin.waterLevelBounds.extents.y;

		bool blockedOnRight = false;
		bool blockedOnLeft = false;

		// check platforms on either side if both are higher then add the origin platform to the list
		// side note: origin platform refers to the platform where the water hits first

		if (index + 1 < masterScript.groundScripts.Count) {
			float thisWaterLevel = masterScript.groundScripts [index + 1].waterLevel.transform.position.y + masterScript.groundScripts [index + 1].waterLevelBounds.extents.y;
			if (thisWaterLevel > lowestLevel) {
				blockedOnRight = true;
			}
		}

		if (index - 1 >= 0) {
			float thisWaterLevel = masterScript.groundScripts [index - 1].waterLevel.transform.position.y + masterScript.groundScripts [index - 1].waterLevelBounds.extents.y;
			if (thisWaterLevel > lowestLevel) {
				blockedOnLeft = true;
			}
		}

		if (blockedOnRight && blockedOnLeft) {
			addOriginToFill = true;
		} else {
			addOriginToFill = false;
		}

	}
	void SetOrderedFillingPlatformsRight ()
	{
		
		int index = groundOrigin.groundIndex;
		float lowestLevel = groundOrigin.waterLevel.transform.position.y + groundOrigin.waterLevelBounds.extents.y;
		bool stop = false;
		int maxLimit = 0;

		// set platforms on right
		levelsToFillRight = new List<Ground>();
		while (!stop && maxLimit < 100 && (index + maxLimit) < (masterScript.groundScripts.Count-1)) {
			maxLimit++;
			float thisWaterLevel = masterScript.groundScripts [index + maxLimit].waterLevel.transform.position.y + masterScript.groundScripts [index + maxLimit].waterLevelBounds.extents.y;
			if (thisWaterLevel > lowestLevel) {
				stop = true;
				maxLimit = 0;
			}else if (thisWaterLevel == lowestLevel){
				levelsToFillRight.Add(masterScript.groundScripts [index + maxLimit]);
			}else if (thisWaterLevel < lowestLevel){
				lowestLevel = thisWaterLevel;// reset the lowest level
				levelsToFillRight.Clear();
				levelsToFillRight.Add(masterScript.groundScripts [index + maxLimit]);
			}
		}

	}
	void SetOrderedFillingPlatformsLeft ()
	{
		
		int index = groundOrigin.groundIndex;
		float lowestLevel = groundOrigin.waterLevel.transform.position.y + groundOrigin.waterLevelBounds.extents.y;
		bool stop = false;
		int maxLimit = 0;

		// set platforms on left
		levelsToFillLeft = new List<Ground>();
		while (!stop && maxLimit < 100 && (index - maxLimit) > 0) {
			maxLimit++;
			float thisWaterLevel = masterScript.groundScripts [index - maxLimit].waterLevel.transform.position.y + masterScript.groundScripts [index - maxLimit].waterLevelBounds.extents.y;
			if (thisWaterLevel > lowestLevel) {
				Debug.Log("StopL");
				stop = true;
				maxLimit = 0;
			}else if (thisWaterLevel == lowestLevel){
				Debug.Log("LevelL");
				levelsToFillLeft.Add(masterScript.groundScripts [index - maxLimit]);
			}else if (thisWaterLevel < lowestLevel){
				Debug.Log("Fill this one: " + (index - maxLimit));
				lowestLevel = thisWaterLevel;// reset the lowest level
				levelsToFillLeft.Clear();
				levelsToFillLeft.Add(masterScript.groundScripts [index - maxLimit]);
			}
		}

	}

	public bool SharedWaterLevel (Ground scriptA, Ground scriptB)
	{
		bool shared = false;
		if (levelsToFillRight.Contains (scriptA) && levelsToFillRight.Contains (scriptB)) {
			shared = true;
		}
		if (levelsToFillLeft.Contains (scriptA) && levelsToFillLeft.Contains (scriptB)) {
			shared = true;
		}

		return shared;
	}

	void ResetFilledPlatforms(){
		for (int i = 0; i < fillingPlatforms.Count; i++) {
			fillingPlatforms[i].currentFlowRateIn -= (flowRate/fillingPlatforms.Count);
		}
	}
	void UpdateFilledPlatforms ()
	{
		ResetFilledPlatforms();
		fillingPlatforms.Clear ();
//		fillingPlatforms.Add(groundOrigin);
		for (int i = 0; i < levelsToFillRight.Count; i++) {
			if (!fillingPlatforms.Contains (levelsToFillRight [i])) {
				fillingPlatforms.Add (levelsToFillRight [i]);
			}
		}
		for (int i = 0; i < levelsToFillLeft.Count; i++) {
			if (!fillingPlatforms.Contains (levelsToFillLeft [i])) {
				fillingPlatforms.Add (levelsToFillLeft [i]);
			}
		}

		// if the water level on either side of the source of the water is >= then fill the origin platform as well
		if (addOriginToFill) {
			fillingPlatforms.Add (groundOrigin);
		}

		for (int i = 0; i < fillingPlatforms.Count; i++) {
			fillingPlatforms[i].waterSource = this;
			fillingPlatforms[i].InFlow(flowRate/fillingPlatforms.Count);
		}
	}

//	public void AddFillingPlatform (Ground fillingPlatform)
//	{
//		Debug.Log ("Adding a filling platform");
//		if (!fillingPlatforms.Contains (fillingPlatform)) {
//			ResetFilledPlatforms();// first reset the currently filled platforms, by removing the previously calculated flowrate in
//			fillingPlatforms.Add(fillingPlatform);// add a new platform... then
//			UpdateFilledPlatforms();// add the newly calculated flowrate in.
//		}
//	}

}
