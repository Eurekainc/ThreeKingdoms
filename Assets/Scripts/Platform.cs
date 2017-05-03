using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public King kingScript;

	public GameObject[] models;// model array ordered by level

	public bool docks = false;
	public bool farming = false;
	public bool logging = false;
	public bool quarry = false;
	public bool mine = false;
	public bool housing = false;
	public bool workshop = false;// train to become builder
	public bool archery = false;// train to become archer

	// food, wood, stone, metal
	public int[] cost = new int[4];

	public int level = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ActivateStructure ()
	{
		for (int i = 0; i < models.Length; i++) {
			if (i == level) {
				models [i].SetActive (true);
			} else {
				models [i].SetActive (false);
			}
		}

		if (housing) {
			Debug.Log("House completed");
			House houseScript = GetComponent<House>();
			houseScript.Activate();
			kingScript.housingPlatformsUnderConstruction.Remove(this);
			kingScript.builtHousingPlatforms.Add(this);
		}else if (farming){
			Debug.Log("Farm completed");
			Farm farmScript = GetComponent<Farm>();
			farmScript.Activate();
			kingScript.farmingPlatformsUnderConstruction.Remove(this);
			kingScript.builtFarmingPlatforms.Add(this);
		}else if (workshop){
			Debug.Log("Workshop completed");
			kingScript.workshopPlatformsUnderConstruction.Remove(this);
			kingScript.builtWorkshopPlatforms.Add(this);
		}else if (mine){
			Debug.Log("Mine completed");
			kingScript.miningPlatformsUnderConstruction.Remove(this);
			kingScript.builtMiningPlatforms.Add(this);
		}else if (archery){
			Debug.Log("Archery range completed");
			kingScript.archeryPlatformsUnderConstruction.Remove(this);
			kingScript.builtArcheryPlatforms.Add(this);
		}
	}

}
