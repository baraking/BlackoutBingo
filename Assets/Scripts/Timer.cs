using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    float seconds;
    int minutes;

    bool isPlaying;

    public TMP_Text countdownDisplay;

    void Start()
    {
        //Get Initial time setup

        //Default to replace
        seconds = 75;

        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying)
        {
            seconds -= Time.deltaTime;
            Debug.Log(seconds);
            FormatTimeUnits();
            countdownDisplay.text = FormatTimer();
        }
        else
        {
            GameOver();
        }
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

    private void GameOver()
    {
        Debug.Log("Times Up!");
    }
}
