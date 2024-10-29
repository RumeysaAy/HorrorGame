using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    private bool canDamage = false;
    private Collider col; // zombinin eli oyuncuya çarptı mı
    private Animator bloodEffect; //ekrana kan efekti ekledim
    private AudioSource hitSound; // zombi vurduğunda ses oynatılacak
    public int damageAmt = 3;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        bloodEffect = GameObject.Find("Blood").GetComponent<Animator>();
        hitSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // animasyonda collider'ı gerekli yerde aktifleştirdim
        if (col.enabled == false)
        {
            canDamage = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // eğer oyuncuya vurduysa
        if (other.CompareTag("Player"))
        {
            if (canDamage == true)
            {
                canDamage = false; // aynı anda bir kere vurmasını sağladım

                if (SaveScript.health > 0)
                {
                    SaveScript.health -= damageAmt;
                }

                if (SaveScript.infection < 100)
                {
                    SaveScript.infection += damageAmt;
                }

                bloodEffect.SetTrigger("blood");
                hitSound.Play();
            }
        }
    }
}
