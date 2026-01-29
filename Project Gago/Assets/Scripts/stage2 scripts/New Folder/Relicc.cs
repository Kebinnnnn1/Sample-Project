using UnityEngine;

public class Relicc : MonoBehaviour
{
    UIManager ui;

    void Start()
    {
        ui = FindObjectOfType<UIManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        RisingFloodd flood = FindObjectOfType<RisingFloodd>();
        if (flood != null)
        {
            flood.StartFloodWithDelay();
        }

        if (ui != null)
        {
            ui.ShowLavaWarning(); // <-- THIS is what should show here
        }

        Destroy(gameObject);
    }
}
