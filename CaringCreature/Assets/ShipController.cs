using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

	private bool bOverWater;

	private Rigidbody2D RBComp;
	public float WaterFrictionFactor = 0.05f;
	public float OverWaterGravityScale = 0.35f;
	public float UnderWaterGravityScale = -0.35f;

	public float UpwardForce = 3.0f; // 9.81 is the opposite of the default gravity, which is 9.81. If we want the boat not to behave like a submarine the upward force has to be higher than the gravity in order to push the boat to the surface


	// Use this for initialization
	void Start () 
	{
		RBComp = GetComponent<Rigidbody2D>();
		RBComp.gravityScale = OverWaterGravityScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//RBComp.gravityScale = bOverWater ? OverWaterGravityScale : UnderWaterGravityScale;
		RBComp.drag = bOverWater ? 0.05f : 5.0f;

		if (!bOverWater)
		{
			RBComp.AddForce(new Vector2(0, UpwardForce));
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("OverWater"))
		{
			bOverWater = true;
			//RBComp.gravityScale = OverWaterGravityScale;
		}
	}
	
	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.CompareTag("OverWater"))
		{
			bOverWater = false;
			//RBComp.gravityScale = UnderWaterGravityScale;
		}
	}
}
