using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKeyboardController
{
    public static float speed = 7;
    public float lookSpeed = 5;
    public static float thrusterForce = 1500;
    public float thrustUseSpeed = 1;
    public static float thrustRegenSpeed = 0.28f;
    public float thrustAmount = 1;
    Hero _hero;

    public HeroKeyboardController(Hero hero)
    {
        _hero = hero;
    }

    public void Update()
    {
        if (!_hero) return;

        //Thrust regen
        thrustAmount += thrustRegenSpeed * Time.deltaTime;
        thrustAmount = Mathf.Clamp(thrustAmount, 0, 1);

        //Thruster force
        var thrust = Vector3.zero;
        if (Input.GetButton("Jump") && thrustAmount > 0)
        {
            thrustAmount -= thrustUseSpeed * Time.deltaTime;
            if (thrustAmount > 0.1f)
            {
                thrust = Vector3.up * thrusterForce;
            }
        }

        //Thrust apply
        _hero.ApplyThruster(thrust);

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
