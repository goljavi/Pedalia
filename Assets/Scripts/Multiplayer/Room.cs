using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Room : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static Room Instance;
    public int currentScene;
    public int multiplayerScene = 1;
    public static DisconnectCause disconnectError =  DisconnectCause.None;

    PhotonView _pv;

    void Awake()
    {
        /* Si estoy creando un nuevo objeto room pero el singleton ya tiene un objeto 
         * cargado (por ejemplo, porque viene de una escena anterior) lo destruyo.*/
        if (Instance && Instance != this)
        {
            if (Instance != this)
            {
                Destroy(Instance.gameObject);
            }
        }
        if (!Instance)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        var errors = GameObject.Find("Errors");
        if (errors && disconnectError != DisconnectCause.None && disconnectError != DisconnectCause.DisconnectByClientLogic)
        {
            errors.GetComponent<Text>().text = "Error: " + disconnectError;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room: OnJoinedRoom()");
        StartGame();
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
        disconnectError = DisconnectCause.None;
        Instantiate(Resources.Load("PlayerUI"));
        PhotonNetwork.Instantiate("HeroController", transform.position, Quaternion.identity);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("Room: OnPlayerLeftRoom()");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        disconnectError = cause;
        SceneManager.LoadScene(0);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        StartCoroutine(LoadMenuWhenDisconnected());
    }

    IEnumerator LoadMenuWhenDisconnected()
    {
        while (PhotonNetwork.InRoom) yield return null;
        SceneManager.LoadScene(0);
    }

    public void LeaveGame()
    {
        Application.Quit();
    }
}
