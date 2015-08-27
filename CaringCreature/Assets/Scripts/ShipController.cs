using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

	private bool bOverWater;
	public bool bIsSinking;
	private float TimeSinking;
	public float SinkingTimeBeforeDestroy = 3.0f;
	private bool bHasHarpoon;

	private bool bIsThrowing;
	private bool bIsReloading;

	public PlayerController EatenBy;
	private float TimeSinceEaten;
	private Vector2 OriginalScale;

	private Rigidbody2D RBComp;
	private Animator AnimCtrl;
	private AudioSource AudioComp;
	private WorldScript WorldInfo;

	public float WaterFrictionFactor = 0.05f;
	public float HarpoonReloadTime = 3.0f;
	public float HarpoonThrowTime = 2.0f;

	public float UpwardForce = 25.0f; // 9.81 is the opposite of the default gravity, which is 9.81. If we want the boat not to behave like a submarine the upward force has to be higher than the gravity in order to push the boat to the surface

	private int WaterLineY;
	private int WaterDetectionThreshold;

	private float Direction;

	public AudioClip[] HarpoonSounds;


	// Use this for initialization
	void Start () 
	{
		RBComp = GetComponent<Rigidbody2D>();
		AnimCtrl = GetComponent<Animator>();
		AudioComp = GetComponent<AudioSource>();

		EatenBy = null;

		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		PlayerController pc = playerObj.GetComponent<PlayerController>();
		WorldInfo = GameObject.FindGameObjectWithTag("World").GetComponent<WorldScript>();

		WaterLineY = pc.WaterLineY;

		Direction = (float)Random.Range(-2500, 2500) / 100.0f;

		bHasHarpoon = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		bOverWater = transform.position.y - 5 > WaterLineY;
		bIsSinking = bIsSinking || transform.position.y < WaterLineY - 25;

		Rigidbody2D RBComp = GetComponent<Rigidbody2D>();
		if (RBComp && !bIsSinking && !bOverWater)
		{
			RBComp.AddForce(new Vector2(Direction * Time.deltaTime, 0.0f));
		}

		if (transform.position.x > 500 || transform.position.x < -500)
		{
			GameObject.Destroy(gameObject);
		}

		Vector3 shipUpVec = transform.rotation * Vector3.up;
		Vector2 shipUpVec2 = new Vector2(shipUpVec.x, shipUpVec.y);
		
		float dot = Vector2.Dot (Vector2.up, shipUpVec2);
		
		if (dot < 0.5f && !bIsSinking)
		{
			bIsSinking = true;

			PlayerController PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
			++PC.ShipsDestroyed;
		}

		if (bIsSinking && transform.position.y < -25)
		{
			GameObject.Destroy(gameObject);
		}

		if (EatenBy)
		{
			TimeSinceEaten += Time.deltaTime;

			const float TimeForEating = 1.0f;
			float T = Mathf.Clamp(TimeSinceEaten / TimeForEating, 0, 1.0f);
			transform.localScale = new Vector3(Mathf.Lerp(OriginalScale.x, 0.0f, T), Mathf.Lerp(OriginalScale.y, 0.0f, T), 1.0f);
			transform.position = Vector3.Lerp(transform.position, EatenBy.transform.position, T);

			if (TimeSinceEaten >= TimeForEating)
			{
				EatenBy.bShipEaten = true;
				GameObject.Destroy(gameObject);
				return;
			}
		}
		else
		{
			//RBComp.gravityScale = bOverWater ? OverWaterGravityScale : UnderWaterGravityScale;
			RBComp.drag = bOverWater ? 0.05f : 5.0f;
			
			if (!bOverWater && !bIsSinking)
			{
				RBComp.AddForce(new Vector2(Direction * Time.deltaTime, UpwardForce * Time.deltaTime));
			}
		}

		if (WorldInfo.State == WorldScript.WorldState.Game)
		{
			UpdateHarpoon();
		}

		UpdateGfx();
	}

	void UpdateHarpoon()
	{
		if (bIsSinking) { return; }

		if (bHasHarpoon && !bIsThrowing && EatenBy == null)
		{
			bIsThrowing = true;
			Invoke ("ThrowHarpoon", HarpoonThrowTime);
		}
		else if (!bIsReloading)
		{
			bIsReloading = true;
			Invoke ("ReloadHarpoon", HarpoonReloadTime);
		}
	}

	void ThrowHarpoon()
	{
		GameObject Player = GameObject.FindGameObjectWithTag("Player");
		if (Player)
		{
			ThrowHarpoonAt(Player);
		}
		bIsThrowing = false;
	}

	public Vector3 BallisticVel(Transform from, Transform target)
	{ 
		Vector3 dir = target.position - from.position; 
		float h = dir.y; dir.y =0; 
		float dist = dir.magnitude; dir.y = dist; dist += h; 
		float vel = Mathf.Sqrt(dist*Physics.gravity.magnitude); 
		return vel*dir.normalized; 
	}

	private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget) {
		// calculate vectors
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;
		toTargetXZ.y = 0;
		
		// calculate xz and y
		float y = toTarget.y;
		float xz = toTargetXZ.magnitude;
		
		// calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
		// where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
		// so xz = v0xz * t => v0xz = xz / t
		// and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
		float t = timeToTarget;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;
		
		// create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        // get direction of xz but with magnitude 1
		result *= v0xz;                                // set magnitude of xz to v0xz (starting speed in xz plane)
		result.y = v0y;                                // set y to v0y (starting speed of y plane)
		
		return result;
	}

	void ThrowHarpoonAt(GameObject obj)
	{
		if (bHasHarpoon)
		{
			Transform harpoonTransform = transform.GetChild(0);// GetComponentInChildren<Transform>();

			Debug.DrawLine(harpoonTransform.position, obj.transform.position, Color.red);

			Object prefab = Resources.Load("harpoon");
			GameObject go = (GameObject)GameObject.Instantiate(prefab, harpoonTransform.position, harpoonTransform.rotation);
			//go.transform.localScale = harpoonTransform.localScale;

			go.GetComponent<Rigidbody2D>().velocity = calculateBestThrowSpeed(harpoonTransform.position, obj.transform.position, 5);
			bHasHarpoon = false;

			AudioComp.clip = HarpoonSounds[Random.Range(0,HarpoonSounds.Length)];
			AudioComp.Play ();
		}
	}

	void ReloadHarpoon()
	{
		bHasHarpoon = true;
		bIsReloading = false;
	}

	void UpdateGfx()
	{
		AnimCtrl.SetBool("bHasHarpoon", bHasHarpoon);
		AnimCtrl.SetBool("bIsSinking", bIsSinking);
	}
	
	void Eaten()
	{
		if (bIsSinking)
		{
			EatenBy = null;
			return;
		}

		GameObject.DestroyObject(GetComponent<BoxCollider2D>());
		GameObject.DestroyObject(GetComponent<Rigidbody2D>());
		TimeSinceEaten = 0.0f;
		OriginalScale = new Vector2(transform.localScale.x, transform.localScale.y);
	}
}
