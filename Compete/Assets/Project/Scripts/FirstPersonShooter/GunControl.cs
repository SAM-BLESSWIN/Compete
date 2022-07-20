using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class GunControl : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs inputs;

    void Update()
    {
        if(inputs.fire)
        {
            Debug.Log("Fired");
            inputs.fire = false;
        }
    }
}
