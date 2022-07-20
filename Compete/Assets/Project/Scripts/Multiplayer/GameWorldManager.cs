using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameWorldManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;   
        PhotonNetwork.Instantiate(playerPrefab.name,transform.position, Quaternion.identity);
    }

    #region Photon Callbacks
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }
    #endregion
}
