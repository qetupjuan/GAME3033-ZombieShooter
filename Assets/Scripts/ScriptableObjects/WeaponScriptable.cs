using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon", order = 2)]
public class WeaponScriptable : EquippableScriptable
{
    public WeaponStats weaponStats;

    public override void UseItem(PlayerController playerController)
    {
        if (Equipped)
        {
            playerController.weaponHolder.UnquipWeapon();
        }
        else
        {
            playerController.weaponHolder.EquipWeapon(this);
            //PlayerEvents.InvokeOnWeaponEquipped(itemPrefab.GetComponent<WeaponComponent>());
        }

        base.UseItem(playerController);
    }
}