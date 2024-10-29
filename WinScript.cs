using UnityEngine;

public class WinScript : MonoBehaviour
{
    public GameObject winMessage;

    // Start is called before the first frame update
    void Start()
    {
        // kazanma mesajı başalngıçta kapalıdır
        winMessage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // aşı alındıysa
            if (SaveScript.gotVaccine == true)
            {
                // oyun kazanılır
                winMessage.SetActive(true);
                // oyun duraklatılır
                Time.timeScale = 0.0f;
            }
        }
    }
}
