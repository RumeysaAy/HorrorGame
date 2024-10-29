using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnPatrol : MonoBehaviour
{
    public Transform[] spawnPoints; // doğma noktaları
    public GameObject[] zombies; // zombi nesneleri
    public int zombieSpawnAmt = 6; // her tetiklendiğinde oluşturulacak zombi sayısı

    private float reSpawnTimer = 10f; // zombi oluşturulduktan reSpawnTimer kadar sonra tekrar oluşturulabilir
    private float resetTimer = 0f; // sayaç

    [HideInInspector] // inspector'de görülmemesi için 
    public bool canSpawn = true; // zombi oluşturulabilir mi?

    public bool houseSpawn = false;

    [HideInInspector]
    public bool spawnForward = true; // sadece önünde bulunan doğma noktalarından zombiler doğar
    public GameObject goingForward;
    public GameObject goingBack;

    private void Start()
    {
        if (houseSpawn == true)
        {
            canSpawn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn == false && houseSpawn == false)
        {
            resetTimer += 1 * Time.deltaTime;
            // collider tetiklendikten reSpawnTimer kadar saniye sonra tekrar tetiklenirse zombi oluşturulur
            if (resetTimer >= reSpawnTimer)
            {
                canSpawn = true;
                resetTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnForward == true)
        {
            spawnPoints = goingForward.GetComponent<SpawnDirection>().targetList;
        }

        if (spawnForward == false)
        {
            spawnPoints = goingBack.GetComponent<SpawnDirection>().targetList;
        }

        // collider oyuncu tarafından tetiklenirse zombieSpawnAmt kadar zombi oluşturulur
        // oyunda en fazla 120 tane zombi olsun
        if (other.CompareTag("Player") && canSpawn == true && SaveScript.zombiesInGameAmt < (120 - zombieSpawnAmt))
        {
            SpawnZombies();
        }

        // zombiler yok edilecek
        if (other.CompareTag("Player") && canSpawn == true && SaveScript.zombiesInGameAmt >= (120 - zombieSpawnAmt))
        {
            // bütün zombiler bulunur
            GameObject[] zombiesToDestroy = GameObject.FindGameObjectsWithTag("zombie");
            // 20 birimden daha fazla uzaklıktaki zombiler yok edilecek
            for (int i = 0; i < zombieSpawnAmt; i++)
            {
                // 
                if (zombiesToDestroy.Length >= zombieSpawnAmt)
                {
                    float furthestDistance = Vector3.Distance(transform.position, zombiesToDestroy[i].transform.position);
                    if (furthestDistance > 30)
                    {
                        Destroy(zombiesToDestroy[i]);
                    }
                }
            }
            SpawnZombies();
        }
    }

    void SpawnZombies()
    {
        for (int i = 0; i < zombieSpawnAmt; i++)
        {
            // zombiler evde doğmadıysa
            if (houseSpawn == false)
            {
                int spawnRandom = Random.Range(0, spawnPoints.Length);
                // herhangi bir doğma noktasında herhangi bir zombi oluşturulacak
                Instantiate(zombies[Random.Range(0, zombies.Length)],
                 new Vector3(spawnPoints[spawnRandom].position.x - Random.Range(0, 10), spawnPoints[spawnRandom].position.y, spawnPoints[spawnRandom].position.z - Random.Range(0, 5)),
                 spawnPoints[spawnRandom].rotation);
            }
            // zombiler evde doğduysa
            else
            {
                Instantiate(zombies[Random.Range(0, zombies.Length)],
                 spawnPoints[i].position,
                 spawnPoints[i].rotation);
            }
            SaveScript.zombiesInGameAmt++;
        }
        canSpawn = false; // ancak 10 saniye sonra tekrar zombi oluşturulabilir
    }
}
