using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class AvatarManager : MonoBehaviour
{
    public Text playerName;
    public List<GameObject> destroyIfNotLocal;
    public List<GameObject> destroyIfLocal;
    PhotonView _pv;

    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        if (_pv.IsMine)
        { 
            _pv.RPC("RPC_AddNameToCharacter", RpcTarget.AllBuffered, PlayerInfo.Instance.playerName);
            gameObject.layer = 9;
            destroyIfLocal.ForEach(x => Destroy(x));
        }
        else
        {
            destroyIfNotLocal.ForEach(x => Destroy(x));
        }
    }

    [PunRPC]
    void RPC_AddNameToCharacter(string name)
    {
        playerName.text = name;
    }
}
