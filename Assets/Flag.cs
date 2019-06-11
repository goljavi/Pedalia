using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    void OnTriggerEnter(Collider info)
    {
        var hero = info.gameObject.GetComponent<Hero>();
        if (hero)
        {
            hero.GetFlag();
            PhotonNetwork.Instantiate("AddFlag", transform.position, Quaternion.identity);
            GetComponent<PhotonView>().RPC("RPC_Destroy", RpcTarget.MasterClient);
        }
       
    }

    [PunRPC]
    void RPC_Destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    
}
