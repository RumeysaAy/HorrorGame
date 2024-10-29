using UnityEngine;

public class BeginScript : MonoBehaviour
{
    // başlangıç ekranı

    void Start()
    {
        // oyun duraklatılır
        Time.timeScale = 0.0f;
        Cursor.visible = true;
    }

    public void Begin()
    {
        // oyun başlatılır
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Destroy(gameObject);
    }
}
