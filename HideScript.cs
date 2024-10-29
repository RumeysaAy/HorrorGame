using UnityEngine;

public class HideScript : MonoBehaviour
{
    // Oyuncunun tetikleme alanı içinde olduğu her kare
    // oyuncu bu dosyanın bağlı olduğu nesneyle temas halinde kaldığı sürece
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerHide"))
        {
            SaveScript.isHidden = true;
        }
    }

    // oyuncu bu alanı terk ettiği zaman
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHide"))
        {
            SaveScript.isHidden = false;
        }
    }
}
