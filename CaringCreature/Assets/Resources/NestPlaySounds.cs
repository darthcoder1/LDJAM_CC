using UnityEngine;
using System.Collections;

public class NestPlaySounds : MonoBehaviour {

	private AudioSource[] sound;
	public float SoundPlayInterval = 4.0f;

	private float TimeSinceLastSoundPlayed;

	// Use this for initialization
	void Start () {

		sound=GetComponents<AudioSource>();
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		TimeSinceLastSoundPlayed += Time.deltaTime;
		if (TimeSinceLastSoundPlayed >= SoundPlayInterval)
		{
			sound[Random.Range(0,sound.Length)].Play();
		}

	}
}
