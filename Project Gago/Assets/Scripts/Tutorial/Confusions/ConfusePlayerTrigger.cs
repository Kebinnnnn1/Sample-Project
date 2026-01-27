using UnityEngine;
using TMPro;
using System.Collections;

public class ConfusePlayerTrigger : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI typingText;

    [Header("Message")]
    [TextArea(2, 4)]
    public string message = "Look for where the map ends.";

    public float typingSpeed = 0.04f;
    public float startDelay = 0.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip voiceClip;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(startDelay);

        if (audioSource && voiceClip)
            audioSource.PlayOneShot(voiceClip);

        typingText.text = "";

        foreach (char c in message)
        {
            typingText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
