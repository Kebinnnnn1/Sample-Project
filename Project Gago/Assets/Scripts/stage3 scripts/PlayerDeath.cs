using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    [Header("References")]
    public Stage3ResetManager stageResetManager;
    public Transform respawnPoint;

    [Header("Death Sound")]
    [Tooltip("The audio clip to play when the player dies")]
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float deathSoundVolume = 1f;

    private bool isDead;

    private void Start()
    {
        // Validate setup on start
        if (deathSound == null)
        {
            Debug.LogError("PlayerDeath: NO DEATH SOUND ASSIGNED! Please assign an AudioClip in the Inspector.");
        }
        else
        {
            Debug.Log("PlayerDeath: Death sound is ready: " + deathSound.name);
        }
    }

    public void Die()
    {
        Debug.Log("PlayerDeath: Die() method called!");
        
        if (isDead)
        {
            Debug.Log("PlayerDeath: Already dead, ignoring...");
            return;
        }
        isDead = true;

        // Play death sound FIRST
        PlayDeathSound();

        StartCoroutine(DeathRoutine());
    }

    private void PlayDeathSound()
    {
        Debug.Log("PlayerDeath: Attempting to play death sound...");
        
        if (deathSound == null)
        {
            Debug.LogError("PlayerDeath: CANNOT PLAY - Death sound is NULL!");
            return;
        }

        // Create a dedicated GameObject for the sound so it persists
        GameObject soundObject = new GameObject("DeathSound");
        soundObject.transform.position = Camera.main != null ? Camera.main.transform.position : transform.position;
        
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = deathSound;
        audioSource.volume = deathSoundVolume;
        audioSource.spatialBlend = 0f; // 2D sound (no 3D positioning)
        audioSource.Play();
        
        // Destroy after clip finishes
        Destroy(soundObject, deathSound.length + 0.5f);
        
        Debug.Log("PlayerDeath: Death sound is now playing! Clip: " + deathSound.name + ", Length: " + deathSound.length + "s");
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (stageResetManager != null)
            stageResetManager.ResetStage();

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = Vector3.zero;
        }

        isDead = false;
    }
}
