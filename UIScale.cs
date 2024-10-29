using UnityEngine;

public class UIScale : MonoBehaviour
{
    public float scaleValue = 1f;
    public float UHDScale = 2f;

    // Start is called before the first frame update
    void Start()
    { // 4K UHD yaptığımızda UI elemanlarının boyutunu iki katına çıkaralım.
        if (Screen.width > 1920)
        {
            scaleValue = UHDScale; // boyut 2 katına çıkarıldı
        }

        this.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }
}
