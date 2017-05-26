using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessedResource : MonoBehaviour {

	public int value = 5;

//	private BoxCollider2D collider;
//	private Bounds myBounds;

	private FallingObject fallingScript;
	public Ground groundPlatform;

	// Falling and vertical movement
//	public LayerMask groundLayer;
//	public float groundPosition = 0f;
//	public bool grounded = false;// made public for debug
//	public float fallSpeed = 0.1f;

	void Start(){
//		collider = GetComponent<BoxCollider2D>();
//		myBounds = collider.bounds;
		fallingScript = GetComponent<FallingObject>();
	}

//	void Update ()
//	{
//		if (!grounded && groundPosition != (transform.position.y - myBounds.extents.y)) {
//			RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, Mathf.Infinity, groundLayer);
//			if (hit.collider != null) {
//				groundPlatform = hit.transform.gameObject.GetComponent<Ground> ();
//				if (!groundPlatform.resources.Contains (this)) {
//					groundPlatform.ResetResourceFetching();
//					groundPlatform.resources.Add (this);
//				}
//				groundPosition = hit.collider.bounds.max.y;
//			}
//		}
//
//		// falling
//		float fallVelocity = 0;
//		if (!grounded) {
//			if (!CheckIfOverGround ()) {
//				fallVelocity = -1 * fallSpeed;
//			} else {
//				transform.position = new Vector3 (transform.position.x, groundPosition + myBounds.extents.y, transform.position.z);
//				grounded = true;
//			}
//		}
//
//		transform.Translate(new Vector3(0, fallVelocity, 0));
//	}

//	bool CheckIfOverGround ()
//	{
//		bool onGround = false;
//		Collider2D ground = Physics2D.OverlapCircle (transform.position, 0.1f, groundLayer);
//
//		// if over a structure and the structure is the target structure
//		if (ground != null) {
//			onGround = true;
//		}
//		return onGround;
//	}

	public void TakeResource ()
	{
		Debug.Log("TAKE RESOURCEEEEE");
		value--;
		if (value <= 0) {
			fallingScript.groundPlatform.resources.Remove(this);
			fallingScript.groundPlatform.ResetResourceFetching();
			gameObject.SetActive(false);
		}
	}
	
}
