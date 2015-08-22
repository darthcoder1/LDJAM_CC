using UnityEngine;
using System.Collections;

public class WorldScript : MonoBehaviour 
{
	private GameObject[] SpawnPoints;
	private bool bSpawnInProgress = false;

	private GameObject Player;
	private GameObject Nest;

	public float SpawnIntervalForFish = 2.0f;
	private float LastSpawnedFishTime = 0.0f;

	// Use this for initialization
	void Start () 
	{
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPosition");
		Player = GameObject.FindGameObjectWithTag("Player");
		Nest = GameObject.FindGameObjectWithTag("Nest");
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

		LastSpawnedFishTime += Time.deltaTime;

		if (LastSpawnedFishTime >= SpawnIntervalForFish)
		{
			LastSpawnedFishTime = 0.0f;
			Vector2 pos = new Vector2();
			pos.x = Player.transform.position.x + (Mathf.Sign(Random.Range(-1,1)) * 150.0f);
			pos.y = Random.Range(-15, Nest.transform.position.y + 15);
			SpawnPrefab("TrumpetFish", pos);
		}
	}

	void SpawnPrefab(string name, Vector2 pos)
	{
		Object prefab = Resources.Load(name);
		GameObject go = (GameObject)GameObject.Instantiate(prefab, new Vector3(pos.x, pos.y, 10.0f), Quaternion.identity);
	}

	void SpawnShip()
	{
		int idx = Random.Range(0, SpawnPoints.Length-1);
		Object prefab = Resources.Load("ship");
		GameObject go = (GameObject)GameObject.Instantiate(prefab, SpawnPoints[idx].transform.position, SpawnPoints[idx].transform.rotation);
		bSpawnInProgress = false;
	}
}
