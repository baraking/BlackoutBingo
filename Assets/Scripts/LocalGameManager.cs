using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocalGameManager : MonoBehaviourPunCallbacks
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

    public enum delayValues { VeryFast = 50, Fast = 100, Ok = 200, Slow = 300 };
    public enum delayScores { VeryFast = 5, Fast = 3, Ok = 2, Slow = 1 };
    public enum meterScores { VeryFast = 100, Fast = 60, Ok = 30, Slow = 15 };

    public PlayerSpawner playerSpawner;
    public static readonly string BINGO = "BINGO";
    public static readonly Color[] COLORS = { Color.blue, Color.red, Color.magenta, Color.green, Color.yellow };

    public static readonly int SHOWN_DRAWN_BALLS = 7;
    public static readonly int DRAWN_BALLS_MOVEMENT_DISTANCE = 150;

    public ScoreBoard scoreBoard;
    public Timer timer;
    public Powerup powerup;

    public int scoreValue = 100;
    public int bingoValue = 1000;
    public int powerupMissPenalty = 25;

    public float timeIntervals;
    float lastNumberPull;
    public int newPulledValue;

    public GameObject bingoTilePrefab;
    public GameObject bingoBoard;

    public GameObject bingoColNamePrefab;
    public GameObject bingoColNamesHeader;

    public GameObject lastDrawnBallHolder;
    public GameObject drawnBallPrefab;

    public GameObject bingoVisualCue;
    public GameObject bingoVisualCuesHolder;

    float gameLength;

    [SerializeField]
    public List<Ball> pulledBalls = new List<Ball>();
    [SerializeField]
    public List<int> clickedTiles = new List<int>();

    public int[,] boardLayout;
    public bool[] notEligableForBingo;

    public int rowValue;
    public int numberThreshold;

    public AudioManager audio;

    public bool isPlayer;

    [PunRPC]
    void initLocalGameManager(string targetPlayer, int newTime, int newScore, int newRowValue, int newNumberThreshold, int[] newBoardData)
    {
        if (PhotonNetwork.LocalPlayer.NickName != targetPlayer)
        {
            return;
        }
        print("Lets Start!");
        gameLength = 0;
        rowValue = newRowValue;
        numberThreshold = newNumberThreshold;
        timer.SetTime(newTime);
        scoreBoard.SetPoints(newScore);
        boardLayout = new int[rowValue, rowValue];
        notEligableForBingo = new bool[(rowValue + 1) * 2];
        audio.GetComponent<AudioManager>();
        lastNumberPull = 0;
        GenerateBoardFromGivenData(rowValue, rowValue, newBoardData);
        audio.audioPlayer.PlayOneShot(audio.casualMusic, 0.25f);
    }

    void Update()
    {
        gameLength += Time.deltaTime;
        if (isPlayer)
        {
            if (timer.isPlaying)
            {
                //gameLength += Time.deltaTime;
            }
        }
    }

    public void GenerateBoardFromGivenData(int rowValue,int colValue, int[] boardTiles)
    {
        for (int i = 0; i < colValue; i++)
        {
            GameObject newColNameTile = Instantiate(bingoColNamePrefab);
            newColNameTile.GetComponent<BingoTile>().UpdateLocalName(BINGO[i].ToString());
            newColNameTile.GetComponent<Image>().color = COLORS[i];
            newColNameTile.transform.SetParent(bingoColNamesHeader.transform, false);

            for (int j = 0; j < rowValue; j++)
            {
                GameObject newTile = Instantiate(bingoTilePrefab);

                newTile.GetComponent<BingoTile>().UpdateLocalValue(boardTiles[j + i * colValue]);
                newTile.transform.SetParent(bingoBoard.transform, false);
                boardLayout[j, i] = boardTiles[j + i * colValue];
            }
        }
    }

    [PunRPC]
    public void RecievePulledNumber(int newNumber)
    {
        if (!timer.isPlaying)
        {
            return;
        }

        newPulledValue = newNumber;
        MovePreviouslyDrawnBallsASide();

        GameObject lastDrawnBall = Instantiate(drawnBallPrefab);
        lastDrawnBall.GetComponent<BingoTile>().UpdateLocalValue(newPulledValue);
        lastDrawnBall.transform.SetParent(lastDrawnBallHolder.transform, false);

        int colorIndex = (newPulledValue - 1) / (rowValue * numberThreshold);
        lastDrawnBall.GetComponent<Image>().color = new Color(COLORS[colorIndex].r, COLORS[colorIndex].g, COLORS[colorIndex].b, 0.25f);

        pulledBalls.Add(new Ball(newPulledValue, gameLength));
        Debug.Log(newPulledValue);
    }

    public void MovePreviouslyDrawnBallsASide()
    {
        if(lastDrawnBallHolder.transform.childCount>= SHOWN_DRAWN_BALLS)
        {
            Destroy(lastDrawnBallHolder.transform.GetChild(0).gameObject);
        }

        foreach(Transform child in lastDrawnBallHolder.transform)
        {
            child.transform.position -= new Vector3(DRAWN_BALLS_MOVEMENT_DISTANCE, 0, 0);
        }
    }

    public void Click(int ballNumber)
    {
        if (timer.isPlaying)
        {
            int addedScore = CalculateNewAddedPoints(EventSystem.current.currentSelectedGameObject.GetComponentInParent<BingoTile>().localNumber);
            Debug.Log("addedScore: " + addedScore);
            if (addedScore > 0)
            {
                scoreBoard.AddPoints(addedScore);
            }
        }
    }

    private GameObject AddBingoVisualCue()
    {
        GameObject newBingoVisualCue = Instantiate(bingoVisualCue);
        newBingoVisualCue.transform.SetParent(bingoVisualCuesHolder.transform, false);
        return newBingoVisualCue;
    }

    public void CheckForBingo()
    {
        int curBingoCheck = 0;
        if (!timer.isPlaying)
        {
            return;
        }

        int curAddedNumberOfBingos = 0;

        int curCheckValue = 0;

        for(int i=0;i< rowValue; i++)
        {
            curCheckValue = 0;
            for (int j=0;j< rowValue; j++)
            {
                if (clickedTiles.Contains(boardLayout[j, i]))
                {
                    curCheckValue++;
                }
            }
            if (curCheckValue== rowValue && !notEligableForBingo[curBingoCheck])
            {
                curAddedNumberOfBingos++;
                notEligableForBingo[curBingoCheck] = true;
                AddBingoVisualCue().transform.position -= new Vector3(0, 105*i, 0);
            }
            curBingoCheck++;
        }

        for (int i = 0; i < rowValue; i++)
        {
            curCheckValue = 0;
            for (int j = 0; j < rowValue; j++)
            {
                if (clickedTiles.Contains(boardLayout[i, j]))
                {
                    curCheckValue++;
                }
            }
            if (curCheckValue == rowValue && !notEligableForBingo[curBingoCheck])
            {
                curAddedNumberOfBingos++;
                notEligableForBingo[curBingoCheck] = true;
                GameObject visualCue = AddBingoVisualCue();
                visualCue.transform.Rotate(new Vector3(0,0,90));
                visualCue.transform.position -= new Vector3(212.5f - 105*i, 200, 0);
            }
            curBingoCheck++;
        }

        curCheckValue = 0;
        for (int i = 0; i < rowValue; i++)
        {
            if (clickedTiles.Contains(boardLayout[i, i]))
            {
                curCheckValue++;
            }
        }
        if (curCheckValue == rowValue && !notEligableForBingo[curBingoCheck])
        {
            curAddedNumberOfBingos++;
            notEligableForBingo[curBingoCheck] = true;
            GameObject visualCue = AddBingoVisualCue();
            visualCue.transform.Rotate(new Vector3(0, 0, 135));
            visualCue.transform.position -= new Vector3(0, 215, 0);
            visualCue.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 75);
        }
        curBingoCheck++;

        curCheckValue = 0;
        for (int i = 0; i < rowValue; i++)
        {
            if (clickedTiles.Contains(boardLayout[rowValue - i - 1, i]))
            {
                curCheckValue++;
            }
        }
        if (curCheckValue == rowValue && !notEligableForBingo[curBingoCheck])
        {
            curAddedNumberOfBingos++;
            notEligableForBingo[curBingoCheck] = true;
            GameObject visualCue = AddBingoVisualCue();
            visualCue.transform.Rotate(new Vector3(0, 0, 45));
            visualCue.transform.position -= new Vector3(0, 215, 0);
            visualCue.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 75);
        }

        if (curAddedNumberOfBingos > 0)
        {
            scoreBoard.AddPoints((int)Mathf.Pow(2, curAddedNumberOfBingos + 1) * bingoValue);
            scoreBoard.AddBingos(curAddedNumberOfBingos);
            audio.audioPlayer.PlayOneShot(audio.bingo, 0.25f);
        }
        Debug.Log("Found " + curAddedNumberOfBingos + " new bingos!");
    }

    public int CalculateNewAddedPoints(int pulledBallValue)
    {
        foreach (Ball ball in pulledBalls)
        {
            if(ball.ballValue == pulledBallValue)
            {
                Debug.Log("Found Ball!");
                if (!clickedTiles.Contains(pulledBallValue))
                {
                    clickedTiles.Add(pulledBallValue);
                    audio.audioPlayer.PlayOneShot(audio.hit,0.5f);
                }

                pulledBalls.Remove(ball);
                EventSystem.current.currentSelectedGameObject.GetComponentInParent<BingoTile>().MarkAsPressedCorrectly();
                Debug.Log(gameLength + "," + ball.timePulled);
                powerup.FillMeter(GetMeterFillBasedOnTime((gameLength - ball.timePulled) * 100));
                return GetPointsMultiplierBasedOnTime((gameLength - ball.timePulled) * 100) * scoreValue;
            }
        }
        EventSystem.current.currentSelectedGameObject.GetComponentInParent<BingoTile>().MarkAsPressedIncorrectly();
        powerup.FillMeter(-powerupMissPenalty);
        audio.audioPlayer.PlayOneShot(audio.miss, 0.5f);
        Debug.Log("Ball not Found!");
        return 0;
    }

    public int GetMeterFillBasedOnTime(float time)
    {
        //Debug.Log(time);
        if (time >= 0 && time < (int)delayValues.VeryFast)
        {
            return (int)meterScores.VeryFast;
        }
        else if (time >= (int)delayValues.VeryFast && time < (int)delayValues.Fast)
        {
            return (int)meterScores.Fast;
        }
        else if (time >= (int)delayValues.Fast && time < (int)delayValues.Ok)
        {
            return (int)meterScores.Ok;
        }
        else if (time >= (int)delayValues.Ok && time < (int)delayValues.Slow)
        {
            return (int)meterScores.Slow;
        }
        else
        {
            return 0;
        }
    }

    public int GetPointsMultiplierBasedOnTime(float time)
    {
        //Debug.Log(time);
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
