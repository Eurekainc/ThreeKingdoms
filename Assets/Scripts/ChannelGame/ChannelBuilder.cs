using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelBuilder : MonoBehaviour {

	public GameObject[] struts;
	public GameObject channel;

	public bool build;

	public bool attachChannel;

	public bool[] attachedIndices;

	// Use this for initialization
	void Start ()
	{
		attachedIndices = new bool[struts.Length];
		for (int i = 0; i < attachedIndices.Length; i++) {
			attachedIndices[i] = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (build) {
			IncreaseStrutHeight (Random.Range (0, 2));
			build = false;
		}

		if (attachChannel) {
			AttachChannel (Random.Range (0, 2));
		}

	}

	void IncreaseStrutHeight(int strutIndex){
		Transform strutTransform = struts[strutIndex].transform;
		strutTransform.localScale = new Vector3(strutTransform.localScale.x, strutTransform.localScale.y + 0.1f, strutTransform.localScale.z);
	}

	void AttachChannel (int strutIndex)
	{
		if (!attachedIndices [strutIndex]) {

			attachedIndices [strutIndex] = true;

			bool allAttached = true;
			Bounds[] strutBounds = new Bounds[struts.Length];

			for (int i = 0; i < attachedIndices.Length; i++) {
				if (!attachedIndices [i]) {
					allAttached = false;
					break;
				}
			}

			if (allAttached) {
				float highestStrut = struts [0].GetComponent<BoxCollider2D> ().bounds.max.y;
				for (int i = 0; i < struts.Length; i++) {
					float strutHeight = struts [i].GetComponent<BoxCollider2D> ().bounds.max.y;
					if (strutHeight > highestStrut) {
						highestStrut = strutHeight;
					}
				}

				channel.transform.position = new Vector3((struts[0].transform.position.x + struts[struts.Length - 1].transform.position.x)*0.5f, highestStrut + 0.1f, channel.transform.position.z);
				channel.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			}

//			Bounds strutBounds = struts[strutIndex].GetComponent<BoxCollider2D>().bounds;
//			channel.transform.position = new Vector3((struts[0].transform.position.x + struts[1].transform.position.x)*0.5f, strutBounds.max.y, channel.transform.position.z);
//			Rigidbody2D strutRb = struts[strutIndex].GetComponent<Rigidbody2D>();
//			HingeJoint2D hj = channel.AddComponent<HingeJoint2D>();// as HingeJoint2D;
//			hj.connectedBody = strutRb;
//			hj.anchor = new Vector2(strutBounds.extents.x, strutBounds.max.y);
//			hj.connectedAnchor = new Vector2(strutBounds.extents.x, strutBounds.max.y);
//			attachedIndices [strutIndex] = true;
		}
	}

}
