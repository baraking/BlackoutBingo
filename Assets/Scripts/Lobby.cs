using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{

    public static readonly string DEFAULT_ROOM_NAME = "The only room";

    public bool hasCreatedRoom;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");

        Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
    }

    private void Update()
    {
        Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        if (PhotonNetwork.CountOfPlayers == 1 && !hasCreatedRoom && Time.time > 5)
        {
            CreateGame();
        }
        Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
        //print("Are all ready? " + CheckPlayersReady());

        //print(Time.time);
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(DEFAULT_ROOM_NAME, new RoomOptions() { MaxPlayers = 4 }, TypedLobby.Default);
        hasCreatedRoom = true;
        Debug.Log("Create the room");
    }

    public void ClickedOnStartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinRoom(DEFAULT_ROOM_NAME, null);
    }
}
