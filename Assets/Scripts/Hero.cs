using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Camera cam;
    public float cameraRotationLimit = 85;
    public float impulseForce = 30;

    Rigidbody _rb;
    PhotonView _pv;
    Vector3 _velocity = Vector3.zero;
    Vector3 _rotation = Vector3.zero;
    float _cameraRotationX = 0;
    float _currentCameraRotationX = 0;
    Vector3 _thrusterForce = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void Rotate(Vector3 rotation)
    {
        _rotation = rotation;
    }

    public void RotateCamera(float cameraRotationX)
    {
        _cameraRotationX = cameraRotationX;
    }

    public void ApplyThruster(Vector3 thrusterForce)
    {
        _thrusterForce = thrusterForce;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if (_velocity != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + _velocity * Time.deltaTime);
        }

        if (_thrusterForce != Vector3.zero)
        {
            _rb.AddForce(_thrusterForce * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        if (_rotation != Vector3.zero) _rb.MoveRotation(_rb.rotation * Quaternion.Euler(_rotation));
        if (cam != null)
        {
            _currentCameraRotationX -= _cameraRotationX;
            _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0, 0);
        }
    }

    public void Fire()
    {
        var projectile = PhotonNetwork.Instantiate("Projectile", transform.position, transform.rotation);
        projectile.transform.forward = cam.transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        var bullet = other.GetComponent<Projectile>();
        if (bullet)
        {
            GetPushed(bullet.transform.forward);
        }
    }

    void GetPushed(Vector3 dir)
    {
        _rb.AddForce(dir * impulseForce, ForceMode.Impulse);
    }
}
