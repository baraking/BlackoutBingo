using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Awake()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        if (PhotonNetwork.LocalPlayer.NickName != 1.ToString())
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        //PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,0,0), Quaternion.identity, 0);
        Instantiate(playerPrefab);
    }
}
