using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float DampingFactor = 0.15f;

	private GameObject PlayerObj;
	private Vector2 CurrentVelocity;

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
		Vector2 camPos = new Vector2(transform.position.x, transform.position.y);
		Vector2 playerPos = new Vector2(PlayerObj.transform.position.x, PlayerObj.transform.position.y);
		Vector2 interp = Vector2.SmoothDamp(camPos, playerPos, ref CurrentVelocity, DampingFactor);
		transform.position = new Vector3(interp.x, interp.y, transform.position.z);
	}
}
