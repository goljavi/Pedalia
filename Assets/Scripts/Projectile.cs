using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float bulletLifetime;
    public float bulletImpulse;

    public PhotonView pv;
    Rigidbody _rb;
    float counter;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = (Vector3)pv.InstantiationData[1];
        transform.forward = (Vector3)pv.InstantiationData[0];
        _rb.AddForce(transform.forward * bulletImpulse, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

        if (!pv.IsMine) return;

        counter += Time.deltaTime;
        if(counter >= bulletLifetime) DestroySelf();
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("BulletRicochet", RpcTarget.All, collision.GetContact(0).normal, transform.position);
        }*/ 
    }

    [PunRPC]
    void BulletRicochet(Vector3 collisionNormal, Vector3 position)
    {
        transform.position = position;
        transform.forward = collisionNormal;
        _rb.AddForce(transform.forward * bulletImpulse / 2, ForceMode.Impulse);
    }

    private void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
