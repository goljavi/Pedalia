using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform[] spawnPoints;

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
}
