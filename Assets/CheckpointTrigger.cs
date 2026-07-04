using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer flagSprite;
    [SerializeField] private Color activatedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Gunakan spawnPoint kalau sudah diassign, fallback ke transform.position
        Vector3 savePosition = spawnPoint != null ? spawnPoint.position : transform.position;

        // Skip kalau checkpoint ini sudah aktif
        if (GameManager.Instance.LastCheckpointPosition == savePosition) return;

        GameManager.Instance.SetCheckpoint(savePosition);
        UpdateVisual(true);

        // Reset visual semua checkpoint lain
        CheckpointTrigger[] allCheckpoints = FindObjectsByType<CheckpointTrigger>(FindObjectsSortMode.None);
        foreach (CheckpointTrigger cp in allCheckpoints)
        {
            if (cp != this) cp.UpdateVisual(false);
        }

        Debug.Log($"[Checkpoint] Aktif di {savePosition}");
    }

    public void UpdateVisual(bool isActive)
    {
        if (flagSprite == null) return;
        flagSprite.color = isActive ? activatedColor : defaultColor;
    }
}