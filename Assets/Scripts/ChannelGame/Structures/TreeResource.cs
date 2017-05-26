using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour {

	private GroundStructure groundStructureScript;

	public float workTime = 2.0f;
	public int cost = 3;

	public Quaternion _targetRotation = Quaternion.EulerAngles(new Vector3(0, 0, 90));
	public float turningRate = 10.0f;

	public float timeToFall = 1.0f;
	private float elapsedTime = 0.0f;
	private bool chopped = false;

	public bool regrowing = false;

	void Start(){
		groundStructureScript = GetComponent<GroundStructure>();
	}

	void Update ()
	{
		if (chopped) {

			float angle = Mathf.Lerp(transform.rotation.z, 90, (elapsedTime / timeToFall));
			transform.rotation = Quaternion.Euler(new Vector3(-180, 0, angle));
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= timeToFall) {
				elapsedTime = 0.0f;
				chopped = false;
				regrowing = true;
				gameObject.SetActive(false);// don't want workerNPC to think its found another structure
				groundStructureScript.FinishAction();
			}

		}
	}

	public void Chop ()
	{
		Debug.Log ("Chopping the tree");
		if (!regrowing) {
			StartCoroutine (ChopTree ());
		} else {
			Debug.Log("Tree is regrowing... remove from platform structure list");
		}
	}

	IEnumerator ChopTree(){
		yield return new WaitForSeconds(workTime);
		Debug.Log("Chopped the tree");
		chopped = true;
	}
}
