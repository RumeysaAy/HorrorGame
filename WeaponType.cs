using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
    // hangi silahın hangisi olduğunu söylememizi sağlayacak bir açılır liste
    public enum typeOfWeapon
    {
        knife, // 0
        cleaver, // 1
        bat, // 2
        axe, // 3
        pistol, // 4
        shotgun, // 5
        sprayCan, // 6
        bottle, // 7
        bottleWithCloth // 8
    }

    public typeOfWeapon chooseWeapon;
}
