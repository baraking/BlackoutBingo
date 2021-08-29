using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum delayScores { AddTime = 1, MultiplyPoints = 2};

    public static readonly int TIME_TO_ADD=10;
    public static readonly int POINTS_MULTIPLIER = 2;
    public static readonly int TIME_FOR_POINTS_MULTIPLIER = 5;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddTime()
    {
        LocalGameManager.Instance.timer.AddTime(TIME_TO_ADD);
    }

    public void MultiplyPoints()
    {
        LocalGameManager.Instance.scoreBoard.AddMultiplier(POINTS_MULTIPLIER, TIME_FOR_POINTS_MULTIPLIER);
    }
}
