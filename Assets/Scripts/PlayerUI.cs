using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public RectTransform thrustBarAmount;
    public GameObject pauseMenu;
    public static PlayerUI Instance;
    [HideInInspector] public bool pauseMenuOpen = false;
    [HideInInspector] public bool victoryShown = false;

    private Text _playerName;
    private Text _flagOwner;
    private Text _flagNumber;
    private Text _victory;
    private GameObject _flagOwnerObj;
    private GameObject _flagNumberObj;
    private GameObject _playerNameObj;
    private GameObject _victoryObj;
    private GameObject _matchBeginObj;
    private GameObject _waitingPlayers;
    public float ThrustAmount { private get; set; }

    void Awake()
    {
        Instance = this;
        _flagOwnerObj = transform.Find("FlagOwner").gameObject;
        _flagNumberObj = transform.Find("FlagNumber").gameObject;
        _victoryObj = transform.Find("VictorySign").gameObject;
        _playerNameObj = transform.Find("PlayerName").gameObject;
        _matchBeginObj = transform.Find("MatchBegin").gameObject;
        _waitingPlayers = transform.Find("WaitingPlayers").gameObject;

        _playerName = _playerNameObj.GetComponent<Text>();
        _flagOwner = _flagOwnerObj.GetComponent<Text>();
        _flagNumber = _flagNumberObj.GetComponent<Text>();
        _victory = _victoryObj.GetComponent<Text>();
    }

    private void Start()
    {
        _flagOwnerObj.SetActive(false);
        _flagNumberObj.SetActive(false);
        _waitingPlayers.SetActive(true);
    }

    void Update()
    {
        thrustBarAmount.localScale = new Vector3(1, ThrustAmount, 1);
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        pauseMenuOpen = pauseMenu.activeSelf;
        Cursor.visible = pauseMenu.activeSelf;
        Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetPlayerUIName(string txt)
    {
        Debug.Log("Recibí: " + txt);
        _playerName.text = txt;
    }

    public void SetFlagOwnerUI(string txt)
    {
        _flagOwner.text = txt + " has the flag";
    }

    public void SetFlagNumberUI(int num)
    {
        _flagNumber.text = "Flags: " + num;
    }

    public void TriggerVictoryUI(string name)
    {
        _victoryObj.SetActive(true);
        _victory.text = name + " Wins!";
        victoryShown = true;

        transform.Find("Crosshair").gameObject.SetActive(false);
        _flagOwnerObj.SetActive(false);
        _flagNumberObj.SetActive(false);
    }

    public void TriggerMatchBegin()
    {
        _flagOwnerObj.SetActive(true);
        _flagNumberObj.SetActive(true);
        _waitingPlayers.SetActive(false);
        _matchBeginObj.SetActive(true);
    }

    public void MatchBegin()
    {
        _flagOwnerObj.SetActive(true);
        _flagNumberObj.SetActive(true);
        _waitingPlayers.SetActive(false);
        _matchBeginObj.SetActive(false);
    }

    public void Disconnect()
    {
        Room.Instance.Disconnect();
    }

    public void LeaveGame()
    {
        Room.Instance.LeaveGame();
    }
}