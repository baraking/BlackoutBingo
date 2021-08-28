using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    private int points = 0;
    private int bingos = 0;
    public TMP_Text pointsDisplay;

    void Start()
    {
        pointsDisplay.text = FormatPoints();
    }

    public void AddPoints(int extraPoints)
    {
        points += extraPoints;
        pointsDisplay.text = FormatPoints();
    }

    public void AddBingos(int addedAmount)
    {
        bingos += addedAmount;
    }

    private string FormatPoints()
    {
        return points.ToString("n0");
    }
}
