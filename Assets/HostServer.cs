using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostServer : MonoBehaviourPunCallbacks
{
    public static HostServer Instance { get; private set; }
    public Dictionary<Player, Hero> heros = new Dictionary<Player, Hero>();
    public Player serverRef;
    public HeroController heroControllerReference;
    public float timeForFlagToReAppear = 1;
    public float timeForExit = 5;

    PhotonView _pv;

    float _flagSpawnCounter;
    Hero _flagOwner;
    bool _win = false;
    Transform[] _spawnPoints;
    int _playerQty;
    bool _matchStarted;

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

    }

    [PunRPC]
    public void SetReferenceToSelf(Player p)
    {
        Instance = this;
        serverRef = p;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient || _win) return;

        if (!_matchStarted)
        {
            if (PhotonNetwork.PlayerList.Length > 2) StartCoroutine(StartMatch());
            return;
        }

        var checkFlag = false;

        foreach (var player in FindObjectsOfType<Hero>())
        {
            if (player.flagCount >= 3) SetWin(player);

            if (player.hasFlag)
            {
                if (player != _flagOwner) SetFlagOwner(player);
                checkFlag = true;
            }
        }

        if (!checkFlag && GameObject.FindGameObjectsWithTag("Flag").Length == 0)
        {
            SetFlagOwner(null);

            _flagSpawnCounter += Time.deltaTime;
            if (_flagSpawnCounter > timeForFlagToReAppear) InstantiateFlag();
        }

        SetThrustUIForEveryone();
    }

    #region host actions
    IEnumerator StartMatch()
    {
        _pv.RPC("RPC_TriggerMatchBegin", RpcTarget.All);
        foreach (var hero in heros) Kill(hero.Value, false);
        _matchStarted = true;
        yield return new WaitForSeconds(3);
        _pv.RPC("RPC_BeginMatch", RpcTarget.AllBufferedViaServer);
    }    

    [PunRPC]
    void RPC_BeginMatch()
    {
        _matchStarted = true;
        PlayerUI.Instance.MatchBegin();
    }

    [PunRPC]
    void RPC_TriggerMatchBegin()
    {
        PlayerUI.Instance.TriggerMatchBegin();
    }

    void InstantiateFlag()
    {
        PhotonNetwork.Instantiate("Flag", GameObject.FindGameObjectsWithTag("FlagSpawn")[0].transform.position, Quaternion.identity);
        _flagSpawnCounter = 0;
    }

    void SetFlagOwner(Hero player)
    {
        _flagOwner = player;
        _pv.RPC("RPC_SetFlagOwnership", RpcTarget.AllBuffered, player ? player.transform.name : "None");
    }

    void SetWin(Hero player)
    {
        _pv.RPC("RPC_Winner", RpcTarget.AllBuffered, player.transform.name);
        _win = true;
    }

    [PunRPC]
    void RPC_SetFlagOwnership(string owner)
    {
        PlayerUI.Instance.SetFlagOwnerUI(owner);
    }

    [PunRPC]
    void RPC_Winner(string winner)
    {
        PlayerUI.Instance.TriggerVictoryUI(winner);
        StartCoroutine(ThrowEveryoneOut());
    }

    IEnumerator ThrowEveryoneOut()
    {
        yield return new WaitForSeconds(timeForExit);
        Room.Instance.Disconnect();
    }

    //PLAYER DEATH
    public void Kill(Hero hero, bool instantiateParticles = true)
    {
        if (!_pv.IsMine) return;
        hero.Die(instantiateParticles);
    }

    void SetThrustUIForEveryone()
    {
        foreach (var hero in heros) _pv.RPC("RPC_SetThrustUI", hero.Key, hero.Value.thrustAmount);
    }

    [PunRPC]
    void RPC_SetThrustUI(float thrustAmount)
    {
        PlayerUI.Instance.ThrustAmount = thrustAmount;
    }
    #endregion

    #region player actions
    //SPAWN
    public void PlayerRequestSpawn()
    {
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
                      new object[] { spawnPoint.forward })
                      .GetComponent<Hero>();

        heros.Add(p, newHero);
        foreach (var item in heros) Debug.Log(item);
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

    // PLAYER Y AXIS ROTATION
    public void PlayerRequestRotate(float mouseXAxis)
    {
        _pv.RPC("RPC_Rotate", serverRef, PhotonNetwork.LocalPlayer, mouseXAxis);
    }

    [PunRPC]
    void RPC_Rotate(Player p, float mouseXAxis)
    {
        if (!_pv.IsMine) return;
        heros[p].Rotate(mouseXAxis);
    }

    // PLAYER CAMERA X AXIS ROTATION
    public void PlayerRequestRotateCamera(float mouseYAxis)
    {
        _pv.RPC("RPC_RotateCamera", serverRef, PhotonNetwork.LocalPlayer, mouseYAxis);
    }

    [PunRPC]
    void RPC_RotateCamera(Player p, float mouseYAxis)
    {
        if (!_pv.IsMine) return;
        heros[p].RotateCamera(mouseYAxis);
    }

    // PLAYER WEAPON FIRE
    public void PlayerRequestFire()
    {
        _pv.RPC("RPC_Fire", serverRef, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void RPC_Fire(Player p)
    {
        if (!_pv.IsMine) return;
        heros[p].Fire();
    }
    #endregion

    public override void OnPlayerLeftRoom(Player player)
    {
        base.OnPlayerLeftRoom(player);
        heros[player].Die();
        PhotonNetwork.Destroy(heros[player].gameObject);
        heros.Remove(player);
    }
}
