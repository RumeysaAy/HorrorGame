using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightVisionScript : MonoBehaviour
{
    private Image zoomBar;
    private Image batteryChunks; // pile erişmek için
    private Camera cam;
    public float batteryPower = 1.0f;
    public float drainTime = 20.0f; // pilin bir parçasının bitme süresi (saniye)

    // Start is called before the first frame update
    void Start()
    {
        zoomBar = GameObject.Find("ZoomBar").GetComponent<Image>();
        batteryChunks = GameObject.Find("BatteryChunks").GetComponent<Image>();
        cam = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        InvokeRepeating("BatteryDrain", drainTime, drainTime); // başladıktan tam 20 saniye sonra ve sonrasında her 20 saniyede bir gerçekleşecek.

        // her gece görüşü açıldığında barın uzunluğu 0.6 oranında olsun
        if (zoomBar != null)
        {
            zoomBar.fillAmount = 0.6f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // yukarı
        {
            if (cam.fieldOfView > 10)
            {
                cam.fieldOfView -= 5;
                zoomBar.fillAmount = cam.fieldOfView / 100; // UI
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // aşağı
        {
            if (cam.fieldOfView < 60)
            {
                cam.fieldOfView += 5;
                zoomBar.fillAmount = cam.fieldOfView / 100; // UI
            }
            // Uzaklaştıkça, kameranın görüş alanını 60'a ulaşıncaya kadar arttırıyoruz.
        }
        // Bu gece görüş modunu açık tuttuğumuz sürece bu pilin azalmaya başlamasını istiyoruz.
        batteryChunks.fillAmount = batteryPower;
    }

    private void BatteryDrain()
    { // pil bittikçe pilin her bir parçasının tek tek gitmesi için
        if (batteryPower > 0.0f)
        {
            // pil 4 parçadan oluştuğundan bir parça 0.25'dir.
            batteryPower -= 0.25f;
        }
    }

    public void StopDrain()
    {
        CancelInvoke("BatteryDrain");
    }
}
