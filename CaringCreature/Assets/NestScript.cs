using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NestScript : MonoBehaviour 
{
	public Text HungerDisplay;
	public float HungerPerSecond = 1.0f / 60;

	private float Hunger;

	// Use this for initialization
	void Start () 
	{
		Hunger = 1.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Hunger -= HungerPerSecond * Time.deltaTime;

		if (HungerDisplay)
		{
			HungerDisplay.text = "Hunger: " + ((int)(Hunger*100)).ToString();
		}
	}

	void Feed()
	{
		Hunger = 1.0f;
	}
}
