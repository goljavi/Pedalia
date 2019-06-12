using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKeyboardController
{
    public static float speed = 7;
    public float lookSpeed = 5;
    Hero _hero;

    public HeroKeyboardController(Hero hero)
    {
        _hero = hero;
    }

    public void Update()
    {
        if (!_hero) return;

        //Jet pack
        _hero.UsingJetpack(Input.GetButton("Jump"));

        //Movement
        _hero.Move(((_hero.transform.right * Input.GetAxisRaw("Horizontal")) + (_hero.transform.forward * Input.GetAxisRaw("Vertical"))).normalized * speed);

        //Player Rotation on Y axis
        _hero.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X"), 0) * lookSpeed);

        //Camera rotation on X axis
        _hero.RotateCamera(Input.GetAxisRaw("Mouse Y") * lookSpeed);

        //Weapon fire
        if (Input.GetButtonDown("Fire1")) _hero.Fire();
    }
}
