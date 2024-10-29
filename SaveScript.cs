using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SaveScript : MonoBehaviour
{
    public static bool inventoryOpen = false;
    public static int weaponID = 0; // WeaponInventory.cs dosyasının erişmesi gerekiyor. UI'den silahı değiştirmek için gerekli
    public static bool[] weaponsPickedUp = new bool[9]; // toplanan/sahip olunan silahlar (hepsi default false)
    public static int itemID = 0;
    public static bool[] itemsPickedUp = new bool[13];
    public static int[] weaponAmts = new int[9]; // her silahtan kaç tane var?
    public static int[] itemAmts = new int[13]; // her item'dan kaç tane var?
    public static int[] ammoAmts = new int[2]; // her mermiden kaç tane var?
    public static bool change = false; // eğer silah toplandıysa true olur
    public static bool isHidden = false; // oyuncu duvarın arkasında saklanıyor mu?
    public static int[] currentAmmo = new int[9]; // O andaki tabancanın ve tüfeğin içerisindeki mermi sayısı
    // Toplamda 9 silah var, bunlar arasında şişe ve kumaş da bulunuyor
    // 4'te tabanca ve 5'te tüfek bulunuyor

    public static float stamina; // dayanıklılık
    public static float infection; // enfeksiyon
    public static int health; // sağlık
    public static GameObject doorObject;

    public static bool gunUsed = false;

    public static Vector3 bottlePos = new Vector3(0, 0, 0); // boş şişenin düştüğü yer
    private bool hasSmashed = false; // şişe parçalandı mı?

    // oyuncuyu takip eden zombiler
    public static List<GameObject> zombiesChasing = new List<GameObject>();

    public static int zombiesInGameAmt = 0; // oyunda kaç zombinin ortaya çıktığını takip edecek

    public static bool generatorOn = false; // Jeneratör açık mı?
    public static GameObject generator; // Jeneratör nesnesi
    public static bool gotVaccine = false; // aşı alındı mı?
    public static GameObject vaccine; // aşı nesnesi

    private GameObject[] zombies; // oyunda var olan zombiler

    public GameObject zombieMessage, deathMessage;

    // Start is called before the first frame update
    void Start()
    {
        stamina = FirstPersonController.FPSstamina;
        health = 100;
        infection = 0;
        inventoryOpen = false;
        weaponID = 0;
        itemID = 0;
        stamina = 100;
        generatorOn = false;
        gotVaccine = false;

        // bıçak her zaman olacak
        weaponsPickedUp[0] = true; // 1. silah true yani 1. silaha sahibim (knife)

        itemsPickedUp[0] = true; // el feneri toplandı mı?
        itemsPickedUp[1] = true; // gece görüşü toplandı mı?

        itemAmts[0] = 1; // el fenerinden 1 tane var
        itemAmts[1] = 1; // gece görüş gözlüğünden 1 tane var
        weaponAmts[0] = 1; // bıçaktan 1 tane var
        ammoAmts[0] = 12; // pistolAmmo'dan 12 tane var
        ammoAmts[1] = 2; // shotgunAmmo'dan 2 tane var

        // ilk başladığımda mermim olmadığından olayı sıfıra eşitledim
        for (int i = 0; i < currentAmmo.Length; i++)
        {
            // yakın dövüş silahlarının mermisi olmaz
            currentAmmo[i] = 2;
        }

        currentAmmo[4] = 12; // tabancanın içerisindeki mermi sayısı

        // sadece sol tuşa basılı tuttuğumda çalışması için spreyin mermi sayısını sıfırladım
        currentAmmo[6] = 0;

        deathMessage.SetActive(false);
        zombieMessage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // oyun içindeki toplam zombi miktarı 120'den fazla mı?
        if (zombiesInGameAmt > 120)
        {
            // oyundaki bütün zombiler
            zombies = GameObject.FindGameObjectsWithTag("zombie");
            // 120'den fazla olan zombiler
            for (int i = 120; i < zombies.Length; i++)
            {
                // yok edilecek
                Destroy(zombies[i]);
            }
        }

        // eğer oyundaki zombi sayısı 0'dan azsa
        if (zombiesInGameAmt < 0)
        {
            zombiesInGameAmt = 0;
        }

        if (FirstPersonController.inventorySwitchedOn == true)
        {
            inventoryOpen = true;

        }
        else if (FirstPersonController.inventorySwitchedOn == false)
        {
            inventoryOpen = false;
        }

        // sol Shift tuşu basılıysa ve yukarı-aşağı yön tuşuna basıyorsa yani koşuyorsa
        // dayanıklılık sıfırın altına inmemeli
        if (Input.GetAxis("Vertical") != 0 && Input.GetKey(KeyCode.LeftShift) && FirstPersonController.FPSstamina > 0.0f)
        {
            // koştukça dayanıklılığı azalacak
            FirstPersonController.FPSstamina -= 10 * Time.deltaTime;
            stamina = FirstPersonController.FPSstamina;
        }

        if (stamina < 100)
        {
            // dayanıklılık 100'den azsa zaman içerisinde artacak
            FirstPersonController.FPSstamina += 3.35f * Time.deltaTime;
            stamina = FirstPersonController.FPSstamina;
        }

        if (stamina >= 100)
        {
            FirstPersonController.FPSstamina = stamina;
        }

        // farenin sol tuşuna basıldığında yani silah kullanırken dayanıklılık azalacak
        // 4 ve 4'ten sonraki silahlar için dayanıklılık azalmasın
        if (Input.GetMouseButtonDown(0) && stamina > 10 && weaponID < 4 && inventoryOpen == false)
        {
            FirstPersonController.FPSstamina -= 10;
            stamina = FirstPersonController.FPSstamina;
        }

        // eğer C'ye basılı tutarsa dayanıklılığı azalacak
        if (Input.GetKey(KeyCode.C))
        {
            FirstPersonController.FPSstamina -= 10f * Time.deltaTime;
            stamina = FirstPersonController.FPSstamina;
        }

        // %50’ye kadar yavaş artacak, %50’den sonra hızla artacak
        if (infection < 50)
        {
            infection += 0.1f * Time.deltaTime;
        }
        if (infection > 49 && infection < 100)
        {
            infection += 0.4f * Time.deltaTime;
        }

        if (change == true) // silah veya item toplandıysa
        {
            change = false;

            for (int i = 1; i < weaponAmts.Length; i++)
            {
                if (weaponAmts[i] > 0)
                {
                    // i. indeksteki silahın i. indeksteki değeri eğer 0'dan büyükse toplanmıştır.
                    weaponsPickedUp[i] = true; // i. indeksteki silah
                }
                else if (weaponAmts[i] == 0)
                {
                    // i. indeksteki silah hiç toplanmamışsa
                    weaponsPickedUp[i] = false;
                }
            }

            for (int i = 2; i < itemAmts.Length; i++)
            {
                if (itemAmts[i] > 0)
                {
                    // i. indeksteki item'ın i. indeksteki değeri eğer 0'dan büyükse toplanmıştır.
                    itemsPickedUp[i] = true; // i. indeksteki item
                }
                else if (itemAmts[i] == 0)
                {
                    // i. indeksteki item hiç toplanmamışsa
                    itemsPickedUp[i] = false;
                }
            }
        }

        // boş şişe atıldıysa
        if (bottlePos != Vector3.zero)
        {
            if (hasSmashed == false)
            {
                StartCoroutine(ResetBottlePos());
                hasSmashed = true; // boş şişe kırıldı
            }
        }

        if (health <= 0)
        {
            // oyuncu öldüyse ekranda ölüm mesajı gösterilir
            deathMessage.SetActive(true);
            StartCoroutine(PauseTime());
        }

        if (infection >= 100)
        {
            // oyuncu zombiye dönüşmüş olur
            zombieMessage.SetActive(true);
            StartCoroutine(PauseTime());
        }
    }

    IEnumerator PauseTime()
    {
        // Animasyon için zaman
        yield return new WaitForSeconds(3.3f);
        Time.timeScale = 0.0f;
    }

    IEnumerator ResetBottlePos()
    {
        // 30 saniye sonra boş şişenin konumu sıfırlanacak
        yield return new WaitForSeconds(30);
        bottlePos = Vector3.zero;
        hasSmashed = false; // bir sonraki şişe için
    }
}

/*
WeaponManager.cs

    // arms@knife > Weapons
    public enum weaponSelect
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
*/
