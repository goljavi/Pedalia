using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float bulletLifetime;
    public float bulletImpulse;
    [HideInInspector] public Hero owner;

    PhotonView _pv;
    
    Rigidbody _rb;
    float counter;


    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.forward = (Vector3)_pv.InstantiationData[0];
        _rb.AddForce(transform.forward * bulletImpulse, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_pv.IsMine) return;
        counter += Time.deltaTime;
        if(counter >= bulletLifetime) DestroySelf();
    }

    private void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var hero = other.gameObject.GetComponent<Hero>();
        if (hero && hero != owner)
        {
            hero.GetPushed(transform.forward);
            PhotonNetwork.Instantiate("BulletExplosion", other.GetContact(0).point, Quaternion.identity);
        }
    }
}
