﻿using UnityEngine;
using System.Collections;

public class WorldScript : MonoBehaviour 
{
	private GameObject[] SpawnPoints;
	private bool bSpawnInProgress = false;

	private GameObject Player;
	private GameObject Nest;

	public float SpawnIntervalForFish = 4.0f;
	public float ShipRespawnTimer = 3.0f;
	private float LastSpawnedFishTime = 0.0f;

	public int ShipsBeforeBoss = 4;
	private string[] FishPrefabs;

	enum WorldState
	{
		Intro,
		Game,
		Extro,
	}

	WorldState State;

	// Use this for initialization
	void Start () 
	{
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPosition");
		Player = GameObject.FindGameObjectWithTag("Player");
		Nest = GameObject.FindGameObjectWithTag("Nest");

		FishPrefabs = new string[3];
		FishPrefabs[0] = "TrumpetFish";
		FishPrefabs[1] = "Stupidwels";
		FishPrefabs[2] = "Glotzfisch";

		State = WorldState.Game;
	}

	void Update()
	{
		switch (State)
		{
		case WorldState.Intro:
			UpdateIntro();
			break;
		case WorldState.Game:
			UpdateGame();
			break;
		case WorldState.Extro:
			UpdateExtro();
			break;
		}
	}

	void UpdateIntro()
	{

	}

	void UpdateExtro()
	{
		
	}
	// Update is called once per frame
	void UpdateGame () 
	{
		GameObject[] AliveShips = GameObject.FindGameObjectsWithTag("Ship");

		if (!bSpawnInProgress && (AliveShips == null || AliveShips.Length <= 0))
		{
			bSpawnInProgress = true;
			Invoke("SpawnShip", ShipRespawnTimer);
		}

		LastSpawnedFishTime += Time.deltaTime;

		if (LastSpawnedFishTime >= SpawnIntervalForFish)
		{
			LastSpawnedFishTime = 0.0f;
			Vector2 pos = new Vector2();
			pos.x = Player.transform.position.x + (Mathf.Sign(Random.Range(-1,1)) * 150.0f);
			pos.y = Random.Range(-15, Nest.transform.position.y + 15);
			SpawnPrefab(FishPrefabs[Random.Range(0, FishPrefabs.Length)], pos);
		}
	}

	void SpawnPrefab(string name, Vector2 pos)
	{
		Object prefab = Resources.Load(name);
		GameObject go = (GameObject)GameObject.Instantiate(prefab, new Vector3(pos.x, pos.y, 10.0f), Quaternion.identity);
	}

	void SpawnShip()
	{
		PlayerController playerCtrl = Player.GetComponent<PlayerController>();

		/*if (playerCtrl.ShipsFed >= ShipsBeforeBoss + 1)
		{
			return;
		}*/

		int idx = Random.Range(0, SpawnPoints.Length-1);
		Object prefab = playerCtrl.ShipsFed < ShipsBeforeBoss ? Resources.Load("ship") : Resources.Load ("LongBoat_LEVEL2");
		GameObject go = (GameObject)GameObject.Instantiate(prefab, SpawnPoints[idx].transform.position, SpawnPoints[idx].transform.rotation);
		bSpawnInProgress = false;
	}
}
