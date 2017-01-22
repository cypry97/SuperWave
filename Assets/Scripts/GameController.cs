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


	private float beginTime;
	private WaveStorage[] waves;
	//private GameObject[] waves;
	private int activeWavesCount;
	//Stores the waves (new ones are the end)
	private PlayerStorage[] players;
	private float distanceToLastWave;

	// Use this for initialization
	void Start ()
	{
		currentSpeed = initialSpeed; 
		beginTime = Time.time;
		activeWavesCount = 0;
		//Spawn players on the map
		players = new PlayerStorage[2];
		GameObject player1 = (GameObject)Instantiate (player1Prefab, new Vector3 (), Quaternion.identity);
		PlayerController player1Controller = player1.GetComponent<PlayerController> ();
		player1Controller.Initialize (0, 0f, "Player1");

		GameObject player2 = (GameObject)Instantiate (player2Prefab, new Vector3 (), Quaternion.identity);
		PlayerController player2Controller = player2.GetComponent<PlayerController> ();
		player2Controller.Initialize (0, 180f, "Player2");
		players [0] = new PlayerStorage (player1, player1Controller);
		players [1] = new PlayerStorage (player2, player2Controller);

		//Spawn waves
		waves = new WaveStorage[16]; //There shouldn't be more than 16 waves at the same time
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
		float distance = waves [activeWavesCount - 1].GetWaveController ().GetRadius ();
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
		for (int i = 0; i < activeWavesCount; i++) {
			//WaveController waveController = waves [i].GetComponent<WaveController> ();
			waves [i].GetWaveController ().IncreaseRadius (currentSpeed * currentTime);
		}

		//Remove Waves outside the screen
		for (int i = 0; i < activeWavesCount; i++) {
			if (waves [i].GetWaveController ().GetRadius () > 6.25f) {
				RemoveWave (i);
				i--;
			}
		}

		foreach (PlayerStorage p in players) {
			if (p.GetGameObject ()) {
				//PlayerController playerController = p.GetComponent<PlayerController> ();
				float angle = p.GetPlayerController ().getAngle ();
				//WaveController waveController = waves [playerController.GetWaveId ()].GetComponent<WaveController> ();
				int pos = p.GetPlayerController ().GetWavePos ();
				if (waves [pos].GetWaveController ().checkPosition (angle) && p.GetPlayerController ().GetWavePos () < activeWavesCount - 1) {
					p.GetPlayerController ().SetWavePos (p.GetPlayerController ().GetWavePos () + 1);
					pos++;
					//waveController = waves [playerController.GetWaveId ()].GetComponent<WaveController> ();
				}
				float radius = waves [pos].GetWaveController ().GetRadius () + .375f;
				p.GetGameObject ().transform.position = new Vector3 (Mathf.Cos (angle) * radius, Mathf.Sin (angle) * radius, 0f);
			}
		}

	}

	void RemoveWave (int pos)
	{
		//Move every wafe after it forward

		Destroy (waves [pos].GetGameObject ());
		activeWavesCount--;
		foreach (PlayerStorage p in players) {
			if (p.GetGameObject()) {
				int waveId = p.GetPlayerController().GetWavePos ();
				if (waveId == pos) {
					Destroy (p.GetGameObject());
				} else if (waveId >= pos) {
					p.GetPlayerController().SetWavePos (waveId - 1);
				}
			}
		}
		for (int j = pos + 1; j <= activeWavesCount; j++) {
			waves [j - 1] = waves [j];
		}
		waves [activeWavesCount] = null; //Just to be sure
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

		waves [activeWavesCount] = new WaveStorage (newWave, newWaveController);
		activeWavesCount++;
	}
}
