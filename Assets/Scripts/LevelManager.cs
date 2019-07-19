using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    PhotonView _pv;
    Transform[] _spawnPoints;

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

}
