using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    [SerializeField] private Guns gun;

    public abstract void Activate();
}
