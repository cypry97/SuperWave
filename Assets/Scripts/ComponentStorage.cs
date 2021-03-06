﻿using UnityEngine;
using System.Collections;

public class PlayerStorage
{
	private GameObject gameObject;
	private PlayerController playerController;

	public PlayerStorage (GameObject gO, PlayerController pC)
	{
		gameObject = gO;
		playerController = pC;
	}

	public GameObject GetGameObject ()
	{
		return gameObject;
	}

	public PlayerController GetPlayerController ()
	{
		return playerController;
	}

	public bool IsAlive ()
	{
		return !(gameObject == null);
	}
}

public class WaveStorage
{
	private GameObject gameObject;
	private WaveController waveController;

	public WaveStorage (GameObject gO, WaveController wC)
	{
		gameObject = gO;
		waveController = wC;
	}

	public GameObject GetGameObject ()
	{
		return gameObject;
	}

	public WaveController GetWaveController ()
	{
		return waveController;
	}
}

public class VortexStorage
{
	private GameObject gameObject;
	private VortexController vortexController;

	public VortexStorage (GameObject gO, VortexController vC)
	{
		gameObject = gO;
		vortexController = vC;
	}

	public GameObject GetGameObject ()
	{
		return gameObject;
	}

	public VortexController GetVortexController ()
	{
		return vortexController;
	}
}
