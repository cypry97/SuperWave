using UnityEngine;
using System.Collections;

public class TriggerKillPlayer : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D coll)
	{
		//Debug.Log ("Collided with " + coll.gameObject.name);
		if (coll.transform.tag == "Player") {
			Destroy (coll.gameObject);
		} 
	}
}
