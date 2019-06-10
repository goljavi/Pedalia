using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    private PhotonView _pv;
    GameObject _hero;
    HeroKeyboardController _hkc;

    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        if (_pv.IsMine)
        {
            Camera.main.gameObject.SetActive(false);
            _hero = PhotonNetwork.Instantiate("Hero", LevelManager.Instance.Pick(), Quaternion.identity);
            _hkc = new HeroKeyboardController(_hero.GetComponent<Hero>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_pv.IsMine) _hkc.Update();
    }
}
