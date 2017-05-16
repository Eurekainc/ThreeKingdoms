using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class GlobalGameScript : MonoBehaviour {

	// THIS IS USED FOR OBJECT POOLING... KEEP LISTS OF ALL INSTANTAITED RESOURCES NOTBEING USED
	public List<GameObject> foods = new List<GameObject>();
	public List<GameObject> woods = new List<GameObject>();
	public List<GameObject> stones = new List<GameObject>();
	public List<GameObject> metals = new List<GameObject>();


	void Awake () {
	}

	void Update () {
		
	}
}
