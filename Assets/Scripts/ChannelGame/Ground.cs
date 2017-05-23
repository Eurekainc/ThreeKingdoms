using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	public ChannelsMaster masterScript;

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


	public int groundIndex;

	// Use this for initialization
	void Start () {

		if (masterScript == null) {
			masterScript = GameObject.Find("ChannelsMaster").GetComponent<ChannelsMaster>();
		}

		waterLevelBounds = waterLevel.GetComponent<BoxCollider2D>().bounds;
	}
	
	// Update is called once per frame
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

		Debug.Log("Platform filling at rate: " + inFlow);
		currentFlowRateIn += inFlow;

	}
	public void OutFlow(float outFlow){

		Debug.Log("Platform draining at rate: " + outFlow);
		currentFlowRateOut += outFlow;

	}

	public void StopFillPlatform(){

		Debug.Log("Stop Platform filling");
		filling = false;
		currentFlowRateIn = 0.0f;

	}

	public void CheckForOverflow ()
	{
		Transform waterTransform = waterLevel.transform;
		if (groundIndex + 1 < masterScript.groundBounds.Count) {

			float currentWaterLevel = waterTransform.position.y + waterLevelBounds.extents.y;

			if (currentWaterLevel >= masterScript.groundScripts [groundIndex + 1].waterLevelBounds.max.y) {
				Debug.Log ("Can Overflow to NEXT platform");
				waterSource.AddFillingPlatform (masterScript.groundScripts [groundIndex + 1]);
				masterScript.groundScripts [groundIndex + 1].waterSource = waterSource;



				if (currentWaterLevel > masterScript.groundScripts [groundIndex + 1].waterLevelBounds.max.y) {
					currentFlowRateOut = currentFlowRateIn;
					overflowingForward = true;
				} else {
					currentFlowRateOut = 0.0f;
					overflowingForward = false;
				}

			}
		} else {
//			Debug.Log ("Can't overflow to next platform... so just fill up for now... should drain into the sea");
		}

		if (groundIndex - 1 > 0) {
			float currentWaterLevel = waterTransform.position.y + waterLevelBounds.extents.y;

			if (currentWaterLevel >= masterScript.groundScripts [groundIndex - 1].waterLevelBounds.max.y) {
				Debug.Log ("Can Overflow to Previous platform");
				waterSource.AddFillingPlatform (masterScript.groundScripts [groundIndex - 1]);
				masterScript.groundScripts [groundIndex - 1].waterSource = waterSource;

				if (currentWaterLevel > masterScript.groundScripts [groundIndex - 1].waterLevelBounds.max.y) {
					currentFlowRateOut = currentFlowRateIn;
					overflowingBack = true;
				} else {
					currentFlowRateOut = 0.0f;
					overflowingBack = false;
				}

			}
		} else {
//			Debug.Log("Can't overflow to previous platform... so just fill up for now... should stop filling when max level is reached");
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
			StopFillPlatform();
		}

	}

	void OverflowForward (int overflowToIndex)
	{
		Debug.Log ("Overflow forward..... " + overflowToIndex);
		float overflowFactor = 0.5f;
		if (overflowingBack) {
			overflowFactor = 1/3;
		}
		overflowingForward = true;
		currentFlowRateOut = flowRate * (1 - overflowFactor);
		masterScript.groundScripts[overflowToIndex].InFlow(flowRate * overflowFactor);
	}
	void OverflowBack(int overflowToIndex){
		Debug.Log("Overflow back..... " + overflowToIndex);
		overflowingBack = true;
		currentFlowRateOut = currentFlowRateIn;
		masterScript.groundScripts[overflowToIndex].InFlow(flowRate);
		masterScript.groundScripts[overflowToIndex].currentFlowRateOut = 0.0f;// no longer draining into the next platform
	}

}
