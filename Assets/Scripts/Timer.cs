using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float seconds;
    int minutes;

    bool wasGameOverPlayedAlready = false;

    public bool isPlaying;

    public TMP_Text countdownDisplay;

    void Start()
    {
        //Get Initial time setup
        //Get pullInterval

        FormatTimeUnits();
        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying)
        {
            seconds -= Time.deltaTime;
            FormatTimeUnits();
            //Debug.Log(GetSecondsLeftForGame());
            countdownDisplay.text = FormatTimer();
        }
        else
        {
            if (!wasGameOverPlayedAlready)
            {
                wasGameOverPlayedAlready = true;
                GameOver();
            }
        }
    }

    public void AddTime(int timeToAdd)
    {
        seconds += timeToAdd;
        FormatTimeUnits();

    }

    private void FormatTimeUnits()
    {
        if (seconds >= 60)
        {
            minutes += 1;
            seconds -= 60;
        }
        if (seconds <= 0)
        {
            if (minutes > 0)
            {
                minutes -= 1;
                seconds += 60;
            }
            else
            {
                isPlaying = false;
            }
        }
    }

    private string FormatTimer()
    {
        string ans = minutes + ":";
        if (seconds < 10)
        {
            ans += "0";
        }
        ans += (int)seconds;
        return ans;
    }

    public float GetSecondsLeftForGame()
    {
        return minutes * 60 + seconds;
    }

    private void GameOver()
    {
        Debug.Log("Times Up!");
        LocalGameManager.Instance.audio.audioPlayer.Stop();
        LocalGameManager.Instance.audio.audioPlayer.PlayOneShot(LocalGameManager.Instance.audio.gameover, 0.5f);
    }
}
