using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNetworkAsset : MonoBehaviour
{
    PhotonView _pv;
    public float seconds = 1;
    float counter;

    private void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        counter += Time.deltaTime;
        if(_pv.IsMine && counter > seconds) PhotonNetwork.Destroy(gameObject);
    }

}
