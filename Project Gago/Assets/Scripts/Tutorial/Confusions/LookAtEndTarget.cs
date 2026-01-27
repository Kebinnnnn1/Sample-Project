using UnityEngine;
using TMPro;
using System.Collections;

public class LookAtEndTarget : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float maxDistance = 30f;
    public float lookTimeRequired = 1.5f;

    [Header("UI")]
    public TextMeshProUGUI typingText;
    public GameObject lookIndicator;

    [TextArea(2, 4)]
    public string completionMessage = "That’s the end.";

    public float typingSpeed = 0.04f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip completionVoice;

    private float lookTimer;
    private bool completed;

    void Start()
    {
        if (lookIndicator)
            lookIndicator.SetActive(false);
    }

    void Update()
    {
        if (completed) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool isLookingAtTarget = false;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.transform == target)
            {
                isLookingAtTarget = true;
                lookTimer += Time.deltaTime;

                if (lookTimer >= lookTimeRequired)
                {
                    Complete();
                }
            }
        }

        if (!isLookingAtTarget)
            lookTimer = 0f;

        if (lookIndicator)
            lookIndicator.SetActive(isLookingAtTarget);
    }

    void Complete()
    {
        completed = true;

        if (lookIndicator)
            lookIndicator.SetActive(false);

        if (audioSource && completionVoice)
            audioSource.PlayOneShot(completionVoice);

        StartCoroutine(TypeCompletion());
    }

    IEnumerator TypeCompletion()
    {
        typingText.text = "";

        foreach (char c in completionMessage)
        {
            typingText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
