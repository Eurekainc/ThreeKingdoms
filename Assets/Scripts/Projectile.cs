using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	private Vector3 startPoint;
	public Vector3 endPoint;
	public bool shoot = false;
	private float elapsedTime = 0.0f;

	// Use this for initialization
	void Start () {
		startPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (shoot) {
			if (elapsedTime >= 1) {
				Destroy(gameObject);
			}
			transform.position = CalculateBezierPoint (elapsedTime, startPoint, new Vector3 (startPoint.x, startPoint.y + 30, startPoint.z), endPoint);
			elapsedTime += Time.deltaTime;
		}

	}

	//	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)// for 2 control points
	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)// for 1 control point
	{
		float u = 1-t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;

		// for a single control point
		Vector3 p = uu * p0; //first term
		p += 2 * u * t * p1; //second term
		p += tt * p2; //third term

		return p;
	}

}
