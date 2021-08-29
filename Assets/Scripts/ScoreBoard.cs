using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    private int points = 0;
    private int bingos = 0;

    public static readonly int ORIGINAL_POINTS_MULTIPLIER = 1;

    private int pointsMultiplier;
    private float multiplierTimeLeft;

    public TMP_Text pointsDisplay;
    public TMP_Text currentMultiplier;

    void Start()
    {
        pointsDisplay.text = FormatPoints();
        pointsMultiplier = ORIGINAL_POINTS_MULTIPLIER;
        FormatMultiplier();
    }

    private void Update()
    {
        if (multiplierTimeLeft > 0)
        {
            multiplierTimeLeft -= Time.deltaTime;
            if(multiplierTimeLeft <= 0)
            {
                multiplierTimeLeft = 0;
                ResetMultipliers();
            }
        }
    }

    public void AddPoints(int extraPoints)
    {
        points += extraPoints*pointsMultiplier;
        pointsDisplay.text = FormatPoints();
    }

    public void AddBingos(int addedAmount)
    {
        bingos += addedAmount;
    }

    public void AddMultiplier(int multiplier,float time)
    {
        pointsMultiplier *= multiplier;
        FormatMultiplier();
        multiplierTimeLeft += time;
    }

    private void ResetMultipliers()
    {
        pointsMultiplier = ORIGINAL_POINTS_MULTIPLIER;
        FormatMultiplier();
    }

    private string FormatPoints()
    {
        return points.ToString("n0");
    }

    private void FormatMultiplier()
    {
        currentMultiplier.text = "X" + pointsMultiplier;
    }
}
