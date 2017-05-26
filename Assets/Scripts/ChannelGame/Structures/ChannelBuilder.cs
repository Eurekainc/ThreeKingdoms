using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelBuilder : MonoBehaviour {

	private GroundStructure groundStructureScript;

	public float workTime = 2.0f;
	public int cost = 3;
	public int amountPaid = 0;

	public GameObject channelPrefab;

	// Use this for initialization
	void Start(){
		groundStructureScript = GetComponent<GroundStructure>();
	}
	
	// Update is called once per frame
	void Update ()
	{

		

	}

	public void Build (WorkerNPC npc)
	{
		Debug.Log("Building the channel");
		StartCoroutine(ConstructChannel(npc));
	}

	IEnumerator ConstructChannel (WorkerNPC npc)
	{
		yield return new WaitForSeconds (workTime);
		amountPaid++;
		Debug.Log ("Built section of the channel");
		npc.hasResource = false;
		if (amountPaid >= cost) {
			Debug.Log("Completed a channel");
			amountPaid = 0;
			GameObject builtChannel = (GameObject) Instantiate(channelPrefab, transform.position, Quaternion.identity);
			groundStructureScript.groundPlatform.structures.Add(builtChannel.GetComponent<GroundStructure>());
		} else {
			Debug.Log("Still building a channel");
		}
		npc.FetchResource();
	}



//	void AttachChannel (int strutIndex)
//	{
//		if (!attachedIndices [strutIndex]) {
//
//			attachedIndices [strutIndex] = true;
//
//			bool allAttached = true;
//			Bounds[] strutBounds = new Bounds[struts.Length];
//
//			for (int i = 0; i < attachedIndices.Length; i++) {
//				if (!attachedIndices [i]) {
//					allAttached = false;
//					break;
//				}
//			}
//
//			if (allAttached) {
//				float highestStrut = struts [0].GetComponent<BoxCollider2D> ().bounds.max.y;
//				for (int i = 0; i < struts.Length; i++) {
//					float strutHeight = struts [i].GetComponent<BoxCollider2D> ().bounds.max.y;
//					if (strutHeight > highestStrut) {
//						highestStrut = strutHeight;
//					}
//				}
//
//				channel.transform.position = new Vector3((struts[0].transform.position.x + struts[struts.Length - 1].transform.position.x)*0.5f, highestStrut + 0.1f, channel.transform.position.z);
//				channel.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
//			}
//
////			Bounds strutBounds = struts[strutIndex].GetComponent<BoxCollider2D>().bounds;
////			channel.transform.position = new Vector3((struts[0].transform.position.x + struts[1].transform.position.x)*0.5f, strutBounds.max.y, channel.transform.position.z);
////			Rigidbody2D strutRb = struts[strutIndex].GetComponent<Rigidbody2D>();
////			HingeJoint2D hj = channel.AddComponent<HingeJoint2D>();// as HingeJoint2D;
////			hj.connectedBody = strutRb;
////			hj.anchor = new Vector2(strutBounds.extents.x, strutBounds.max.y);
////			hj.connectedAnchor = new Vector2(strutBounds.extents.x, strutBounds.max.y);
////			attachedIndices [strutIndex] = true;
//		}
//	}

}
