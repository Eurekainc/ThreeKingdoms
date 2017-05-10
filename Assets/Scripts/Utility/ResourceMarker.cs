using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMarker : MonoBehaviour {

	public King kingScript;

	public int resourceIndex;

	public float speed = 10f;
	public bool selected;
	public float timeToTransition = 0.5f;

	public Transform elementToMove;

	private bool selectedOn = false;
	private float elapsedTime = 0.0f;
	private Vector3 originalPosition;
	private Vector3 selectedPosition;

	void Start(){
		originalPosition = elementToMove.position;
		selectedPosition = new Vector3(originalPosition.x, originalPosition.y + 10, originalPosition.z);
	}
    
    void Update ()
	{
		if (selected && !selectedOn) {
			Debug.Log("Selectttt");
			Select (elapsedTime / timeToTransition);
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= timeToTransition) {
				elapsedTime = 0.0f;
				selectedOn = true;
				selectedPosition = transform.position;
			}
		}

		Debug.Log("selectedOn: " + selectedOn);
		if (!selected && selectedOn) {
			Debug.Log("Deselectttt");
			DeSelect(elapsedTime / timeToTransition);
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= timeToTransition) {
				elapsedTime = 0.0f;
				selectedOn = false;
			}
		}

		elementToMove.Rotate(Vector3.forward, speed * Time.deltaTime);
    }

    void Select(float fraction){
		elementToMove.position = Vector3.Lerp(elementToMove.position, selectedPosition, fraction);
    }

	void DeSelect(float fraction){
		elementToMove.position = Vector3.Lerp(elementToMove.position, originalPosition, fraction);
    }

    public int RemoveResource ()
	{
		if (kingScript.availableResources [resourceIndex] > 0) {
			kingScript.availableResources [resourceIndex]--;
		}

		return resourceIndex;
    }
	public int AddResource(){
		kingScript.availableResources [resourceIndex]++;

		return resourceIndex;
    }

}
