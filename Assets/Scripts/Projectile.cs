using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	private Vector3 startPoint;
	public Vector3 endPoint;
	public bool shoot = false;
	private float elapsedTime = 0.0f;
	public GameObject arrowModel;

	// Use this for initialization
	void Start () {
		startPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (shoot) {
			if (elapsedTime >= 1) {
				Destroy (gameObject);
			}

			// arrow angle
			float arrowDirection = Mathf.Sign (endPoint.x - startPoint.x);
//			if (elapsedTime > 0 && elapsedTime < 0.25) {
//				arrowModel.transform.rotation = Quaternion.Euler (0, 0, 45 * arrowDirection);
//			} else if (elapsedTime > 0.25 && elapsedTime < 0.4) {
//				arrowModel.transform.rotation = Quaternion.Euler (0, 0, 30 * arrowDirection);
//			} else if (elapsedTime > 0.4 && elapsedTime < 0.6) {
//				arrowModel.transform.rotation = Quaternion.Euler (0, 0, 0);
//			}else if (elapsedTime > 0.6 && elapsedTime < 0.75){
//				arrowModel.transform.rotation = Quaternion.Euler(0, 0, -30 * arrowDirection);
//			}else if (elapsedTime > 0.75){
//				arrowModel.transform.rotation = Quaternion.Euler(0, 0, -45 * arrowDirection);
//			}

			arrowModel.transform.rotation = Quaternion.Slerp(Quaternion.Euler (0, 0, 45 * arrowDirection), Quaternion.Euler (0, 0, -45 * arrowDirection), elapsedTime);

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
