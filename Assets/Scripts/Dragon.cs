using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steer2D;

public class Dragon : MonoBehaviour {

	public int hp = 1000;

	public bool goToTarget = false;
	public Vector3 target;

	private SteeringAgent steeringAgent;
	private Seek seekScript;

	// for debugging
	private Vector3 startingPosition;

	// Use this for initialization
	void Start () {
		steeringAgent = GetComponent<SteeringAgent>();
		seekScript = GetComponent<Seek>();

		startingPosition = transform.position;// for debugging
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (goToTarget && target != null) {
			seekScript.enabled = true;
			seekScript.TargetPoint = target;

			if (Vector3.Distance (transform.position, target) < 10) {
				target = startingPosition;
				seekScript.TargetPoint = target;
			}

		}else if (goToTarget && seekScript.enabled){
			seekScript.enabled = false;
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag("Arrow")){
			Debug.Log("I've been hit!!!!!");
		}
	}

}
