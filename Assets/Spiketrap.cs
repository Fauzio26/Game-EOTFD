using UnityEngine;

public class Spiketrap : MonoBehaviour
{
    [Header("Damage & Knockback")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackX = 8f;
    [SerializeField] private float knockbackY = 14f;

    // Dipanggil saat player pertama kali menyentuh spike
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    // Dipanggil setiap frame selama player masih di atas spike
    // Ini yang fix bug player bisa berdiri di spike
    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // Kalau masih invincible, skip — akan dicoba lagi frame berikutnya
        if (playerHealth.IsInvincible) return;

        float dirX = other.transform.position.x >= transform.position.x ? 1f : -1f;
        Vector2 knockback = new Vector2(dirX * knockbackX, knockbackY);

        playerHealth.TakeDamage(damageAmount, knockback);
    }
}