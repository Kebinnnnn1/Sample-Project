using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RisingFloodd : MonoBehaviour
{
    [Header("Flood Settings")]
    public float riseSpeed = 0.5f;
    public float startDelay = 3f;

    bool isActive;

    UIManager ui;
    AudioSource alarm;
    CameraShake cameraShake;

    void Start()
    {
        ui = FindObjectOfType<UIManager>();
        alarm = GetComponent<AudioSource>();
        cameraShake = Camera.main.GetComponent<CameraShake>();

        
if (cameraShake == null)
{
    Debug.LogError("CameraShake NOT FOUND on Main Camera");
}
else
{
    Debug.Log("CameraShake FOUND");
}
    }

    void Update()
    {
        if (!isActive) return;

        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void StartFloodWithDelay()
    {
        StartCoroutine(StartFloodCoroutine());
    }

    IEnumerator StartFloodCoroutine()
    {
        // UI + Alarm
        if (ui != null)
        {
            ui.ShowLavaWarning();
            StartCoroutine(ui.Countdown(5));
        }

        if (alarm != null)
        {
            alarm.Play();
        }

        yield return new WaitForSeconds(startDelay);

        if (ui != null)
        {
            ui.HideLavaWarning();
        }

        if (alarm != null)
        {
            alarm.Stop();
        }

        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.5f, 0.2f));
        }

        isActive = true;
    }
}
