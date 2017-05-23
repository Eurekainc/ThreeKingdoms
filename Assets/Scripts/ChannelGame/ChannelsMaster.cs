using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class ChannelsMaster : MonoBehaviour {

	public List<Ground> groundScripts = new List<Ground>();
	public List<Bounds> groundBounds = new List<Bounds>();
//	public List<WaterSource> waterSourceScripts = new List<WaterSource>();

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < groundScripts.Count; i++) {
			groundScripts[i].groundIndex = i;
			groundBounds.Add(groundScripts[i].gameObject.GetComponent<BoxCollider2D>().bounds);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

//	public void OpenChannel(WaterSource waterSourceScript){
//
//		int channelIndex = waterSourceScripts.IndexOf(waterSourceScript);
//		WaterSource ws = waterSourceScripts[channelIndex];
//		Ground g = groundScripts[channelIndex + 1];
//
//		g.InFlow(ws.flowRate);
//
//	}
//
//	public void CloseChannel(WaterSource waterSourceScript){
//
//		int channelIndex = waterSourceScripts.IndexOf(waterSourceScript);
//		WaterSource ws = waterSourceScripts[channelIndex];
//		Ground g = groundScripts[channelIndex + 1];
//
//		g.StopFillPlatform();
//
//	}

	public Bounds GetNextGroundBounds (int groundIndex)
	{
		if (groundIndex + 1 < groundBounds.Count) {
			return groundBounds [groundIndex + 1];
		} else {
			Debug.Log("At end of the platforms");
			return groundBounds [groundIndex];
		}
	}
	public Bounds GetPrevGroundBounds (int groundIndex)
	{
		if (groundIndex - 1 > 0) {
			return groundBounds [groundIndex - 1];
		} else {
			Debug.Log("At end of the platforms 0");
			return groundBounds [groundIndex];
		}
	}

}
