using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class Ground : MonoBehaviour {

	public ChannelsMaster masterScript;

	// This platform's bounds
	private BoxCollider2D collider;
	public Bounds myBounds;

	public WaterSource waterSource;

	public GameObject waterLevel;
	public Bounds waterLevelBounds;
	public bool filling = false;
	public bool emptying = false;
	public float flowRate = 0.0f;
	public float currentFlowRateIn = 0.0f;
	public float currentFlowRateOut = 0.0f;

	public float elapsedTime = 0.0f;
	public float fillingRate = 1.0f;//update level every x seconds

	// regulate overflowing
	public bool overflowingForward = false;
	public bool overflowingBack = false;

	// Structure
	// could be a channel, windmill, tree, etc...
	public List<GroundStructure> structures = new List<GroundStructure>();
	public List<ProcessedResource> resources = new List<ProcessedResource>();
	public List<WorkerNPC> workers = new List<WorkerNPC>();


	public int groundIndex;

	void Start () {

		if (masterScript == null) {
			masterScript = GameObject.Find("ChannelsMaster").GetComponent<ChannelsMaster>();
		}

		collider = GetComponent<BoxCollider2D>();
		myBounds = collider.bounds;

		waterLevelBounds = waterLevel.GetComponent<BoxCollider2D>().bounds;
	}

	void Update ()
	{

		CheckAccumulation ();

		if (filling) {
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= fillingRate) {
				elapsedTime = 0.0f;
				IncreaseWaterLevel ();
			};
			CheckForOverflow ();
		} else if (emptying) {
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= fillingRate) {
				elapsedTime = 0.0f;
				DecreaseWaterLevel ();
			};
		}

	}

	void CheckAccumulation ()
	{
		flowRate = currentFlowRateIn - currentFlowRateOut;

		if (flowRate > 0) {
			filling = true;
			emptying = false;
		} else if (flowRate < 0) {
			Debug.Log("Empty the platform");
			filling = false;
			emptying = true;
		}else if (flowRate == 0){
			filling = false;
			emptying = false;
		}
	}

	public void InFlow(float inFlow){
		currentFlowRateIn += inFlow;

	}
	public void OutFlow(float outFlow){
		currentFlowRateOut += outFlow;

	}

	public void CheckForOverflow ()
	{
		Transform waterTransform = waterLevel.transform;
		float currentWaterLevel = waterTransform.position.y + waterLevelBounds.extents.y;

		// if the next platform's water level is lower
		if (groundIndex + 1 < masterScript.groundBounds.Count) {
			if (currentWaterLevel > masterScript.groundScripts [groundIndex + 1].waterLevelBounds.max.y) {
				if (!waterSource.SharedWaterLevel(this, masterScript.groundScripts [groundIndex + 1])){
					Debug.Log("----!!!!!!!-------Can overflow NEXT " + groundIndex);
					waterSource.SetFillingOrder();// tell the WaterSource.cs to recalculate which platforms to fill
				}
			}
		}

		// if the previous platform's water level is lower
		if (groundIndex - 1 > 0) {
			if (currentWaterLevel > masterScript.groundScripts [groundIndex - 1].waterLevelBounds.max.y) {
				if (!waterSource.SharedWaterLevel(this, masterScript.groundScripts [groundIndex - 1])){
					Debug.Log("----!!!!!!!-------Can overflow PREV " + groundIndex);
					waterSource.SetFillingOrder();// tell the WaterSource.cs to recalculate which platforms to fill
				}
			}
		}
	}


	void IncreaseWaterLevel ()
	{
		Transform waterTransform = waterLevel.transform;
		waterTransform.position = new Vector3 (waterTransform.position.x, waterTransform.position.y + flowRate, waterTransform.position.z);
	}

	void DecreaseWaterLevel ()
	{
		Transform waterTransform = waterLevel.transform;
		waterTransform.position = new Vector3(waterTransform.position.x, waterTransform.position.y + flowRate, waterTransform.position.z);

		if ((waterTransform.position.y + waterLevelBounds.extents.y) <= masterScript.groundBounds[groundIndex].max.y) {
			Debug.Log("Stop draining the platform... its empty");
		}

	}

	// Worker management
	public void ResetResourceFetching ()
	{
		for (int i = 0; i < workers.Count; i++) {
			if (workers [i].fetchingResource) {
				workers [i].FetchResource();
			}
		}
	}

}
