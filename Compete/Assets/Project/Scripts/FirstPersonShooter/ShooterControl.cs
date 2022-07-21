using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Photon.Pun;
using UnityEngine.UI;

public class ShooterControl : MonoBehaviourPun, IDamageable
{
    [SerializeField] private BaseGun gun;
    [SerializeField] private Image[] healthBar;

    private StarterAssetsInputs inputs;
    private MultiplayerManager multiplayerManager;

    private const float MAX_HEALTH = 100f;
    private float currentHealth;

    private const float FALL_LIMIT = -10f;

    private int currentLife;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            inputs = GetComponent<StarterAssetsInputs>();
            multiplayerManager = PhotonView.Find((int)photonView.InstantiationData[0]).GetComponent<MultiplayerManager>();
            currentLife = (int)photonView.InstantiationData[1];
            
            for(int i=0;i<currentLife;i++)
            {
                healthBar[i].fillAmount = 1;
            }
        }
    }

    private void Start()
    {
        currentHealth = MAX_HEALTH;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (inputs.fire)
        {
            gun.Activate();
            inputs.fire = false;
        }

        if (transform.position.y < FALL_LIMIT)
        {
            RemoveLife();
        }
    }

    public void TakeDamage(float damage)
    {
        photonView.RPC(nameof(RPC_TakeDamage), photonView.Owner, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar[currentLife-1].fillAmount = currentHealth / MAX_HEALTH;

        if (currentHealth <= 0)
        {
            RemoveLife();
        }
    }

    private void RemoveLife()
    {
        currentLife--;
        CheckLives();
    }

    private void CheckLives()
    {
        if (currentLife == 0)
        {
            Dead();
        }
        else
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        multiplayerManager.PlayerRespawn(currentLife);
    }

    private void Dead()
    {
        multiplayerManager.PlayerDead();
    }
}
 