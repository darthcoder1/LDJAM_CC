using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private Rigidbody2D RBComp;

	public float ClickForceStrength = 0.2f;

	// Use this for initialization
	void Start () 
	{
		RBComp = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 cursorPosVec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
		Vector2 cursorPos = new Vector2(cursorPosVec3.x, cursorPosVec3.y);

		//Vector2 forceDir = Vector2.
		Debug.DrawLine(transform.position, cursorPosVec3, Color.red);
		//RBComp.AddForce()

	}
}
