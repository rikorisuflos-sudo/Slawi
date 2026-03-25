using UnityEngine;

public class SlimeDirecter : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject player;

    [SerializeField] private float speed = 5f;

    private void FixedUpdate()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }
    }
