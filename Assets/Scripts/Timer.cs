using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Timer : MonoBehaviour
{
    public float seconds;
    int minutes;

    bool wasGameOverPlayedAlready = false;

    public bool isPlaying;
    bool gameStarted;

    public TMP_Text countdownDisplay;

    void Start()
    {
        //Get Initial time setup
        //Get pullInterval

        //FormatTimeUnits();
        //isPlaying = true;
    }

    void Update()
    {
        if (gameStarted)
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
    }

    public void SetTime(int newSeconds)
    {
        minutes = 0;
        seconds = newSeconds;
        FormatTimeUnits();
        wasGameOverPlayedAlready = false;
        isPlaying = true;
        gameStarted = true;
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
        RemoteGameManager.Instance.photonView.RPC("SendPlayerEndGameData", RpcTarget.Others, int.Parse(PhotonNetwork.LocalPlayer.NickName), LocalGameManager.Instance.scoreBoard.GetPoints());
    }
}
