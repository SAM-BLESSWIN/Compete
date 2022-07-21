using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject instantiatedPlayer;

    private const int MAX_LIVES = 3;
    private int currentLife;

    private void Awake()
    {
        if(photonView.IsMine)
        {
            currentLife = MAX_LIVES;
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        instantiatedPlayer = PhotonNetwork.Instantiate(playerPrefab.name,
            spawnPoint.position, spawnPoint.rotation,0,
            new object[] {photonView.ViewID,currentLife});
    }

    public void PlayerRespawn(int currentLife)
    {
        this.currentLife = currentLife;
        PhotonNetwork.Destroy(instantiatedPlayer);
        photonView.RPC(nameof(SpawnMessage), RpcTarget.All);
        CreatePlayer();
    }

    public void PlayerDead()
    {
        PhotonNetwork.Destroy(instantiatedPlayer);
        Invoke(nameof(WaitOnDestroy), 2);
    }

    private void WaitOnDestroy()
    {
        photonView.RPC(nameof(GameOver), RpcTarget.All, PhotonNetwork.NickName);
    }

    [PunRPC]
    private void SpawnMessage()
    {
        GameWorldManager.instance.ActivateSpawnMessage();
    }

    [PunRPC]
    private void GameOver(string deadPlayerName)
    {
        GameWorldManager.instance.GameOver(deadPlayerName);
    }

}
