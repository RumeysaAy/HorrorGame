using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Standart modda yani gece görüş modu aktif değilken karaktere el feneri vereceğim
// el fenerimiz olduğu sürece pil gücü azalmaya başlayacak
// f tuşuna basıldığında el feneri açılacak veya kapanacak

public class FlashLightScript : MonoBehaviour
{
    private Image batteryChunks;
    public float batteryPower = 1.0f;
    public float drainTime = 2;

    // bu dosya her çağrıldığında bu metot çalıştırılır.
    void OnEnable()
    {
        batteryChunks = GameObject.Find("FLBatteryChunks").GetComponent<Image>();
        InvokeRepeating("FLBatteryDrain", drainTime, drainTime); // el fenerini kapattığımızda pil azalmaya devam eder
    }

    // Update is called once per frame
    void Update()
    {
        batteryChunks.fillAmount = batteryPower;
    }

    private void FLBatteryDrain()
    {
        if (batteryPower > 0.0f)
        {
            batteryPower -= 0.25f;
        }
    }

    // LookMode.cs dosyasında çağıracağım
    public void StopDrain()
    { // el fenerini kapattığımızda pilin azalmasını engellemek için
        CancelInvoke("FLBatteryDrain");
    }
}
