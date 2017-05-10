using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetCameraPosition = new Vector3(target.position.x, (target.position.y +  Camera.main.orthographicSize * 0.9f), transform.position.z);
		transform.position = Vector3.SmoothDamp(transform.position, targetCameraPosition, ref velocity, smoothTime);
	}
}
