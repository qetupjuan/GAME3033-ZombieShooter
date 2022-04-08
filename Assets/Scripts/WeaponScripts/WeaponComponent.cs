using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None, Pistol, MachineGun
}

public enum WeaponFiringPattern
{
    SemiAuto, FullAuto, ThreeShotBurst, FiveShotBurst
}

[System.Serializable]
public struct WeaponStats
{
    public WeaponType weaponType;
    public string weaponName;
    public float damage;
    public int bulletsInClip;
    public int clipSize;
    public float fireStartDelay;
    public float fireRate;
    public WeaponFiringPattern weaponFiringPattern;
    public float fireDistance;
    public bool repeating;
    public LayerMask weaponHitLayers;
    public int totalBullets;

}

public class WeaponComponent : MonoBehaviour
{
    public Transform gripLocation;
    public Transform firingEffectLocation;

    protected WeaponHolder weaponHolder;
    [SerializeField]
    protected ParticleSystem firingEffect;

    [SerializeField]
    public WeaponStats weaponStats;

    public bool isFiring = false;
    public bool isReloading = false;
    protected Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        firingEffect.transform.parent = firingEffectLocation;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(WeaponHolder _weaponHolder, WeaponScriptable weaponScriptable)
    {
        weaponHolder = _weaponHolder;
        if (weaponScriptable)
        {
            weaponStats = weaponScriptable.weaponStats;
        }
    }

    public virtual void StartFiringWeapon()
    {
        isFiring = true;
        if(weaponStats.repeating)
        {
            InvokeRepeating(nameof(FireWeapon), weaponStats.fireStartDelay, weaponStats.fireRate);
        }
        else
        {
            FireWeapon();
        }
    }

    public virtual void StopFiringWeapon()
    {
        isFiring = false;
        CancelInvoke(nameof(FireWeapon));
        if(firingEffect.isPlaying)
        {
            firingEffect.Stop();
        }
    }

    protected virtual void FireWeapon()
    {
        print("Firing weapon!");
        weaponStats.bulletsInClip--;
    }

    public virtual void StartReloading()
    {
        isReloading = true;
        ReloadWeapon();
    }   
    
    public virtual void StopReloading()
    {
        isReloading = false;
        if (firingEffect.isPlaying)
        {
            firingEffect.Stop();
        }
    }

    protected virtual void ReloadWeapon()
    {
        //if (bulletsToReload < 0)
        //{
        //    weaponStats.totalBullets -= weaponStats.bulletsInClip;
        //    weaponStats.bulletsInClip = weaponStats.clipSize;
        //}
        //else
        //{
        //    weaponStats.bulletsInClip = weaponStats.totalBullets;
        //    weaponStats.totalBullets = 0;
        //}
        int bulletsToReload = weaponStats.totalBullets - (weaponStats.clipSize - weaponStats.bulletsInClip);

        if (bulletsToReload > 0)
        {
            weaponStats.totalBullets = bulletsToReload;
            weaponStats.bulletsInClip = weaponStats.clipSize;
        }
        else
        {
            weaponStats.bulletsInClip += weaponStats.totalBullets;
            weaponStats.totalBullets = 0;
        }
    }
}
