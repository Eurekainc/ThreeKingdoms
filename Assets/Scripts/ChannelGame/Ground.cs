using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	public ChannelsMaster masterScript;

	public GameObject waterLevel;
	private Bounds waterLevelBounds;
	public bool filling = false;
	public float currentFlowRateIn;

	public float elapsedTime = 0.0f;
	public float fillingRate = 1.0f;//update level every x seconds

	public int groundIndex;

	// Use this for initialization
	void Start () {

		if (masterScript == null) {
			masterScript = GameObject.Find("ChannelsMaster").GetComponent<ChannelsMaster>();
		}

		waterLevelBounds = waterLevel.GetComponent<BoxCollider2D>().bounds;
	}
	
	// Update is called once per frame
	void Update () {

		

		if (filling){
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= fillingRate){
				elapsedTime = 0.0f;
				IncreaseWaterLevel();
			};
		}

	}

	public void FillPlatform(float flowRate){

		Debug.Log("Platform filling at rate: " + flowRate);
		filling = true;
		currentFlowRateIn = flowRate;

	}

	public void StopFillPlatform(){

		Debug.Log("Stop Platform filling");
		filling = false;
		currentFlowRateIn = 0.0f;

	}

	void IncreaseWaterLevel ()
	{
		Transform waterTransform = waterLevel.transform;
//		waterTransform.localScale = new Vector3 (waterTransform.localScale.x, waterTransform.localScale.y + currentFlowRate, waterTransform.localScale.z);
		waterTransform.position = new Vector3(waterTransform.position.x, waterTransform.position.y + currentFlowRateIn, waterTransform.position.z);

		Debug.Log("waterTransform.position.y: " + waterTransform.position.y);

		if ((waterTransform.position.y + waterLevelBounds.extents.y) >= masterScript.GetNextGroundBounds (groundIndex).max.y) {
			StopFillPlatform();
		}

	}

}
