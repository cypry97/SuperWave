﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	//public float angularVelocity = 0.0625f;
	private int waveId;
	private float angle;
	private string inputName;
	private float angularVelocity;
	private float angularSpeedMultiplier = 1f;

	private AudioSource audio;

	public Animator animator;

	public void Initialize (int wId, float ang, string iN, float aV)
	{
		waveId = wId;
		angle = ang * Mathf.PI / 180f;
		inputName = iN;
		angularVelocity = aV;
	}

	public int GetWavePos ()
	{
		return waveId;
	}

	public void SetWavePos (int id)
	{
		waveId = id;
	}

	public float getAngle ()
	{
		return angle;
	}

	public void SetMultiplier(float newMultiplier){
		angularSpeedMultiplier = newMultiplier;
	}

	void Start ()
	{
		animator = gameObject.GetComponent<Animator> ();
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		angle += Input.GetAxis (inputName) * angularVelocity * angularSpeedMultiplier;
		if (angle > 2 * Mathf.PI) {
			angle -= 2 * Mathf.PI;
		} else if (angle < 0f	) {
			angle += 2 * Mathf.PI;
		}
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, angle * 180f / Mathf.PI));

		if (Input.GetAxis (inputName) > 0f) {
			GetComponent<SpriteRenderer> ().flipY = true;
			if (animator.isActiveAndEnabled) {
				animator.SetBool ("isMoving", true);
			}
			if (!audio.isPlaying) {
				audio.Play ();
			}
		} else if (Input.GetAxis (inputName) < 0f) {
			GetComponent<SpriteRenderer> ().flipY = false;
			if (animator.isActiveAndEnabled) {
				animator.SetBool ("isMoving", true);
			}
			if (!audio.isPlaying) {
				audio.Play ();
			}
		} else if (animator.isActiveAndEnabled){
			animator.SetBool ("isMoving", false);
			if (audio.isPlaying) {
				audio.Stop();
			}
		}
	}
}
