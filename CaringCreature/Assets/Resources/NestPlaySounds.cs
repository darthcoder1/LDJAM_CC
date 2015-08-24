using UnityEngine;
using System.Collections;

public class NestPlaySounds : MonoBehaviour {

	public AudioSource[] sound;
	public int AudioTicks = 0; //counts to add delays at which the audio is played
	public int TrackIndex = 1; //


	// Use this for initialization
	void Start () {

		sound=GetComponents<AudioSource>();
	
	}
	
	// Update is called once per frame
	void Update () {

		AudioTicks++;
		//sound[TrackIndex].Play();

		if (AudioTicks == 120) {
		
			sound[TrackIndex].Play();
			AudioTicks = 0;

			if(TrackIndex <5)
			{
				TrackIndex++;

			}
			else
			{
				TrackIndex=0;
			}
		
		}	

	}
}
