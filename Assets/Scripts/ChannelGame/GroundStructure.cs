using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroundStructure : MonoBehaviour {

	// structure types
	public bool tree = false;
	private TreeResource treeScript;
	public bool strut = false;
	private Strut strutScript;
	public bool channel = false;
	private Channel channelScript;
	public bool windmill = false;
	public bool waterWheel = false;

	public WorkerNPC workerScript;

	// Use this for initialization
	void Start () {
		if (tree) {
			treeScript = GetComponent<TreeResource>();
		} else if (strut) {
			strutScript = GetComponent<Strut>();
		} else if (channel) {
			channelScript = GetComponent<Channel>();
		} else if (windmill) {
			Debug.Log("Get the windmill");
		} else if (waterWheel) {
			Debug.Log("Get the waterWheel");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// my classes

	public void TakeAction (WorkerNPC npc)
	{
		Debug.Log ("Perform Action... custom to each ");
		workerScript = npc;
		if (tree) {
			Debug.Log ("Chop the tree");
			treeScript.Chop();
		} else if (strut) {
			Debug.Log("Build the strut");
			strutScript.Build();
		} else if (channel) {
			Debug.Log("Build the channel");
		} else if (windmill) {
			Debug.Log("Build the windmill");
		} else if (waterWheel) {
			Debug.Log("Build the waterWheel");
		}
	}

	public void FinishAction(){
		workerScript.FinishedWork();
	}

}
