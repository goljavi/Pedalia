using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance;
    public string playerName;

    private void OnEnable()
    {
        /* Si estoy creando un nuevo objeto pero el singleton ya tiene un objeto 
         * cargado (por ejemplo, porque viene de una escena anterior) lo destruyo.*/
        if (Instance && Instance != this) Destroy(Instance.gameObject);
        if (!Instance) Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
