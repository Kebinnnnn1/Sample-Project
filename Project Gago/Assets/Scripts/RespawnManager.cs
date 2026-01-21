using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    public Transform spawnPoint;
    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isRespawning = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        AutoFitFadeImage();
    }

    void AutoFitFadeImage()
    {
        if (fadeImage == null) return;

        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localPosition = Vector3.zero;

        // start transparent
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }

    public void Respawn(GameObject player)
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine(player));
    }

IEnumerator RespawnRoutine(GameObject player)
{
    isRespawning = true;

    // Fade to black
    yield return Fade(0f, 1f);

    // 🔁 RESET ALL MOVING PLATFORMS WHILE SCREEN IS BLACK
    foreach (MovingPlatformTrigger p in FindObjectsOfType<MovingPlatformTrigger>())
    {
        p.ResetPlatform();
    }

    // Disable movement before teleport
    CharacterController cc = player.GetComponent<CharacterController>();
    Rigidbody rb = player.GetComponent<Rigidbody>();

    if (cc != null)
        cc.enabled = false;

    if (rb != null)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    // Teleport player
    player.transform.position = spawnPoint.position;

    yield return null;

    // Re-enable movement
    if (cc != null)
        cc.enabled = true;

    if (rb != null)
        rb.isKinematic = false;

    // Fade back in
    yield return Fade(1f, 0f);

    isRespawning = false;
}

IEnumerator Fade(float from, float to)
{
    // 🔹 INSTANTLY SET START ALPHA
    Color c = fadeImage.color;
    c.a = from;
    fadeImage.color = c;

    float t = 0f;

    while (t < fadeDuration)
    {
        t += Time.deltaTime;
        c.a = Mathf.Lerp(from, to, t / fadeDuration);
        fadeImage.color = c;
        yield return null;
    }

    c.a = to;
    fadeImage.color = c;
}
}
