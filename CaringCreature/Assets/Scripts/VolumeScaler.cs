using UnityEngine;
using System.Collections;

public class VolumeScaler : MonoBehaviour 
{
	private GameObject Player;

	public float HearingDistance = 30;

	// Use this for initialization
	void Start () 
	{
		Player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		float dist = (Player.transform.position - transform.position).magnitude;

		float volume = 1.0f - Mathf.Clamp(dist / HearingDistance,  0, 1);
		AudioSource[] components = GetComponents<AudioSource> ();

		foreach(AudioSource comp in components)
		{
			comp.volume = volume;
			comp.enabled = volume > 0;
		}
	}
}
