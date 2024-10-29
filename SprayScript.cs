using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprayScript : MonoBehaviour
{
    public Image sprayFill;
    public float sprayAmount = 1.0f; // sprey başlangıçta tamamen dolu olacak
    public float drainTime = 0.1f;

    private void OnEnable()
    {
        // sprey miktarı her ekranda görüntülendiğinde güncellensin
        sprayFill.fillAmount = sprayAmount; // sprey görüntüsünü güncelliyorum
    }

    void Update()
    {
        // sola basılı tutarsa spray kullanılacak ve azalacak
        if (Input.GetMouseButton(0))
        {
            // sola basılı tuttuğum sürece her saniye spreyin miktarı azalacak.
            sprayAmount -= drainTime * Time.deltaTime;
            sprayFill.fillAmount = sprayAmount; // sprey görüntüsünü güncelliyorum
        }
    }
}
