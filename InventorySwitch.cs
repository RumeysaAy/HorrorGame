using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySwitch : MonoBehaviour
{
    public GameObject weaponPanel, itemsPanel, combinePanel;

    // Start is called before the first frame update
    void Start()
    {
        // Inventory'i açtığımızda weapon paneli açılsın
        weaponPanel.SetActive(true);
        itemsPanel.SetActive(false);
        combinePanel.SetActive(false);
    }

    public void SwitchItemsOn()
    {
        itemsPanel.SetActive(true);
        weaponPanel.SetActive(false);
        // Items panelini açtığımızda birleştirme paneli gözükmesin
        combinePanel.SetActive(false);
    }

    public void SwitchWeaponsOn()
    {
        weaponPanel.SetActive(true);
        itemsPanel.SetActive(false);
        // Weapon panelini açtığımızda birleştirme paneli gözükmesin
        combinePanel.SetActive(false);
    }
}
