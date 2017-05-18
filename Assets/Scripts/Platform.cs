using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public King kingScript;

	public GameObject[] models;// model array ordered by level
	public GameObject completedModel;
	public bool constructing = false;// made public for debugging
	private int currentModelSection = 0;

	public float constructionSpeed = 100.0F;
	public float constructSectionTime = 2.0f;
    private float elapsedTime = 0.0f;
    private float[] journeyLength;
	private Vector3[] partRotations;




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

	public bool underConstruction = false;

	public int level = 0;

	// Use this for initialization
	void Start ()
	{
		journeyLength = new float[models.Length];
		partRotations = new Vector3[models.Length];
		for (int i = 0; i < models.Length; i++) {
			journeyLength[i] = Vector3.Distance(models[i].transform.localPosition, transform.position);
//			partRotations[i] = models[i].transform.localEulerAngles;
//			Debug.Log("LocalEuler: " + models[i].transform.localEulerAngles);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (constructing) {

			if (currentModelSection < models.Length) {
				float fracJourney = elapsedTime / constructSectionTime;

				// local transform
				models [currentModelSection].transform.localPosition = Vector3.Lerp (models [currentModelSection].transform.localPosition, Vector3.zero, fracJourney);
				// local rotation
				models [currentModelSection].transform.localRotation = Quaternion.Lerp (models [currentModelSection].transform.localRotation, Quaternion.Euler (-180, 0, 0), fracJourney);

				elapsedTime += Time.deltaTime;

				if (fracJourney >= 0.85) {
					models [currentModelSection].transform.localPosition = Vector3.zero;// just snap the structure part into place
					models [currentModelSection].transform.localRotation = Quaternion.Euler (-180, 0, 0);
					elapsedTime = 0.0f;
					constructing = false;
					currentModelSection++;
				}
			} else {
				constructing = false;
			}

		}
	}

	public void ConstructSection ()
	{
		constructing = true;
	}

	public void ActivateStructure ()
	{
		underConstruction = false;

		if (completedModel != null) {
			completedModel.SetActive (true);
		} else {
			models [models.Length - 1].SetActive (true);
		}

		// Position all pieces of structure which haven't been placed yet
		for (int i = 0; i < models.Length; i++) {
			models [i].transform.localPosition = Vector3.zero;
			models [i].transform.localRotation = Quaternion.Euler (-180, 0, 0);
		}

//		for (int i = 0; i < models.Length; i++) {
//			if (i == level) {
//				models [i].SetActive (true);
//			} else {
//				models [i].SetActive (false);
//			}
//		}

		// completed builds on platforms
		if (housing) {
			House houseScript = GetComponent<House>();
			houseScript.Activate();
			kingScript.housingPlatformsUnderConstruction.Remove(this);
			kingScript.builtHousingPlatforms.Add(this);
		}else if (farming){
			Farm farmScript = GetComponent<Farm>();
			farmScript.Activate();
			kingScript.farmingPlatformsUnderConstruction.Remove(this);
			kingScript.builtFarmingPlatforms.Add(this);
		}else if (workshop){
			Workshop workshopScript = GetComponent<Workshop>();
			workshopScript.Activate();
			kingScript.workshopPlatformsUnderConstruction.Remove(this);
			kingScript.builtWorkshopPlatforms.Add(this);
		}else if (mine){
			kingScript.miningPlatformsUnderConstruction.Remove(this);
			kingScript.builtMiningPlatforms.Add(this);
		}else if (archery){
			Archery archeryScript = GetComponent<Archery>();
			archeryScript.Activate();
			kingScript.archeryPlatformsUnderConstruction.Remove(this);
			kingScript.builtArcheryPlatforms.Add(this);
		}
	}

}
