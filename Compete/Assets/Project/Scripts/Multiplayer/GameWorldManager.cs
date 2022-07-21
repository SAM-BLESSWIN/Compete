using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;

public class GameWorldManager : MonoBehaviourPunCallbacks
{
    public static GameWorldManager instance;
    [SerializeField] private GameObject photonPrefab;

    [Header("GameOver UI References")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject spawnMessageUI;

    private bool gameOver = false;

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
        if (!PhotonNetwork.IsConnectedAndReady) return;   
        PhotonNetwork.Instantiate(photonPrefab.name,Vector3.zero, Quaternion.identity);
    }

    public void ActivateSpawnMessage()
    {
        spawnMessageUI.SetActive(true);
        Invoke(nameof(TurnOffUI), 2f);
    }

    private void TurnOffUI()
    {
        spawnMessageUI.SetActive(false);
    }

    public void GameOver(string deadPlayerName)
    {
        if (PhotonNetwork.NickName == deadPlayerName)
        {
            messageText.text = FirebaseDatabaseManager.instance.localPlayerData.playerDisplayerName + " lost";
        }
        else
        {
            messageText.text = FirebaseDatabaseManager.instance.localPlayerData.playerDisplayerName + " won";
            int totalwins = FirebaseDatabaseManager.instance.localPlayerData.playerTotalWins + 1;
            FirebaseDatabaseManager.instance.UpdateTotalWins(totalwins);
        }

        gameOver = true;
        gameOverUI.SetActive(true);
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }


    #region Photon Callbacks
    public override void OnLeftRoom()
    {
        FirebaseDatabaseManager.instance.remotePlayerData = default;
        GameManager.instance.LoadScene(1);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        FirebaseDatabaseManager.instance.remotePlayerData = default;

        if (gameOver) return;

        GameOver(otherPlayer.NickName);
    }
    #endregion
}
