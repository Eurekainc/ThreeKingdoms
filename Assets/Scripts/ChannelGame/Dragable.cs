﻿// Credit: http://wiki.unity3d.com/index.php?title=DragObject

using UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(Rigidbody2D))]
public class Dragable : MonoBehaviour
{
 
	public int normalCollisionCount = 1;
	public float moveLimit = .5f;
	public float collisionMoveFactor = .01f;
	public float addHeightWhenClicked = 0.0f;
	public bool freezeRotationOnDrag = true;
	public Camera cam  ;
	private Rigidbody2D myRigidbody ;
	private Transform myTransform  ;
	private bool canMove = false;
	private float yPos;
	private float gravitySetting ;
	private bool freezeRotationSetting ;
	private float sqrMoveLimit ;
	private int collisionCount = 0;
	private Transform camTransform ;
 
	void Start () 
	{
	    myRigidbody = GetComponent<Rigidbody2D>();
	    myTransform = transform;
	    if (!cam) 
		{
	        cam = Camera.main;
	    }
	    if (!cam) 
		{
	        Debug.LogError("Can't find camera tagged MainCamera");
	        return;
	    }
	    camTransform = cam.transform;
	    sqrMoveLimit = moveLimit * moveLimit;   // Since we're using sqrMagnitude, which is faster than magnitude
	}
 
	void OnMouseDown () 
	{
	    canMove = true;
	    myTransform.Translate(Vector3.up*addHeightWhenClicked);
	    gravitySetting = myRigidbody.gravityScale;
	    freezeRotationSetting = myRigidbody.freezeRotation;
		myRigidbody.gravityScale = 0;
	    myRigidbody.freezeRotation = freezeRotationOnDrag;
	    yPos = myTransform.position.y;
	}
 
	void OnMouseUp () 
	{
	    canMove = false;
		myRigidbody.gravityScale = gravitySetting;
	    myRigidbody.freezeRotation = freezeRotationSetting;
		if (myRigidbody.gravityScale == 0) 
		{
			Vector3 pos = myTransform.position;
	        pos.y = yPos-addHeightWhenClicked;
	        myTransform.position = pos;
	    }
	}
 
	void OnCollisionEnter () 
	{
	    collisionCount++;
	}
 
	void OnCollisionExit () 
	{
	    collisionCount--;
	}
 
	void FixedUpdate () 
	{
	    if (!canMove)
		{
			return;
		}
 
	    myRigidbody.velocity = Vector3.zero;
//	    myRigidbody.angularVelocity = Vector3.zero;
 
//		Vector3 pos = myTransform.position;
//	    pos.y = yPos;
//	    myTransform.position = pos;
 
	    Vector3 mousePos = Input.mousePosition;
	    Vector2 move = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camTransform.position.y - myTransform.position.y)) - myTransform.position;
//	    move.y = 0.0f;
	    if (collisionCount > normalCollisionCount)		
		{
	        move = move.normalized*collisionMoveFactor;
	    }
	    else if (move.sqrMagnitude > sqrMoveLimit) 
		{
	        move = move.normalized*moveLimit;
	    }
 
	    myRigidbody.MovePosition(myRigidbody.position + move);
	}
}
