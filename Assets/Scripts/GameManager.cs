using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public PlayerInputManager inputManager;
	public GameObject playerA;
	public GameObject playerB;

	public GameObject playerPrefabA;
	public GameObject playerPrefabB;


	public Transform spawnPointA;
	public Transform spawnPointB;
	public bool waitingForPlayers = true;
	public Animator waitingForPlayersUI;

	int currentLevel = 0;

	public int LOADLEVEL = -1;

	private void Awake()
	{
		Instance = this;
		//StartCoroutine(LoadFirstLevel());
	}

	void OnPlayerJoined(PlayerInput input)
	{
		if (!inputManager.joiningEnabled)
			return;

		if (playerA == null)
		{
			playerA = input.gameObject;
			inputManager.playerPrefab = playerPrefabB;

			playerA.transform.position = spawnPointA.position;
		}
		else
		{
			playerB = input.gameObject;

			playerB.transform.position = spawnPointB.position;

			inputManager.DisableJoining();
            waitingForPlayers = false;
            waitingForPlayersUI.SetTrigger("Stop");

            //AudioManager.instance.Play("Ready");
        }

	}

	IEnumerator LoadFirstLevel()
	{
		if (LOADLEVEL == -1)
		{
			currentLevel = GetRandomLevelIndex();
		}
		else
		{
			currentLevel = LOADLEVEL;
		}

		SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);

		yield return 0;

		spawnPointA = GameObject.FindGameObjectWithTag("SpawnPointA").transform;
		spawnPointB = GameObject.FindGameObjectWithTag("SpawnPointB").transform;
	}

	int GetRandomLevelIndex()
	{
		int random = currentLevel;
		while (random == currentLevel)
		{
			random = Random.Range(1, SceneManager.sceneCountInBuildSettings);
		}
		return random;
	}

}
