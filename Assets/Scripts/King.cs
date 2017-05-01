using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour {

	public float cycleTime = 10.0f;
	private float currentTime = 0.0f;
	public int days = 0;

	// platforms
	public List<GameObject> platforms = new List<GameObject>();
	[HideInInspector]
	public List<Platform> platformScripts = new List<Platform>();
	[HideInInspector]
	public List<Platform> farmingPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> loggingPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> quarryPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> minePlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> housingPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> workshopPlatforms = new List<Platform>();
	[HideInInspector]
	public List<Platform> archeryPlatforms = new List<Platform>();

	public Platform docks;

	// All NPCs
	public List<GameObject> npcs = new List<GameObject>();
	public List<NPC> npcScripts = new List<NPC>();

	public List<NPC> peasants = new List<NPC>();
	public List<NPC> builders = new List<NPC>();
	public List<NPC> archers = new List<NPC>();

	// build / work / combat times/delays
	public float offloadTime = 1.0f;
	public float farmTime = 2.0f;
	public float buildTime = 2.0f;
	public float reloadTime = 2.0f;

	// resources
	public int food = 0;
	public int wood = 0;
	public int stone = 0;
	public int metal = 0;

	// structures
	public int farms = 0;
	public int quarries = 0;
	public int forrests = 0;
	public int houses = 0;
	public int keeps = 0;
	public int castles = 0;
	public int archeryRange = 0;
	public int archeryLookout = 0;
	public int archeryTower = 0;
	public int boats = 0;

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < platforms.Count; i++) {
			platformScripts.Add (platforms [i].GetComponent<Platform> ());
		}

		for (int i = 0; i < platformScripts.Count; i++) {
			if (platformScripts [i].farming) {
				farmingPlatforms.Add(platformScripts [i]);
			}else if(platformScripts [i].logging){
				loggingPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].quarry){
				quarryPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].mine){
				minePlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].housing){
				housingPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].workshop){
				workshopPlatforms.Add(platformScripts[i]);
			}
			else if(platformScripts [i].archery){
				archeryPlatforms.Add(platformScripts[i]);
			}
		}

		for (int i = 0; i < npcs.Count; i++) {
			npcScripts.Add(npcs[i].GetComponent<NPC>());
			// TODO: make sure to update the King script when NPCs move between islands
			npcScripts[i].kingScript = this;// assign the King script to each NPC
		}

		// the init function
		ReOrganise();
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentTime += Time.deltaTime;
		if (currentTime >= cycleTime) {
			currentTime = 0.0f;
			days++;
			Debug.Log("New day..... " + days);
			ReOrganise();
		}
	}

	void ResetNPCLists(){
		peasants = new List<NPC>();
		builders = new List<NPC>();
		archers = new List<NPC>();
	}

	public void UpdatePeasantCount ()
	{
		ResetNPCLists();
		for (int i = 0; i < npcScripts.Count; i++) {
			if (npcScripts [i].occupation == 0) {
				peasants.Add(npcScripts[i]);
			}else if (npcScripts [i].occupation == 1){
				builders.Add(npcScripts[i]);
			}
			else if (npcScripts [i].occupation == 2){
				archers.Add(npcScripts[i]);
			}
		}
	}


	void SendNPCToPlatform(NPC npc, Transform platform, Platform platformScript){
		npc.GoToPlatform(platform, platformScript);
	}

	public void ReOrganise ()
	{
		int resourceState = CheckResources ();
		switch (resourceState) {
			case 0:
				Debug.Log("All resources above 0");
				break;
			case 1:
				Debug.Log("Get more food");
				AssignLogging();
				break;
			case 2:
				Debug.Log("Get more wood");
				break;
			case 3:
				Debug.Log("Get more stone");
				break;
			case 4:
				Debug.Log("Get more metal");
				break;
			default:
				Debug.Log("Fall through ReOrganise in King.cs");
				break;
		}
	}

	int CheckResources ()
	{
		if (food <= 0) {
			return 1;
		} else if (wood <= 0) {
			return 2;
		} else if (stone <= 0) {
			return 3;
		} else if (metal <= 0) {
			return 4;
		} else {
			return 0;
		}
	}

	// Assign tasks

	void AssignLogging ()
	{
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].active && npcScripts [i].occupation == 0) {
				SendNPCToPlatform(npcScripts [i], farmingPlatforms[0].transform, farmingPlatforms[0]);
			}
		}
	}



}
