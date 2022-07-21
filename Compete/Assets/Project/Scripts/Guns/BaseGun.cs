using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    [SerializeField] protected Guns gun;

    public GameObject bulletImpactPrefab;

    public abstract void Activate();
}
