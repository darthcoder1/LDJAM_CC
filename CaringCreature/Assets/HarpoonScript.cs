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
}
