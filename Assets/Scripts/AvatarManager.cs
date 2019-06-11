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
    public GameObject particles;
    PhotonView _pv;

    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        if (_pv.IsMine)
        {
            bool nameFound = true;
            var playerName = PlayerInfo.Instance.playerName;

            while (nameFound)
            {
                nameFound = false;
                foreach (var hero in FindObjectsOfType<Hero>())
                {
                    if(hero.transform.name == playerName)
                    {
                        nameFound = true;
                        playerName += " Copy";
                        break;
                    }
                }
            }

            PlayerInfo.Instance.playerName = playerName;
            _pv.RPC("RPC_AddNameToCharacter", RpcTarget.AllBuffered, playerName);

            gameObject.SetLayerRecursively(9);
            gunContainer.SetLayerRecursively(11);
            particles.SetLayerRecursively(0);
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
        transform.name = name;
        playerName.text = name;
    }

    void DisableLocalElements()
    {
        localCanvas.SetActive(false);
    }

    void DisableOtherPlayerElements()
    {
        cam.enabled = false;
        audioListener.enabled = false;
        gunCamera.enabled = false;
    } 
}
