using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostServer : MonoBehaviourPun
{
    public static HostServer Instance { get; private set; }
    public Dictionary<Player, Hero> heros = new Dictionary<Player, Hero>();
    public Player serverRef;
    public HeroController heroControllerReference;
    public float respawnTime = 3;

    bool _instantiateParticles;
    int _flagCount;
    PhotonView _pv;
    GameObject _sceneCamera;
    AudioListener _sceneAudioListener;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
        if (!Instance)
        {
            if (_pv.IsMine) _pv.RPC("SetReferenceToSelf", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }

        _sceneCamera = GameObject.Find("SceneCamera");
        _sceneAudioListener = _sceneCamera.GetComponent<AudioListener>();
    }

    [PunRPC]
    public void SetReferenceToSelf(Player p)
    {
        Instance = this;
        serverRef = p;
    }

    //SPAWN
    public void PlayerRequestSpawn()
    {
        _sceneCamera.SetActive(false);
        if (_sceneAudioListener) _sceneAudioListener.enabled = false;
        _pv.RPC("SpawnHero", serverRef, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void SpawnHero(Player p)
    {
        if (!_pv.IsMine) return;

        var spawnPoint = LevelManager.Instance.Pick();

        var newHero = PhotonNetwork.Instantiate("Hero", 
                      spawnPoint.position, 
                      Quaternion.identity, 0, 
                      new object[] { _instantiateParticles, spawnPoint.forward, _flagCount })
                      .GetComponent<Hero>();

        heros.Add(p, newHero);
        _pv.RPC("RPC_ActivateClientCamera", p);
        foreach (var item in heros) Debug.Log(item);
    }

    //PLAYER DEATH
    public void Die(Hero hero, bool instantiateParticles = true)
    {
        if (!_pv.IsMine) return;
        hero.gameObject.SetActive(false);
        _instantiateParticles = instantiateParticles;
        StartCoroutine(Respawn(hero));
    }

    IEnumerator Respawn(Hero hero)
    {
        yield return new WaitForSeconds(respawnTime);
        hero.gameObject.SetActive(false);
    }

    //PLAYER MOVE
    public void PlayerRequestMove(float h, float v)
    {
        _pv.RPC("RPC_Move", serverRef, PhotonNetwork.LocalPlayer, h, v);
    }

    [PunRPC]
    void RPC_Move(Player p, float h, float v)
    {
        if (!_pv.IsMine) return;
        heros[p].Move(h, v);
    }

    //PLAYER JETPACK
    public void PlayerRequestUseJetpack()
    {
        _pv.RPC("RPC_UseJetpack", serverRef, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void RPC_UseJetpack(Player p)
    {
        if (!_pv.IsMine) return;
        heros[p].UsingJetpack();
    }


}
