using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterScript : MonoBehaviour
{
    public GameObject lighterObj;

    void OnEnable()
    {
        lighterObj.SetActive(true);
    }

    void OnDisable()
    {
        lighterObj.SetActive(false);
    }
}

/*

Sprey seçildiğinde çakmak aktif edilecek

Çakmağın yalnızca sprey kutusu açıkken açılmasını 
ve sprey kutusu açık olmadığında kapanmasını istiyorum

*/