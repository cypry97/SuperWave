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
	private float minRadius = 1f;
	[SerializeField]
	private float maxRadius = 5f;
	[SerializeField]
	private float spawnTimer = 5f;
	[SerializeField]
	private float minGapAngle = 30f;
//in degrees
	[SerializeField]
	private float maxGapAngle = 60f;
//in degrees

	[SerializeField]
	private GameObject player1Prefab;
	[SerializeField]
	private GameObject player2Prefab;
	[SerializeField]
	private GameObject wavePrefab;
	[SerializeField]
	private GameObject vortexPrefab;

	//private Queue waves = new Queue(); //Stores the waves, in the order they are created
	private float lastSpawnTime;
	private GameObject[] waves;
	private int wavesCount;
	//Stores the waves (new ones are the end)
	private GameObject[] players;
	//Stores the players

	// Use this for initialization
	void Start ()
	{
		currentSpeed = initialSpeed;
		lastSpawnTime = Time.time;
		wavesCount = 0;
		//Spawn players on the map
		players = new GameObject[1];
		players [0] = (GameObject)Instantiate (player1Prefab, new Vector3 (), Quaternion.identity);
		PlayerController playerController = players [0].GetComponent<PlayerController> ();
		playerController.Initialize (0, 0f);

		//Spawn waves
		waves = new GameObject[maxWaves];
		MakeWave (3f);
		MakeWave (2f);
		MakeWave (1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdatePositions ();
		TrySpawnWave ();
	}

	void TrySpawnWave ()
	{
		if (Time.time - lastSpawnTime > spawnTimer) {
			lastSpawnTime = Time.time;
			MakeWave ();
		}

	}

	void UpdatePositions ()
	{
		//Update the waves
		for (int i = 0; i < wavesCount; i++) {
			WaveController waveController = waves [i].GetComponent<WaveController> ();
			waveController.IncreaseRadius (currentSpeed * Time.deltaTime);
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

	void RemoveWave(int pos) {
		//Move every wafe after it forward
		foreach(GameObject p in players){
			if (p.GetComponent<PlayerController> ().GetWaveId () == pos) {
				Destroy (p);
			}
		
			for (int j = pos + 1; j < wavesCount; j++) {
				waves [j - 1] = waves [j];
			}
		}
	}

	void MakeWave (float radius = .125f)
	{
		GameObject newWave = (GameObject)Instantiate (wavePrefab);

		WaveController newWaveController = newWave.GetComponent<WaveController> ();
		int gapNum = Random.Range (1, 4);
		//Debug.Log (gapNum);
		Vector2[] gaps = new Vector2[gapNum];
		float slice = 360f / (float)gapNum;

		for (int i = 0; i < gapNum; i++) {
			float sliceBeginning = i * (slice);
			float sliceEnd = (i + 1) * slice; 
			gaps [i].x = Random.value * (slice - minGapAngle) + sliceBeginning;
			gaps [i].y = gaps [i].x + Mathf.Min ((minGapAngle + Random.value * (maxGapAngle - minGapAngle)), sliceEnd);

			gaps [i] *= Mathf.PI / 180f;

			//Debug.Log (gaps [i]);
		}

		newWaveController.Initalize (radius, gaps);

		Debug.Log (wavesCount);
		waves [wavesCount++] = newWave;
	}
}
