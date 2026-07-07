using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    private bool hasTriggered = false; // Mencegah fungsi ganti lagu terpanggil berkali-kali

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Memeriksa apakah yang menyentuh area ini adalah Player
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Tandai bahwa lagu sudah diganti
            
            // Memanggil fungsi ganti musik di AudioManager
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ChangeToBossMusic();
            }
        }
    }
}