using UnityEngine;
using System.Collections;

public class VortexController : MonoBehaviour
{

	[SerializeField]
	private float rotationRate = 60f;

	private int wavePos;
	private float angle;

	// Update is called once per frame
	void Update ()
	{
		transform.Rotate (new Vector3 (0f, 0f, rotationRate * Time.deltaTime));
	}

	void OnTriggerEnter2D (Collider2D coll)
	{
		//Debug.Log ("Collided with " + coll.gameObject.name);
		if (coll.transform.tag == "Player") {
			Destroy (coll.gameObject);
		} 
	}

	public void Initialize(int wP, float a) {
		wavePos = wP;
		angle = a;
	}

	public float GetAngle(){
		return angle;
	}

	public int GetWavePos() {
		return wavePos;
	}

	public void SetWavePos(int wP){
		wavePos = wP;
	}
}
