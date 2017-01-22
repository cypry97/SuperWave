using UnityEngine;
using System.Collections;

public class Splashscreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("Die", 2f);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void Die() {
		gameObject.SetActive (false);
	}
}
