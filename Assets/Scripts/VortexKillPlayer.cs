using UnityEngine;
using System.Collections;

public class VortexKillPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	void OnTriggerEnter2D(Collider2D coll) {
		Debug.Log ("Collided with " + coll.gameObject.name);
		if (coll.transform.tag == "Player") {
			Destroy (coll.gameObject);
		} 
	}
}
