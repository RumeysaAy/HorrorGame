using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// FirstPersonCharacter nesnesinde bulunur
public class PickupsScript : MonoBehaviour
{
    // vurulan her şeyi kaydetmemiz gerekiyor
    private RaycastHit hit; // vurulan herhangi bir nesneyi depolayacak
    public LayerMask excludeLayers; // seçilen Layer'lar görmezden gelinir. sadece pickups layer'ını seçmedim. seçilen katmanlardaki nesneler algılanmayacak
    public GameObject pickupPanel; // baktığımız silahı gösteren panel
    public float pickupDisplayDistance = 8f;

    public Image mainImage; // ışının çarptığı silahın resminin koyulacağı yer
    public Sprite[] weaponIcons; // silahların resmi
    public Sprite[] itemIcons; // item'ların resmi
    public Sprite[] ammoIcons; // mermilerin resmi
    public Text mainTitle; // ışının çarptığı silahın adının koyulacağı yer
    public string[] weaponTitles; // silahların adı
    public string[] itemTitles; //item'ların adı
    public string[] ammoTitles; // mermilerin ismi

    private int objID = 0; // hangi silah türüne vurduğumuza bağlı değişecek (WeaponType.cs)
    private AudioSource audioPlayer;
    public GameObject doorMessageObj; // mesajın görüntülenebilmesi için
    public GameObject generatorMessageObj;
    public GameObject vaccineMessageObj;
    public Text doorMessage; // e'ye tıklandığında kapının açılabileceğine dair mesaj
    public AudioClip[] pickupSounds;

    // pistol için küre atışı yapacağım
    private RaycastHit gunHit; // çarpıştığı nesne
    // shotgun için küre atışı yapacağım
    private RaycastHit[] shotgunHits; // çarpıştığı nesneler
    // shotgun silahının aynı anda birden fazla zombi öldürebilmesini istiyorum

    // Start is called before the first frame update
    void Start()
    {
        pickupPanel.SetActive(false);
        audioPlayer = GetComponent<AudioSource>();
        doorMessageObj.SetActive(false);
        generatorMessageObj.SetActive(false);
        vaccineMessageObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // oyuncunun bir şeye çarpıp çarpmadığını tespit etmek için sürekli olarak bu ışını dünyaya fırlatmasını istiyoruz
        // ışının sonunda, çizginin sonunda ve herhangi bir yerde bir küre çizer. o kürenin çarptığı şey daha sonra tespit edilir.
        // oyuncunun konumu, yarıçap, dünyada ileriye doğru hareket etsin, ne vuruldu?, max ne kadar ileri gitsin?, seçilen katman dışındakiler(~)
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 30, ~excludeLayers)) // Pickups Layer'ındaki bir nesneye çarpıp çarpmadığını tespit edelim.
        {
            // oyuncunun konumu ile vurulan nesnenin konumu arasındaki mesafe 8'den küçükse ise panel açılsın
            if (Vector3.Distance(transform.position, hit.transform.position) < pickupDisplayDistance)
            {
                // yalnızca weapon etiketi olan nesneleri tespit ettiğimden emin olmak istiyorum.
                if (hit.transform.gameObject.CompareTag("weapon")) // yalnızca bir silahsa
                {
                    // ışının çarptığı nesne silahsa panel açılsın
                    pickupPanel.SetActive(true);

                    // hangi silahı işaret ettiğimizi tespit edebilmek için WeaponType.cs dosyasını kullanacağız.
                    objID = (int)hit.transform.gameObject.GetComponent<WeaponType>().chooseWeapon;
                    // ışının çarptığı silaha bağlı olarak başlığın ve görselin değişmesi için
                    mainImage.sprite = weaponIcons[objID];
                    mainTitle.text = weaponTitles[objID];

                    // e'ye bastığımızda silahı alacağız ve SaveScript.cs dosyasına silaha sahip olduğumuzu kaydedeceğiz
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // hangi silah alınmışsa o silahın indeksindeki değer 1 olur
                        SaveScript.weaponAmts[objID]++;

                        audioPlayer.clip = pickupSounds[3];

                        audioPlayer.Play(); // silah alındığında ses oynatılacak

                        SaveScript.change = true; // silah toplandığı için

                        // silahı aldığımız için yok edeceğiz
                        Destroy(hit.transform.gameObject, 0.2f);
                    }
                }
                // yalnızca item etiketi olan nesneleri tespit ettiğimden emin olmak istiyorum.
                else if (hit.transform.gameObject.CompareTag("item")) // yalnızca bir item ise
                {
                    // ışının çarptığı nesne item ise panel açılsın
                    pickupPanel.SetActive(true);

                    // hangi item'ı işaret ettiğimizi tespit edebilmek için ItemsType.cs dosyasını kullanacağız.
                    objID = (int)hit.transform.gameObject.GetComponent<ItemsType>().chooseItem;
                    // ışının çarptığı item'a bağlı olarak başlığın ve görselin değişmesi için
                    mainImage.sprite = itemIcons[objID];
                    mainTitle.text = itemTitles[objID];


                    // e'ye bastığımızda item'ı alacağız ve SaveScript.cs dosyasına item'a sahip olduğumuzu kaydedeceğiz
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // hangi item alınmışsa o item'ın indeksindeki değer 1 olur
                        SaveScript.itemAmts[objID]++;

                        audioPlayer.clip = pickupSounds[3];

                        audioPlayer.Play(); // item alındığında ses oynatılacak

                        SaveScript.change = true; // item toplandığı için

                        // item'ı aldığımız için yok edeceğiz
                        Destroy(hit.transform.gameObject, 0.2f);
                    }
                }
                // yalnızca ammo etiketi olan nesneleri tespit ettiğimden emin olmak istiyorum.
                else if (hit.transform.gameObject.CompareTag("ammo")) // yalnızca bir ammo ise
                {
                    // ışının çarptığı nesne ammo ise panel açılsın (cephane/mermi)
                    pickupPanel.SetActive(true);

                    // hangi cephaneyi işaret ettiğimizi tespit edebilmek için AmmoType.cs dosyasını kullanacağız.
                    objID = (int)hit.transform.gameObject.GetComponent<AmmoType>().chooseAmmo;
                    // ışının çarptığı cephaneye bağlı olarak başlığın ve görselin değişmesi için
                    mainImage.sprite = ammoIcons[objID];
                    mainTitle.text = ammoTitles[objID];


                    // e'ye bastığımızda mermiyi alacağız ve SaveScript.cs dosyasına mermiye sahip olduğumuzu kaydedeceğiz
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (objID == 0) // ışın, pistol mermisine çarptıysa
                        {
                            SaveScript.ammoAmts[0] += 12; // pistol mermi sayısı 12 artar
                        }

                        if (objID == 1) // ışın, shotgun mermisine çarptıysa
                        {
                            SaveScript.ammoAmts[1] += 8; // shotgun mermi sayısı 8 artar
                        }

                        audioPlayer.clip = pickupSounds[3];

                        audioPlayer.Play(); // mermi alındığında ses oynatılacak

                        SaveScript.change = true; // mermi toplandığı için

                        // mermiyi aldığımız için yok edeceğiz
                        Destroy(hit.transform.gameObject, 0.2f);
                    }
                }
                // yalnızca door etiketi olan nesneleri tespit ettiğimden emin olmak istiyorum.
                else if (hit.transform.gameObject.CompareTag("door")) // yalnızca bir kapı ise
                {
                    // hangi kapı?
                    SaveScript.doorObject = hit.transform.gameObject;

                    // hangi kapıyı işaret ettiğimizi tespit edebilmek için DoorType.cs dosyasını kullanacağız.
                    objID = (int)hit.transform.gameObject.GetComponent<DoorType>().chooseDoor;

                    // eğer kapı kilitliyse ihtiyacımız olan anahtarı yazdırır
                    if (hit.transform.gameObject.GetComponent<DoorType>().locked == true)
                    {
                        if (hit.transform.gameObject.GetComponent<DoorType>().electricDoor == false)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Locked. You need to use the " + hit.transform.gameObject.GetComponent<DoorType>().chooseDoor + " key!";
                        }

                        // kapı elektrikli ve jeneratör açılmamışsa
                        if (hit.transform.gameObject.GetComponent<DoorType>().electricDoor == true && SaveScript.generatorOn == false)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "This door needs a power supply to open";
                        }
                    }

                    // kapı elektrikliyse ve jeneratör açılmışsa
                    if (hit.transform.gameObject.GetComponent<DoorType>().electricDoor == true && SaveScript.generatorOn == true)
                    {
                        // kapı açık değilse
                        if (hit.transform.gameObject.GetComponent<DoorType>().opened == false)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to open the door";
                        }
                    }

                    // mesajı görünür hale getirelim
                    doorMessageObj.SetActive(true);

                    // mesaj
                    doorMessage.text = hit.transform.gameObject.GetComponent<DoorType>().message;

                    // e'ye bastığımızda eğer kapı kilitli değilse açılır
                    if (Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<DoorType>().locked == false)
                    {
                        audioPlayer.clip = pickupSounds[objID];
                        audioPlayer.Play(); // kapı açıldığında ses oynatılacak
                        /*
                            Creaking door (gıcırdayan kapı): dolaplar için ses
                            Old door creaking (eski gıcırdayan kapı): ev için ses
                            Large metal rusty door (büyük paslı metal kapı): kabin için ses
                        */

                        if (hit.transform.gameObject.GetComponent<DoorType>().opened == false)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to close the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened = true;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Open");
                        }
                        else if (hit.transform.gameObject.GetComponent<DoorType>().opened == true)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to open the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened = false;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Close");
                        }
                    }
                }
                // oyuncu eğer jeneratöre bakıyorsa
                else if (hit.transform.gameObject.CompareTag("generator"))
                {
                    SaveScript.generator = hit.transform.gameObject; // jeneratör
                    // jeneratör açık değilse
                    if (SaveScript.generatorOn == false)
                    {
                        // oyuncuya jeneratörle ne yapılacağı hakkında bilgi verecek
                        generatorMessageObj.SetActive(true);
                    }
                    // jeneratör açıksa çalışıyorsa
                    if (SaveScript.generatorOn == true)
                    {
                        generatorMessageObj.SetActive(false);
                    }
                }

                // oyuncu eğer aşıya bakıyorsa
                else if (hit.transform.gameObject.CompareTag("vaccine"))
                {
                    SaveScript.vaccine = hit.transform.gameObject; // aşı
                    vaccineMessageObj.SetActive(true);
                }
            }
            else
            {
                // oyuncu hiçbir şeye bakmıyorsa

                // pickups katmanında fakat etiketi weapon olmayan nesne (item)
                pickupPanel.SetActive(false);
                doorMessageObj.SetActive(false);

                // eğer bir kapıya bakmıyorsa
                SaveScript.doorObject = null;
                // oyuncu kapıya bakmadığı zamanlarda kapıyı açmak için anahtarı kullanamayacak

                generatorMessageObj.SetActive(false);
                SaveScript.generator = null;

                vaccineMessageObj.SetActive(false);
                SaveScript.vaccine = null;
            }
        }
        else
        {
            // katmanı pickups değilse 
            pickupPanel.SetActive(false);
        }

        // silah için yeniden küre oluşturacağım
        // ışın kameradan gelecek (bu dosyanın bulunduğu nesneden)
        // çap, yönü(ileriye doğru ateş edecek), vurulan nesne, ışın ne kadar uzağa gidebilir?
        if (Physics.SphereCast(transform.position, 0.01f, transform.forward, out gunHit, 500))
        {
            // vurulan "body" isimli nesne mi? (her zombide bulunur)
            // oyuncu tabancayı mı kullanıyor? (pistol)
            if (gunHit.transform.gameObject.name == "Body" && SaveScript.weaponID == 4)
            {
                // farenin sol tuşuna basıldı mı? ve eğer mermi varsa
                if (Input.GetMouseButtonDown(0) && SaveScript.currentAmmo[4] > 0)
                {
                    // vurulan Body nesnesindeki ZombieGunDamage.cs dosyasındaki SendGunDamage fonksiyonu
                    // Tops nesnesindeki ZombieDamage.cs dosyasındaki GunDamage fonksiyonunu çağıracak
                    // bu fonksiyon zombiyi öldürür
                    // Zombinin vurulduğu noktaya kan efekti ekleyeceğim için vurulduğu noktayı aktaracağım (gunHit.point)
                    gunHit.transform.gameObject.GetComponent<ZombieGunDamage>().SendGunDamage(gunHit.point);
                }
            }
        }

        // oyuncu shotgun kullanıyorsa yani elinde shotgun varsa ve eğer mermi varsa
        if (SaveScript.weaponID == 5 && SaveScript.currentAmmo[5] > 0)
        {
            // shotgun ile vurulan nesneler
            shotgunHits = Physics.SphereCastAll(transform.position, 0.3f, transform.forward, 50);

            for (int i = 0; i < shotgunHits.Length; i++)
            {
                // vurulan nesnenin ismi "Body" mi?
                if (shotgunHits[i].transform.gameObject.name == "Body")
                {
                    // farenin sol tuşuna basıldıysa
                    if (Input.GetMouseButtonDown(0))
                    {
                        // zombi ölür
                        shotgunHits[i].transform.gameObject.GetComponent<ZombieGunDamage>().SendGunDamage(shotgunHits[i].point);
                    }
                }
            }
        }
    }
}

// bu dosya FirstPersonCharacter nesnesine bileşen olarak eklenmiştir.

