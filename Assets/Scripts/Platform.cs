using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[RequireComponent(typeof(BoxCollider2D))]
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
	public Vector3[] partPositions;// made public for debug purposes
	public Quaternion[] partRotations;// made public for debug purposes

	// Attacking enemies deconstruct structures
	private int deconstructedModelSection;
	public bool deconstruct = false;// made public for debug purposes


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

	public int[] initialCost;

	public bool underConstruction = false;

	public int level = 0;

	// Use this for initialization
	void Start ()
	{
		journeyLength = new float[models.Length];
		partRotations = new Quaternion[models.Length];
		partPositions = new Vector3[models.Length];

		for (int i = 0; i < models.Length; i++) {
			journeyLength [i] = Vector3.Distance (models [i].transform.localPosition, transform.position);
			partRotations [i] = models [i].transform.localRotation;
			partPositions [i] = models [i].transform.localPosition;
		}

		// setup the initial cost
		initialCost = new int[cost.Length];
		for (int i = 0; i < cost.Length; i++) {
			initialCost[i] = cost[i];
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (constructing) {
			Debug.Log ("CurrentModel Section......." + currentModelSection);
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
					// if constructed something, then its been un-deconstructed
					if (deconstructedModelSection > 0) {
						deconstructedModelSection--;
					}
				}
			} else {
				constructing = false;
			}

		}

		// When being attacked
		if (deconstruct) {
			int deconstructIndex = (models.Length - 1) - deconstructedModelSection;
			Debug.Log ("Deconstruct Model Section......." + deconstructIndex);
			if (deconstructIndex > 0) {

				float fracJourney = elapsedTime / constructSectionTime;

				// local transform
				models [deconstructIndex].transform.localPosition = Vector3.Lerp (models [deconstructIndex].transform.localPosition, partPositions [deconstructIndex], fracJourney);
				// local rotation
				models [deconstructIndex].transform.localRotation = Quaternion.Lerp (models [deconstructIndex].transform.localRotation, partRotations [deconstructIndex], fracJourney);

				elapsedTime += Time.deltaTime;

				if (fracJourney >= 0.85) {
					models [deconstructIndex].transform.localPosition = partPositions [deconstructIndex];// just snap the structure part into place
					models [deconstructIndex].transform.localRotation = partRotations [deconstructIndex];
					elapsedTime = 0.0f;
					deconstruct = false;
					currentModelSection = deconstructIndex;// the model section to construct is now the same as the section that was just deconstructed
					deconstructedModelSection++;
					DamageStructure ();
				}

			} else {
				deconstruct = false;
			}

		}
	}

	public void ConstructSection ()
	{
		constructing = true;
	}

	public void DeConstructSection ()
	{
		deconstruct = true;
	}

	void DamageStructure ()
	{
		List<int> nonZeroIndices = new List<int> ();
		for (int i = 0; i < initialCost.Length; i++) {
			if (initialCost [i] > 0) {
				nonZeroIndices.Add (i);
			}
		}
		Debug.Log("nonZeroIndices: " + nonZeroIndices);
		int randomIndex = nonZeroIndices[Random.Range (0, nonZeroIndices.Count)];// integer version of Random.Range excludes the end value, so doesn't need to be Count - 1
		cost [randomIndex]++;

		if (farming) {
			if (!kingScript.farmingPlatformsUnderConstruction.Contains (this)) {
				kingScript.farmingPlatformsUnderConstruction.Add (this);
			}
		}else if (housing){
			if (!kingScript.housingPlatformsUnderConstruction.Contains (this)) {
				kingScript.housingPlatformsUnderConstruction.Add (this);
			}
		}else if (workshop){
			if (!kingScript.farmingPlatformsUnderConstruction.Contains (this)) {
				kingScript.workshopPlatformsUnderConstruction.Add (this);
			}
		}else if (archery){
			if (!kingScript.farmingPlatformsUnderConstruction.Contains (this)) {
				kingScript.archeryPlatformsUnderConstruction.Add (this);
			}
		}

		kingScript.UpdateNPCs ();// should tell builders to get building

	}

	public void ActivateStructure ()
	{
		currentModelSection = 0;// reset the model section
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
