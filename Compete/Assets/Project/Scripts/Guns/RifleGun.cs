using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RifleGun : BaseGun
{
    [SerializeField] private Camera playerCamera;


    PhotonView photonview;
    private void Start()
    {
        photonview = GetComponent<PhotonView>();
    }

    public override void Activate()
    {
        Shoot();
    }

    private void Shoot()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f));
        ray.origin = playerCamera.transform.position;

        photonview.RPC(nameof(RPC_GunFireAudio), RpcTarget.All);

        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            Debug.DrawLine(playerCamera.transform.position, hit.point, Color.red,3f);
            hit.collider.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(gun.damage);

            photonview.RPC(nameof(RPC_ShootImpact), RpcTarget.All, hit.point,hit.normal);
        }

    }

    [PunRPC]
    private void RPC_GunFireAudio()
    {
        AudioSource.PlayClipAtPoint(gun.fireSound,transform.position, 0.1f);
    }

    [PunRPC]
    private void RPC_ShootImpact(Vector3 hitPosition,Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpact = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal));
            Destroy(bulletImpact,5f);
            bulletImpact.transform.SetParent(colliders[0].transform);
        }

    }
}
