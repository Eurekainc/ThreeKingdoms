using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strut : MonoBehaviour {

	public float workTime = 2.0f;
	public int cost = 3;

	public float heightIncrease = 0.01f;

	public void Build ()
	{
		Debug.Log("Building the strut");
		StartCoroutine(BuildStrut());
	}

	IEnumerator BuildStrut(){
		yield return new WaitForSeconds(workTime);
		Debug.Log("Built the strut");
		transform.position = new Vector3 (transform.position.x, transform.position.y + heightIncrease, transform.position.z);
		StartCoroutine(BuildStrut());
	}
}
