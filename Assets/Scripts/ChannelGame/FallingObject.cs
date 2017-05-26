using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour {

	public bool npc = false;
	public bool structure = false;
	public bool resource = false;

	private BoxCollider2D collider;
	private Bounds myBounds;

	public Ground groundPlatform;

	// Falling and vertical movement
	public LayerMask groundLayer;
	public float groundPosition = 0f;
	public bool grounded = false;// made public for debug
	public float fallSpeed = 0.1f;


	void Start(){
		collider = GetComponent<BoxCollider2D>();
		myBounds = collider.bounds;



	}
	
	void Update ()
	{
		if (!grounded && groundPosition != (transform.position.y - myBounds.extents.y)) {
			RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, Mathf.Infinity, groundLayer);
			if (hit.collider != null) {
				groundPlatform = hit.transform.gameObject.GetComponent<Ground> ();
				AddToGroundPlatformList(groundPlatform);
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

		transform.Translate(new Vector3(0, fallVelocity, 0));
	}

	bool CheckIfOverGround ()
	{
		bool onGround = false;
		Collider2D ground = Physics2D.OverlapCircle (transform.position, 0.1f, groundLayer);

		// if over a structure and the structure is the target structure
		if (ground != null) {
			onGround = true;
		}
		return onGround;
	}

	void AddToGroundPlatformList (Ground groundPlatform)
	{
		if (npc) {
			WorkerNPC npcScript = GetComponent<WorkerNPC>();
			if (!groundPlatform.workers.Contains (npcScript)) {
				groundPlatform.ResetResourceFetching ();
				groundPlatform.workers.Add (npcScript);
			}
		} else if (structure) {
			GroundStructure groundStructureScript = GetComponent<GroundStructure>();
			if (!groundPlatform.structures.Contains (groundStructureScript)) {
				groundPlatform.ResetResourceFetching ();
				groundPlatform.structures.Add (groundStructureScript);
			}
		} else if (resource) {
			ProcessedResource processedResourceScript = GetComponent<ProcessedResource>();
			if (!groundPlatform.resources.Contains (processedResourceScript)) {
				groundPlatform.ResetResourceFetching ();
				groundPlatform.resources.Add (processedResourceScript);
			}
		}
	}
}
