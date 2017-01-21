using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour
{
	[SerializeField]
	private GameObject wavePointPrefab;

	private float radius;
	private Vector2[] gaps;
	private float rotationSpeed;
	private int resolution = 128;

	private GameObject[] wavePoints;

	public void Initalize (float r, Vector2[] g)
	{
		radius = r;
		gaps = g;

		wavePoints = new GameObject[resolution];

		int currentGap = 0;

		for (int i = 0; i < resolution; i++) {
			float angle = 2 * Mathf.PI / resolution * i;
			if (currentGap < gaps.Length && angle >= gaps [currentGap].x) {
				if (angle > gaps [currentGap].y) {
					currentGap++;
					Vector3 v = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0f);
					wavePoints [i] = (GameObject)Instantiate (wavePointPrefab, v * radius, Quaternion.identity);
					wavePoints [i].transform.parent = gameObject.transform;
				} else {
					//Debug.Log ("Got here!");
				}
			} else {
				Vector3 v = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0f);
				wavePoints [i] = (GameObject)Instantiate (wavePointPrefab, v * radius, Quaternion.identity);
				wavePoints [i].transform.parent = gameObject.transform;
			}

		}
	}

	public void IncreaseRadius (float newR)
	{
		radius += newR;
		Draw ();
	}

	public void ResetRadius (float newR)
	{
		radius = newR;
		Draw ();
	}

	public float GetRadius ()
	{
		return radius;
	}

	public void Draw ()
	{
		//int resolution = 8 + (int)(radius*Mathf.PI * 8f);
		//Debug.Log (resolution);
		float fResolution = (float)resolution;

		//foreach (var p in wavePoints) {
		//if (p) {
		//Destroy (p);
		//}
		//}
		//wavePoints = new GameObject[2 * resolution];

		for (int i = 0; i < resolution; i++) {
			if (wavePoints [i]) {
				float angle = 2 * Mathf.PI / fResolution * i;
				Vector3 v = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0f);

				wavePoints [i].transform.position = v * radius;
				//wavePoints[i+1] = (GameObject)Instantiate (wavePointPrefab, v * (radius - thickness), Quaternion.identity); //Debug
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnDestroy ()
	{
		for (int i = 0; i < resolution; i++) {
			if (wavePoints [i]) {
				Destroy (wavePoints [i]);
			}
		}
	}

	public bool checkPosition (float angle)
	{
		Debug.Log (angle);
		foreach (Vector2 gap in gaps) {
			if (angle >= gap.x && angle <= gap.y) {
				return true;
			}
		}
		return false;
	}

}
