using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSmash : MonoBehaviour
{
    private AudioSource audioPlayer;
    private Rigidbody rb;
    private bool playSound = false;
    public GameObject bottleParent;
    public float destroyTime = 3f; // şişenin yok olma süresi

    // molotof kokteyli için
    public bool flames = false;
    public GameObject explosion; // çarptığı nesnede bu efekti oluşturacağım

    // Start is called before the first frame update
    void Start()
    {
        // bu bileşenler bu dosyayla aynı nesnede bulunur
        audioPlayer = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        // şişe herhangi bir nesneyle çarpışmayabilir bu yüzden
        // şişe 20 saniye sonra yok edilsin
        Destroy(bottleParent, 20);
    }

    // eğer fırlatılan şişe herhangi bir nesne ile çarpışırsa
    private void OnCollisionEnter(Collision other)
    {
        // bir kez kırılma sesi oynatılacak
        if (playSound == false)
        {
            playSound = true;
            audioPlayer.Play();
            rb.isKinematic = true; // şişe çarptığı yerde kalacak
            // bu dosyanın bulunduğu nesnenin parent nesnesini yok edeceğim

            // şişenin çarptığı andaki konumu
            SaveScript.bottlePos = this.transform.position;

            Destroy(bottleParent, destroyTime); // destroyTime saniye sonra
        }

        if (flames == true)
        {
            // molotof kokteylinin çarptığı nesnede efekt oluşturdum
            Instantiate(explosion, this.transform.position, this.transform.rotation);
        }
    }
}
