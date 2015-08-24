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
	private WorldScript WorldInfo;
	private bool bIsEating;

	private GameObject[] GrownBabies;

	// Use this for initialization
	void Start () 
	{
		Hunger = 1.0f;
		PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		AnimCtrl = GetComponent<Animator>();
		AudioComp = GetComponent<AudioSource>();
		WorldInfo = GameObject.FindGameObjectWithTag("World").GetComponent<WorldScript>();

		GrownBabies = GameObject.FindGameObjectsWithTag("GrownBaby");
		foreach(GameObject obj in GrownBabies)
		{
			obj.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (WorldInfo.State != WorldScript.WorldState.Game)
		{
			return;
		}

		Hunger = Mathf.Max(Hunger - HungerPerSecond * Time.deltaTime, 0);

		bool bSurvived = PC.ShipsFed > WorldInfo.ShipsBeforeBoss;
		AnimCtrl.SetBool("bIsDead", Hunger <= 0.0f);
		AnimCtrl.SetBool("bIsEating", bIsEating && !bSurvived);
		AnimCtrl.SetBool("bSurvived", bSurvived);

		if (HungerDisplay)
		{
			HungerDisplay.text = "Hunger: " + ((int)(Hunger*100)).ToString();
		}

		if (Hunger <= 0)
		{
			GameObject.FindGameObjectWithTag("World").SendMessage("TriggerExtro_BabiesDied");
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
		else if (Hunger < 0.75f && HungerSoundLastPlayedTime > HungerSoundPlayInterval && !AudioComp.isPlaying)
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

		Invoke("PlayEatingSound", 0.5f);
	}

	void PlayEatingSound()
	{
		AudioComp.clip = MunchingSounds[Random.Range (0, MunchingSounds.Length)];
		AudioComp.Play();
	}

	void StopFeeding()
	{
		bIsEating = false;
	}

	void BabiesGrow()
	{
		foreach(GameObject obj in GrownBabies)
		{
			obj.SetActive(true);
		}
	}
}
