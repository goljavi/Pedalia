using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform[] spawnPoints;

    float counter;

    private void OnEnable()
    {
        if (!Instance) Instance = this;
    }

    public Vector3 Pick()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }

    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        StartCoroutine(LoadMenuWhenDisconnected());
    }

    IEnumerator LoadMenuWhenDisconnected()
    {
        while (PhotonNetwork.InRoom) yield return null;
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var checkFlag = false;
        var players = FindObjectsOfType<Hero>();


        foreach (Hero player in players)
        {
            if (player.flagCount >= 3)
            {
                //Win
            }

            if (player.hasFlag)
            {
                //PlayerUI.instance.SetFlagOwnerUI(player.transform.name);
                checkFlag = true;
            }
        }

        if (!checkFlag && GameObject.FindGameObjectsWithTag("Flag").Length == 0)
        {
            counter += Time.deltaTime;
            if (counter > 1)
            {
                //PlayerUI.instance.SetFlagOwnerUI("None");
                PhotonNetwork.Instantiate("Flag", GameObject.FindGameObjectsWithTag("FlagSpawn")[0].transform.position, Quaternion.identity);
                counter = 0;
            }

        }
    }
}
