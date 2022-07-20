using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class MultiplayerManager : MonoBehaviourPun
{
    [Header("Player Controls References")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private GunControl gunControl;

    [Header("Camera References")]
    [SerializeField] private GameObject playerCameraRoot;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject followCamera;

    [Header("Mobile UI Inputs")]
    [SerializeField]private GameObject mobileInputs;

    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(playerCameraRoot);
            Destroy(mainCamera);
            Destroy(followCamera);
            Destroy(mobileInputs);

            firstPersonController.enabled = false;
            gunControl.enabled = false;
        }
    }
}
