using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAttack : MonoBehaviour
{
    [Header("Key bindings")]
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode shotgunKey = KeyCode.Alpha1;
    public KeyCode revolverKey = KeyCode.Alpha2;
    public KeyCode whipKey = KeyCode.Alpha3;
    public KeyCode reloadKey = KeyCode.R;

    [Header("Weapons values")]
    [SerializeField] private float damageShotgun = 25f;
    [SerializeField] private float damageRevolver = 50f;
    [SerializeField] private float damageWhip = 75f;
    [SerializeField] private bool shotgunAvailable;
    [SerializeField] private bool revolverAvailable;
    [SerializeField] private bool whipAvailable;
    [SerializeField] int startWeaponID; // 0 - shotgun, 1 - revolver, 2 - whip
    private int weaponID;
    [SerializeField] private Transform shotgunPatternHelper;
    private Vector3[] shotgunVectors = new Vector3[6];
    [SerializeField] private float shotgunRange = 50f;
    [SerializeField] private float revolverRange = 80f;
    [SerializeField] private float whipRange = 7f;
    [SerializeField] private GameObject bulletImpactPrefab;

    [Header("UI")]
    [SerializeField] private GameObject shotgunArsenalIcon;
    [SerializeField] private GameObject revolverArsenalIcon;
    [SerializeField] private GameObject whipArsenalIcon;
    [SerializeField] private RectTransform whipHit;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Ammo")]
    [SerializeField] protected int magShotgun = 2;
    [SerializeField] protected int ammoShotgun = 100;
    [SerializeField] protected int magRevolver = 6;
    [SerializeField] protected int ammoRevolver = 100;

    private Animator weaponsAnim;
    private Transform cameraT;
    private LayerMask hitableLayers;
    private bool canAttack = true;
    private bool isAttacking;
    private bool isReloading;
    private float attackAnimTimeShotgun;
    private float reloadAnimTimeShotgun;
    private float attackAnimTimeRevolver;
    private float reloadAnimTimeRevolver;
    private float attackAnimTimeWhip;
    private float cooldownShotgun;
    private float cooldownRevolver;


    void Start()
    {
        cameraT = GameObject.FindGameObjectWithTag("Camera").transform;
        hitableLayers = LayerMask.GetMask("Enemy", "Ground", "Obstacle");
        // Set weapons animator and get info about its clips
        weaponsAnim = GameObject.FindGameObjectWithTag("WeaponsCanvas").GetComponent<Animator>();
        if (weaponsAnim)
        {
            AnimationClip[] clips = weaponsAnim.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                switch (clip.name)
                {
                    case "ShotgunAttack":
                        attackAnimTimeShotgun = clip.length;
                        break;
                    case "ShotgunReload":
                        reloadAnimTimeShotgun = clip.length;
                        break;
                    case "RevolverAttack":
                        attackAnimTimeRevolver = clip.length;
                        break;
                    case "RevolverReload":
                        reloadAnimTimeRevolver = clip.length;
                        break;
                    case "WhipAttack":
                        attackAnimTimeWhip = clip.length;
                        break;
                }
            }
            cooldownShotgun = attackAnimTimeShotgun + reloadAnimTimeShotgun;
            cooldownRevolver = attackAnimTimeRevolver + reloadAnimTimeRevolver;
        }
        else Debug.Log("weaponsAnim not found");

        // Pick start weapon
        switch (startWeaponID)
        {
            case 0:
                SwitchToShotgun();
                break;
            case 1:
                SwitchToRevolver();
                break;
            case 2:
                SwitchToWhip();
                break;
        }
        // toggle visibility of available arsenal
        if (shotgunAvailable)
            shotgunArsenalIcon.SetActive(true);
        else
            shotgunArsenalIcon.SetActive(false);
        if (revolverAvailable)
            revolverArsenalIcon.SetActive(true);
        else
            revolverArsenalIcon.SetActive(false);
        if (whipAvailable)
            whipArsenalIcon.SetActive(true);
        else
            whipArsenalIcon.SetActive(false);

    }

    void Update()
    {
        // Ammo check
        if (magShotgun <= 0 && weaponID == 0)
        {
            canAttack = false;
            if (ammoShotgun > 0 && !isAttacking && !isReloading)
            {
                isReloading = true;
                StartCoroutine(Reload(0f));
                StartCoroutine(ResetReload(reloadAnimTimeShotgun));
            }
        }
        if (magRevolver <= 0 && weaponID == 1)
        {
            canAttack = false;
            if (ammoRevolver > 0 && !isAttacking && !isReloading)
            {
                isReloading = true;
                StartCoroutine(Reload(0f));
                StartCoroutine(ResetReload(reloadAnimTimeRevolver));
            }
        }

        // Attack
        if (canAttack && Input.GetKeyDown(attackKey))
        {
            Attack();
        }

        // Reload
        if (weaponID == 1 && magRevolver < 6 && ammoRevolver > 0 && !isAttacking && !isReloading && Input.GetKeyDown(reloadKey))
        {
            StartCoroutine(Reload(attackAnimTimeRevolver));
            StartCoroutine(ResetReload(cooldownRevolver));
        }

        // Weapons switch
        if (!isAttacking)
        {
            if (shotgunAvailable && Input.GetKeyDown(shotgunKey))
            {
                StopAllCoroutines();
                SwitchToShotgun();
            }
            else if (revolverAvailable && Input.GetKeyDown(revolverKey))
            {
                StopAllCoroutines();
                SwitchToRevolver();
            }
            else if (whipAvailable && Input.GetKeyDown(whipKey))
            {
                StopAllCoroutines();
                SwitchToWhip();
            }
        }
    }

    void Attack()
    {
        canAttack = false;
        isAttacking = true;
        weaponsAnim.SetBool("attacking", true);

        if (weaponID == 0) // Shotgun
        {
            magShotgun -= 2;
            ammoText.text = magShotgun + "/" + ammoShotgun;
            ShotgunHitCheck();
            if (ammoShotgun > 0)
            {
                StartCoroutine(Reload(attackAnimTimeShotgun));
                StartCoroutine(ResetReload(cooldownShotgun));
            }
            else
            {
                StartCoroutine(SetAttackingToFalse(attackAnimTimeShotgun));
            }
        }
        else if (weaponID == 1) //Revolver
        {
            magRevolver -= 1;
            ammoText.text = magRevolver + "/" + ammoRevolver;
            if (magRevolver <= 0 && ammoRevolver > 0)
            {
                StartCoroutine(Reload(attackAnimTimeRevolver));
                StartCoroutine(ResetReload(cooldownRevolver));
            }
            else
            {
                StartCoroutine(SetAttackingToFalse(attackAnimTimeRevolver));
            }
        }
        else if (weaponID == 2) // Whip
        {
            whipHit.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            float scale = Random.Range(0.7f, 1f);
            whipHit.localScale = new Vector3(scale, scale, scale);
            StartCoroutine(Reload(attackAnimTimeWhip));
            StartCoroutine(ResetReload(attackAnimTimeWhip));
        }
        else Debug.Log("weaponID unknown");
    }

    IEnumerator Reload(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        isReloading = true;
        weaponsAnim.SetBool("attacking", false);
        weaponsAnim.SetBool("reloading", true);
    }

    IEnumerator ResetReload(float delay)
    {
        yield return new WaitForSeconds(delay);
        weaponsAnim.SetBool("reloading", false);
        isReloading = false;
        canAttack = true;
        if (weaponID == 0)
        {
            magShotgun += 2;
            ammoShotgun -= 2;
            ammoText.text = magShotgun + "/" + ammoShotgun;
        }
        else if (weaponID == 1)
        {
            if (magRevolver + ammoRevolver >= 6)
            {
                ammoRevolver -= (6 - magRevolver);
                magRevolver = 6;
            }
            else
            {
                magRevolver += ammoRevolver;
                ammoRevolver = 0;
            }
            ammoText.text = magRevolver + "/" + ammoRevolver;
        }
    }

    IEnumerator SetAttackingToFalse(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        weaponsAnim.SetBool("attacking", false);
        canAttack = true;
    }

    void SwitchToShotgun()
    {
        weaponID = 0;
        weaponsAnim.SetInteger("weaponID", weaponID);
        weaponsAnim.SetTrigger("switch");
        weaponsAnim.SetBool("reloading", false);
        canAttack = true;
        isReloading = false;
        ammoText.gameObject.SetActive(true);
        ammoText.text = magShotgun + "/" + ammoShotgun;
    }

    void SwitchToRevolver()
    {
        weaponID = 1;
        weaponsAnim.SetInteger("weaponID", weaponID);
        weaponsAnim.SetTrigger("switch");
        weaponsAnim.SetBool("reloading", false);
        canAttack = true;
        isReloading = false;
        ammoText.gameObject.SetActive(true);
        ammoText.text = magRevolver + "/" + ammoRevolver;
    }

    void SwitchToWhip()
    {
        weaponID = 2;
        weaponsAnim.SetInteger("weaponID", weaponID);
        weaponsAnim.SetTrigger("switch");
        weaponsAnim.SetBool("reloading", false);
        canAttack = true;
        isReloading = false;
        ammoText.gameObject.SetActive(false);
    }

    void ShotgunHitCheck()
    {
        for (int i = 0; i < 6; i++)
        {
            shotgunPatternHelper.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(-1.5f, 1.5f), 50f);
            shotgunVectors[i] = (shotgunPatternHelper.position - cameraT.position).normalized;
            Debug.DrawRay(cameraT.position, shotgunVectors[i] * shotgunRange, Color.red, 10f);

            if (Physics.Raycast(cameraT.position, shotgunVectors[i], out RaycastHit hitInfo, shotgunRange, hitableLayers))
            {
                GameObject hitObj = hitInfo.collider.gameObject;
                if (hitObj.CompareTag("Enemy"))
                {
                    Enemy enemy = hitObj.GetComponentInParent<Enemy>();
                    enemy.TakeDamage(damageShotgun);
                }
                else if (hitObj.CompareTag("Obstacle") || hitObj.CompareTag("Ground"))
                {
                    GameObject bulletImpact = Instantiate(bulletImpactPrefab, transform.position, Quaternion.LookRotation(hitInfo.normal));
                    bulletImpact.transform.position = hitInfo.point - bulletImpact.transform.worldToLocalMatrix.MultiplyVector(transform.forward * 0.001f);
                    bulletImpact.transform.Rotate(0f, 0f, Random.Range(0f, 360f));
                }
            }
        }
    }

    public void RevolverHitCheck()
    {
        Debug.DrawRay(cameraT.position, cameraT.transform.forward * revolverRange, Color.red, 10f);

        if (Physics.Raycast(cameraT.position, cameraT.transform.forward, out RaycastHit hitInfo, revolverRange, hitableLayers))
        {
            GameObject hitObj = hitInfo.collider.gameObject;
            if (hitObj.CompareTag("Enemy"))
            {
                Enemy enemy = hitObj.GetComponentInParent<Enemy>();
                enemy.TakeDamage(damageRevolver);
            }
            else if (hitObj.CompareTag("Obstacle") || hitObj.CompareTag("Ground"))
            {
                GameObject bulletImpact = Instantiate(bulletImpactPrefab, transform.position, Quaternion.LookRotation(hitInfo.normal));
                bulletImpact.transform.position = hitInfo.point - bulletImpact.transform.worldToLocalMatrix.MultiplyVector(transform.forward * 0.001f); ;
                bulletImpact.transform.Rotate(0f, 0f, Random.Range(0f, 360f));
            }
        }
    }

    public void WhipHitCheck()
    {
        Debug.DrawRay(cameraT.position, cameraT.transform.forward * whipRange, Color.red, 10f);

        if (Physics.Raycast(cameraT.position, cameraT.transform.forward, out RaycastHit hitInfo, whipRange))
        {
            GameObject hitObj = hitInfo.collider.gameObject;
            if (hitObj.CompareTag("Enemy"))
            {
                Enemy enemy = hitObj.GetComponentInParent<Enemy>();
                enemy.TakeDamage(damageWhip);
            }
        }
    }
}
