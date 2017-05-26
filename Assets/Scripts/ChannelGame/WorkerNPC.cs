using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class WorkerNPC : MonoBehaviour {

	private Rigidbody2D rb;
	private Bounds myBounds;
	private CircleCollider2D collider;

	// The platform / ground area that worker is on
	public Ground groundPlatform;

	// Falling and vertical movement
	public LayerMask groundLayer;
	public float groundPosition = 0f;
	public bool grounded = false;// made public for debug

	private bool gotTask = false;
	public LayerMask structureLayer;
	public GroundStructure structureToBuild;

	// moving
	public bool moving = false;// made public for debug
	public float speed = 0.1f;
	public float fallSpeed = 0.1f;
	public int direction = 0;// made public for debug

	// fetching resource
	public bool fetchingResource = false;
	public LayerMask resourceLayer;

	// Use this for initialization
	void Start () {
		collider = GetComponent<CircleCollider2D>();
		myBounds = collider.bounds;
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (gotTask && structureToBuild != null) {
			GoToStructure ();
			gotTask = false;
		}

		if (!grounded && groundPosition != (transform.position.y - myBounds.extents.y)) {
			RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, Mathf.Infinity, groundLayer);
			if (hit.collider != null) {
				groundPlatform = hit.transform.gameObject.GetComponent<Ground> ();
				groundPosition = hit.collider.bounds.max.y;
			}
		}

		// falling
		float fallVelocity = 0;
		if (!grounded) {
			if (!CheckIfOverGround ()) {
				fallVelocity = -1 * fallSpeed;
			} else {
				transform.position = new Vector3 (transform.position.x, groundPosition + myBounds.extents.y, transform.position.z);
				grounded = true;
			}
		}

		if (moving) {
			CheckEdge();
			// checking if arrived at target
			if (CheckIfOverStructure ()) {
				ArriveAtStructure ();
			}
		}

		transform.Translate(new Vector3(direction * speed, fallVelocity, 0));

	}


	// My Classes

	void FindWork(){
		if (groundPlatform.structures.Count > 0) {
			structureToBuild = groundPlatform.structures [0];
			gotTask = true;
		} else {
			Idle();
		}
	}

	bool CheckIfOverStructure ()
	{
		bool arrived = false;
		Collider2D structure = Physics2D.OverlapCircle (transform.position, 0.1f, structureLayer);

		// if over a structure and the structure is the target structure
		if (structure != null) {
			if (structureToBuild.transform == structure.transform) {
				arrived = true;
			}
		}

		CheckEdge();

		return arrived;
	}

	bool CheckIfOverResource ()
	{
		bool arrived = false;
		Collider2D resource = Physics2D.OverlapCircle (transform.position, 0.1f, resourceLayer);

		// if over a resource
		if (resource != null) {
			fetchingResource = f
		}

		CheckEdge();

		return arrived;
	}

	bool CheckIfOverGround ()
	{
		bool onGround = false;
		Collider2D ground = Physics2D.OverlapCircle (transform.position, 0.1f, groundLayer);

		// if over a structure and the structure is the target structure
		if (ground != null) {
			FindWork();
			onGround = true;
		}
		return onGround;
	}

	void FindDirection(Transform targetTransform){
		if (targetTransform.position.x > transform.position.x) {
			Debug.Log ("Move right (+1)");
			direction = 1;
			moving = true;
		} else if (targetTransform.position.x < transform.position.x) {
			Debug.Log ("Move left (-1)");
			direction = -1;
			moving = true;
		}
	}

	void CheckEdge(){
		// contingency if miss structure, don't walk off the edge of the platform
		if (transform.position.x > groundPlatform.myBounds.max.x) {
			direction = -1;
		}else if (transform.position.x < groundPlatform.myBounds.min.x){
			direction = 1;
		}
	}

	void Idle ()
	{

		moving = true;

//		direction = Random.Range (0, 1) > 0.5f ? 1 : -1;
		direction = 1;
		
		CheckEdge();
	}

	void FetchResource(){
		FindDirection(groundPlatform.resources[0].transform);
		fetchingResource = true;
		CheckIfOverResource ();
	}

	void GoToStructure ()
	{
		if (structureToBuild != null) {
			if (CheckIfOverStructure()) {
				ArriveAtStructure ();
			} else {
				FindDirection(structureToBuild.transform);
			}
		}
	}

	void ArriveAtStructure(){
		moving = false;
		direction = 0;
		structureToBuild.TakeAction(this);
		Debug.Log("Arrived at structure");
	}

	public void FinishedWork(){
		Debug.Log("Finished work... move on");
		groundPlatform.structures.Remove(structureToBuild);
		FindWork();
	}

}
