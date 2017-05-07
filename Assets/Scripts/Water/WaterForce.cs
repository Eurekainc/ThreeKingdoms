using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterForce : MonoBehaviour {

	public GameObject sea;
	private Water waterScript;

	public GameObject boat;
	private Bounds boatBounds;

	public float splashVelocity = 1.0f;
	public bool makeWave = false;

	public float waveDelay = 3f;
	private float elapsedTime = 0.0f;

	void Start ()
	{
		waterScript = sea.GetComponent<Water> ();
		if (boat != null) {
			boatBounds = boat.GetComponent<BoxCollider2D> ().bounds;
		}
		InvokeRepeating("MakeWaves", 2.0f, 1.0f);
	}

	void Update ()
	{


		float inputH = Input.GetAxisRaw ("Horizontal");
		//transform.Translate(new Vector2(0.1f * inputH, 0));

//		if (elapsedTime >= waveDelay && inputH != 0) {
//			makeWave = true;
//		}
//		elapsedTime += Time.deltaTime;

		if (makeWave) {
			makeWave = false;
			if (boat != null) {
				waterScript.Splash (boatBounds.max.x, -1 * splashVelocity);
			} else {
				waterScript.Splash (waterScript.xpositions [0], -1 * splashVelocity);
			}
//			for (int i = 0; i < waterScript.xpositions.Length; i++) {
//				if (i % 10 == 0) {
//					waterScript.Splash (waterScript.xpositions [i], -1 * splashVelocity);
//				}
//			}
		}
	}

	void MakeWaves(){
		makeWave = !makeWave;
	}

}
