using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float bulletLifetime;
    public float bulletImpulse;

    PhotonView _pv;
    Rigidbody _rb;
    float counter;
    Player owner;


    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        _rb.AddForce(transform.forward * bulletImpulse, ForceMode.Impulse);
        if(_pv.IsMine) owner = PhotonNetwork.LocalPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_pv.IsMine) return;

        counter += Time.deltaTime;
        if(counter >= bulletLifetime) DestroySelf();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_pv.IsMine) DestroySelf();
    }

    private void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
