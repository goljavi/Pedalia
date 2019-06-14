using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public GameObject cam;
    public float cameraRotationLimit = 85;
    public float impulseForce = 30;
    public Animator anim;
    public Transform bulletSpawnPoint;
    public GameObject playerHasFlagParticles;
    public GameObject jetpackParticles;
    public float thrusterForce = 1500;
    public float thrustUseSpeed = 1;
    public float thrustRegenSpeed = 0.28f;
    public float thrustAmount = 1;

    [HideInInspector] public HeroController heroControllerInstance;
    [HideInInspector] public bool hasFlag = false;
    [HideInInspector] public int flagCount;

    Rigidbody _rb;
    PhotonView _pv;
    Vector3 _velocity = Vector3.zero;
    Vector3 _rotation = Vector3.zero;
    float _cameraRotationX = 0;
    float _currentCameraRotationX = 0;
    Vector3 _thrusterForce = Vector3.zero;
    bool _jetpackParticlesValueBefore = false;
    bool usingJetpack = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _pv = GetComponent<PhotonView>();
        if((bool)_pv.InstantiationData[0]) PhotonNetwork.Instantiate("PlayerSpawn", transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        transform.forward = (Vector3)_pv.InstantiationData[1];
        _pv.RPC("RPC_FlagUpdate", RpcTarget.All, (int)_pv.InstantiationData[2]);
    }

    private void Update()
    {
        if (!_pv.IsMine) return;
        PerformAnimations();
        ThrusterWorks();
        SetUI();
    }

    void SetUI()
    {
        PlayerUI.Instance.ThrustAmount = thrustAmount;
    }

    void ThrusterWorks()
    {
        //Thrust regen
        thrustAmount += thrustRegenSpeed * Time.deltaTime;
        thrustAmount = Mathf.Clamp(thrustAmount, 0, 1);

        //Thruster force
        var thrust = Vector3.zero;
        if (usingJetpack && thrustAmount > 0)
        {
            thrustAmount -= thrustUseSpeed * Time.deltaTime;
            if (thrustAmount > 0.1f)
            {
                thrust = Vector3.up * thrusterForce;
            }
        }

        //Thrust apply
        ApplyThruster(thrust);
    }

    public void UsingJetpack(bool value)
    {
        usingJetpack = value;
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
        if (!_pv.IsMine) return;
        PerformMovement();
        PerformRotation();
    }

    void PerformAnimations()
    {
        if (_velocity != Vector3.zero) anim.SetBool("Running", true);
        else anim.SetBool("Running", false);

        if (_rb.velocity.y > 0.1f || _rb.velocity.y < -0.1f) anim.SetBool("Jumping", true);
        else anim.SetBool("Jumping", false);
    }

    void PerformMovement()
    {
        if (_velocity != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + _velocity * Time.deltaTime);
        }

        if (_thrusterForce != Vector3.zero)
        {
            SetJetpackParticles(true);
            _rb.AddForce(_thrusterForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            SetJetpackParticles(false);
        }
    }

    public void SetJetpackParticles(bool value)
    {
        if (value != _jetpackParticlesValueBefore)
        {
            _jetpackParticlesValueBefore = value;
            _pv.RPC("RPC_SetJetpackParticles", RpcTarget.All, value);
        }
    }

    [PunRPC]
    void RPC_SetJetpackParticles(bool value)
    {
        jetpackParticles.SetActive(value);
    }

    void PerformRotation()
    {
        if (_rotation != Vector3.zero) _rb.MoveRotation(_rb.rotation * Quaternion.Euler(_rotation));
        _pv.RPC("RPC_PerformCamRotation", RpcTarget.All, _cameraRotationX);
    }

    [PunRPC]
    void RPC_PerformCamRotation(float cameraRotationX)
    {
        if (!cam) return;
        _currentCameraRotationX -= cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        cam.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0, 0);
    }

    public void Fire()
    {
        if (!_pv.IsMine) return;
        var projectile = PhotonNetwork.Instantiate("Projectile", bulletSpawnPoint.position, transform.rotation, 0, new object[] { cam.transform.forward, bulletSpawnPoint.position });
    }

    private void OnCollisionEnter(Collision other)
    {
        var bullet = other.gameObject.GetComponent<Projectile>();
        if (bullet && bullet.pv.Owner.ActorNumber != _pv.Owner.ActorNumber)
        {
            _pv.RPC("RPC_GetPushed", _pv.Owner, bullet.transform.forward);
            PhotonNetwork.Instantiate("BulletExplosion", other.GetContact(0).point, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_pv.IsMine && other.gameObject.layer == 12) Die();
        if (_pv.IsMine && hasFlag && other.gameObject.layer == 13) RemoveFlag();
    }

    public void Die()
    {
        _pv.RPC("RPC_Die", RpcTarget.All);
        PhotonNetwork.Instantiate("DeathExplosion", transform.position, Quaternion.identity);
        heroControllerInstance.Die();
    }

    public void GetFlag()
    {
        hasFlag = true;
        _pv.RPC("RPC_GetFlag", RpcTarget.All);
    }

    public void RemoveFlag()
    {
        PlayerUI.Instance.SetFlagNumberUI(flagCount + 1);
        _pv.RPC("RPC_RemoveFlag", RpcTarget.All);
    }

    [PunRPC]
    void RPC_Die()
    {
        hasFlag = false;
    }

    [PunRPC]
    void RPC_FlagUpdate(int flags)
    {
        flagCount = flags;
    }

    [PunRPC]
    void RPC_GetFlag()
    {
        hasFlag = true;
        playerHasFlagParticles.SetActive(true);
    }

    [PunRPC]
    void RPC_RemoveFlag()
    {
        hasFlag = false;
        playerHasFlagParticles.SetActive(false);
        flagCount++;
        if(heroControllerInstance) heroControllerInstance.AddFlag();
        PhotonNetwork.Instantiate("LeaveFlag", transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    [PunRPC]
    void RPC_GetPushed(Vector3 dir)
    {
        _rb.AddForce(dir * impulseForce, ForceMode.Impulse);
        _rb.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);
    }

    [PunRPC]
    void RPC_Debug(string str)
    {
        Debug.Log(str);
    }
}
