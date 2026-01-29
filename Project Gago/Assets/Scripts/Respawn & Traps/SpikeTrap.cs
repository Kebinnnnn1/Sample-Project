using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    public Transform spike;
    public Vector3 moveOffset;
    public float moveSpeed = 6f;
    public float disappearDelay = 0.5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool triggered = false;
    private bool moving = false;

    void Start()
    {
        startPos = spike.position;
        targetPos = startPos + moveOffset;
    }

    void Update()
    {
        if (!moving) return;

        spike.position = Vector3.MoveTowards(
            spike.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(spike.position, targetPos) < 0.01f)
        {
            moving = false;
            StartCoroutine(DisappearAfterDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            moving = true;
        }
    }

    IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay);
        spike.gameObject.SetActive(false);
    }

    // 🔁 CALLED ON RESPAWN
    public void ResetTrap()
    {
        StopAllCoroutines();
        triggered = false;
        moving = false;

        spike.position = startPos;
        spike.gameObject.SetActive(true);
    }
}
