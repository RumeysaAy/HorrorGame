using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
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


    public weaponSelect chosenWeapon;
    public GameObject[] weapons; // weapon prefab
    // private int weaponID = 0; // UI'den silahı değiştirmek için gerekli. SaveScript.cs dosyasından çekilir
    private Animator anim;
    private AudioSource audioPlayer;
    public AudioClip[] weaponSounds;
    private int currentWeaponID;
    private bool spraySoundOn = false; // spray kullanıldığı sırada ses efektini açmak için
    public GameObject sprayPanel; // sprey bittiyse kullanılmasını engelleyeceğim
    public static bool emptyBottleThrow = false; // boş şişeyi fırlatmak için bunu çağıracağım
    public static bool fireBottleThrow = false; // molotof kokteylini fırlatmak için bunu çağıracağım

    // bir şişe atma işlemi tamamlanmadan herhangi bir saldırıyı engellemek için
    private AnimatorStateInfo animInfo;
    private bool canAttack = true;

    private bool sprayEmpty = false; // eğer bir sprey boşsa yani bitmişse
    private bool stopSpray = false; // spreyi kullanmayı durdurmak için kullanacağım

    public AudioClip[] reloadSounds; // pistol ve shotgun için mermi yükleme ses efekti

    // Start is called before the first frame update
    void Start()
    {
        SaveScript.weaponID = (int)chosenWeapon;
        anim = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        ChangeWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        // oyun duraklatılmamışsa
        if (Time.timeScale == 1.0f)
        {
            /*
            if (Input.GetKeyDown(KeyCode.X)) // x'e tıklayarak silahları değiştirebiliriz
            {
                if (SaveScript.weaponID < weapons.Length - 1)
                {
                    SaveScript.weaponID++;
                    ChangeWeapons();
                }
            }

            if (Input.GetKeyDown(KeyCode.Z)) // z'ye tıklayarak silahları değiştirebiliriz
            {
                if (weaponID > 0)
                {
                    weaponID--;
                    ChangeWeapons();
                }
            }*/

            animInfo = anim.GetCurrentAnimatorStateInfo(0); // animatörün 0. katmanı BaseLayer
            if (animInfo.IsTag("BottleThrown")) // bu etiketi kullanan animasyonları algılayacak
            {
                // Eğer tag’ı BottleThrown olan durum oynatılıyorsa
                canAttack = false;
            }
            else
            {
                // bir şişe atma işlemi gerçekleşmiyorsa
                canAttack = true;
            }

            if (SaveScript.weaponID != currentWeaponID)
            {
                ChangeWeapons();
            }

            // şişe fırlatılan bir animasyon oynatılmadığı sürece yeni bir saldırı gerçekleşebilir
            // bir şişe atma işlemi tamamlanmadan oyuncu tarafından herhangi bir saldırının engellenmesi için
            if (Input.GetMouseButtonDown(0) && canAttack == true) // sol fare tuşu
            {
                if (SaveScript.inventoryOpen == false)
                {
                    // SaveScript.weaponID -> şu anda kullanılan silah
                    // dayanıklılık 20'den fazlaysa silah kullanılabilir, saldırabilir
                    if (SaveScript.currentAmmo[SaveScript.weaponID] > 0 && SaveScript.stamina > 20) // kullanılan silahın içerisindeki mermi sayısı > 0
                    {
                        // kullanılan silahın içerisinde mermi varsa saldırsın
                        anim.SetTrigger("Attack"); // saldırı animasyonu
                        audioPlayer.clip = weaponSounds[SaveScript.weaponID]; // ses
                        audioPlayer.Play();

                        // yakın dövüş silahlarının mermisi olmadığından mermi sayısını azaltmıyorum

                        // kullanılan silah tabanca veya tüfek ise
                        if (SaveScript.weaponID == 4 || SaveScript.weaponID == 5)
                        {
                            // saldırıda mermi kullanıldığı için kullanılan silahın içerisindeki mermi sayısı azaltılır
                            SaveScript.currentAmmo[SaveScript.weaponID]--;
                            // oyuncu 4 ve 5 numaralı silahları kullandığı için zombiler sesten etkilenecek ve oyuncuya doğru gelecekler
                            SaveScript.gunUsed = true;
                        }
                    }
                    else
                    {
                        // kullanılan silah tabanca veya tüfek ise
                        if (SaveScript.weaponID == 4 || SaveScript.weaponID == 5)
                        {
                            // eğer şu anda kullanılan tabancanın veya tüfeğin mermisi bittiyse
                            // ses efektini değiştireceğim
                            audioPlayer.clip = weaponSounds[9];
                            audioPlayer.Play();
                        }
                    }
                }
            }

            // eğer sol tuşa basılı tutuluyorsa ve sprey bitmediyse
            if (Input.GetMouseButton(0) && sprayPanel.GetComponent<SprayScript>().sprayAmount > 0.0f)
            {
                sprayEmpty = false; // bir sprey bitmemiş
                stopSpray = false; // bir sprey kullanılıyor

                // eğer sprey kullanılıyorsa
                if (SaveScript.weaponID == 6 && SaveScript.inventoryOpen == false)
                {
                    if (spraySoundOn == false) // spray action durumuna her karede geçilmemesi için
                    {
                        // saldırı animasyonu sadece bir defa oynatılmış olacak
                        spraySoundOn = true;

                        // Attack parametresi ile spray action durumuna geçilir.
                        anim.SetTrigger("Attack");

                        // ses
                        StartCoroutine(StartSpraySound());
                    }
                }
            }

            // eğer sol tuşa basılı tutmayı bıraktığım anda veya bir sprey bittiyse
            if (Input.GetMouseButtonUp(0) || sprayPanel.GetComponent<SprayScript>().sprayAmount <= 0.0f)
            {
                // eğer sprey kullanılıyorsa
                if (SaveScript.weaponID == 6 && SaveScript.inventoryOpen == false && stopSpray == false)
                {
                    stopSpray = true; // bir sprey kullanılmıyor (bittiği için)

                    // Release parametresi ile spray return durumuna geçilir
                    anim.SetTrigger("Release");

                    // saldırı durumundan yani spray action durumundan çıktığı için
                    spraySoundOn = false;

                    // Spreyi kullanmadığım zaman yani sola basılı tutmayı bıraktığım an ses dursun
                    audioPlayer.Stop();
                    audioPlayer.loop = false;
                }
            }

            // bir sprey miktarı sıfır mı?
            if (sprayPanel.GetComponent<SprayScript>().sprayAmount <= 0.0f && sprayEmpty == false)
            {
                sprayEmpty = true; // bir sprey bitmiştir
                SaveScript.weaponAmts[6]--; // bir sprey azaltılır
                if (SaveScript.weaponAmts[6] == 0) // eğer hiç sprey kalmadıysa
                {
                    // envanter menüsünde sprey gözükmesin
                    SaveScript.weaponsPickedUp[6] = false;
                }
            }

            // eğer kullanılan silah pistol veya shotgun ise
            if (SaveScript.weaponID == 4 || SaveScript.weaponID == 5)
            {
                // sağ fare tuşuna basıldı mı?
                if (Input.GetMouseButtonDown(1))
                {
                    // yedekte mermi varsa
                    if (SaveScript.ammoAmts[SaveScript.weaponID - 4] > 0)
                    {
                        // kullanılan silahın yedekteki mermilerinin yüklenmesi
                        SaveScript.currentAmmo[SaveScript.weaponID] += SaveScript.ammoAmts[SaveScript.weaponID - 4];
                        SaveScript.ammoAmts[SaveScript.weaponID - 4] = 0; // yedekteki mermiler yüklendiğinden sıfırlanır
                        anim.SetTrigger("Reload");
                        // pistol: reloadSounds[0], shotgun: reloadSounds[1]
                        audioPlayer.clip = reloadSounds[SaveScript.weaponID - 4];
                        audioPlayer.Play();
                    }
                }
            }
        }
    }

    private void ChangeWeapons()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        weapons[SaveScript.weaponID].SetActive(true); // o anda seçili olan silahı açmak istiyoruz.
        chosenWeapon = (weaponSelect)SaveScript.weaponID;
        anim.SetInteger("WeaponID", SaveScript.weaponID);
        anim.SetBool("weaponChanged", true); // sürekli olarak çağrılmasını engellemek ve animasyonu bitirmesini sağlamak için
        currentWeaponID = SaveScript.weaponID;

        Move();
        StartCoroutine(WeaponReset());
    }

    private void Move()
    {
        // shotgun seçildiğinde arms@knife Z konumu 0.46 olmalı.
        // Default olarak 0.66 olmalı.
        switch (chosenWeapon)
        {
            case weaponSelect.knife:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponSelect.cleaver:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponSelect.bat:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponSelect.axe:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponSelect.pistol:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponSelect.shotgun:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.46f);
                break;
            case weaponSelect.sprayCan:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponSelect.bottle:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
        }
    }

    // Boş şişe fırlatma (ThrowingEmptyBottle) animasyonunun 2.saniyesinde BottleThrowEmpty() fonksiyonu çalıştırılacak.
    public void BottleThrowEmpty()
    {
        // boş şişe atılsın
        emptyBottleThrow = true; // WeaponManager’daki if bloğu çalıştırılır
    }

    public void BottleThrowFire()
    {
        // molotof kokteyli atılsın
        fireBottleThrow = true;
    }

    // var olan diğer bir şişeyi kullanmak için
    public void LoadAnotherBottle()
    {
        if (SaveScript.weaponID == 7)
        {
            ChangeWeapons();
        }
    }

    // var olan diğer bir molotof kokteylini kullanmak için
    // UseButtonCombine düğmesine basıldığında bu fonksiyon çağrılacak
    public void LoadAnotherFireBottle()
    {
        if (SaveScript.weaponID == 8)
        {
            ChangeWeapons();
        }
    }

    IEnumerator WeaponReset()
    {
        yield return new WaitForSeconds(0.5f); // animasyonu bitirmesi için gereken zaman
        anim.SetBool("weaponChanged", false);
    }

    IEnumerator StartSpraySound()
    {
        yield return new WaitForSeconds(0.3f);
        audioPlayer.clip = weaponSounds[SaveScript.weaponID];
        audioPlayer.Play();
        audioPlayer.loop = true;
    }
}

/*
Knife 0
Cleaver 1
BaseballBat 2
HRR_Axe_01(balta) 3
Pistol 4
Shotgun 5
sprayCan 6
bottle 7

WeaponID ikiden az olduğu sürece, yani sıfır veya bir olduğu sürece knife idle animasyon oynatılacak

WeaponID > 1  veya WeaponID < 6 ise bat idle (2 veya 3)

Weapon ID'si 4'e ulaştığında pistol idle oynamasını istiyoruz

5 olursa shotgun idle oynamasını istiyoruz.

6 ise spray idle

7 ise bottle idle
*/
