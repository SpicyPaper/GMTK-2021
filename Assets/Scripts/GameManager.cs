using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInputManager inputManager;
    public Camera mainCamera;
    public GameObject playerA;
    public GameObject playerB;

    public GameObject playerPrefabA;
    public GameObject playerPrefabB;

    public GameObject MagnetPointALeft;
    public GameObject MagnetPointARight;
    public GameObject MagnetPointBLeft;
    public GameObject MagnetPointBRight;

    public Transform spawnPointA;
    public Transform spawnPointB;
    public bool waitingForPlayers = true;
    public Animator waitingForPlayersUI;

    int currentLevel = 0;

    public int LOADLEVEL = -1;

    private void OnEnable()
    {
        CharacterController2D.OnTakeDamageEvent += OnPlayerHit;
        ScoreHandler.OnFinishedEvent += OnFinishedMenu;
    }

    private void OnDisable()
    {
        CharacterController2D.OnTakeDamageEvent -= OnPlayerHit;
        ScoreHandler.OnFinishedEvent -= OnFinishedMenu;
    }

    private void Awake()
    {
        Instance = this;
        //StartCoroutine(LoadFirstLevel());
    }

    private void OnFinishedMenu()
    {
        if (playerA != null && playerB != null)
        {
            playerA.transform.GetChild(0).GetComponent<CharacterController2D>().ResetCharacter();
            playerB.transform.GetChild(0).GetComponent<CharacterController2D>().ResetCharacter();
        }
    }

    private void OnPlayerHit(bool redWon)
    {
        playerA.transform.GetChild(0).position = spawnPointA.position;
        playerB.transform.GetChild(0).position = spawnPointB.position;

        playerA.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerB.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        playerA.transform.GetChild(1).GetChild(0).GetComponent<MagnetLauncherManager>().Reset();
        playerA.transform.GetChild(1).GetChild(1).GetComponent<MagnetLauncherManager>().Reset();

        playerB.transform.GetChild(1).GetChild(0).GetComponent<MagnetLauncherManager>().Reset();
        playerB.transform.GetChild(1).GetChild(1).GetComponent<MagnetLauncherManager>().Reset();
    }

    void OnPlayerJoined(PlayerInput input)
    {
        if (!inputManager.joiningEnabled)
            return;

        if (playerA == null)
        {
            playerA = input.transform.parent.gameObject;
            inputManager.playerPrefab = playerPrefabB;

            input.transform.position = spawnPointA.position;
            MagnetFieldHandler magnetFieldHandler = playerA.GetComponent<MagnetFieldHandler>();
            magnetFieldHandler.magnetPointLeft = MagnetPointALeft;
            magnetFieldHandler.magnetPointRight = MagnetPointARight;

            List<PointEffector2D> points = magnetFieldHandler.characterController.GetComponent<CharacterController2D>().pointEffectors;
            points.Add(MagnetPointALeft.GetComponent<PointEffector2D>());
            points.Add(MagnetPointARight.GetComponent<PointEffector2D>());

            magnetFieldHandler.magnetLauncherLeft.magnetPoint = MagnetPointALeft;
            magnetFieldHandler.magnetLauncherLeft.mainCamera = mainCamera;
            magnetFieldHandler.magnetLauncherRight.magnetPoint = MagnetPointARight;
            magnetFieldHandler.magnetLauncherRight.mainCamera = mainCamera;

            magnetFieldHandler.magnetLauncherLeft.isPlayerRed = true;
            magnetFieldHandler.magnetLauncherRight.isPlayerRed = true;

            MagnetPointALeft.GetComponent<MagnetManager>().manager = magnetFieldHandler.magnetLauncherLeft;
            MagnetPointARight.GetComponent<MagnetManager>().manager = magnetFieldHandler.magnetLauncherRight;

            magnetFieldHandler.magnetLauncherLeft.Init();
            magnetFieldHandler.magnetLauncherRight.Init();
        }
        else
        {
            playerB = input.transform.parent.gameObject;

            input.transform.position = spawnPointB.position;
            MagnetFieldHandler magnetFieldHandler = playerB.GetComponent<MagnetFieldHandler>();
            magnetFieldHandler.magnetPointLeft = MagnetPointBLeft;
            magnetFieldHandler.magnetPointRight = MagnetPointBRight;

            List<PointEffector2D> points = magnetFieldHandler.characterController.GetComponent<CharacterController2D>().pointEffectors;
            points.Add(MagnetPointBLeft.GetComponent<PointEffector2D>());
            points.Add(MagnetPointBRight.GetComponent<PointEffector2D>());

            magnetFieldHandler.magnetLauncherLeft.magnetPoint = MagnetPointBLeft;
            magnetFieldHandler.magnetLauncherLeft.mainCamera = mainCamera;
            magnetFieldHandler.magnetLauncherRight.magnetPoint = MagnetPointBRight;
            magnetFieldHandler.magnetLauncherRight.mainCamera = mainCamera;

            magnetFieldHandler.magnetLauncherLeft.isPlayerRed = false;
            magnetFieldHandler.magnetLauncherRight.isPlayerRed = false;

            MagnetPointBLeft.GetComponent<MagnetManager>().manager = magnetFieldHandler.magnetLauncherLeft;
            MagnetPointBRight.GetComponent<MagnetManager>().manager = magnetFieldHandler.magnetLauncherRight;

            magnetFieldHandler.magnetLauncherLeft.Init();
            magnetFieldHandler.magnetLauncherRight.Init();

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
