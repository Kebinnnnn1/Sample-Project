using UnityEngine;
using UnityEngine.UI;

public class LookAtEndWithProgress : MonoBehaviour
{
    [Header("Target (3D World Object)")]
    public Transform endTarget;
    public float maxDistance = 30f;
    public float lookTimeRequired = 1.5f;

    [Header("UI")]
    public Image lookProgress;

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip completionSound;

    private float lookTimer;
    private bool completed;

    void Start()
    {
        // Make sure cursor is visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (lookProgress != null)
            lookProgress.fillAmount = 0f;
    }

    void Update()
    {
        // Safety checks
        if (completed) return;
        if (endTarget == null || lookProgress == null) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool lookingAtEnd = false;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.transform == endTarget)
            {
                lookingAtEnd = true;
                lookTimer += Time.deltaTime;
            }
        }

        if (!lookingAtEnd)
            lookTimer = 0f;

        lookProgress.fillAmount = lookTimer / lookTimeRequired;

        if (lookTimer >= lookTimeRequired)
            Complete();
    }

    void Complete()
    {
        completed = true;

        lookProgress.fillAmount = 1f;

        if (audioSource != null && completionSound != null)
            audioSource.PlayOneShot(completionSound);

        Debug.Log("END confirmed");
    }
}
