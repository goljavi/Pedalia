using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotationsPerMinuteX;
    public float rotationsPerMinuteY;
    public float rotationsPerMinuteZ;

    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
            transform.Rotate(6 * rotationsPerMinuteX * Time.deltaTime, 6 * rotationsPerMinuteY * Time.deltaTime, 6 * rotationsPerMinuteZ * Time.deltaTime);
    }
}
