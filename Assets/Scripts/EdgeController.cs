using UnityEngine;
using System.Collections;

public class EdgeController : MonoBehaviour {
	[SerializeField]
	private GameObject edgePrefab;

	// Use this for initialization
	void Start () {
		GameObject newEgde;
		for (int i = -4; i <= 4; i++) {
			newEgde = (GameObject)Instantiate (edgePrefab, new Vector3 ((float)i, -4.975f, 0f), Quaternion.Euler(0f, 0f, 0f));
			newEgde.transform.parent = gameObject.transform;
			newEgde = (GameObject)Instantiate (edgePrefab, new Vector3 ((float)i, 4.975f, 0f), Quaternion.Euler(0f, 0f, 180f));
			newEgde.transform.parent = gameObject.transform;
			newEgde = (GameObject)Instantiate (edgePrefab, new Vector3 (-4.975f, (float)i, 0f), Quaternion.Euler(0f, 0f, 270f));
			newEgde.transform.parent = gameObject.transform;
			newEgde = (GameObject)Instantiate (edgePrefab, new Vector3 (4.975f, (float)i, 0f), Quaternion.Euler(0f, 0f, 90f));
			newEgde.transform.parent = gameObject.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
