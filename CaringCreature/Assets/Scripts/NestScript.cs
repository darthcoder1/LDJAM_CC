using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NestScript : MonoBehaviour 
{
	public Text HungerDisplay;
	public float HungerPerSecond = 1.0f / 60;

	private float Hunger;
	private PlayerController PC;
	private Animator AnimCtrl;
	private bool bIsEating;

	// Use this for initialization
	void Start () 
	{
		Hunger = 1.0f;
		PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		AnimCtrl = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Hunger = Mathf.Max(Hunger - HungerPerSecond * Time.deltaTime, 0);

		AnimCtrl.SetBool("bIsDead", Hunger <= 0.0f);
		AnimCtrl.SetBool("bIsEating", bIsEating);

		if (HungerDisplay)
		{
			HungerDisplay.text = "Hunger: " + ((int)(Hunger*100)).ToString();
		}

		if (Hunger <= 0)
		{
			PC.SendMessage("GameOver");

			GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
			mainCam.GetComponent<CameraScript>().target = gameObject;
		}
	}

	void Feed()
	{
		Hunger = 1.0f;
		bIsEating = true;
	}

	void StopFeeding()
	{
		bIsEating = false;
	}
}
