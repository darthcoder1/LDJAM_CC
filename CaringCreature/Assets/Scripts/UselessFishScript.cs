using UnityEngine;
using System.Collections;

public class UselessFishScript : MonoBehaviour 
{

	private GameObject Player;
	private Vector3 Direction;
	private float Speed;

	// Use this for initialization
	void Start () 
	{
		Player = GameObject.FindGameObjectWithTag("Player");
		Direction = transform.position.x < Player.transform.position.x ? Vector3.right : Vector3.left;
		Speed = Random.Range(5,20);

		float distToPlayer = (Player.transform.position - transform.position).magnitude;
		if (distToPlayer > 250.0f)
		{
			GameObject.Destroy(gameObject);
		}

		float scale = Mathf.Clamp(Random.value, 0.1f, 0.5f);
		Vector3 addScale = new Vector3(transform.localScale.x * Mathf.Sign(Direction.x) * scale, transform.localScale.y*scale, transform.localScale.z*scale);
		transform.localScale = addScale;

		GetComponent<SpriteRenderer>().color = new Color(255,255,255,100);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 newPos = transform.position + Direction * Speed * Time.deltaTime;
		newPos.y += Mathf.Sin(newPos.x) * Time.deltaTime;
		transform.position = newPos;
	}
}
