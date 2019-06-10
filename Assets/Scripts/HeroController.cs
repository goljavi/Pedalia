using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float respawnTime = 3;
    PhotonView _pv;
    Hero _hero;
    HeroKeyboardController _hkc;
    GameObject _sceneCamera;

    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        _sceneCamera = GameObject.Find("SceneCamera");
        SpawnHero();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pv.IsMine && _hkc != null) _hkc.Update();
    }

    void SpawnHero()
    {
        if (!_pv.IsMine) return;
        _sceneCamera.SetActive(false);
        _hero = PhotonNetwork.Instantiate("Hero", LevelManager.Instance.Pick(), Quaternion.identity).GetComponent<Hero>();
        _hkc = new HeroKeyboardController(_hero);
        _hero.heroControllerInstance = this;
    }

    public void Die()
    {
        if (!_pv.IsMine) return;
        PhotonNetwork.Destroy(_hero.gameObject);
        _hkc = null;
        _sceneCamera.SetActive(true);
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnHero();
    }
}
