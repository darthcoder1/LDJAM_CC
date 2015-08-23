using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NestScript : MonoBehaviour 
{
	public Text HungerDisplay;
	public float HungerPerSecond = 1.0f / 60;

	private float Hunger;
	private PlayerController PC;

	// Use this for initialization
	void Start () 
	{
		Hunger = 1.0f;
		PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Hunger = Mathf.Max(Hunger - HungerPerSecond * Time.deltaTime, 0);

		if (HungerDisplay)
		{
			HungerDisplay.text = "Hunger: " + ((int)(Hunger*100)).ToString();
		}

		if (Hunger <= 0)
		{
			GetComponent<Animator>().SetTrigger("IsDead");
			PC.SendMessage("GameOver");
		}
	}

	void Feed()
	{
		Hunger = 1.0f;
	}
}
