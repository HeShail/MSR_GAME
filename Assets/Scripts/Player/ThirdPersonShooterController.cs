using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using StarterAssets;
using UnityEngine;
using TMPro;
public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private int selectedWeapon=0;
    [SerializeField] private GameObject weaponSlot0; //Character secondary weapons (pistol)
    [SerializeField] private GameObject weaponSlot1; //Character primary weapons (rifle)
    [SerializeField] private GameObject weaponSlot2; //Character main weapon (Great Axe, longbow)
    [SerializeField] private bool isArmed;
    [SerializeField] private bool isAiming = false;
    [SerializeField] private CinemachineVirtualCamera aimVC;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private List<GameObject> weaponList;
    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity=0.5f;
    [SerializeField] private TextMeshProUGUI ammoText;
    public Cinemachine.CinemachineImpulseSource rifleImpulseSource;
    public Cinemachine.CinemachineImpulseSource glockImpulseSource;
    private int _animIDBackwardsMove;
    public GameObject body;
    private bool _isReloading;
    private bool _hasAnimator;
    private bool movingBackwards=false;
    private Animator _animator;
    private ThirdPersonController playerController;
    private PlayerInputs _input;
    private Vector2 screenCenterPoint;
    private int rifleAmmo;
    private int pistolAmmo;
    private int rifleMagazine;
    private int pistolMagazine;
    private float _automaticGunShootTime;
    public Transform _rifleMouthLoc;
    public ParticleSystem shootPS;
    public Transform _pistolMouthLoc;
    public ParticleSystem pistol_shootPS;
    public ParticleSystem proyectilPS;
    public GameObject impactPSPrefab;
    public Rig pistol_IK;


    void Awake()
    {
        aimVC.gameObject.SetActive(false);
    }
    void Start()
    {
        weaponList = new List<GameObject>();
        weaponList.Capacity = 3;
        playerController = GetComponent<ThirdPersonController>();
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        _hasAnimator = body.TryGetComponent(out _animator);
        _animIDBackwardsMove = Animator.StringToHash("backwardsMove");
        debugTransform.GetComponent<MeshRenderer>().enabled = false;
        weaponList.Add(weaponSlot0);
        weaponList.Add(weaponSlot1);
        weaponList.Add(weaponSlot2);
        rifleAmmo = 60;
        pistolAmmo = 20;
        rifleMagazine = 60;
        pistolMagazine = 20;
        _automaticGunShootTime = 0.2f;
        ammoText.gameObject.SetActive(false);
        ScrollWeaponSelection();
    }

    // Update is called once per frame
    void Update()
    {

        Aim();
        ScrollWeaponSelection();
        ShootAndReload();

        if (isAiming && selectedWeapon == 1) pistol_IK.weight = 1; else pistol_IK.weight = 0;

        if (Input.mouseScrollDelta.y < 0)
        {
            if (selectedWeapon > 0) selectedWeapon--; else selectedWeapon = weaponList.Count;
            ScrollWeaponSelection();
            AnimatorLayer();

        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            if (selectedWeapon < weaponList.Count - 1) selectedWeapon++; else selectedWeapon = 0;
            ScrollWeaponSelection();
            AnimatorLayer();


        }

        if (selectedWeapon > 0 && selectedWeapon < 3)
        {
            ammoText.gameObject.SetActive(true);
            if (selectedWeapon == 1) ammoText.text = "AMMO: " + pistolMagazine + "/ " + pistolAmmo;
            if (selectedWeapon == 2) ammoText.text = "AMMO: " + rifleMagazine + "/ " + rifleAmmo;

        }
    }

    private void AnimatorLayer()
    {
        if (_hasAnimator && isArmed) {

            switch (selectedWeapon)
            {
                case 0:
                    _animator.SetLayerWeight(1, 0f);
                    _animator.SetLayerWeight(2, 0f);
                    _animator.SetLayerWeight(3, 0f);
                    break;
                case 1:
                    _animator.SetLayerWeight(1, 0f);
                    _animator.SetLayerWeight(2, 1f);
                    _animator.SetLayerWeight(3, 0f);
                    break;
                case 2:
                    _animator.SetLayerWeight(1, 1f);
                    _animator.SetLayerWeight(2, 0f);
                    _animator.SetLayerWeight(3, 0f);
                    break;
                case 3:
                    _animator.SetLayerWeight(1, 0f);
                    _animator.SetLayerWeight(2, 0f);
                    _animator.SetLayerWeight(3, 1f);
                    break;
            }
        }
    }

    private void Aim()
    {
        Vector3 mouseWorldPos = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, hitLayer))
        {
            mouseWorldPos = raycastHit.point;
        }
        debugTransform.position = raycastHit.point;
        if (isArmed && selectedWeapon >= 1)
        {
            if (GetComponent<PlayerInputs>().aim)
            {
                //Aim point detection
                debugTransform.GetComponent<MeshRenderer>().enabled = true;
                

                //Camera blending at shoulder close up
                isAiming = true;
                aimVC.gameObject.SetActive(true);
                playerController.SetSensitivity(aimSensitivity);
                Vector3 worldAimTarget = mouseWorldPos;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
                if (Input.GetKey(KeyCode.S))
                {
                    _animator.SetBool(_animIDBackwardsMove, true);
                    movingBackwards = true;
                }
                else
                {
                    _animator.SetBool(_animIDBackwardsMove, false);
                    movingBackwards = false;
                }
            }
            else
            {
                aimVC.gameObject.SetActive(false);
                debugTransform.GetComponent<MeshRenderer>().enabled = false;
                playerController.SetSensitivity(normalSensitivity);
                isAiming = false;
                movingBackwards = false;

            }


            if (!isAiming && _hasAnimator)
            {
                _animator.SetBool(_animIDBackwardsMove, false);
            }
        }

        _animator.SetBool("aiming", isAiming);

    }

    public void ScrollWeaponSelection()
    {
        switch (selectedWeapon)
        {
            case 0:
                weaponSlot0.SetActive(false);
                weaponSlot1.SetActive(false);
                weaponSlot2.SetActive(false);
                break;

            case 1:

                weaponSlot0.SetActive(false);
                weaponSlot1.SetActive(true);
                weaponSlot2.SetActive(false);
                break;

            case 2:

                weaponSlot0.SetActive(false);
                weaponSlot1.SetActive(false);
                weaponSlot2.SetActive(true);
                break;

            case 3:

                weaponSlot0.SetActive(true);
                weaponSlot1.SetActive(false);
                weaponSlot2.SetActive(false);
                break;

        }

    }

    public void AddWeapon(GameObject weapon)
    {
        if (weaponList.Count < weaponList.Capacity)
        {
            if (_input.interact) weaponList.Add(weapon);

        }else
        {

        }
    }
    void ShootAndReload()
    {
        if (selectedWeapon == 2)
        {
            if ((Input.GetMouseButton(0)) && isAiming)
            {
                _rifleMouthLoc.LookAt(debugTransform.position);
                ammoText.text = "AMMO: " + rifleMagazine;
                if (rifleMagazine <= 0)
                {
                    if (rifleAmmo >= 60)
                    {
                        rifleMagazine = 60;
                        rifleAmmo -= 60;
                    }
                    else
                    {
                        rifleMagazine += rifleAmmo;
                        //Invocar inmovilización y trigger de recarga
                    }

                }
                else
                {
                    if (_automaticGunShootTime <= 0f)
                    {
                        _automaticGunShootTime = 0.1f;
                        rifleMagazine--;
                        rifleImpulseSource.GenerateImpulse();
                        if (_rifleMouthLoc != null)
                        {
                            shootPS.Play();
                            GameObject impacto = Instantiate(impactPSPrefab, debugTransform.position, Quaternion.identity);
                            Destroy(impacto, 2f);
                            //proyectilPS.Play();
                        }
                        //Partículas aquí

                    }
                    _automaticGunShootTime -= Time.deltaTime;

                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (rifleAmmo >= 60)
                {
                    rifleAmmo += rifleMagazine;
                    rifleMagazine = 60;
                    rifleAmmo -= 60;
                }
                else
                {
                    rifleAmmo += rifleMagazine;
                    rifleMagazine += rifleAmmo;
                }
                //Invocar inmovilización y trigger de recarga

            }
        }

        if (selectedWeapon == 1)
        {
            if ((Input.GetMouseButton(0) && isAiming))
            {
                _pistolMouthLoc.LookAt(debugTransform.position);
                ammoText.text = "AMMO: " + pistolMagazine;
                if (rifleMagazine <= 0)
                {
                    if (pistolAmmo >= 20)
                    {
                        pistolMagazine = 20;
                        pistolAmmo -= 20;
                    }
                    else
                    {
                        pistolMagazine += pistolAmmo;
                        //Invocar inmovilización y trigger de recarga
                    }

                }
                else
                {
                    if (_automaticGunShootTime <= 0f)
                    {
                        _automaticGunShootTime = 0.1f*3;
                        pistolMagazine--;
                        glockImpulseSource.GenerateImpulse();
                        if (_rifleMouthLoc != null)
                        {
                            pistol_shootPS.Play();
                            GameObject impacto = Instantiate(impactPSPrefab, debugTransform.position, Quaternion.identity);
                            Destroy(impacto, 2f);
                            //proyectilPS.Play();
                        }
                        //Partículas aquí

                    }
                    _automaticGunShootTime -= Time.deltaTime;

                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (pistolAmmo >= 20)
                {
                    pistolAmmo += pistolMagazine;
                    pistolMagazine = 20;
                    pistolAmmo -= 20;
                }
                else
                {
                    pistolAmmo += pistolMagazine;
                    pistolMagazine += pistolAmmo;
                }
                //Invocar inmovilización y trigger de recarga

            }
        }

    }

    public int GetWeapon()
    {
        return selectedWeapon;
    }
    public bool IsMovingBackwards()
    {
        return movingBackwards;
    }
    public bool GetAimStatus()
    {
        return isAiming;
    }
}
