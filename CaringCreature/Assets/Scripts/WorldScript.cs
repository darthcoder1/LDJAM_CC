using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldScript : MonoBehaviour 
{
	public Text ExtroMessage;
	public Text RestartText;
	private GameObject[] SpawnPoints;
	private bool bSpawnInProgress = false;

	private GameObject Player;
	private GameObject Nest;

	public float SpawnIntervalForFish = 4.0f;
	public float ShipRespawnTimer = 3.0f;
	private float LastSpawnedFishTime = 0.0f;

	public int ShipsBeforeBoss = 4;
	private string[] FishPrefabs;

	public enum WorldState
	{
		Intro,
		Game,
		Extro,
		WaitForRestart,
		Restart,
	}

	enum EndState
	{
		BabiesSurvived,
		CreatureDied,
		BabiesDied,
	}

	public WorldState State;
	EndState EndStatus;

	void WaitForRestart()
	{
		State = WorldState.WaitForRestart;
	}

	void TriggerExtro_Survived()
	{
		EndStatus = EndState.BabiesSurvived;
		State = WorldState.Extro;
	}

	void TriggerExtro_BabiesDied()
	{
		EndStatus = EndState.BabiesDied;
		State = WorldState.Extro;
	}

	void TriggerExtro_CreatureDied()
	{
		EndStatus = EndState.CreatureDied;
		State = WorldState.Extro;
	}

	// Use this for initialization
	void Start () 
	{
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPosition");
		Player = GameObject.FindGameObjectWithTag("Player");
		Nest = GameObject.FindGameObjectWithTag("Nest");

		FishPrefabs = new string[6];
		FishPrefabs[0] = "TrumpetFish";
		FishPrefabs[1] = "Stupidwels";
		FishPrefabs[2] = "Glotzfisch";
		FishPrefabs[3] = "Krakenkatze";
		FishPrefabs[4] = "Korkenzieherfisch";
		FishPrefabs[5] = "Squarefish";

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
		case WorldState.WaitForRestart:
			RestartText.enabled = true;

			if (Input.GetMouseButtonDown(0))
			{
				State = WorldState.Restart;
			}
			break;
		case WorldState.Restart:
			Application.LoadLevel(Application.loadedLevel);
			break;
		}
	}

	void UpdateIntro()
	{

	}

	void UpdateExtro()
	{
		GameObject NestObj = GameObject.FindGameObjectWithTag("Nest");
		GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");

		switch (EndStatus)
		{
		case EndState.BabiesDied:
			mainCam.GetComponent<CameraScript>().target = NestObj;
			ExtroMessage.text = "Your babies starved to death ... ";
			ExtroMessage.enabled = true;
			Invoke ("WaitForRestart", 2.0f);
			break;
		case EndState.CreatureDied:
			ExtroMessage.text = "You died! And your babies will follow you soon ... ";
			ExtroMessage.enabled = true;
			Invoke ("WaitForRestart", 2.0f);
			break;
		case EndState.BabiesSurvived:
			mainCam.GetComponent<CameraScript>().target = NestObj;
			NestObj.SendMessage("BabiesGrow");
			Invoke ("WaitForRestart", 2.0f);
			break;
		}
	}

	// Update is called once per frame
	void UpdateGame () 
	{
		PlayerController playerCtrl = Player.GetComponent<PlayerController>();
		
		if (playerCtrl.ShipsFed > ShipsBeforeBoss)
		{
			TriggerExtro_Survived();
		}

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
		Object prefab = playerCtrl.ShipsFed < ShipsBeforeBoss-1 ? Resources.Load("ship") : Resources.Load ("LongBoat_LEVEL2");
		GameObject go = (GameObject)GameObject.Instantiate(prefab, SpawnPoints[idx].transform.position, SpawnPoints[idx].transform.rotation);
		bSpawnInProgress = false;
	}
}
