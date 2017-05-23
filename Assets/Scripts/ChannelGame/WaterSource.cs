using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSource : MonoBehaviour {

	public ChannelsMaster masterScript;

	public bool openChannel = false;
	public bool closeChannel = false;
	public bool running = false;
	public float flowRate = 1.0f;

	// Use this for initialization
	void Start ()
	{
		if (masterScript == null) {
			masterScript = GameObject.Find("ChannelsMaster").GetComponent<ChannelsMaster>();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (openChannel) {
			masterScript.OpenChannel(this);
			openChannel = false;
		}
		if (closeChannel) {
			masterScript.CloseChannel(this);
			closeChannel = false;
		}
	}
}
