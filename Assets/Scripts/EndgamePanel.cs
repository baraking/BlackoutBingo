using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndgamePanel : MonoBehaviour
{
    public TMP_Text playerNames;
    public TMP_Text ScoreValues;

    public void ClearData()
    {
        playerNames.text = "";
        ScoreValues.text = "";
    }

    public void AddPlayerData(string player, string scoreValue)
    {
        AddPlayerName(player);
        AddScoreValue(scoreValue);
    }

    private void AddPlayerName(string player)
    {
        playerNames.text += player + "\n";
    }

    private void AddScoreValue(string scoreValue)
    {
        ScoreValues.text += scoreValue + "\n";
    }
}
