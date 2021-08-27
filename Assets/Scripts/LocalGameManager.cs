using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    private static LocalGameManager _instance;

    public static LocalGameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public enum delayValues { VeryFast = 25, Fast = 75, Ok = 150, Slow = 300 };
    public enum delayScores { VeryFast = 5, Fast = 3, Ok = 2, Slow = 1 };

    public ScoreBoard scoreBoard;
    public Timer timer;

    public int scoreValue = 100;

    public float timeIntervals;
    float lastNumberPull;

    float gameLength;

    [SerializeField]
    public List<Ball> pulledBalls = new List<Ball>();
    public int tmpCounter;

    void Start()
    {
        lastNumberPull = 0;
    }

    void Update()
    {
        gameLength += Time.deltaTime;
        if(lastNumberPull + timeIntervals<= gameLength)
        {
            PullNumber();
        }
    }

    public int PullNumber()
    {
        tmpCounter++;
        pulledBalls.Add(new Ball(tmpCounter,gameLength));
        Debug.Log(tmpCounter);
        lastNumberPull = gameLength;
        return tmpCounter;
    }

    public void Click(int ballNumber)
    {
        int addedScore = CalculateNewAddedPoints(ballNumber);
        Debug.Log("addedScore: " + addedScore);
        if (addedScore > 0)
        {
            scoreBoard.AddPoints(addedScore);
        }
    }

    public int CalculateNewAddedPoints(int pulledBallValue)
    {
        foreach(Ball ball in pulledBalls)
        {
            if(ball.ballValue== pulledBallValue)
            {
                Debug.Log("Found Ball!");
                return GetPointsMultiplierBasedOnTime((gameLength - lastNumberPull) * 100) * scoreValue;
            }
        }
        Debug.Log("Ball not Found!");
        return 0;
    }

    public int GetPointsMultiplierBasedOnTime(float time)
    {
        Debug.Log(time);
        if(time>=0 && time< (int)delayValues.VeryFast)
        {
            return (int)delayScores.VeryFast;
        }
        else if (time >= (int)delayValues.VeryFast && time < (int)delayValues.Fast)
        {
            return (int)delayScores.Fast;
        }
        else if (time >= (int)delayValues.Fast && time < (int)delayValues.Ok)
        {
            return (int)delayScores.Ok;
        }
        else if (time >= (int)delayValues.Ok && time < (int)delayValues.Slow)
        {
            return (int)delayScores.Slow;
        }
        else
        {
            return 0;
        }
    }
}
