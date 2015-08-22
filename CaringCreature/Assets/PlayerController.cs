﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private Rigidbody2D RBComp;
	private SpriteRenderer SpriteComp;

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
	
	public Sprite NormalSprite;
	public Sprite MouthOpenSprite;
	public Sprite FatSprite;

	public PolygonCollider2D NormalCollider;
	public PolygonCollider2D MouthOpenCollider;
	public PolygonCollider2D FatCollider;

	private bool bOverWater = false;
	private bool bMouthOpen = false;
	public bool bShipEaten = false;

	private Vector2 CurPlayerDir;

	public float VomitDotHigherThan = 0.8f;
	public float VomitRange = 40.0f;

	// Use this for initialization
	void Start () 
	{
		RBComp = GetComponent<Rigidbody2D>();
		RBComp.gravityScale = UnderWaterGravityScale;

		SpriteComp = GetComponent<SpriteRenderer>();

		bShipEaten = false;
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

		UpdateGfx();
		UpdateScore();
	}

	void UpdateGfx()
	{
		if (bMouthOpen)
		{
			SpriteComp.sprite = MouthOpenSprite;
			NormalCollider.enabled = false;
			FatCollider.enabled = false;
			MouthOpenCollider.enabled = true;
		}
		else if (bShipEaten)
		{
			SpriteComp.sprite = FatSprite;
			NormalCollider.enabled = false;
			FatCollider.enabled = true;
			MouthOpenCollider.enabled = false;
		}
		else 
		{
			SpriteComp.sprite = NormalSprite;
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

		GameObject NestObj = GameObject.FindGameObjectWithTag("Nest");
		NestScript Nest = NestObj.GetComponent<NestScript>();

		Vector3 dirToNestVec3 = (Nest.transform.position - transform.position);
		float distanceToNest = dirToNestVec3.magnitude;
		dirToNestVec3.Normalize();
		Vector2 dirToNest = new Vector2(dirToNestVec3.x, dirToNestVec3.y);

		float dot = Vector2.Dot (CurPlayerDir, dirToNest);

		if (dot > VomitDotHigherThan && distanceToNest < VomitRange)
		{
			++ShipsFed;
			Nest.SendMessage("Feed");
		}

	}

	void FinishVomit()
	{
		if (bShipEaten)
		{
			bShipEaten = false;
			VomitPS.enableEmission = false;
			bMouthOpen = false;
		}
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
}
