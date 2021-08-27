using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    private int points = 0;
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

    private string FormatPoints()
    {
        return points.ToString("n0");
    }
}
