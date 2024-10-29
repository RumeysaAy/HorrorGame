using UnityEngine;

public class LoadOff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SwitchOff", 1);
    }

    void SwitchOff()
    {
        // 1 saniye sonra bu dosyanın bağlı olduğu nesne devre dışı bırakılır
        // bu nesnenin alt nesneleri bütün zombilerdir.
        this.gameObject.SetActive(false);
    }
}
