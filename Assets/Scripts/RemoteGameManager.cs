using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RemoteGameManager : MonoBehaviourPunCallbacks
{
    private static RemoteGameManager _instance;

    public static RemoteGameManager Instance { get { return _instance; } }

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

    public PlayerSpawner playerSpawner;

    public static readonly int ROW_VALUE = 5;
    public static readonly int NUMBER_THRESHOLD = 3;

    public int startingTimeInSeconds = 90;
    public int startingPoints = 0;

    float gameLength;
    float lastNumberPull;
    public int newPulledValue;
    public float timeIntervals;

    float timer;

    public bool isHost;

    [SerializeField]
    public List<int>[] localBoardOptions = new List<int>[ROW_VALUE];
    [SerializeField]
    public List<int> possibleBalls = new List<int>();

    public void InitPlayers()
    {
        print("Init Player!");
        photonView.RPC("GetMessage", RpcTarget.Others);
        LocalGameManager.Instance.photonView.RPC("initLocalGameManager", RpcTarget.Others, startingTimeInSeconds, startingPoints, ROW_VALUE, NUMBER_THRESHOLD, CreateBoard());
    }

    [PunRPC]
    public void GetMessage()
    {
        print("I got the message!");
    }

    private void Update()
    {
        if (isHost)
        {
            gameLength += Time.deltaTime;
            if (lastNumberPull + timeIntervals <= gameLength)
            {
                //send pullNumber to each player
                //RecievePulledNumber(PullNumber());
            }
        }
    }

    public int[] CreateBoard()
    {
        for (int i = 0; i < localBoardOptions.GetLength(0); i++)
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
            for (int j = 0; j < ROW_VALUE; j++)
            {
                int index = random.Next(localBoardOptions[j].Count);
                boardDataToSend[j + i * ROW_VALUE] = localBoardOptions[j][index];
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
            //should notify all players to stop play
        }

        lastNumberPull = gameLength;
        return newPulledValue;
    }
}
