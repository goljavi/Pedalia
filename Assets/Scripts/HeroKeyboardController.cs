using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKeyboardController
{
    public static float speed = 7;
    public float lookSpeed = 5;
    Hero _hero;

    bool _jump;
    float _horizontalAxis;
    float _verticalAxis;
    float _mouseXAxis;
    float _mouseYAxis;
    bool _fire;

    public HeroKeyboardController(Hero hero)
    {
        _hero = hero;
    }

    public void Update(bool frozen)
    {
        if (!_hero) return;

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
        _hero.UsingJetpack(_jump);

        //Movement
        _hero.Move(((_hero.transform.right * _horizontalAxis) + (_hero.transform.forward * _verticalAxis)).normalized * speed);

        //Player Rotation on Y axis
        _hero.Rotate(new Vector3(0, _mouseXAxis, 0) * lookSpeed);

        //Camera rotation on X axis
        _hero.RotateCamera(_mouseYAxis * lookSpeed);

        //Weapon fire
        if (_fire) _hero.Fire();
    }
}
