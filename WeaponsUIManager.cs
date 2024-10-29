using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsUIManager : MonoBehaviour
{
    public GameObject pistolPanel, shotgunPanel, sprayPanel;
    public Text pistolTotalAmmo, pistolCurrentAmmo;
    public Text shotgunTotalAmmo, shotgunCurrentAmmo;
    private bool panelOn = false;

    // Start is called before the first frame update
    void Start()
    {
        pistolPanel.SetActive(false);
        shotgunPanel.SetActive(false);
        sprayPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveScript.weaponID == 4) // eğer seçilen silah tabancaysa
        {
            if (panelOn == false) // panel kapalıysa
            {
                panelOn = true; // panel açılsın

                // tabanca paneli gözüksün
                pistolPanel.SetActive(true);
            }
        }

        if (SaveScript.weaponID == 5) // eğer seçilen silah tüfekse
        {
            if (panelOn == false) // panel kapalıysa
            {
                panelOn = true; // panel açılsın

                // tüfek paneli gözüksün
                shotgunPanel.SetActive(true);
            }
        }

        // Spreyi kullandığımda sprey paneli açılsın
        if (SaveScript.weaponID == 6) // eğer seçilen silah spreyse
        {
            if (panelOn == false) // panel kapalıysa
            {
                panelOn = true; // panel açılsın

                // sprey paneli gözüksün
                sprayPanel.SetActive(true);
            }
        }

        if (SaveScript.inventoryOpen == true)
        {
            // inventory paneli açıksa hepsini kapat
            pistolPanel.SetActive(false);
            shotgunPanel.SetActive(false);
            sprayPanel.SetActive(false);

            panelOn = false; // panel kapalı
        }
    }

    // Update fonksiyonundan 4 kat daha hızlı çalışır
    // kullanıcı arayüzü nesnelerini güncellemek için
    private void OnGUI()
    {
        pistolTotalAmmo.text = SaveScript.ammoAmts[0].ToString();
        shotgunTotalAmmo.text = SaveScript.ammoAmts[1].ToString();
        pistolCurrentAmmo.text = SaveScript.currentAmmo[4].ToString();
        shotgunCurrentAmmo.text = SaveScript.currentAmmo[5].ToString();
    }
}
