using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChaseCharacter : MonoBehaviour
{
    [Header("References")]
    public Transform player;           // Reference to the player's transform
    public Animator animator;          // Reference to the enemy's Animator component
    public AudioSource audioSource;    // Audio source for monster sounds

    [Header("Movement Settings")]
    public float moveSpeed = 5f;       // The enemy's move speed
    public float rotationSpeed = 5f;   // The speed at which the enemy rotates
    public float chaseRange = 10f;     // The distance at which the enemy starts chasing the player
    public float deathRange = .75f;    // The distance at which the enemy catches the player
    public float groundHeight = 0f;    // The fixed ground height for the enemy (e.g., 0 for ground level)

    [Header("Audio Clips")]
    public AudioClip runningSound;     // Loop sound for running
    public AudioClip[] growlSounds;    // Random growls while chasing

    private bool isPlayingRunSound = false;
    private float nextGrowlTime = 0f;

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        ChasePlayer(distance); 
        PlayerDeath(distance);  
    }

    private void PlayerDeath(float distance)
    {
        if (distance < deathRange)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void ChasePlayer(float distance)
    {
        // Always lock enemy to ground height
        Vector3 currentPosition = transform.position;
        currentPosition.y = groundHeight;

        if (distance < chaseRange)
        {
            // Calculate the direction towards the player (ignoring vertical movement)
            Vector3 targetPos = player.position;
            targetPos.y = groundHeight;

            Vector3 direction = (targetPos - currentPosition).normalized;

            // Move the enemy towards the grounded player position
            transform.position = Vector3.MoveTowards(currentPosition, targetPos, moveSpeed * Time.deltaTime);

            // Rotate smoothly towards the player
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }


            // 🔊 Play running sound loop
            if (!isPlayingRunSound && runningSound != null)
            {
                audioSource.clip = runningSound;
                audioSource.loop = true;
                audioSource.Play();
                isPlayingRunSound = true;
            }

            // 🔊 Play random growls sometimes
            if (growlSounds.Length > 0 && Time.time > nextGrowlTime)
            {
                AudioClip growl = growlSounds[Random.Range(0, growlSounds.Length)];
                audioSource.PlayOneShot(growl);
                nextGrowlTime = Time.time + Random.Range(3f, 6f); // growl every 3–6 seconds
            }
        }
        else
        {
            // Stay grounded in idle state
            transform.position = currentPosition;
            animator.SetBool("isRunning", false);

            // 🔇 Stop running loop sound
            if (isPlayingRunSound)
            {
                audioSource.Stop();
                isPlayingRunSound = false;
            }
        }
    }
}
