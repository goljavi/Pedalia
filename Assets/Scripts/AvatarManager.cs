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

    public Camera cam;
    public Camera gunCamera;
    public AudioListener audioListener;

    public GameObject graphics;
    public GameObject localCanvas;

    public GameObject gunContainer;
    PhotonView _pv;

    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        if (_pv.IsMine)
        { 
            _pv.RPC("RPC_AddNameToCharacter", RpcTarget.AllBuffered, PlayerInfo.Instance.playerName);

            gameObject.SetLayerRecursively(9);
            gunContainer.SetLayerRecursively(11);
            DisableLocalElements();
        }
        else
        {
            DisableOtherPlayerElements();
        }
    }

    [PunRPC]
    void RPC_AddNameToCharacter(string name)
    {
        playerName.text = name;
    }

    void DisableLocalElements()
    {
        graphics.SetActive(false);
        localCanvas.SetActive(false);
    }

    void DisableOtherPlayerElements()
    {
        cam.enabled = false;
        audioListener.enabled = false;
        gunCamera.enabled = false;
    } 
}
