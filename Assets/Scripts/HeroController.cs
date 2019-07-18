using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public Hero hero;
    
    PhotonView _pv;
    HeroKeyboardController _hkc;
    GameObject _sceneCamera;
    AudioListener _sceneAudioListener;

    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        if (!_pv.IsMine) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(RequestPlayerSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        if (_pv.IsMine && _hkc != null)
        {
            _hkc.Update(PlayerUI.Instance.pauseMenuOpen);
        }
    }

    public void AddFlag()
    {
       // _flagCount++;
    }

    IEnumerator RequestPlayerSpawn()
    {
        bool finished = false;
        while (!finished)
        {
            if (HostServer.Instance)
            {
                _hkc = new HeroKeyboardController();
                HostServer.Instance.PlayerRequestSpawn();
                finished = true;
            }
            yield return null;
        }
    }
    
}
