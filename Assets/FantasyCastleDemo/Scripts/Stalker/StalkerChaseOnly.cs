using UnityEngine;
using UnityEngine.SceneManagement;

public class StalkerChaseOnly : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 999f;
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 8f;
    public float wallCheckDistance = 1.2f;

    private void Update()
    {
        if (player == null)
            return;

        Vector3 target = player.position;
        target.y = transform.position.y;

        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        if (distance <= detectionRadius && distance > 0.1f)
        {
            direction.Normalize();

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * rotationSpeed
            );

            Vector3 origin = transform.position + Vector3.up * 1f;

            if (!Physics.Raycast(origin, direction, wallCheckDistance))
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player || other.transform.root == player)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("MainMenu");
        }
    }
}