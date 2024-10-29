using UnityEngine;

public class SpawnDirection : MonoBehaviour
{
    public bool forward = true;
    public bool back = false;
    public Transform[] targetList;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (forward == true)
            {
                gameObject.GetComponentInParent<ZombieSpawnPatrol>().spawnForward = true;
            }

            if (back == true)
            {
                gameObject.GetComponentInParent<ZombieSpawnPatrol>().spawnForward = false;
            }
        }
    }
}



