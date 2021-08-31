using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class Lobby : MonoBehaviourPunCallbacks
{

    public static readonly string DEFAULT_ROOM_NAME = "The only room";
    public static readonly int TIMEOUT_FOR_CONNECTION = 5;

    public bool hasCreatedRoom;

    public TMP_Text playerName;

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
        //Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        if (!hasCreatedRoom && Time.time > TIMEOUT_FOR_CONNECTION)
        {
            PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.CountOfPlayers.ToString();
            if (PhotonNetwork.CountOfPlayers == 1)
            {
                CreateGame();
                playerName.text = "Host is Ready";
            }
            else
            {
                JoinGame();
                playerName.text = "Player " + (PhotonNetwork.CountOfPlayers - 1).ToString();
            }
            
            //Debug.Log(PhotonNetwork.LocalPlayer.NickName);
            
        }
        //Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
        //print("Are all ready? " + CheckPlayersReady());

        //print(Time.time);
        //Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(DEFAULT_ROOM_NAME, new RoomOptions() { MaxPlayers = 4, IsOpen = true }, TypedLobby.Default);
        hasCreatedRoom = true;
        Debug.Log("Create the room");
    }

    public void ClickedOnStartGame()
    {
        /*if (PhotonNetwork.LocalPlayer.NickName == 1.ToString())
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("GameScene");
        }*/

        if(PhotonNetwork.IsMasterClient)
{
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public void JoinGame()
    {
        hasCreatedRoom = true;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinRoom(DEFAULT_ROOM_NAME, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

    }

    public override void OnLeftLobby()
    {

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }
}
