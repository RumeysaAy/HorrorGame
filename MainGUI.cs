using UnityEngine;
using UnityEngine.UI;

public class MainGUI : MonoBehaviour
{
    public Text healthAmt; // sağlık
    public Text staminaAmt; // dayanıklılık
    public Text infectionAmt; // enfeksiyon

    // Update is called once per frame
    void Update()
    {
        healthAmt.text = SaveScript.health + "%";
        staminaAmt.text = SaveScript.stamina.ToString("F0") + "%"; // virgülden sonra 0 basamak
        infectionAmt.text = SaveScript.infection.ToString("F0") + "%"; // virgülden sonra 0 basamak
    }
}
