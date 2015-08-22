using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private Rigidbody2D RBComp;

	public float ClickForceStrength = 0.25f;
	public float MaxVelocity = 2.0f;
	public float WaterFrictionFactor = 0.05f;
	public float RotationSpeed = 0.1f;
	public float OverWaterGravityScale = 0.35f;
	public float UnderWaterGravityScale = 0.05f;

	private bool bOverWater = false;

	// Use this for initialization
	void Start () 
	{
		RBComp = GetComponent<Rigidbody2D>();
		RBComp.gravityScale = UnderWaterGravityScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 cursorPosVec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
		Vector2 cursorPos = new Vector2(cursorPosVec3.x, cursorPosVec3.y);

		Vector3 CurrentPlayerDir = transform.rotation * Vector3.right;
		Debug.DrawLine(transform.position, transform.position + CurrentPlayerDir * 10.0f, Color.blue);

		Color debugLineCol = Color.green;
		if (!bOverWater)
		{
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
			
			RBComp.velocity += -RBComp.velocity * WaterFrictionFactor * Time.deltaTime;
		}

		if (RBComp.velocity.magnitude > 0.1)
		{
			Vector2 v = RBComp.velocity;
			float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}

		Debug.DrawLine(transform.position, cursorPosVec3, debugLineCol);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("OverWater"))
		{
			bOverWater = true;
			RBComp.gravityScale = OverWaterGravityScale;
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.CompareTag("OverWater"))
		{
			bOverWater = false;
			RBComp.gravityScale = UnderWaterGravityScale;
		}
	}
}
