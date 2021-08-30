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

    [SerializeField]
    public bool[] playersEndGame;
    [SerializeField]
    public int[] playersScore;

    public void InitPlayers()
    {
        Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        Debug.Log("Number of Players: " + PhotonNetwork.PlayerList.Length);

        if (PhotonNetwork.PlayerList.Length<2)
        {
            return;
        }

        playersEndGame = new bool[PhotonNetwork.PlayerList.Length - 1];
        playersScore = new int[PhotonNetwork.PlayerList.Length - 1];

        for (int i=2;i<= PhotonNetwork.PlayerList.Length; i++)
        {
            LocalGameManager.Instance.photonView.RPC("initLocalGameManager", RpcTarget.Others, i.ToString(),startingTimeInSeconds, startingPoints, ROW_VALUE, NUMBER_THRESHOLD, CreateBoard());
        }
    }

    private void Update()
    {
        if (isHost)
        {
            gameLength += Time.deltaTime;
            if (lastNumberPull + timeIntervals <= gameLength)
            {
                LocalGameManager.Instance.photonView.RPC("RecievePulledNumber", RpcTarget.Others, PullNumber());
            }
        }
    }

    public int[] CreateBoard()
    {
        possibleBalls = new List<int>();
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

    [PunRPC]
    public void SendPlayerEndGameData(int player, int score)
    {
        playersEndGame[player - 2] = true;
        playersScore[player - 2] = score;

        if (AreAllPlayersDone())
        {
            OpenEndGameMenu();
        }
    }

    private bool AreAllPlayersDone()
    {
        for (int i = 0; i < playersEndGame.Length; i++)
        {
            if (!playersEndGame[i])
            {
                return false;
            }
        }
        return true;
    }

    public void OpenEndGameMenu()
    {
        print("The Game is Over!");
        for (int i = 0; i < playersScore.Length; i++)
        {
            print("Player: " + (i + 1) + ", Points: " + playersScore[i]);
        }

        //ContinueGame();
    }

    public void ContinueGame()
    {
        playersEndGame = new bool[PhotonNetwork.PlayerList.Length - 1];

        for (int i = 2; i <= PhotonNetwork.PlayerList.Length; i++)
        {
            LocalGameManager.Instance.photonView.RPC("initLocalGameManager", RpcTarget.Others, i.ToString(), startingTimeInSeconds, playersScore[i - 2], ROW_VALUE, NUMBER_THRESHOLD, CreateBoard());
        }
    }
}
