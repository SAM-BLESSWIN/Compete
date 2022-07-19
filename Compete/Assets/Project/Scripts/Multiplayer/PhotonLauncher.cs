using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    private const string GAME_VERSION = "0.0.1";
    private void Start()
    {
        Debug.Log("connecting to server...");
        PhotonNetwork.NickName = FirebaseDatabaseManager.instance.localPlayerData.playerDisplayerName;
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callback Methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server!!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from server for reason : "+cause);
    }
    #endregion
}
