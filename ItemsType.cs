using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsType : MonoBehaviour
{
    public enum typeOfItem
    {
        flashlight,
        nightVision,
        lighter,
        rags,
        healthPack,
        pills,
        waterBottle,
        apple,
        flashlightBattery,
        nightVisionBattery,
        houseKey,
        cabinKey,
        jerryCan
    }

    public typeOfItem chooseItem;
}
