using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby Instance;
    public GameObject room;
    public GameObject playerInfo;
    public GameObject playButton;
    public GameObject hostButton;
    public GameObject exitButton;
    public GameObject nameInput;
    public Text playerName;
    bool _isHost;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in FindObjectsOfType<Room>()) Destroy(item.gameObject);
        foreach (var item in FindObjectsOfType<PlayerInfo>()) Destroy(item.gameObject);

        Instantiate(room);
        Instantiate(playerInfo);
        playButton.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play(bool isHost = false)
    {
        _isHost = isHost;
        Room.Instance.isHost = isHost;

        nameInput.SetActive(false);
        hostButton.SetActive(false);
        playButton.SetActive(false);
        exitButton.SetActive(false);
        PlayerInfo.Instance.playerName = string.IsNullOrWhiteSpace(playerName.text) ? "Player" : playerName.text;

        if (PhotonNetwork.IsConnected) OnConnectedToMaster();
        else PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (_isHost) CreateRoom();
        else PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Lobby: OnJoinRandomFailed()");
        var errors = GameObject.Find("Errors");
        if (errors) errors.GetComponent<Text>().text = "No host was found";
        nameInput.SetActive(true);
        hostButton.SetActive(true);
        playButton.SetActive(true);
        exitButton.SetActive(true);
    }

    void CreateRoom()
    {
        Debug.Log("Lobby: CreateRoom()");
        PhotonNetwork.CreateRoom("Room: " + Random.Range(0, 1000), new RoomOptions() { MaxPlayers = 5 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Lobby: OnCreateRoomFailed()");
        nameInput.SetActive(true);
        hostButton.SetActive(true);
        playButton.SetActive(true);
        exitButton.SetActive(true);
    }

    public void LeaveGame() => Application.Quit();
}
