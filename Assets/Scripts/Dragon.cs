using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steer2D;

public class Dragon : MonoBehaviour {

	private GlobalGameScript globalGameScript;

	public int hp = 1000;

	public bool goToTarget = false;
	public Vector3 target;

	public bool terrorize = false;
	public float speed = 1.0f;
	private int direction = -1;

	private SteeringAgent steeringAgent;
	private Seek seekScript;

	// for debugging
	private Vector3 startingPosition;

	// Use this for initialization
	void Start () {

		if (globalGameScript == null) {
			globalGameScript = GameObject.Find("GlobalGameScript").GetComponent<GlobalGameScript>();
		}

		steeringAgent = GetComponent<SteeringAgent>();
		seekScript = GetComponent<Seek>();

		startingPosition = transform.position;// for debugging
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if (goToTarget && target != null) {
//			seekScript.enabled = true;
//			seekScript.TargetPoint = target;
//
//			if (Vector3.Distance (transform.position, target) < 10) {
//				target = startingPosition;
//				seekScript.TargetPoint = target;
//			}
//
//		}else if (goToTarget && seekScript.enabled){
//			seekScript.enabled = false;
//		}

		if (terrorize) {
			transform.Translate (new Vector3 (direction * speed, 0, 0));
			if (transform.position.x >= globalGameScript.islands [0].transform.position.x || transform.position.x <= globalGameScript.islands [globalGameScript.islands.Count - 1].transform.position.x) {
				direction *= -1;// change direction when reaching end islands
			}
		}

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag("Arrow")){
			Debug.Log("I've been hit!!!!!");
		}

		if (col.CompareTag("Structure")){
			Debug.Log("Attack structure");
		}
	}

}
