using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsInventory : MonoBehaviour
{
    public Sprite[] bigIcons;
    public Image bigIcon;
    public string[] titles;
    public Text title;
    public string[] descriptions;
    public Text description;
    public Button[] itemButtons;
    public GameObject useButton;
    public Text amtsText; // item'dan kaç tane var

    private AudioSource audioPlayer;
    public AudioClip click, select;
    private int chosenItemNumber;

    private int updateHealth;
    private float updateStamina;
    private float updateInfection;

    private bool addHealth = false;
    private bool addStamina = false;
    private bool reduceInfection = false;

    public GameObject flashlightPanel;
    public GameObject nightVisionPanel;
    private bool flashLightRefill = false;
    private bool nightVisionRefill = false;

    public GameObject electricDoorObj;
    // jeneratör çalıştırıldığında lambalar yanacak
    public GameObject electricLight1;
    public GameObject electricLight2;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        bigIcon.sprite = bigIcons[0];
        title.text = titles[0];
        description.text = descriptions[0];

        useButton.SetActive(false);

        // lambalar jeneratör çalıştırıldığında yanacak
        electricLight1.SetActive(false);
        electricLight2.SetActive(false);
    }

    private void OnEnable()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (SaveScript.itemsPickedUp[i] == false)
            {
                itemButtons[i].image.color = new Color(1, 1, 1, 0.06f);
                itemButtons[i].image.raycastTarget = false;
            }

            if (SaveScript.itemsPickedUp[i] == true)
            {
                itemButtons[i].image.color = new Color(1, 1, 1, 1f);
                itemButtons[i].image.raycastTarget = true;
            }
        }

        // Seçilen item'ın miktarı sıfırsa el feneri kullanılsın
        if (SaveScript.itemAmts[chosenItemNumber] <= 0)
        {
            ChooseItem(0);
        }

        // güncel miktarın arayüzde görüntülenmesi için
        // envanter menüsü her açıldığında seçilen item’ın miktarı güncellensin
        ChooseItem(chosenItemNumber);
    }

    // bu fonksiyon herhangi bir item'a tıklandığında çalışır
    public void ChooseItem(int itemNumber)
    {
        bigIcon.sprite = bigIcons[itemNumber];
        title.text = titles[itemNumber];
        description.text = descriptions[itemNumber];

        if (audioPlayer != null)
        {
            audioPlayer.clip = click;
            audioPlayer.Play();
        }

        chosenItemNumber = itemNumber;
        amtsText.text = "Amount: " + SaveScript.itemAmts[itemNumber]; // item'ın miktarı

        if (itemNumber < 4) // 0: el feneri, 1: gece görüşü, 2: çakmak, 3: kumaş
        {
            // el feneri, gece görüşü, çakmak ve kumaş zaten default olarak olacağından
            // use butonunun bu ögelerde olmasına gerek yok
            useButton.SetActive(false);
        }
        else
        {
            useButton.SetActive(true);
        }

        // eğer el feneri piline (8) tıklanmadıysa
        if (itemNumber != 8)
        {
            flashLightRefill = false;
        }

        // eğer gece görüş piline (9) tıklanmadıysa
        if (itemNumber != 9)
        {
            nightVisionRefill = false;
        }
    }

    public void AddHealth(int healthUpdate)
    {
        updateHealth = healthUpdate;
        addHealth = true;
    }

    public void AddStamina(int staminaUpdate)
    {
        updateStamina = staminaUpdate;
        addStamina = true;
    }

    public void ReduceInfection(int infectionUpdate)
    {
        updateInfection = infectionUpdate;
        reduceInfection = true;
    }

    // FLBatteryButton'a tıkladığımda bu fonksiyon çalışacak
    public void FillFLBattery()
    {
        flashLightRefill = true;
    }

    // NVBatteryButton'a tıkladığımda bu fonksiyon çalışacak
    public void FillNVBattery()
    {
        nightVisionRefill = true;
    }

    // kullan butonuna tıklandığında çağrılır.
    public void AssignItem()
    {
        SaveScript.itemID = chosenItemNumber;
        audioPlayer.clip = select;
        audioPlayer.Play();

        // Anahtarı alıp kullandığımda miktarı azalmamalı
        if (chosenItemNumber != 10 && chosenItemNumber != 11)
        {
            // kullan butonuna tıkladığımda seçilen item'ın miktarı bir azaltılır
            SaveScript.itemAmts[chosenItemNumber]--;
            ChooseItem(chosenItemNumber); // seçilen item’ın miktarı güncellensin

            // eğer seçilen item'ın miktarı sıfırsa
            if (SaveScript.itemAmts[chosenItemNumber] == 0)
            {
                // item yok, toplanılmadı
                SaveScript.itemsPickedUp[chosenItemNumber] = false;
                // kullan butonu devre dışı bıraktım
                useButton.SetActive(false);
            }
        }

        if (addHealth == true)
        {
            addHealth = false;
            if (SaveScript.health < 100)
            {
                SaveScript.health += updateHealth;
            }
            if (SaveScript.health > 100)
            {
                SaveScript.health = 100;
            }
        }

        if (addStamina == true)
        {
            addStamina = false;
            if (SaveScript.stamina < 100)
            {
                SaveScript.stamina += updateStamina;
            }
            if (SaveScript.stamina > 100)
            {
                SaveScript.stamina = 100;
            }
        }

        if (reduceInfection == true)
        {
            reduceInfection = true;
            if (SaveScript.infection > 0.0f)
            {
                SaveScript.infection -= updateInfection;
            }
            if (SaveScript.infection < 0.0f)
            {
                SaveScript.infection = 0.0f;
            }
        }

        if (flashLightRefill == true)
        {
            flashLightRefill = false;
            flashlightPanel.GetComponent<FlashLightScript>().batteryPower = 1.0f;
        }

        if (nightVisionRefill == true)
        {
            nightVisionRefill = false;
            nightVisionPanel.GetComponent<NightVisionScript>().batteryPower = 1.0f;
        }

        if (chosenItemNumber == 10) // ev anahtarı
        {
            if (SaveScript.doorObject != null)
            {
                // hangi kapı?
                // cabinet = 0, house = 1, cabin = 2
                if ((int)SaveScript.doorObject.GetComponent<DoorType>().chooseDoor == 1)
                {
                    // bu kapı bir evin kapısıysa
                    // kilitli mi?
                    if (SaveScript.doorObject.GetComponent<DoorType>().locked == true)
                    {
                        // kapı açılır
                        SaveScript.doorObject.GetComponent<DoorType>().locked = false;
                    }
                }
            }
        }

        if (chosenItemNumber == 11) // kabin anahtarı
        {
            if (SaveScript.doorObject != null)
            {
                // hangi kapı?
                if ((int)SaveScript.doorObject.GetComponent<DoorType>().chooseDoor == 2)
                {
                    // bu kapı kabin kapısıysa
                    // kilitli mi?
                    if (SaveScript.doorObject.GetComponent<DoorType>().locked == true)
                    {
                        // kapı açılır
                        SaveScript.doorObject.GetComponent<DoorType>().locked = false;
                    }
                }
            }
        }

        if (chosenItemNumber == 12) // yakıt
        {
            // jeneratöre bakıyor mu? jeneratör var mı?
            if (SaveScript.generator != null)
            {
                // jeneratör çalıştırılır
                SaveScript.generatorOn = true;

                SaveScript.generator.GetComponent<AudioSource>().Play();

                // yakıt jeneratör için kullanıldığında kapı açılacak
                electricDoorObj.GetComponent<DoorType>().locked = false;

                // lambalar jeneratör çalıştırıldığında yanacak
                electricLight1.SetActive(true);
                electricLight2.SetActive(true);
            }
        }
    }
}
