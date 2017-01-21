using UnityEngine;
using System.Collections;

public class CircleGenerator : MonoBehaviour
{
	[SerializeField]
	private float radius = 1.0f;
	[SerializeField]
	private float thickness = .2f;
	[SerializeField]
	private int resolution = 64;

	public GameObject wavePoint;
	//public Sprite wavePoint;
	private float startTime;
	private GameObject[] wavePoints = { };

	// Use this for initialization
	void Start ()
	{
		Regenerate ();
		startTime = Time.time;
	}

	void Regenerate ()
	{
		resolution = 16 + (int)(radius * Mathf.PI * 16f);
		float fResolution = (float)resolution;

		foreach (var p in wavePoints) {
			Destroy (p);
		}
		wavePoints = new GameObject[2 * resolution];

		for (int i = 0; i < 2 * resolution; i += 2) {
			float angle = Mathf.PI / fResolution * i;
			Vector3 v = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0f);

			wavePoints [i] = (GameObject)Instantiate (wavePoint, v * radius, Quaternion.identity); //Debug
			wavePoints [i + 1] = (GameObject)Instantiate (wavePoint, v * (radius - thickness), Quaternion.identity); //Debug
		}
	}


	// Update is called once per frame
	void Update ()
	{
		float newRadius = (Time.time - startTime);
		if (newRadius >= 4f) {
			startTime = Time.time;
			newRadius = (Time.time - startTime);
		}
		radius = newRadius;
		Regenerate ();
	}
}
