using UnityEngine;

public class BreathingScript : MonoBehaviour
{
    private AudioSource audioPlayer;
    private bool heavyBreathing = false;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // dayanıklılık 20'den azsa ağır nefes alma sesini oynatacağım
        if (SaveScript.stamina < 20 && heavyBreathing == false)
        {
            heavyBreathing = true;
            audioPlayer.Play();
        }

        if (SaveScript.stamina > 19 && heavyBreathing)
        {
            heavyBreathing = false;
            audioPlayer.Stop();
        }
    }
}
