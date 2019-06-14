using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float respawnTime = 3;
    public Hero hero;
    
    PhotonView _pv;
    HeroKeyboardController _hkc;
    GameObject _sceneCamera;
    AudioListener _sceneAudioListener;
    bool _instantiateParticles;
    int _flagCount;

    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        if (!_pv.IsMine) return;
        _sceneCamera = GameObject.Find("SceneCamera");
        _sceneAudioListener = _sceneCamera.GetComponent<AudioListener>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LevelManager.Instance.heroControllerInstance = this;
        SpawnHero();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pv.IsMine && _hkc != null)
        {
            _hkc.Update(PlayerUI.Instance.pauseMenuOpen);
        }
    }

    void SpawnHero()
    {
        if (!_pv.IsMine) return;
        _sceneCamera.SetActive(false);
        if(_sceneAudioListener) _sceneAudioListener.enabled = false;

        var spawnPoint = LevelManager.Instance.Pick();
        hero = PhotonNetwork.Instantiate("Hero", spawnPoint.position, Quaternion.identity, 0, new object[] { _instantiateParticles, spawnPoint.forward, _flagCount }).GetComponent<Hero>();
        _hkc = new HeroKeyboardController(hero);
        hero.heroControllerInstance = this;
    }

    public void Die(bool instantiateParticles = true)
    {
        if (!_pv.IsMine) return;
        PhotonNetwork.Destroy(hero.gameObject);
        _hkc = null;
        _sceneCamera.SetActive(true);
        _sceneAudioListener.enabled = true;
        _instantiateParticles = instantiateParticles;
        StartCoroutine(Respawn());
    }

    public void AddFlag()
    {
        _flagCount++;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnHero();
    }
}
