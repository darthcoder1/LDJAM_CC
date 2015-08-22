using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float DampingFactor = 0.15f;

	private GameObject PlayerObj;
	private float CurrentVelocity;

	// Use this for initialization
	void Start () 
	{
		PlayerObj = GameObject.FindGameObjectWithTag("Player");

		// set initial camara 
		transform.position = new Vector3(PlayerObj.transform.position.x, PlayerObj.transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () 
	{
		float interpX = Mathf.SmoothDamp(transform.position.x, PlayerObj.transform.position.x, ref CurrentVelocity, DampingFactor);
		//transform.position = new Vector3(interpX, transform.position.y, transform.position.z);
	}
}
