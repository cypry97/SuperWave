using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	//Editable in the editor
	[SerializeField]
	private float initialSpeed = 0.0625f;
	[SerializeField]
	private float rampUpRate = 1f;
	[SerializeField]
	private float currentSpeed;
	[SerializeField]
	private int minWaves = 3;
	[SerializeField]
	private int maxWaves = 16;
	[SerializeField]
	private float spawnTimer = 5f;
	[SerializeField]
	private float minGapAngle = 30f;
	//in degrees
	[SerializeField]
	private float maxGapAngle = 60f;
	//in degrees
	[SerializeField]
	private float minWaveSeparation = .75f;
	[SerializeField]
	private float maxWaveSeparation = 1.5f;

	[SerializeField]
	private GameObject player1Prefab;
	[SerializeField]
	private GameObject player2Prefab;
	[SerializeField]
	private GameObject wavePrefab;
	[SerializeField]
	private GameObject vortexPrefab;

	//private Queue waves = new Queue(); //Stores the waves, in the order they are created
	private float beginTime;
	private GameObject[] waves;
	private int wavesCount;
	//Stores the waves (new ones are the end)
	private GameObject[] players;
	private float distanceToLastWave;

	// Use this for initialization
	void Start ()
	{
		currentSpeed = initialSpeed;
		beginTime = Time.time;
		wavesCount = 0;
		//Spawn players on the map
		players = new GameObject[2];
		players [0] = (GameObject)Instantiate (player1Prefab, new Vector3 (), Quaternion.identity);
		PlayerController playerController = players [0].GetComponent<PlayerController> ();
		playerController.Initialize (0, 0f, "Player1");

		players [1] = (GameObject)Instantiate (player2Prefab, new Vector3 (), Quaternion.identity);
		playerController = players [1].GetComponent<PlayerController> ();
		playerController.Initialize (0, 180f, "Player2");

		//Spawn waves
		waves = new GameObject[maxWaves];
		MakeWave (3f);
		MakeWave (2f);
		MakeWave (1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentSpeed = initialSpeed + Mathf.Pow (rampUpRate * (Time.time - beginTime) / 60f, 2f);

		UpdatePositions ();
		TrySpawnWave ();
	}

	void TrySpawnWave ()
	{
		float distance = waves [wavesCount - 1].GetComponent<WaveController> ().GetRadius ();
		if (distance > minWaveSeparation) {
			if (Random.value * maxWaveSeparation - distance < 0f) {
				MakeWave ();
			}
		}
	}

	void UpdatePositions ()
	{
		//Update the waves
		float currentTime = Time.deltaTime;
		for (int i = 0; i < wavesCount; i++) {
			WaveController waveController = waves [i].GetComponent<WaveController> ();
			waveController.IncreaseRadius (currentSpeed * currentTime);
		}

		//Remove Waves outside the screen
		for (int i = 0; i < wavesCount; i++) {
			if (waves [i].GetComponent<WaveController> ().GetRadius () > 6.25f) {
				RemoveWave (i);
				i--;
			}
		}

		foreach (GameObject p in players) {
			if (p) {
				PlayerController playerController = p.GetComponent<PlayerController> ();
				float angle = playerController.getAngle ();
				WaveController waveController = waves [playerController.GetWaveId ()].GetComponent<WaveController> ();
				if (waveController.checkPosition (angle) && playerController.GetWaveId () < wavesCount - 1) {
					playerController.SetWaveId (playerController.GetWaveId () + 1);
					waveController = waves [playerController.GetWaveId ()].GetComponent<WaveController> ();
				}
				float radius = waveController.GetRadius () + .375f;
				p.transform.position = new Vector3 (Mathf.Cos (angle) * radius, Mathf.Sin (angle) * radius, 0f);
			}
		}

	}

	void RemoveWave (int pos)
	{
		//Move every wafe after it forward

		Destroy (waves [pos]);
		wavesCount--;
		foreach (GameObject p in players) {
			if (p) {
				int waveId = p.GetComponent<PlayerController> ().GetWaveId ();
				if (waveId == pos) {
					Destroy (p);
				} else if (waveId >= pos) {
					p.GetComponent<PlayerController> ().SetWaveId (waveId - 1);
				}
			}
		}
		for (int j = pos + 1; j <= wavesCount; j++) {
			waves [j - 1] = waves [j];
		}
		waves [wavesCount] = null;
	}

	void MakeWave (float radius = .125f)
	{
		GameObject newWave = (GameObject)Instantiate (wavePrefab);

		WaveController newWaveController = newWave.GetComponent<WaveController> ();
		int gapNum = Random.Range (1, 4);
		Vector2[] gaps = new Vector2[gapNum];
		float slice = 360f / (float)gapNum;

		for (int i = 0; i < gapNum; i++) {
			float sliceBeginning = i * (slice);
			float sliceEnd = (i + 1) * slice; 
			gaps [i].x = Random.value * (slice - minGapAngle) + sliceBeginning;
			gaps [i].y = gaps [i].x + Mathf.Min ((minGapAngle + Random.value * (maxGapAngle - minGapAngle)), sliceEnd);

			gaps [i] *= Mathf.PI / 180f;
		}

		newWaveController.Initalize (radius, gaps);

		waves [wavesCount] = newWave;
		wavesCount++;
	}
}
