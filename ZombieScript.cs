using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour
{
    public enum ZombieType
    {
        // Zombies animator controller layer
        shuffle, // 0
        dizzy, // 1
        alert // 2
    }

    public enum ZombieState
    {
        // animator parameters
        Idle, // 0
        Walking, // 1
        Eating // 2
    }

    public ZombieType zombieStyle; // layer
    public ZombieState chooseState; // parametre
    public float yAdjustment = 0.0f; // Shuffle
    private Animator anim;
    private AnimatorStateInfo animInfo; // animatörü dinler, hangi animasyonların oynatıldığını tespit etmek
    private NavMeshAgent agent;

    // State Idle:0, Walking:1, Eating:2
    public bool randomState = false; // false: tek bir durum oynar
    public float randomTiming = 5f; // her 5 saniyede bir karakter farklı bir duruma geçecektir
    private int newState = 0; // hangi yeni durumda oynayacak
    private int currentState; // mevcut durum

    private GameObject[] targets; // zombinin hedefleri
    private float[] walkSpeed = { 0.15f, 1.0f, 0.75f }; // zombinin hızları
    private float distanceToTarget;
    private int currentTarget = 0;

    private float distanceToPlayer; // Zombi ile oyuncu arasındaki mesafe
    private GameObject player; // oyuncu
    private float zombieAlertRange = 10f; // eğer oyuncu, zombiden bundan daha az uzaktaysa, zombi oyuncunun farkına varır
    private bool awareOfPlayer = false; // zombi, başlangıçta oyuncunun farkında olmayacak
    private bool adding = true;

    private AudioSource chaseMusicPlayer; // zombi oyuncuyu takip ettiğinde oynatılacak

    private float attackDistance = 2f; // saldırı mesafesi

    private float rotateSpeed = 2.5f; // dönüş hızı

    private AudioSource zombieSound;

    private float gunAlertRange = 400f; // pistol veya shotgun kullandığında zombilerin fark etme mesafesi

    public bool isAngry = false; // zombiler şişenin düştüğü yöne doğru gidecekler

    private float hiddenRange = 2f; // oyuncu saklanmış olsa bile zombiler oyuncuya iki birimden daha yakınsa oyuncuyu kovalayacaklar

    public bool startInHouse = false; // evin içerisindeki zombiler oyuncuyu takip etsin

    private float alertSpeed; // zombinin oyuncuyu takip ettiğindeki animasyon hızı

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        zombieSound = GetComponent<AudioSource>();

        // zombinin yürüyeceği hedefler
        targets = GameObject.FindGameObjectsWithTag("Target");

        player = GameObject.Find("FPSController"); // isime göre arama yapılacak

        chaseMusicPlayer = GameObject.Find("ChaseMusic").GetComponent<AudioSource>();

        // takip etme aralığı rastgele belirlenecek
        zombieAlertRange = Random.Range(55f, 200f);

        // bu yüzden seçilen katmanın weight = 1’e eşitledim
        // shuffle 1, dizzy 2, alert 3 katman
        anim.SetLayerWeight(((int)zombieStyle + 1), 1);

        if (zombieStyle == ZombieType.shuffle)
        {
            // eğer shuffle layer animasyon oynatılıyorsa zombinin y eksenine ekleme yaptım
            transform.position = new Vector3(transform.position.x, transform.position.y + yAdjustment, transform.position.z);
        }
        // denetçiden seçeceğim ve tetiklenecek, animasyon oynatılacak
        anim.SetTrigger(chooseState.ToString());

        currentState = (int)chooseState;

        if (randomState == true)
        {
            // her 5 saniyede bir bu işlevi çağır
            InvokeRepeating("SetAnimState", randomTiming, randomTiming);
        }

        // zombi ilk nesneye/hedefe doğru yürüyecek
        agent.destination = targets[0].transform.position;
        // zombinin türüne göre hızı farklı olacaktır
        agent.speed = walkSpeed[(int)zombieStyle];

        // zombinin oyuncuyu takip ettiğindeki animasyon hızı
        alertSpeed = Random.Range(1.0f, 2.0f);

        // mevcut hedef rastgele belirlenir
        currentTarget = Random.Range(0, targets.Length);
    }

    void Update()
    {
        // eğer zombi yaşıyorsa
        if (anim.GetBool("isDead") == false)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position); // oyuncuya olan mesafe

            if (SaveScript.bottlePos == Vector3.zero)
            {
                // boş şişe atılmadıysa
                isAngry = false;
            }

            if (SaveScript.bottlePos != Vector3.zero && distanceToPlayer > attackDistance && isAngry == false)
            {
                // boş şişe atıldıysa çarptıysa
                // zombinin oyuncuya olan uzaklığı saldırı mesafesinden fazlaysa
                // zombiler şişenin düştüğü, çarpıştığı yere doğru gidecekler
                agent.destination = SaveScript.bottlePos;
                anim.SetBool("attacking", false);
                chooseState = ZombieState.Walking;
            }
            else
            {
                if (distanceToPlayer <= attackDistance)
                {
                    // eğer oyuncu saldırı menzilindeyse
                    agent.isStopped = true; // zombi dursun
                    anim.SetBool("attacking", true); // zombi saldırı animasyonu
                    anim.speed = 1.0f; // zombinin saldırı anındaki hızı

                    // zombi düşmana doğru saldırsın
                    Vector3 pos = (player.transform.position - transform.position).normalized;
                    Quaternion posRotation = Quaternion.LookRotation(new Vector3(pos.x, 0, pos.z));
                    // slerp: bir tür enterpolasyon, belirli bir süre boyunca A noktasından B noktasına gitmektir.
                    transform.rotation = Quaternion.Slerp(transform.rotation, posRotation, rotateSpeed * Time.deltaTime); // A, B
                                                                                                                          // A: düşmanın şu anki konumu
                }
                else
                {
                    // oyuncu saldırı menzilinde değilse
                    anim.SetBool("attacking", false); // zombi saldırmasın

                    if (SaveScript.zombiesChasing.Count > 0)
                    {
                        // zombi/zombiler oyuncuyu takip ediyor
                        if (chaseMusicPlayer.volume < 0.4f)
                        {
                            // yalnızca tek bir karede gerçekleşmesi için
                            if (chaseMusicPlayer.isPlaying == false)
                            {
                                chaseMusicPlayer.Play();
                            }

                            // sesin yüksekliği 0.4'ya ulaşana kadar zamanla yükselteceğim
                            chaseMusicPlayer.volume += 0.3f * Time.deltaTime;
                        }
                    }

                    if (SaveScript.zombiesChasing.Count == 0)
                    {
                        // zombi/zombiler oyuncuyu takip etmiyor
                        if (chaseMusicPlayer.volume > 0.0f)
                        {
                            // sesin yüksekliği 0'a ulaşana kadar zamanla azaltacağım
                            chaseMusicPlayer.volume -= 0.5f * Time.deltaTime;
                        }

                        if (chaseMusicPlayer.volume == 0.0f)
                        {
                            chaseMusicPlayer.Stop();
                        }
                    }



                    distanceToTarget = Vector3.Distance(transform.position, targets[currentTarget].transform.position);

                    animInfo = anim.GetCurrentAnimatorStateInfo((int)zombieStyle); // hangi katman dinlenecek

                    // oyuncu zombiye yeterince yakınsa ve zombi yürüyorsa
                    if (distanceToPlayer < zombieAlertRange && chooseState == ZombieState.Walking)
                    {
                        // zombi hedeflerden birine doğru yürümek yerine artık dönüp oyuncuya doğru yürüyecek
                        agent.destination = player.transform.position;
                        awareOfPlayer = true; // zombi oyuncunun farkında
                        // zombi oyuncuyu takip ettiğindeki animasyon hızı
                        anim.speed = alertSpeed;

                        if (adding == true)
                        {
                            // oyuncuyu takip eden zombi eklenecek
                            // bu script dosyasını içeren nesne içerisinde mi?
                            // bu betiğin eklendiği öğenin adına bakacak
                            if (SaveScript.zombiesChasing.Contains(this.gameObject))
                            {
                                // eklenmesin
                                adding = false;
                                return;
                            }
                            else
                            {
                                // liste şu anda bu oyun nesnesini içermiyor, o zaman ekleyeceğim
                                SaveScript.zombiesChasing.Add(this.gameObject);
                                adding = false; // ekledim
                            }
                        }
                    }



                    // oyuncu zombiye yeterince yakınsa ve zombi evde doğduysa
                    if (distanceToPlayer < 10 && startInHouse == true)
                    {
                        // zombi hedeflerden birine doğru yürümek yerine artık dönüp oyuncuya doğru yürüyecek
                        agent.destination = player.transform.position;
                        awareOfPlayer = true; // zombi oyuncunun farkında
                        chooseState = ZombieState.Walking;
                        anim.SetTrigger("Walking");
                        ZombiesStartInHouse();

                        if (adding == true)
                        {
                            // oyuncuyu takip eden zombi eklenecek
                            // bu script dosyasını içeren nesne içerisinde mi?
                            // bu betiğin eklendiği öğenin adına bakacak
                            if (SaveScript.zombiesChasing.Contains(this.gameObject))
                            {
                                // eklenmesin
                                adding = false;
                                return;
                            }
                            else
                            {
                                // liste şu anda bu oyun nesnesini içermiyor, o zaman ekleyeceğim
                                SaveScript.zombiesChasing.Add(this.gameObject);
                                adding = false; // ekledim
                            }
                        }
                    }



                    if (distanceToPlayer > zombieAlertRange)
                    {
                        awareOfPlayer = false; // zombi oyuncunun farkında değil
                        anim.speed = 1.0f; // zombinin oyuncuyu takip etmiyorkenki animasyon hızı
                        // zombi eğer oyuncuyu takip etmiyorsa listeden çıkardım
                        // liste bu zombiyi içeriyor mu?
                        if (SaveScript.zombiesChasing.Contains(this.gameObject))
                        {
                            SaveScript.zombiesChasing.Remove(this.gameObject);
                            adding = true; // çıkarıldığı için eklenebilir
                        }
                    }

                    // eğer zombi oyuncudan 200'den uzaktaysa yok edilecek
                    if (distanceToPlayer > 200)
                    {
                        awareOfPlayer = false; // zombi oyuncunun farkında değil
                                               // zombi eğer oyuncuyu takip etmiyorsa listeden çıkardım
                                               // liste bu zombiyi içeriyor mu?
                        if (SaveScript.zombiesChasing.Contains(this.gameObject))
                        {
                            SaveScript.zombiesChasing.Remove(this.gameObject);
                            adding = true; // çıkarıldığı için eklenebilir
                        }
                        Destroy(gameObject); // zombi yok edilir
                    }

                    if (animInfo.IsTag("motion")) // etiketi motion olan animasyonlar hareket ederken oynatılır
                    {
                        // yürüme animasyonunu oynattığı sürece, animasyonun şu anda etkin olup olmadığını
                        // geçiş aşamasında hangi katman dinlenecek
                        if (anim.IsInTransition((int)zombieStyle))
                        {
                            // yürüyüş animasyonu olmayan bir şeye geçiş olduğu sürece
                            agent.isStopped = true; // zombi durur
                                                    // hareketi etkili bir şekilde durduracak
                        }
                    }

                    if (chooseState == ZombieState.Walking)
                    {
                        if (distanceToTarget < 1.5f) // hedefe olan mesafe az mı?
                        {
                            if (currentTarget < targets.Length - 1)
                            {
                                // bunların herhangi biri arasında yürüyebilir
                                currentTarget = Random.Range(0, targets.Length);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // zombi öldüyse

            // zombi eğer oyuncuyu takip etmiyorsa listeden çıkardım
            // liste bu zombiyi içeriyor mu?
            if (SaveScript.zombiesChasing.Contains(this.gameObject))
            {
                SaveScript.zombiesChasing.Remove(this.gameObject);
                adding = true; // çıkarıldığı için eklenebilir
            }

            if (SaveScript.zombiesChasing.Count == 0)
            {
                // zombi/zombiler oyuncuyu takip etmiyor
                if (chaseMusicPlayer.volume > 0.0f)
                {
                    // sesin yüksekliği 0'a ulaşana kadar zamanla azaltacağım
                    chaseMusicPlayer.volume -= 0.5f * Time.deltaTime;
                }

                if (chaseMusicPlayer.volume == 0.0f)
                {
                    chaseMusicPlayer.Stop();
                }
            }

            CancelInvoke(); // zombiden gelen sesi durdurmak için
            Destroy(gameObject, 20); // 20 saniye sonra zombi yok edilecek
        }

        // eğer oyuncu pistol veya shotgun kullandıysa zombilerin oyuncuyu fark etme mesafesi artar
        // böylece oyuncu zombilerden uzak olsa bile zombiler oyuncuya doğru gelirler
        if (SaveScript.gunUsed == true)
        {
            zombieAlertRange = gunAlertRange;
            // 10 saniye sonra menzil sıfırlanacak
            StartCoroutine(ResetGunRange());
        }
        else if (SaveScript.isHidden == true)
        {
            // oyuncu eğer saklanmış ise zombi ile arasındaki mesafe 
            // hiddenRange'den kısa olduğu anda zombi oyuncuyu fark eder
            zombieAlertRange = hiddenRange;
        }
        else
        {
            zombieAlertRange = 10f;
        }
    }

    // bu dosyanın bağlı olduğu nesne yok edildiğinde çalışır
    private void OnDestroy()
    {
        // zombinin oyuncu ile arasındaki mesafe 200'den büyükse zombi yok edilir
        // oyuncu zombiyi öldürdüğünde zombi yok edilir
        // oyundaki toplam zombi sayısı 1 azalır
        SaveScript.zombiesInGameAmt--;

        Debug.Log(SaveScript.zombiesInGameAmt);
    }

    void SetAnimState()
    {
        // zombi, oyuncunun farkında değilse durum değiştirebilir
        if (awareOfPlayer == false)
        {
            // Idle:0, Walking:1, Eating:2
            // yeni durumu rastgele hale getirdim
            newState = Random.Range(0, 3); // 0, 1, 2
            if (newState != currentState)
            {
                // durum değişir
                chooseState = (ZombieState)newState;
                currentState = (int)chooseState;
                anim.SetTrigger(chooseState.ToString());
            }
        }
        if (awareOfPlayer == true)
        {
            chooseState = ZombieState.Walking;
        }

        zombieSound.Play();
    }

    // Walk animasyonundaki event
    public void WalkOn()
    {
        // zombi ilerleyebilir
        agent.isStopped = false;
        // zombi currentTarget nesneye/hedefe doğru yürüyecek
        agent.destination = targets[currentTarget].transform.position;
    }

    // Walk animasyonundaki event
    public void WalkOff()
    {
        // zombi durur
        agent.isStopped = true;
    }

    void ZombiesStartInHouse()
    {
        // evde bulunan zombi oyuncuyu takip ettiğinde ses oynatılacak
        if (chaseMusicPlayer.isPlaying == false)
        {
            chaseMusicPlayer.Play();
        }
        chaseMusicPlayer.volume = 0.35f;
    }

    IEnumerator ResetGunRange()
    {
        // silah menzili sıfırlamadan önce 10 saniye beklenecek
        yield return new WaitForSeconds(10);
        // en fazla gunAlertRange kadar uzaklıkta olan zombiler
        // 10 saniye içerisinde aniden oyuncuya doğru yürümeye başlayacak.
        // 10 saniye sonra zombi, zombieAlertRange'den daha fazla uzaktaysa
        // oyuncuyu takip etmeyi bırakır
        SaveScript.gunUsed = false;
    }
}
