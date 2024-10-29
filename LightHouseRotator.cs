using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHouseRotator : MonoBehaviour
{
    public float rotateSpeed = 3.5f;

    // Update is called once per frame
    void Update()
    {
        // Time.deltaTime: her saniyede bir güncellenmesini sağlar
        // dolayısıyla hızlı ve yavaş bilgisayarda aynı anda döner
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        // forward: 0, 0, 1
    }
}
