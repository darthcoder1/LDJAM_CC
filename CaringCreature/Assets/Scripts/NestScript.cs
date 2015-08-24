using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NestScript : MonoBehaviour 
{
	public Text HungerDisplay;
	public float HungerPerSecond = 1.0f / 60;

	public AudioClip[] MunchingSounds;
	public AudioClip[] DyingSounds;
	public AudioClip[] HungerSounds;
	public AudioClip[] DesperateHungerSounds;

	public float HungerSoundPlayInterval = 4.0f;
	private float HungerSoundLastPlayedTime;

	private float Hunger;
	private PlayerController PC;
	private Animator AnimCtrl;
	private AudioSource AudioComp;
	private bool bIsEating;

	// Use this for initialization
	void Start () 
	{
		Hunger = 1.0f;
		PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		AnimCtrl = GetComponent<Animator>();
		AudioComp = GetComponent<AudioSource>();
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

		UpdateSFX();
	}

	void UpdateSFX()
	{
		HungerSoundLastPlayedTime += Time.deltaTime;
		if (Hunger <= 0.01f && !bIsEating)
		{
			AudioComp.clip = DyingSounds[Random.Range (0, DyingSounds.Length)];
			AudioComp.Play();
		}
		else if (HungerSoundLastPlayedTime > HungerSoundPlayInterval && !bIsEating)
		{
			HungerSoundLastPlayedTime = 0.0f;

			AudioClip clipToPlay = null;
			if (Hunger > 0.5f)
			{
				clipToPlay = HungerSounds[Random.Range (0, HungerSounds.Length)];
			}
			else
			{
				clipToPlay = DesperateHungerSounds[Random.Range(0, DesperateHungerSounds.Length)];
			}
			AudioComp.clip = clipToPlay;
			AudioComp.Play();
		}
	}

	void Feed()
	{
		Hunger = 1.0f;
		bIsEating = true;

		AudioComp.clip = MunchingSounds[Random.Range (0, MunchingSounds.Length)];
		AudioComp.Play();
		
	}

	void StopFeeding()
	{
		bIsEating = false;
	}
}
