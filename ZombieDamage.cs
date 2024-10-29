using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDamage : MonoBehaviour
{
    private bool damaging = true; // hasarın yalnızca tek bir karede oluşması için
    private int zombieHealth = 100;
    private Animator zombieAnim;
    private AudioSource damagePlayer;
    private bool death = false; // zombi öldü mü?

    public GameObject bloodSplat; // oyuncu zombiye vurduğunda oynatılacak efekt
    public string[] weaponTag; // silahlar
    public int[] damageAmts; // hasar miktarları
    public AudioClip[] damageSounds; // her silah için farklı sesler

    private bool flameDeath = false;

    // Start is called before the first frame update
    void Start()
    {
        zombieAnim = GetComponentInParent<Animator>();
        damagePlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // sol fare tuşuna basıldığında hasar verilebilir
            damaging = true;
        }

        if (zombieHealth <= 0)
        {
            if (death == false)
            {
                // zombi ölecek
                death = true;
                zombieAnim.SetTrigger("dead");
                zombieAnim.SetBool("isDead", true);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < weaponTag.Length; i++)
        {
            if (other.CompareTag(weaponTag[i]))
            {
                if (damaging == true)
                {
                    damaging = false;
                    // çarpan silaha göre zombi hasar alacak
                    zombieHealth -= damageAmts[i];
                    Debug.Log("Zombinin Sağlığı: " + zombieHealth);
                    // kan parçacık efekti, bıçağın zombiyle temas ettiği noktada oluşturulacak
                    Vector3 pos = other.ClosestPoint(transform.position);
                    Instantiate(bloodSplat, pos, other.transform.rotation);

                    // zombiler hasar aldığında sinirlenecek ve
                    // boş şişenin düştüğü yere gitmekten vazgeçecek, oyuncuya saldıracak
                    this.transform.gameObject.GetComponentInParent<ZombieScript>().isAngry = true;

                    // silaha göre hasar sesi oynatılacak
                    damagePlayer.clip = damageSounds[i];
                    damagePlayer.Play();

                    // eğer oyuncu sopa kullanıyorsa react animasyonunu çalıştıracağım
                    if (weaponTag[i] == "bat")
                    {
                        zombieAnim.SetTrigger("react");
                    }

                    if (weaponTag[i] == "axe")
                    {
                        zombieAnim.SetTrigger("axeReact");
                    }
                }
            }
        }
    }

    public void GunDamage(Vector3 hitPoint)
    {
        zombieHealth -= 100; // zombi ölür
        if (death == false)
        {
            // zombinin silahla vurulduğu yerde kan efekti oluşturdum
            // this: bu dosyanın bağlı olduğu nesne.
            Instantiate(bloodSplat, hitPoint, this.transform.rotation);
            // böylece bu olayı yalnızca tek bir karede gerçekleştirebilirim
            death = true;
            // ölüm animasyonu oynatılacak
            zombieAnim.SetTrigger("dead");
            // diğer animasyonların oynatılmasını engellemek için
            zombieAnim.SetBool("isDead", true);
        }
    }

    public void FlameDeath()
    {
        if (flameDeath == false)
        {
            flameDeath = true;
            StartCoroutine(ZombieFireWalk());
        }
    }

    IEnumerator ZombieFireWalk()
    {
        yield return new WaitForSeconds(5);
        if (death == false)
        {
            // böylece bu olayı yalnızca tek bir karede gerçekleştirebilirim
            death = true;
            // ateş ile ölüm animasyonu oynatılacak
            zombieAnim.SetTrigger("fireDie");
            // diğer animasyonların oynatılmasını engellemek için
            zombieAnim.SetBool("isDead", true);
        }
    }
}
