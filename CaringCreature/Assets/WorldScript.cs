using UnityEngine;
using System.Collections;

public class WorldScript : MonoBehaviour 
{
	private GameObject[] SpawnPoints;
	private bool bSpawnInProgress = false;

	// Use this for initialization
	void Start () 
	{
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPosition");
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameObject[] AliveShips = GameObject.FindGameObjectsWithTag("Ship");

		if (!bSpawnInProgress && (AliveShips == null || AliveShips.Length <= 0))
		{
			bSpawnInProgress = true;
			Invoke("SpawnShip", 3.0f);
		}
	}

	void SpawnShip()
	{
		int idx = Random.Range(0, SpawnPoints.Length-1);
		Object prefab = Resources.Load("ship");
		GameObject go = (GameObject)GameObject.Instantiate(prefab, SpawnPoints[idx].transform.position, SpawnPoints[idx].transform.rotation);
		bSpawnInProgress = false;
	}
}
