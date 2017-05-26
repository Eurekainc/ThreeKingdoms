using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroundStructure : MonoBehaviour {

	// the platform where the structure is located
	public Ground groundPlatform;

	// structure types
	public bool tree = false;
	private TreeResource treeScript;
	public bool strut = false;
	private Strut strutScript;
	public bool channelBuilder = false;
	private ChannelBuilder channelBuilderScript;
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
		}
		else if (channelBuilder) {
			channelBuilderScript = GetComponent<ChannelBuilder>();
		}
		  else if (channel) {
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
			if (npc.hasResource){
				strutScript.Build(npc);
			}else{
				npc.FetchResource();
			}
		}else if (channelBuilder) {
			Debug.Log("Construct the channel");
			if (npc.hasResource){
				channelBuilderScript.Build(npc);
			}else{
				npc.FetchResource();
			}
		} else if (channel) {
			Debug.Log("Build the channel");
			// no resources needed to erect a channel
			channelScript.Build(npc);
		} else if (windmill) {
			Debug.Log("Build the windmill");
		} else if (waterWheel) {
			Debug.Log("Build the waterWheel");
		}
	}

	public void FinishAction(){
		workerScript.FinishedWork();
	}

	public void Landed(){
		if (channel){
			channelScript.Landed();
		}
	}

}
