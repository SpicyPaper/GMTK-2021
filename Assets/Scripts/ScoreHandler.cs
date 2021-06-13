using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] private GameObject container = null;
    [SerializeField] private TMP_Text redScoreText = null;
    [SerializeField] private TMP_Text greenScoreText = null;

    [SerializeField] private float fadeTime = 1;
    [SerializeField] private float timeDisplayed = 3;

    public delegate void FinishedEvent();
    public static event FinishedEvent OnFinishedEvent;

    private int redScore = 0;
    private int greenScore = 0;

    private float startTime;

    private void OnEnable()
    {
        CharacterController2D.OnTakeDamageEvent += ShowNewScore;
    }

    private void OnDisable()
    {
        CharacterController2D.OnTakeDamageEvent -= ShowNewScore;
    }

    private void Update()
    {
        if (Time.time - startTime > timeDisplayed)
        {
            OnFinishedEvent();
            container.SetActive(false);
        }
    }

    private void ShowNewScore(bool redWon)
    {
        if (redWon)
        {
            redScore++;
        }
        else
        {
            greenScore++;
        }

        redScoreText.text = redScore.ToString();
        greenScoreText.text = greenScore.ToString();
        container.SetActive(true);

        startTime = Time.time;
    }
}
