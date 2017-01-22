using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	public float angularVelocity = 0.0625f;
	private int waveId;
	private float angle;
	private string inputName;

	public void Initialize (int wId, float ang, string iN)
	{
		waveId = wId;
		angle = ang * Mathf.PI / 180f;
		inputName = iN;
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

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		angle += Input.GetAxis (inputName) * angularVelocity;
		if (angle > 2 * Mathf.PI) {
			angle -= 2 * Mathf.PI;
		} else if (angle < 0f	) {
			angle += 2 * Mathf.PI;
		}
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, angle * 180f / Mathf.PI));
		if (Input.GetAxis (inputName) > 0f) {
			GetComponent<SpriteRenderer> ().flipY = true;
		} else if (Input.GetAxis (inputName) < 0f) {
			GetComponent<SpriteRenderer> ().flipY = false;
		}
	}
}
