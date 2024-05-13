using UnityEngine;

public class PropBreak : MonoBehaviour
{
    public float health = 10f;
    public GameObject destructionEffect; // Particle effect for destruction

    bool isLive = true;

    Collider2D coll;
    Rigidbody2D rb;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // Freeze position and rotation
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && isLive)
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
                
            }
        }
        else if (collision.CompareTag("Player")) // Check if the collision is with the player
        {
            // Prevent the player from going through the prop
            Physics2D.IgnoreCollision(coll, collision, true);
        }

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isLive = false;
        coll.enabled = false; // Disable collider to prevent further interactions
        rb.bodyType = RigidbodyType2D.Static; // Set Rigidbody type to Static to prevent movement
        //Instantiate(destructionEffect, transform.position, Quaternion.identity); // Spawn destruction effect
        Destroy(gameObject); // Destroy the prop object
    }
}
