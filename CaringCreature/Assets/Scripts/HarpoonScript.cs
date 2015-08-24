using UnityEngine;
using System.Collections;

public class HarpoonScript : MonoBehaviour 
{
	private Rigidbody2D RBComp;

	// Use this for initialization
	void Start () 
	{
		RBComp = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (RBComp.velocity.magnitude > 0.1)
		{
			Vector2 v = RBComp.velocity;
			float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + 135;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (!coll.gameObject.CompareTag ("Ship") && !coll.gameObject.CompareTag("Nest") && !coll.gameObject.CompareTag("Harpoon"))
		{
			float penetrationDepth = 2.5f;
			Rigidbody2D HarpoonRB = RBComp;
			Vector2 harpoonDir = HarpoonRB.velocity.normalized;
			transform.position = new Vector3(transform.position.x + harpoonDir.x * penetrationDepth,
			                                 transform.position.y + harpoonDir.y * penetrationDepth,
			                                 transform.position.z);
			
			transform.parent = coll.gameObject.transform;
			HarpoonRB.isKinematic = true;
			GetComponent<PolygonCollider2D>().enabled = false;
		}
	}
}
