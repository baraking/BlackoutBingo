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
    public int bingoValue = 1000;

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
    public List<int> possibleBalls = new List<int>();
    [SerializeField]
    public List<int> clickedTiles = new List<int>();

    [SerializeField]
    public List<int>[] localBoardOptions = new List<int>[ROW_VALUE];

    public int[,] boardLayout = new int[ROW_VALUE, ROW_VALUE];

    public bool[] notEligableForBingo = new bool[(ROW_VALUE + 1) * 2];

    public AudioManager audio;

    void Start()
    {
        audio.GetComponent<AudioManager>();
        lastNumberPull = 0;
        GenerateBoardFromGivenData(ROW_VALUE, ROW_VALUE, CreateBoard());
        audio.audioPlayer.PlayOneShot(audio.casualMusic, 0.25f);
    }

    void Update()
    {
        if (timer.isPlaying)
        {
            gameLength += Time.deltaTime;
            if (lastNumberPull + timeIntervals <= gameLength)
            {
                RecievePulledNumber(PullNumber());
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

    public int[] CreateBoard()
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
        int[] boardDataToSend = new int[ROW_VALUE * ROW_VALUE];

        for (int i = 0; i < ROW_VALUE; i++)
        {
            GameObject newColNameTile = Instantiate(bingoColNamePrefab);
            newColNameTile.GetComponent<BingoTile>().UpdateLocalName(BINGO[i].ToString());
            newColNameTile.GetComponent<Image>().color = COLORS[i];
            newColNameTile.transform.SetParent(bingoColNamesHeader.transform, false);

            for (int j = 0; j < ROW_VALUE; j++)
            {
                int index = random.Next(localBoardOptions[j].Count);
                boardDataToSend[j+i*ROW_VALUE] = localBoardOptions[j][index];
                localBoardOptions[j].RemoveAt(index);
            }
        }
        return boardDataToSend;
    }

    public int PullNumber()
    {
        var random = new System.Random();
        int index = random.Next(possibleBalls.Count);
        newPulledValue = possibleBalls[index];

        possibleBalls.RemoveAt(index);

        if (possibleBalls.Count < 1)
        {
            timer.isPlaying = false;
            //should notify all players to stop play
        }

        lastNumberPull = gameLength;
        return newPulledValue;
    }

    public void RecievePulledNumber(int newNumber)
    {
        MovePreviouslyDrawnBallsASide();

        GameObject lastDrawnBall = Instantiate(drawnBallPrefab);
        lastDrawnBall.GetComponent<BingoTile>().UpdateLocalValue(newPulledValue);
        lastDrawnBall.transform.SetParent(lastDrawnBallHolder.transform, false);

        int colorIndex = (newPulledValue - 1) / (ROW_VALUE * NUMBER_THRESHOLD);
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

        for(int i=0;i< ROW_VALUE; i++)
        {
            curCheckValue = 0;
            for (int j=0;j< ROW_VALUE; j++)
            {
                if (clickedTiles.Contains(boardLayout[j, i]))
                {
                    curCheckValue++;
                }
            }
            if (curCheckValue== ROW_VALUE && !notEligableForBingo[curBingoCheck])
            {
                curAddedNumberOfBingos++;
                notEligableForBingo[curBingoCheck] = true;
                AddBingoVisualCue().transform.position -= new Vector3(0, 105*i, 0);
            }
            curBingoCheck++;
        }

        for (int i = 0; i < ROW_VALUE; i++)
        {
            curCheckValue = 0;
            for (int j = 0; j < ROW_VALUE; j++)
            {
                if (clickedTiles.Contains(boardLayout[i, j]))
                {
                    curCheckValue++;
                }
            }
            if (curCheckValue == ROW_VALUE && !notEligableForBingo[curBingoCheck])
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
        for (int i = 0; i < ROW_VALUE; i++)
        {
            if (clickedTiles.Contains(boardLayout[i, i]))
            {
                curCheckValue++;
            }
        }
        if (curCheckValue == ROW_VALUE && !notEligableForBingo[curBingoCheck])
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
        for (int i = 0; i < ROW_VALUE; i++)
        {
            if (clickedTiles.Contains(boardLayout[ROW_VALUE - i - 1, i]))
            {
                curCheckValue++;
            }
        }
        if (curCheckValue == ROW_VALUE && !notEligableForBingo[curBingoCheck])
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
                return GetPointsMultiplierBasedOnTime((gameLength - ball.timePulled) * 100) * scoreValue;
            }
        }
        EventSystem.current.currentSelectedGameObject.GetComponentInParent<BingoTile>().MarkAsPressedIncorrectly();
        audio.audioPlayer.PlayOneShot(audio.miss, 0.5f);
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
