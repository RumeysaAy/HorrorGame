using UnityEngine;

public class ZombieGunDamage : MonoBehaviour
{
    // Tops nesnesinde ZombieDamage.cs bulunur
    public GameObject zombieDamageObj; // Tops nesnesi
    public GameObject flames; // zombinin üzerindeki ateş efekti
    public Material skinBurn; // zombinin yanmış gözükmesi için
    public GameObject[] LODs;  // bu dosyanın bağlı olduğu nesnenin, çocuk nesneleri
    private Animator bodyAnim; // zombi zamanla yanacak

    private void Start()
    {
        bodyAnim = GetComponent<Animator>();
    }

    public void SendGunDamage(Vector3 hitPoint)
    {
        // Zombinin vurulduğu noktaya (hitPoint) kan efekti ekleyeceğim
        zombieDamageObj.GetComponent<ZombieDamage>().GunDamage(hitPoint);
    }

    private void OnParticleCollision(GameObject other)
    {
        zombieDamageObj.GetComponent<ZombieDamage>().FlameDeath();
        flames.SetActive(true); // zombinin üzerindeki parçacık efekti aktifleştirilir

        foreach (GameObject skin in LODs)
        {
            skin.GetComponent<Renderer>().material = skinBurn;
        }

        bodyAnim.SetTrigger("burn");
    }
}
