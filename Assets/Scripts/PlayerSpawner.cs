using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public LocalGameManager localPlayer;
    public RemoteGameManager hostPrefab;

    private void Awake()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        if (PhotonNetwork.LocalPlayer.NickName != 1.ToString())
        {
            SpawnPlayer();
        }
        else
        {
            SpawnHost();
        }
    }

    public void SpawnPlayer()
    {
        LocalGameManager.Instance.isPlayer = true;
    }

    public void SpawnHost()
    {
        RemoteGameManager.Instance.isHost = true;
        RemoteGameManager.Instance.InitPlayers();
    }
}
