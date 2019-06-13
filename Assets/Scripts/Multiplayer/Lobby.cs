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
    public GameObject nameInput;
    public Text playerName;

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

    public override void OnConnectedToMaster()
    {
        Debug.Log("Lobby: OnConnectedToMaster()");
        //PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinRandomRoom();
    }

    public void Play()
    {
        Debug.Log("Lobby: Play()");
        PhotonNetwork.ConnectUsingSettings();
        playButton.SetActive(false);
        nameInput.SetActive(false);
        PlayerInfo.Instance.playerName = string.IsNullOrWhiteSpace(playerName.text) ? "Player" : playerName.text;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Lobby: OnJoinRandomFailed()");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Lobby: CreateRoom()");
        PhotonNetwork.CreateRoom("Room: " + Random.Range(0, 1000), new RoomOptions() { MaxPlayers = 5 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Lobby: OnCreateRoomFailed()");
        playButton.SetActive(true);
        nameInput.SetActive(true);
    }
}
