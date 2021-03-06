﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public GameObject lText, rText;
	public GameObject victoryScreen;
	private Text leftText, rightText;
	public GameObject mainMenu;
	public AudioSource backgroundSound;
	public AudioSource waveSound;

	[SerializeField]
	private float initialSpeed = 1f;
	[SerializeField]
	private float playerAngularVelocity = 1f;
	[SerializeField]
	private float rampUpRate = 1f;
	[SerializeField]
	private float currentSpeed;
	[SerializeField]
	private float minGapAngle = 30f;
	[SerializeField]
	private float maxGapAngle = 60f;
	[SerializeField]
	private float minWaveSeparation = .75f;
	[SerializeField]
	private float maxWaveSeparation = 1.5f;
	[SerializeField]
	private float gameTime = 2f;


	[SerializeField]
	private GameObject player1Prefab;
	[SerializeField]
	private GameObject player2Prefab;
	[SerializeField]
	private GameObject wavePrefab;
	[SerializeField]
	private GameObject vortexPrefab;


	private bool isRunning = false;
	private float beginTime;
	private WaveStorage[] waves;
	private int activeWavesCount;
	private PlayerStorage[] players;
	private float distanceToLastWave;
	private VortexStorage[] vortexes;
	private int vortexCount;
	private WaveStorage[] inactiveWaves;
	private int inactiveWavesCount;

	// Use this for initialization
	void Start ()
	{
		//currentSpeed = initialSpeed; 
		beginTime = Time.time;
		activeWavesCount = 0;
		//Spawn players on the map

		players = new PlayerStorage[2];

		//Spawn waves
		waves = new WaveStorage[16]; //There shouldn't be more than 16 waves at the same time

		vortexes = new VortexStorage[16];
		vortexCount = 0;

		inactiveWaves = new WaveStorage[16];
		inactiveWavesCount = 0;

		leftText = lText.GetComponent<Text> ();
		rightText = rText.GetComponent<Text> ();
		victoryScreen.SetActive (false);

		backgroundSound = gameObject.GetComponents<AudioSource> () [0];
		waveSound = gameObject.GetComponents<AudioSource> () [1];
	}

	public void GameStart ()
	{
		isRunning = true;
		beginTime = Time.time;

		GameObject player1 = (GameObject)Instantiate (player1Prefab, new Vector3 (), Quaternion.identity);
		PlayerController player1Controller = player1.GetComponent<PlayerController> ();
		player1Controller.Initialize (1, 0f, "Player1", playerAngularVelocity);

		GameObject player2 = (GameObject)Instantiate (player2Prefab, new Vector3 (), Quaternion.identity);
		PlayerController player2Controller = player2.GetComponent<PlayerController> ();
		player2Controller.Initialize (1, 180f, "Player2", playerAngularVelocity);

		players [0] = new PlayerStorage (player1, player1Controller);
		players [1] = new PlayerStorage (player2, player2Controller);

		MakeWave (3f);
		MakeWave (2f);
		MakeWave (1f);
		UpdatePositions ();
	}

	public void GameEnd ()
	{
		isRunning = false;

		Text text = victoryScreen.transform.FindChild("Text").GetComponent<Text> ();
		if (players [0].GetGameObject () && players[1].GetGameObject()) {
			text.text = "DRAW";
			text.color = Color.yellow;
		}
		if (players [0].GetGameObject ()) {
			text.text = "GREEN WINS";
			text.color = Color.green;
		}
		else if (players[1].GetGameObject()){
			text.text = "RED WINS";
			text.color = Color.red;
		}
		else {
			text.text = "DRAW";
			text.color = Color.yellow;
		}
		victoryScreen.SetActive (true);

		foreach (PlayerStorage p in players) {
			if (p.GetGameObject ()) {
				Destroy (p.GetGameObject ());
			}
		}
		for (int i = 0; i < vortexCount; i++) {
			Destroy (vortexes [i].GetGameObject ());
		}

		for (int i = 0; i < activeWavesCount; i++) {
			inactiveWaves [inactiveWavesCount] = waves [i];
			inactiveWaves [inactiveWavesCount].GetWaveController ().ResetRadius (.125f);
			inactiveWavesCount++;
		}

		Invoke ("RestoreMainMenu", 5f); 
	}

	void RestoreMainMenu() {
		mainMenu.GetComponent<QuitApplication> ().Quit ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (!isRunning || Time.time - beginTime <= 3f) {
			return;
		}

		if (Time.time - beginTime - 3f > gameTime * 60f) {
			GameEnd ();
			return;
		}

		if (!backgroundSound.isPlaying) {
			backgroundSound.Play ();
		}

		float multiplier = 1f + Mathf.Pow (rampUpRate * (Time.time - beginTime) / 60f, 2f);
		//multiplier = 1f;
		currentSpeed = initialSpeed * multiplier;
		foreach (PlayerStorage p in players) {
			if (p.GetGameObject ()) {
				p.GetPlayerController ().SetMultiplier (multiplier);
			}
		}

		UpdatePositions ();
		TrySpawnWave ();

		foreach (PlayerStorage p in players) {
			if (!p.GetGameObject ()) {
				GameEnd ();
			}
		}


		leftText.text = "Time Remaining\n" + ((int)(120f - Time.time + beginTime +3f)).ToString() + "s";
		rightText.text = leftText.text;
	}

	void TrySpawnWave ()
	{
		float distance = waves [activeWavesCount - 1].GetWaveController ().GetRadius ();
		if (distance > minWaveSeparation) {
			if (Random.value * maxWaveSeparation - distance < 0f) {
				MakeWave ();
				AttachVortex (activeWavesCount - 1);
			}
		}
	}

	void UpdatePositions ()
	{
		//Update the waves
		float currentTime = Time.deltaTime;
		for (int i = 0; i < activeWavesCount; i++) {
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
				float angle = p.GetPlayerController ().getAngle ();
				int pos = p.GetPlayerController ().GetWavePos ();
				if (waves [pos].GetWaveController ().checkPosition (angle) && p.GetPlayerController ().GetWavePos () < activeWavesCount - 1 && 
					waves[pos + 1].GetWaveController().GetRadius() >= .5f) {
					p.GetPlayerController ().SetWavePos (p.GetPlayerController ().GetWavePos () + 1);
					pos++;
				}
				float radius = waves [pos].GetWaveController ().GetRadius () + .375f;
				p.GetGameObject ().transform.position = new Vector3 (Mathf.Cos (angle) * radius, Mathf.Sin (angle) * radius, 0f);
			}
		}

		for (int i = 0; i < vortexCount; i++) {
			float angle = vortexes [i].GetVortexController ().GetAngle ();
			float radius = waves [vortexes [i].GetVortexController ().GetWavePos ()].GetWaveController ().GetRadius () + .125f;
			vortexes [i].GetGameObject ().transform.position = new Vector3 (Mathf.Cos (angle) * radius, Mathf.Sin (angle) * radius, 0f);
		}

	}

	void RemoveWave (int pos)
	{
		//Move every wafe after it forward

		//Destroy (waves [pos].GetGameObject ());
		inactiveWaves [inactiveWavesCount] = waves [pos];
		inactiveWaves [inactiveWavesCount].GetWaveController ().ResetRadius (.125f);
		inactiveWavesCount++;

		activeWavesCount--;
		foreach (PlayerStorage p in players) {
			if (p.GetGameObject ()) {
				int waveId = p.GetPlayerController ().GetWavePos ();
				if (waveId == pos) {
					Destroy (p.GetGameObject ());
				} else if (waveId >= pos) {
					p.GetPlayerController ().SetWavePos (waveId - 1);
				}
			}
		}

		for (int i = 0; i < vortexCount; i++) {
			int wavePos = vortexes [i].GetVortexController ().GetWavePos ();
			if (wavePos == pos) {
				Destroy (vortexes [i].GetGameObject ());
				for (int j = i + 1; j < vortexCount; j++) {
					vortexes [j - 1] = vortexes [j];
				}
				vortexCount--;
				vortexes [vortexCount] = null;
				i--;
			} else if (wavePos >= pos) {
				vortexes [i].GetVortexController ().SetWavePos (wavePos - 1);
			}
		}

		for (int j = pos + 1; j <= activeWavesCount; j++) {
			waves [j - 1] = waves [j];
		}
		waves [activeWavesCount] = null; //Just to be sure
	}

	void MakeWave (float radius = .125f)
	{
		GameObject newWave;
		WaveController newWaveController;

		if (inactiveWavesCount > 1) {
			newWave = inactiveWaves [inactiveWavesCount - 1].GetGameObject ();
			newWaveController = inactiveWaves [inactiveWavesCount - 1].GetWaveController ();
			inactiveWavesCount--;
		} else {
			newWave = (GameObject)Instantiate (wavePrefab);
			newWaveController = newWave.GetComponent<WaveController> ();
		}

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

		waveSound.Play ();
	}

	void AttachVortex (int pos)
	{
		float chance = Random.value;
		if (chance < (Time.time - beginTime) / (gameTime * 60f) + 0.03125f) {
			return;
		}

		int maxTries = 3;

		int count = 1;
		while (count <= maxTries) {
			float angle = Random.value * Mathf.PI * 2;
			if (!waves [pos].GetWaveController ().checkPosition (angle) && !waves [pos - 1].GetWaveController ().checkPosition (angle)) {
				GameObject newVortex = (GameObject)Instantiate (vortexPrefab);
				VortexController newVortexController = newVortex.GetComponent<VortexController> ();
				newVortexController.Initialize (pos, angle);

				vortexes [vortexCount] = new VortexStorage (newVortex, newVortexController);
				vortexCount++;
				count = 3;
			}

			count++;
		}
	}
}
