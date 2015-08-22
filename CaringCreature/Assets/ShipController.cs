using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

	private bool bOverWater;
	private bool bIsSinking;

	public PlayerController EatenBy;
	private float TimeSinceEaten;
	private Vector2 OriginalScale;

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

		EatenBy = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (EatenBy)
		{
			TimeSinceEaten += Time.deltaTime;

			const float TimeForScaling = 1.0f;
			float T = Mathf.Clamp(TimeSinceEaten / TimeForScaling, 0, 1.0f);
			transform.localScale = new Vector3(Mathf.Lerp(OriginalScale.x, 0.0f, T), Mathf.Lerp(OriginalScale.y, 0.0f, T), 1.0f);
			transform.position = Vector3.Lerp(transform.position, EatenBy.transform.position, T);

			if (T > 0.95f)
			{
				EatenBy.ShipsInStomach += 1;
				GameObject.Destroy(this);
				return;
			}
		}
		else
		{
			//RBComp.gravityScale = bOverWater ? OverWaterGravityScale : UnderWaterGravityScale;
			RBComp.drag = bOverWater ? 0.05f : 5.0f;
			
			if (!bOverWater)
			{
				RBComp.AddForce(new Vector2(0, UpwardForce));
			}
		}
	}
	
	void Eaten()
	{
		GameObject.DestroyObject(GetComponent<BoxCollider2D>());
		GameObject.DestroyObject(GetComponent<Rigidbody2D>());
		TimeSinceEaten = 0.0f;
		OriginalScale = new Vector2(transform.localScale.x, transform.localScale.y);
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
