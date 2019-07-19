using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKeyboardController
{
    bool _jumpPress;
    bool _jumpRelease;
    float _horizontalAxis;
    float _verticalAxis;
    float _mouseXAxis;
    float _mouseYAxis;
    bool _fire;

    public HeroKeyboardController()
    {
    }

    public void Update(bool frozen)
    {
        if (!frozen)
        {
            _jumpPress = Input.GetButtonDown("Jump");
            _jumpRelease = Input.GetButtonUp("Jump");
            _horizontalAxis = Input.GetAxisRaw("Horizontal");
            _verticalAxis = Input.GetAxisRaw("Vertical");
            _mouseXAxis = Input.GetAxisRaw("Mouse X");
            _mouseYAxis = Input.GetAxisRaw("Mouse Y");
            _fire = Input.GetButtonDown("Fire1");
        }
        else
        {
            _jumpPress = false;
            _horizontalAxis = 0;
            _verticalAxis = 0;
            _mouseXAxis = 0;
            _mouseYAxis = 0;
            _fire = false;
        }

        //Jet pack
        if (_jumpPress) HostServer.Instance.PlayerRequestUseJetpack();
        if (_jumpRelease) HostServer.Instance.PlayerRequestReleaseJetpack();

        //Movement
        HostServer.Instance.PlayerRequestMove(_horizontalAxis,  _verticalAxis);

        //Player Rotation on Y axis
        HostServer.Instance.PlayerRequestRotate(_mouseXAxis);

        //Camera rotation on X axis
        HostServer.Instance.PlayerRequestRotateCamera(_mouseYAxis);

        //Weapon fire
        if (_fire) HostServer.Instance.PlayerRequestFire();
    }
}
