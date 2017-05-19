using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSensor : MonoBehaviour {

	public Player playerScript;

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Food")) {
//			Debug.Log("Over food");
			playerScript.canLoadFood = true;
			playerScript.currentKingScript.SelectResource(0);
		}
		if (col.CompareTag ("Wood")) {
//			Debug.Log("Over wood");
			playerScript.canLoadWood = true;
			playerScript.currentKingScript.SelectResource(1);
		}
		if (col.CompareTag ("Stone")) {
//			Debug.Log("Over stone");
			playerScript.canLoadStone = true;
			playerScript.currentKingScript.SelectResource(2);
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.CompareTag ("Food")) {
//			Debug.Log("Leaving food");
			playerScript.canLoadFood = false;
			playerScript.currentKingScript.DeSelectResource(0);
		}
		if (col.CompareTag ("Wood")) {
//			Debug.Log("Leaving wood");
			playerScript.canLoadWood = false;
			playerScript.currentKingScript.DeSelectResource(1);
		}
		if (col.CompareTag ("Stone")) {
//			Debug.Log("Leaving stone");
			playerScript.canLoadStone = false;
			playerScript.currentKingScript.DeSelectResource(2);
		}
	}
	

}
