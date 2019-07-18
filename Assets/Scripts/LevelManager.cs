using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public float timeForFlagToReAppear = 1;
    public float timeForExit = 5;

    [HideInInspector] public HeroController heroControllerInstance;

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
        _spawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
    }

    private void OnEnable()
    {
        if (!Instance) Instance = this;
    }

    public Transform Pick()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || _win) return;

        if (!_matchStarted)
        {
            //if (PhotonNetwork.PlayerList.Length > 1) StartCoroutine(StartMatch());
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
    }

    /*IEnumerator StartMatch()
    {
        _pv.RPC("RPC_DieBeforeMatch", RpcTarget.All);
        _matchStarted = true;
        //yield return new WaitForSeconds(heroControllerInstance.respawnTime);
        _pv.RPC("RPC_BeginMatch", RpcTarget.AllBufferedViaServer);
    }*/

    [PunRPC]
    void RPC_BeginMatch()
    {
        _matchStarted = true;
        PlayerUI.Instance.MatchBegin();
    }

    [PunRPC]
    void RPC_DieBeforeMatch()
    {
        //heroControllerInstance.Die(false);
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
}
