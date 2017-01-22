using UnityEngine;
using System.Collections;

public class VortexRotator : MonoBehaviour {

	[SerializeField]
	private float rotationRate = 60f;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0f, 0f, rotationRate * Time.deltaTime));
	}
}
