using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI yourTimeText;
    [SerializeField] private GameObject winScreen;
    private bool hasWon = false;

    private float timer = 0;

    private DeathManager dm;
    private void Start()
    {
        dm = FindAnyObjectByType<DeathManager>();


        StartGame();
    }

    private string timerString;
    private void Update()
    {
        if (!hasWon)timer += Time.deltaTime / 86400;

        float t = timer * 24f;
        float hours = Mathf.Floor(t);
        t *= 60;
        float minutes = Mathf.Floor(t % 60);
        t *= 60;
        float seconds = Mathf.Floor(t % 60);
        t *= 1000;
        float milliseconds = Mathf.Floor(t % 1000);
        timerString = minutes.ToString("00") + ":" + seconds.ToString() + "." + milliseconds.ToString();
        timerText.text = timerString;
    }
    public void StartGame()
    {
        winScreen.SetActive(false);
        timer = 0;
        Debug.Log("YO");
        hasWon = false;
        dm.RestartGame();
    }


    public void WinGame()
    {
        yourTimeText.text = "Your Time: " + timerString;
        winScreen.SetActive(true);
        hasWon = true;
    }
}
