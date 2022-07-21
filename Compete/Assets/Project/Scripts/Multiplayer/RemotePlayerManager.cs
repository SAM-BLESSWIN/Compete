using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;
using UnityEngine.InputSystem;

public class RemotePlayerManager : MonoBehaviourPun
{
    [Header("Player Controls References")]
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    [Header("Camera References")]
    [SerializeField] private GameObject playerCameraRoot;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject followCamera;

    [Header("UI Canvas")]
    [SerializeField]private GameObject mobileInputsCanvas;
    [SerializeField] private GameObject playerCanvas;


    [Header("Guns")]
    [SerializeField] private GameObject riflegun;

    void Start()
    {
        if(photonView.IsMine)
        {
            riflegun.transform.parent = mainCamera.transform;
            Destroy(this);
        }
        else
        {
            Destroy(playerCameraRoot);
            Destroy(mainCamera);
            Destroy(followCamera);
            Destroy(mobileInputsCanvas);
            Destroy(playerCanvas);
            Destroy(starterAssetsInputs);
        }
    }
}
