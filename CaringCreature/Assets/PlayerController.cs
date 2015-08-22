using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private Rigidbody2D RBComp;

	public float ClickForceStrength = 0.25f;
	public float MaxVelocity = 2.0f;
	public float WaterFriction = 0.1f;

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

		Color debugLineCol = Color.green;
		if (Input.GetMouseButton(0))
		{
			Vector2 forceDir = (cursorPos - playerPos).normalized;

			RBComp.AddForce(forceDir * ClickForceStrength, ForceMode2D.Impulse);
			if (RBComp.velocity.magnitude > MaxVelocity)
			{
				RBComp.velocity = (RBComp.velocity / RBComp.velocity.magnitude) * MaxVelocity;
			}
			debugLineCol = Color.red;

		}

		Debug.DrawLine(transform.position, cursorPosVec3, debugLineCol);
	}
}
