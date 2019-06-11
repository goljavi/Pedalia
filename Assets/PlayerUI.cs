using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    public RectTransform thrustBarAmount;
    public GameObject pauseMenu;
    public GameObject console;
    public static PlayerUI instance;
    public static bool pauseMenuOpen = false;
    public static bool consoleOpen = false;
    public static bool victoryShown = false;
    private Text _playerName;
    private Text _flagOwner;
    private Text _flagNumber;
    private Text _victory;
    private GameObject _playerNameObj;
    private GameObject _flagOwnerObj;
    private GameObject _flagNumberObj;
    private GameObject _victoryObj;

    void Start()
    {
        
    }

    void Awake()
    {
        instance = this;
        _playerNameObj = GameObject.Find("PlayerName");
        _flagOwnerObj = GameObject.Find("FlagOwner");
        _flagNumberObj = GameObject.Find("FlagNumber");
        _victoryObj = GameObject.Find("VictorySign");
        _playerName = _playerNameObj.GetComponent<Text>();
        _flagOwner = _flagOwnerObj.GetComponent<Text>();
        _flagNumber = _flagNumberObj.GetComponent<Text>();
        _victory = _victoryObj.GetComponent<Text>();
    }

    void Update()
    {
        //thrustBarAmount.localScale = new Vector3(1, controller.thrustAmount, 1);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ToggleConsole();
        }

    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        pauseMenuOpen = pauseMenu.activeSelf;
    }
    public void ToggleConsole()
    {
        console.SetActive(!console.activeSelf);
        consoleOpen = console.activeSelf;
    }

    public void SetPlayerUIName(string txt)
    {
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
        _victory.text = name + " Wins!";
        victoryShown = true;

        GameObject.Find("Crosshair").SetActive(false);
        _flagOwnerObj.SetActive(false);
        _flagNumberObj.SetActive(false);
    }
}