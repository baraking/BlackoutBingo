using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public enum delayValues { VeryFast = 50, Fast = 100, Ok = 200, Slow = 300 };
    public enum delayScores { VeryFast = 5, Fast = 3, Ok = 2, Slow = 1 };

    public static readonly int ROW_VALUE = 5;
    public static readonly int NUMBER_THRESHOLD = 3;

    public static readonly string BINGO = "BINGO";
    public static readonly Color[] COLORS = { Color.blue, Color.red, Color.magenta, Color.green, Color.yellow };

    public static readonly int SHOWN_DRAWN_BALLS = 7;
    public static readonly int DRAWN_BALLS_MOVEMENT_DISTANCE = 150;

    public ScoreBoard scoreBoard;
    public Timer timer;

    public int scoreValue = 100;

    public float timeIntervals;
    float lastNumberPull;
    public int newPulledValue;

    public GameObject bingoTilePrefab;
    public GameObject bingoBoard;

    public GameObject bingoColNamePrefab;
    public GameObject bingoColNamesHeader;

    public GameObject lastDrawnBallHolder;
    public GameObject drawnBallPrefab;

    float gameLength;

    [SerializeField]
    public List<Ball> pulledBalls = new List<Ball>();
    [SerializeField]
    public List<int> possibleBalls = new List<int>();
    [SerializeField]
    public List<int> clickedTiles = new List<int>();

    [SerializeField]
    public List<int>[] localBoardOptions = new List<int>[ROW_VALUE];

    void Start()
    {
        lastNumberPull = 0;
        CreateBoard();
    }

    void Update()
    {
        if (timer.isPlaying)
        {
            gameLength += Time.deltaTime;
            if (lastNumberPull + timeIntervals <= gameLength)
            {
                PullNumber();
            }
        }
    }

    public void CreateBoard()
    {
        for(int i=0;i< localBoardOptions.GetLength(0); i++)
        {
            localBoardOptions[i] = new List<int>();
            for (int j = 1; j <= ROW_VALUE * NUMBER_THRESHOLD; j++)
            {
                localBoardOptions[i].Add(j + i * ROW_VALUE * NUMBER_THRESHOLD);
                //print(j + i * ROW_VALUE * NUMBER_THRESHOLD);
                possibleBalls.Add(j + i * ROW_VALUE * NUMBER_THRESHOLD);
            }
        }

        var random = new System.Random();

        for (int i = 0; i < ROW_VALUE; i++)
        {
            GameObject newColNameTile = Instantiate(bingoColNamePrefab);
            newColNameTile.GetComponent<BingoTile>().UpdateLocalName(BINGO[i].ToString());
            newColNameTile.GetComponent<Image>().color = COLORS[i];
            newColNameTile.transform.SetParent(bingoColNamesHeader.transform, false);

            for (int j = 0; j < ROW_VALUE; j++)
            {
                GameObject newTile = Instantiate(bingoTilePrefab);

                int index = random.Next(localBoardOptions[j].Count);

                newTile.GetComponent<BingoTile>().UpdateLocalValue(localBoardOptions[j][index]);
                newTile.transform.SetParent(bingoBoard.transform, false);

                localBoardOptions[j].RemoveAt(index);
            }
        }
    }

    public int PullNumber()
    {
        MovePreviouslyDrawnBallsASide();

        var random = new System.Random();
        int index = random.Next(possibleBalls.Count);
        newPulledValue = possibleBalls[index];

        GameObject lastDrawnBall = Instantiate(drawnBallPrefab);
        lastDrawnBall.GetComponent<BingoTile>().UpdateLocalValue(newPulledValue);
        lastDrawnBall.transform.SetParent(lastDrawnBallHolder.transform, false);

        pulledBalls.Add(new Ball(newPulledValue, gameLength));
        Debug.Log(newPulledValue);
        possibleBalls.RemoveAt(index);
        clickedTiles.Add(newPulledValue);

        lastNumberPull = gameLength;
        return newPulledValue;
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
            int addedScore = CalculateNewAddedPoints(ballNumber);
            Debug.Log("addedScore: " + addedScore);
            if (addedScore > 0)
            {
                scoreBoard.AddPoints(addedScore);
            }
        }
    }

    public int CalculateNewAddedPoints(int pulledBallValue)
    {
        foreach(Ball ball in pulledBalls)
        {
            if(ball.ballValue == pulledBallValue)
            {
                Debug.Log("Found Ball!");
                pulledBalls.Remove(ball);
                EventSystem.current.currentSelectedGameObject.GetComponentInParent<BingoTile>().MarkAsPressedCorrectly();
                return GetPointsMultiplierBasedOnTime((gameLength - ball.timePulled) * 100) * scoreValue;
            }
        }
        EventSystem.current.currentSelectedGameObject.GetComponentInParent<BingoTile>().MarkAsPressedIncorrectly();
        Debug.Log("Ball not Found!");
        return 0;
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
