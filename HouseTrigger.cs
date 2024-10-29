using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    public GameObject spawnZone;

    // oyuncu kapıdan geçerken tetikleyecek ve içeride zombiler oluşturulacak
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 
            spawnZone.GetComponent<ZombieSpawnPatrol>().canSpawn = true;
        }
    }
}
