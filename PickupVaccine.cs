using UnityEngine;

public class PickupVaccine : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // e'ye basıldı mı?
        if (Input.GetKeyDown(KeyCode.E))
        {
            // aşıya bakıyorsa aşı nesnesi vardır
            if (SaveScript.vaccine != null)
            {
                // aşı alındı
                SaveScript.gotVaccine = true;
                Destroy(gameObject); // aşı yok edildi
            }
        }
    }
}
