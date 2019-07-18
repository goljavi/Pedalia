using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKeyboardController
{
    bool _jump;
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
        Debug.Log("HeroKeyboardController");
        if (!frozen)
        {
            _jump = Input.GetButton("Jump");
            _horizontalAxis = Input.GetAxisRaw("Horizontal");
            _verticalAxis = Input.GetAxisRaw("Vertical");
            _mouseXAxis = Input.GetAxisRaw("Mouse X");
            _mouseYAxis = Input.GetAxisRaw("Mouse Y");
            _fire = Input.GetButtonDown("Fire1");
        }
        else
        {
            _jump = false;
            _horizontalAxis = 0;
            _verticalAxis = 0;
            _mouseXAxis = 0;
            _mouseYAxis = 0;
            _fire = false;
        }

        //Jet pack
        if(_jump) HostServer.Instance.PlayerRequestUseJetpack();

        //Movement
        HostServer.Instance.PlayerRequestMove(_horizontalAxis,  _verticalAxis);

        //Player Rotation on Y axis
        //_hero.Rotate(new Vector3(0, _mouseXAxis, 0) * lookSpeed);

        //Camera rotation on X axis
        //_hero.RotateCamera(_mouseYAxis * lookSpeed);

        //Weapon fire
        //if (_fire) _hero.Fire();
    }
}
