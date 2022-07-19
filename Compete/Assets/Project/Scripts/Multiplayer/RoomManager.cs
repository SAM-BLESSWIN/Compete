using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;

public class MapData
{
    public const string MAPKEY = "map";
    public const string MAPTYPE = "onevsone";
}

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    private string mapType;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;    
    }

    public void JoinRoom()
    {
        mapType = MapData.MAPTYPE;
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {MapData.MAPKEY, mapType}
        };
        PhotonNetwork.JoinRandomRoom(customRoomProperties,0);
    }

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" +mapType+ UnityEngine.Random.Range(0, 99999);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        string[] roomPropertiesInLobby = { "map" };
        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {MapData.MAPKEY,mapType},
        };
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions,TypedLobby.Default);
    }

    #region Photon Callback Methods

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the default lobby");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created Successfully!!! \n Room Name : "+ PhotonNetwork.CurrentRoom.Name);
    }

    #endregion
}
