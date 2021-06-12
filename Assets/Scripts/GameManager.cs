using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	public PlayerInputManager inputManager;
	public GameObject playerA;
	public GameObject playerB;

	public GameObject playerPrefabA;
	public GameObject playerPrefabB;


	public Transform spawnPointA;
	public Transform spawnPointB;
	public bool waitingForPlayers = true;

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
            //waitingForPlayersUI.SetTrigger("Stop");

            //AudioManager.instance.Play("Ready");
        }

	}

}
