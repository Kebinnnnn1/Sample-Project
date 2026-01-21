using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform spawnPoint;

    private Rigidbody rb;
    private IResettableTrap[] traps;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        traps = FindObjectsOfType<MonoBehaviour>(true) as IResettableTrap[];
    }

    public void Respawn()
    {
        // Reset player
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Reset all traps
        foreach (IResettableTrap trap in traps)
        {
            trap.ResetTrap();
        }
    }
}
