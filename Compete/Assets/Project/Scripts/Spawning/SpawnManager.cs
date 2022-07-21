using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    private Transform[] spawnTransform;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        spawnTransform = GetComponentsInChildren<Transform>();
    }

    public Transform GetSpawnPoint()
    {
        return spawnTransform[Random.Range(0,spawnTransform.Length)];
    }
}
