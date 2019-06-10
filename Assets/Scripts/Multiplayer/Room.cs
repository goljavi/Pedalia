using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class Room : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static Room Instance;
    public int currentScene;
    public int multiplayerScene = 1;

    PhotonView _pv;

    void Awake()
    {
        /* Si estoy creando un nuevo objeto room pero el singleton ya tiene un objeto 
         * cargado (por ejemplo, porque viene de una escena anterior) lo destruyo.*/
        if (Instance && Instance != this) Destroy(Instance.gameObject);
        if (!Instance) Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room: OnJoinedRoom()");
        if(PhotonNetwork.IsMasterClient) StartGame();
    }

    void StartGame()
    {
        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    void OnSceneFinishedLoading(Scene s, LoadSceneMode mode)
    {
        currentScene = s.buildIndex;
        if(currentScene == multiplayerScene)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate("HeroController", transform.position, Quaternion.identity);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("Room: OnPlayerLeftRoom()");
    }
}
