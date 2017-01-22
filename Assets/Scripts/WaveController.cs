using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour
{
	[SerializeField]
	private GameObject wavePointPrefab;

	[SerializeField]
	private float delta = 1f/64f;


	//public bool debug = false;


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
		float fResolution = (float)resolution;

		for (int i = 0; i < resolution; i++) {
			if (wavePoints [i]) {
				float angle = 2 * Mathf.PI / fResolution * i;
				Vector3 v = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0f);

				wavePoints [i].transform.position = v * radius;
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
		foreach (Vector2 gap in gaps) {
			if (angle >= gap.x + delta && angle <= gap.y - delta) {
				//Debug.Log (gap.x.ToString () + " " + angle.ToString () + " " + gap.y.ToString ());
				return true;
			}
		}
		return false;
	}

}
