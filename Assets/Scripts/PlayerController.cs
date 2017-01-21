using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	public float angularVelocity = 0.0625f;
	private int waveId;
	private float angle;

	public void Initialize (int wId, float ang)
	{
		waveId = wId;
		angle = ang;
	}

	public int GetWaveId ()
	{
		return waveId;
	}

	public void SetWaveId (int id)
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
		angle += Input.GetAxis ("Player1") * angularVelocity;
		if (angle > 2 * Mathf.PI) {
			angle -= 2 * Mathf.PI;
		} else if (angle < -2 * Mathf.PI) {
			angle += 2 * Mathf.PI;
		}
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, angle * 180f / Mathf.PI));
		if (Input.GetAxis ("Player1") > 0f) {
			GetComponent<SpriteRenderer> ().flipY = true;
		} else if (Input.GetAxis ("Player1") < 0f) {
			GetComponent<SpriteRenderer> ().flipY = false;
		}
		//Debug.Log (angle);
	}
}
