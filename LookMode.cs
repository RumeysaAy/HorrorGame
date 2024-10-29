using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LookMode : MonoBehaviour
{
    private PostProcessVolume vol;
    public PostProcessProfile standard; // normal görüş
    public PostProcessProfile nightVision; // gece görüşü
    public PostProcessProfile inventory; // envanter profili/görüşü
    public GameObject nightVisionOverlay; // gece görüş UI
    public GameObject flashLightOverlay; // el feneri UI
    public GameObject inventoryMenu; // UI (canva)
    public GameObject combinePanel;
    private Light flashLight; // el fenerini kapatıp açmak için kullanacağız
    private bool nightVisionOn = false;
    private bool flashLightOn = false;
    public GameObject pointer; // ekranın merkezini gösterir (UI)

    // Start is called before the first frame update
    void Start()
    {
        vol = GetComponent<PostProcessVolume>();
        flashLight = GameObject.Find("FlashLight").GetComponent<Light>(); // el fenerinin(spotlight) ışık bileşeni
        flashLight.enabled = false;
        nightVisionOverlay.SetActive(false);
        flashLightOverlay.SetActive(false);
        inventoryMenu.SetActive(false);
        vol.profile = standard;
    }

    // Update is called once per frame
    void Update()
    {
        // eğer gece görüşü kapalıysa N'e basıldığında açılsın
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (SaveScript.inventoryOpen == false)
            {
                if (nightVisionOn == false)
                {
                    vol.profile = nightVision;
                    nightVisionOverlay.SetActive(true);
                    nightVisionOn = true;
                    NightVisionOff(); // Pilimizde güç yoksa gece görüşünün hiç açılmasını istemeyiz.
                }
                else if (nightVisionOn == true)
                { // eğer gece görüşü açıksa N'e tıklandığında kapansın
                    vol.profile = standard;
                    nightVisionOverlay.SetActive(false);
                    nightVisionOverlay.GetComponent<NightVisionScript>().StopDrain(); // gece görüşü pilin azalmasını engeller
                    this.gameObject.GetComponent<Camera>().fieldOfView = 60; // gece görüşünde yapılan yakınlaştırma gece görüşü kapatıldığında sıfırlansın
                    nightVisionOn = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (SaveScript.inventoryOpen == false)
            {
                if (flashLightOn == false)
                {
                    flashLightOverlay.SetActive(true);
                    flashLight.enabled = true; // el fenerinin ışığını aç
                    flashLightOn = true;
                    FlashLightSwitchOff();
                }
                else if (flashLightOn == true)
                {
                    flashLightOverlay.SetActive(false);
                    flashLight.enabled = false; // el fenerinin ışığını kapat
                    flashLightOverlay.GetComponent<FlashLightScript>().StopDrain(); // el feneri pilin azalmasını engeller
                    flashLightOn = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.I)) // envanter
        {
            if (SaveScript.inventoryOpen == false)
            {
                // Envanter menümüzü etkin bir şekilde eklediğimizde, arkasındaki her şey bulanıklaşacak. (InventoryProfile)
                vol.profile = inventory;
                inventoryMenu.SetActive(true); // Envanter menüsü UI (Canva)
                // envanter açıkken gece görüşü ve el feneri iptal edilmeli

                if (flashLightOn == true)
                {
                    flashLightOverlay.SetActive(false);
                    flashLight.enabled = false;
                    flashLightOverlay.GetComponent<FlashLightScript>().StopDrain();
                    flashLightOn = false;
                }

                if (nightVisionOn == true)
                {
                    nightVisionOverlay.SetActive(false);
                    nightVisionOverlay.GetComponent<NightVisionScript>().StopDrain();
                    this.gameObject.GetComponent<Camera>().fieldOfView = 60;
                    nightVisionOn = false;
                }
            }
            else if (SaveScript.inventoryOpen == true)
            {
                // envanter açıksa ı'ye tıklayıp kapatacağım.
                vol.profile = standard;
                combinePanel.SetActive(false); // UI
                inventoryMenu.SetActive(false); // UI
            }
        }

        if (nightVisionOn == true)
        {
            NightVisionOff(); // Pilimizde güç yoksa gece görüşünün hiç açılmasını istemeyiz.
        }

        if (flashLightOn == true)
        {
            FlashLightSwitchOff();
        }

        // veya oyun duraklatıldığında (başlangıç ekranı)(begin button)
        if (SaveScript.inventoryOpen == true || Time.timeScale == 0)
        {
            // eğer envanter menü açıksa imleç görünür olsun
            Cursor.visible = true;
            // ekranın merkezi envanter açıkken gözükmesin 
            pointer.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            pointer.SetActive(true);
        }
    }

    private void NightVisionOff()
    { // Pil gücü bittiğinde yani sıfıra ulaştığında gece görüş modu kapanıp standart mod tekrar açılacak.
        if (nightVisionOverlay.GetComponent<NightVisionScript>().batteryPower <= 0)
        {
            vol.profile = standard;
            nightVisionOverlay.SetActive(false);
            this.gameObject.GetComponent<Camera>().fieldOfView = 60; // gece görüşünde yapılan yakınlaştırma gece görüşü kapatıldığında sıfırlansın
            nightVisionOn = false;
        }
    }

    private void FlashLightSwitchOff()
    {
        if (flashLightOverlay.GetComponent<FlashLightScript>().batteryPower <= 0)
        {
            flashLightOverlay.SetActive(false);
            flashLight.enabled = false;
            flashLightOverlay.GetComponent<FlashLightScript>().StopDrain();
            flashLightOn = false;
        }
    }
}
