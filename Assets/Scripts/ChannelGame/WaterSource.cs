﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class WaterSource : MonoBehaviour {

	public ChannelsMaster masterScript;

	public ParticleSystem waterParticlesR;
	public ParticleSystem waterParticlesL;

	// add a small space to the raycast origin so that it doesn't collide with itself
	private float raycastOffset = 0.01f;
	public LayerMask waterHitLayer;

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
			WaterfallHit ();
			openChannel = false;
		}
		if (closeChannel) {
			waterParticlesR.Stop();
			waterParticlesL.Stop();
			StopFilling();
			closeChannel = false;
		}

	}

	void StopFilling ()
	{
		for (int i = 0; i < fillingPlatforms.Count; i++) {
			fillingPlatforms[i].currentFlowRateIn -= (flowRate/fillingPlatforms.Count);
		}
		fillingPlatforms.Clear();
		levelsToFillLeft.Clear();
		levelsToFillRight.Clear();
	}

	void WaterfallHit ()
	{
		float rayCastXPos;
		Debug.Log("transform.eulerAngles.z: " + transform.eulerAngles.z);
		if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180) {
			waterParticlesL.Play();
			rayCastXPos = myBounds.min.x - raycastOffset;
		} else {
			waterParticlesR.Play();
			rayCastXPos = myBounds.max.x + raycastOffset;
		}

		RaycastHit2D hit = Physics2D.Raycast (new Vector3 (rayCastXPos, myBounds.min.y + raycastOffset, transform.position.z), -Vector2.up, Mathf.Infinity, waterHitLayer);
		if (hit.collider != null) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
				Debug.Log("The ground was hit");
				Ground groundScript = hit.transform.gameObject.GetComponent<Ground> ();
				groundOrigin = groundScript;// test
				SetFillingOrder ();
			}else if (hit.transform.gameObject.layer == LayerMask.NameToLayer ("Channel")){
				Debug.Log("A channel was hit");
				WaterSource waterSourceScript = hit.transform.gameObject.GetComponent<WaterSource>();
				// add some delay related to the length of the channel
				waterSourceScript.openChannel = true;
			}
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

}
