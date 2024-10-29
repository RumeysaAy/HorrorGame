using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorType : MonoBehaviour
{
    public enum typeOfDoor
    {
        cabinet,
        house,
        cabin
    }

    public typeOfDoor chooseDoor;
    public bool opened = false; // kapı açık mı?
    public bool locked = false; // kapı kilitli mi?
    [HideInInspector]
    public string message = "Press E to open the door";
    private Animator anim;
    public bool electricDoor = false; // elektrikli bir kapı mı?

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        if (opened == true)
        {
            anim.SetTrigger("Open");
            message = "Press E to close the door";
        }
    }
}
