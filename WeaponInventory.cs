using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    public Sprite[] bigIcons;
    public Image bigIcon;
    public string[] titles;
    public Text title;
    public string[] descriptions;
    public Text description;
    public Button[] weaponButtons;
    public Text amtsText; // silahtan kaç tane var

    private AudioSource audioPlayer;
    public AudioClip click, select;
    private int chosenWeaponNumber;

    // ögeleri birleştirmek için kullanılacak:
    public GameObject useButton, combineButton;
    public GameObject combinePanel, combineUseButton;
    public Image[] combineItems; // çakmak ve kumaş vardır
    public GameObject sprayPanel; // bir sprey miktarını güncel olarak görüntülemek için

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        // ilk olarak menüde knife yani bıçak görüntülensin
        bigIcon.sprite = bigIcons[0];
        title.text = titles[0];
        description.text = descriptions[0];

        combinePanel.SetActive(false);
        combineButton.SetActive(false);
    }

    private void OnEnable() // envanter menüsü her açıldığında
    {
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            if (SaveScript.weaponsPickedUp[i] == false)
            {
                // silah toplanmamışsa buton kullanılamaz
                weaponButtons[i].image.color = new Color(1, 1, 1, 0.06f); // alfa azaltılır, görünürlük azalır
                weaponButtons[i].image.raycastTarget = false; // butona tıklanamaz
            }

            if (SaveScript.weaponsPickedUp[i] == true)
            {
                // silah toplanmışsa buton kullanılır hale getirilir
                weaponButtons[i].image.color = new Color(1, 1, 1, 1f);
                weaponButtons[i].image.raycastTarget = true; // butona tıklanabilir
            }
        }

        if (chosenWeaponNumber < 6)
        {
            combinePanel.SetActive(false);
            combineButton.SetActive(false);
        }

        // hierarchy > Canvas > InventoryMenu > ItemsMenu > LighterButton >  OnClick() Lighter = 2
        if (SaveScript.itemsPickedUp[2] == true) // çakmak toplandı mı? sahip miyiz?
        {
            combineItems[0].color = new Color(1, 1, 1, 1); // eğer sahipsek çakmak resmi alfa = 1
        }
        else if (SaveScript.itemsPickedUp[2] == false) // çakmak toplanmadıysa
        {
            combineItems[0].color = new Color(1, 1, 1, 0.06f); // resim alfa = 0.06 soluklaşır
        }

        // hierarchy > Canvas > InventoryMenu > ItemsMenu > RagsButton >  OnClick() Rags = 3
        if (SaveScript.itemsPickedUp[3] == true) // // kumaş toplandı mı? sahip miyiz?
        {
            combineItems[1].color = new Color(1, 1, 1, 1); // eğer sahipsek kumaş resmi alfa = 1
        }
        else if (SaveScript.itemsPickedUp[3] == false) // kumaş toplanmadıysa
        {
            combineItems[1].color = new Color(1, 1, 1, 0.06f); // resim alfa = 0.06 soluklaşır
        }

        // Seçilen silahın miktarı sıfırsa bıçak kullanılsın
        if (SaveScript.weaponAmts[chosenWeaponNumber] < 1)
        {
            ChooseWeapon(0);
        }

        // güncel miktarın arayüzde görüntülenmesi için
        // envanter menüsü her açıldığında seçilen silahın miktarı güncellensin
        ChooseWeapon(chosenWeaponNumber);
    }

    public void ChooseWeapon(int weaponNumber)
    {
        bigIcon.sprite = bigIcons[weaponNumber];
        title.text = titles[weaponNumber];
        description.text = descriptions[weaponNumber];
        if (audioPlayer != null)
        {
            audioPlayer.clip = click;
            audioPlayer.Play();
        }
        chosenWeaponNumber = weaponNumber;
        amtsText.text = "Amount: " + SaveScript.weaponAmts[weaponNumber]; // miktar

        if (chosenWeaponNumber > 5) // 6(spray) ve 7(bottle)
        {
            combineButton.SetActive(true);
            combinePanel.SetActive(false); // her silah seçtiğimizde combine panelin kapanması gerekiyor
        }

        if (chosenWeaponNumber < 6)
        {
            combinePanel.SetActive(false); // her silah seçtiğimizde combine panelin kapanması gerekiyor
            combineButton.SetActive(false);
        }

        if (chosenWeaponNumber == 6)
        {
            // sprey için use düğmesini devre dışı bırakacağım
            useButton.SetActive(false);
        }
        else
        {
            useButton.SetActive(true);
        }
    }

    public void CombineAction()
    {
        // bu fonksiyon CombineButton'a tıklandığında çalışacak (Inspector'de tanımlanmıştır)
        combinePanel.SetActive(true); //combine bölümü açılır

        // hierarchy > Canvas > InventoryMenu > WeaponMenu > SprayButton > OnClick() > spray = 6
        if (chosenWeaponNumber == 6) // sprey seçildi mi?
        {
            // Spreyi sadece çakmak ile birleştirebildiğimiz için combine panelinde kumaşı görmemize gerek yok
            // hierarchy > Canvas > InventoryMenu > WeaponMenu
            combineItems[1].transform.gameObject.SetActive(false);

            // sprey için sadece çakmağa ihtiyaç var
            if (SaveScript.itemsPickedUp[2] == true)
            {
                combineUseButton.SetActive(true);
            }
            else if (SaveScript.itemsPickedUp[2] == false)
            {
                combineUseButton.SetActive(false);
            }
        }

        // hierarchy > Canvas > InventoryMenu > WeaponMenu > BottleButton > OnClick() > bottle = 7
        if (chosenWeaponNumber == 7) // şişe seçildi mi?
        {
            combineItems[1].transform.gameObject.SetActive(true); // kumaş şişe içi gerekli

            // bottle(şişe) için çakmak ve kumaşa ihtiyaç var
            if (SaveScript.itemsPickedUp[2] == true && SaveScript.itemsPickedUp[3] == true)
            {
                combineUseButton.SetActive(true);
            }
            else if (SaveScript.itemsPickedUp[2] == false || SaveScript.itemsPickedUp[3] == false)
            {
                combineUseButton.SetActive(false);
            }
        }
    }

    // combine panelindeki use butonuna tıklandığında bu fonksiyon çağrılacak
    public void CombineAssignWeapon() // UseButtonCombine
    {
        if (chosenWeaponNumber == 6) // seçilen silah spray ise
        {
            SaveScript.weaponID = chosenWeaponNumber;

            // bu sprey tamamen bittiyse ve yeniden sprey seçilirse yeni sprey tamamen dolu olacaktır.
            if (sprayPanel.GetComponent<SprayScript>().sprayAmount <= 0.0f)
            {
                // bir spreyin tamamen dolu olduğunu gösterir
                sprayPanel.GetComponent<SprayScript>().sprayAmount = 1.0f;
            }
        }

        if (chosenWeaponNumber == 7) // seçilen silah bottle ise
        {
            chosenWeaponNumber += 1; // 8. indeksteki silah olan bottleWithCloth kullanılsın
            SaveScript.weaponID = chosenWeaponNumber;
            // kumaş ve çakmak alınmıştır ve bottle seçilmiştir
            // birleştirirsek bottleWithCloth oluşturulur
        }

        audioPlayer.clip = select;
        audioPlayer.Play();
    }

    public void AssignWeapon() // UseButton
    {
        SaveScript.weaponID = chosenWeaponNumber;
        audioPlayer.clip = select;
        audioPlayer.Play();
    }
}

/*
knife, // 0
cleaver, // 1
bat, // 2
axe, // 3
pistol, // 4
shotgun, // 5
sprayCan, // 6
bottle // 7
*/
