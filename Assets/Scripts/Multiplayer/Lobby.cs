using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby Instance;
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
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Lobby: OnConnectedToMaster()");
        PhotonNetwork.AutomaticallySyncScene = true;
        playButton.SetActive(true);
    }

    public void Play()
    {
        Debug.Log("Lobby: Play()");
        playButton.SetActive(false);
        nameInput.SetActive(false);
        PlayerInfo.Instance.playerName = string.IsNullOrWhiteSpace(playerName.text) ? "Player" : playerName.text;
        PhotonNetwork.JoinRandomRoom();
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
        CreateRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
