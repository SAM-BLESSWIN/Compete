using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomUI : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject JoinRoomUI;

    [Header("Data UI References")]
    [SerializeField] private Image firstPlayerAvatar;
    [SerializeField] private Image secondPlayerAvatar;

    [SerializeField] private TMP_Text firstPlayerName;
    [SerializeField] private TMP_Text secondPlayerName;

    [SerializeField] private TMP_Text firstPlayerTotalWins;
    [SerializeField] private TMP_Text secondPlayerTotalWins;

    [Header("Button UI References")]
    [SerializeField] private GameObject joinBtn;
    [SerializeField] private GameObject startGameBtn;

    [Header("Character Sprite Data")]
    [SerializeField] private Characters character;


    #region UI Callback Methods
    public void TryJoiningRoom()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        RoomManager.instance.JoinRoom();
    }

    public void StartGame()
    {
        GameManager.instance.PhotonLoadScene(2);
    }

    #endregion


    #region Pun Callbacks
    public override void OnJoinedLobby()
    {
        menuUI.gameObject.SetActive(false);
        JoinRoomUI.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.LogFormat("The local player: {0} joined to current room {1}", PhotonNetwork.NickName, PhotonNetwork.CurrentRoom.Name);
        Debug.LogFormat("Players in room : {0} / {1}", PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);

        firstPlayerAvatar.sprite =
            character.characterSprites[FirebaseDatabaseManager.instance.localPlayerData.playerCharacterId];
        firstPlayerName.text = FirebaseDatabaseManager.instance.localPlayerData.playerDisplayerName;
        firstPlayerTotalWins.text = "Total Wins : "+FirebaseDatabaseManager.instance.localPlayerData.playerTotalWins.ToString();

        joinBtn.GetComponent<Button>().interactable = false;
        joinBtn.GetComponentInChildren<TMP_Text>().text = "waiting...";

        if (PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            Player hostPlayer = PhotonNetwork.PlayerList[0];
            StartCoroutine(FirebaseDatabaseManager.instance.LoadRemotePlayerData(
                hostPlayer.NickName,() => SetRemotePlayerData()));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("The remote player: {0} joined to current room {1}", newPlayer.NickName, PhotonNetwork.CurrentRoom.Name);
        Debug.LogFormat("Players in room : {0} / {1}", PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);

        StartCoroutine(FirebaseDatabaseManager.instance.LoadRemotePlayerData(newPlayer.NickName,
            ()=> SetRemotePlayerData()));
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        secondPlayerAvatar.sprite = null;
        secondPlayerName.text = "-";
        secondPlayerTotalWins.text = "-";

        joinBtn.gameObject.SetActive(true);
        startGameBtn.gameObject.SetActive(false);

        FirebaseDatabaseManager.instance.remotePlayerData = default;
    }
    #endregion

    private void SetRemotePlayerData()
    {
        secondPlayerAvatar.sprite = character.characterSprites[FirebaseDatabaseManager.instance.remotePlayerData.playerCharacterId];
        secondPlayerName.text = FirebaseDatabaseManager.instance.remotePlayerData.playerDisplayerName;
        secondPlayerTotalWins.text = "Total Wins : " + FirebaseDatabaseManager.instance.remotePlayerData.playerTotalWins.ToString();

        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            joinBtn.gameObject.SetActive(false);
            startGameBtn.gameObject.SetActive(true);
        }
    }
}
