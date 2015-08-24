using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private Rigidbody2D RBComp;

	public Text GameOverScreen;
	public Text ShipsDestroyedDisplay;
	public Text ShipsEatenDisplay;
	public Text ShipsFedDisplay;

	private bool bIsGameOver = false;
	public int ShipsDestroyed = 0;
	public int ShipsEaten = 0;
	public int ShipsFed = 0;
	
	public float ClickForceStrength = 0.25f;
	public float MaxVelocity = 2.0f;
	public float WaterFrictionFactor = 0.05f;
	public float RotationSpeed = 0.1f;
	public float OverWaterGravityScale = 0.35f;
	public float UnderWaterGravityScale = 0.05f;
	public ParticleSystem VomitPS;
	public int WaterLineY;
	public int WaterDetectionThreshold;
	public int MaxHits = 3;
	public bool bSimpliefiedControls = true;

	public PolygonCollider2D NormalCollider;
	public PolygonCollider2D MouthOpenCollider;
	public PolygonCollider2D FatCollider;

	private bool bOverWater = false;
	private bool bMouthOpen = false;
	public bool bShipEaten = false;
	private bool bFeeding = false;

	private Animator AnimCtrl;

	private Vector2 CurPlayerDir;
	private int ReceivedHits;

	// Use this for initialization
	void Start () 
	{
		RBComp = GetComponent<Rigidbody2D>();
		RBComp.gravityScale = UnderWaterGravityScale;

		AnimCtrl = GetComponent<Animator>();
		bShipEaten = false;

		ShipsDestroyed = 0;
		ShipsEaten = 0;
		ShipsFed = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.DrawLine(new Vector3(-1000, WaterLineY, transform.position.z),
		               new Vector3( 1000, WaterLineY, transform.position.z),
		               Color.black);

		Vector3 cursorPosVec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
		Vector2 cursorPos = new Vector2(cursorPosVec3.x, cursorPosVec3.y);

		Vector3 rotatedUpVec = transform.rotation * Vector3.up;
		Vector2 rotatedUpVec2 = new Vector2(rotatedUpVec.x, rotatedUpVec.y);
		
		float dot = Vector2.Dot (Vector2.up, rotatedUpVec2);

		Vector3 CurrentPlayerDir = transform.rotation * Vector3.right;
		CurPlayerDir = new Vector2(CurrentPlayerDir.x, CurrentPlayerDir.y);
		Debug.DrawLine(transform.position, transform.position + CurrentPlayerDir * 10.0f, Color.blue);

		if (bOverWater)
		{
			bOverWater = VomitPS.transform.position.y > (WaterLineY - WaterDetectionThreshold);
		}
		else
		{
			bOverWater = VomitPS.transform.position.y > (WaterLineY + WaterDetectionThreshold);
		}

		RBComp.gravityScale = bOverWater ? OverWaterGravityScale : UnderWaterGravityScale;

		Color debugLineCol = Color.green;
		if (!bOverWater)
		{
			if (Input.GetMouseButton(0) && !bFeeding)
			{
				Vector2 forceDir = (cursorPos - playerPos);
				float dirLen = forceDir.magnitude;
				forceDir.Normalize();

				if (dirLen < 5.0f)
				{
					forceDir = CurPlayerDir;
				}
				
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

		if (!bSimpliefiedControls)
		{
			if (Input.GetMouseButtonDown(1))
			{
				if (bShipEaten)
				{
					++ShipsEaten;
					// vomit
					StartVomit();
				}
				else
				{
					bMouthOpen = true;
				}
			}
			else if (Input.GetMouseButtonUp(1))
			{
				bMouthOpen = false;
			}
		}
		else
		{
			bMouthOpen = bOverWater;
		}


		UpdateGfx();
		UpdateScore();
	}

	void UpdateGfx()
	{
		AnimCtrl.SetBool("bMouthOpen", bMouthOpen);
		AnimCtrl.SetBool("bIsFat", bShipEaten);
	
		if (RBComp.velocity.magnitude > 0.1)
		{
			Vector3 dir = transform.rotation * Vector3.right;
			Vector2 dir2 = new Vector2(dir.x, dir.y);
			
			if (dir2.x < 0)
			{
				transform.localRotation *= Quaternion.Euler(180, 0, 0);
			}
			else
			{
				transform.localRotation *= Quaternion.Euler(0, 0, 0);
			}
		}


		if (bMouthOpen)
		{
			NormalCollider.enabled = false;
			FatCollider.enabled = false;
			MouthOpenCollider.enabled = true;
		}
		else if (bShipEaten)
		{
			NormalCollider.enabled = false;
			FatCollider.enabled = true;
			MouthOpenCollider.enabled = false;
		}
		else 
		{
			NormalCollider.enabled = true;
			FatCollider.enabled = false;
			MouthOpenCollider.enabled = false;
		}
	}

	void UpdateScore()
	{
		ShipsDestroyedDisplay.text = "Ships Destroyed: " + ShipsDestroyed.ToString();
		ShipsEatenDisplay.text = "Ships Eaten: " + ShipsEaten.ToString();
		ShipsFedDisplay.text = "Ships Fed: " + ShipsFed.ToString();
	}

	void StartVomit()
	{
		VomitPS.enableEmission = true;
		Invoke ("FinishVomit", 1.0f);
		bMouthOpen = true;
		bFeeding = true;

		if (!bSimpliefiedControls)
		{
			GameObject NestObj = GameObject.FindGameObjectWithTag("Nest");
			NestScript Nest = NestObj.GetComponent<NestScript>();
			
			Collider2D[] collObjs = Physics2D.OverlapPointAll(VomitPS.transform.position);
			
			foreach (Collider2D coll in collObjs)
			{
				if (coll.gameObject.CompareTag("Nest"))
				{
					++ShipsFed;
					Nest.SendMessage("Feed");
				}
			}
		}
		else
		{
			GameObject NestObj = GameObject.FindGameObjectWithTag("Nest");
			NestScript Nest = NestObj.GetComponent<NestScript>();

			++ShipsFed;
			Nest.SendMessage("Feed");
		}
	}

	void FinishVomit()
	{
		if (bShipEaten)
		{
			bShipEaten = false;
			bMouthOpen = false;
		}
		bFeeding = false;
		VomitPS.enableEmission = false;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Ship") && bMouthOpen && bOverWater && !bShipEaten && RBComp.velocity.y <= 0)
		{
			ShipController ship = collision.gameObject.GetComponent<ShipController>();
			ship.EatenBy = this;
			ship.SendMessage("Eaten");
		}
	}

	void GameOver()
	{
		bIsGameOver = true;
		GameOverScreen.enabled = true;
		Invoke("RestartMap", 3.0f);
	}

	void RestartMap()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag("Harpoon"))
		{
			// move it deeper into the creature

			float penetrationDepth = 1.0f;
			Rigidbody2D HarpoonRB = coll.gameObject.GetComponent<Rigidbody2D>();
			Vector2 harpoonDir = HarpoonRB.velocity.normalized;
			coll.transform.position = new Vector3(coll.transform.position.x + harpoonDir.x * penetrationDepth,
			                                      coll.transform.position.y + harpoonDir.y * penetrationDepth,
			                                      coll.transform.position.z);

			coll.transform.parent = gameObject.transform;
			HarpoonRB.isKinematic = true;
			coll.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
			if (++ReceivedHits >= MaxHits)
			{
				Die();
			}
		}
		else if (coll.gameObject.CompareTag("Nest"))
		{
			StartVomit();
		}
	}

	void Die()
	{
		bIsGameOver = true;
		GameOver ();
	}
}
