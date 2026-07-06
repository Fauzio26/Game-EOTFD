using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset; // offset dalam world space, bukan local

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    private void LateUpdate()
    {
        // Kunci hadap ke kamera
        if (mainCamera != null)
            transform.rotation = mainCamera.transform.rotation;

        // Kunci posisi mengikuti Boss pakai world position
        if (target != null)
        {
            // Pakai target.position (world) + worldOffset
            // Tidak terpengaruh scale Boss sama sekali
            transform.position = new Vector3(
                target.position.x + worldOffset.x,
                target.position.y + worldOffset.y,
                target.position.z + worldOffset.z
            );
        }

        // Reset scale ke absolute agar tidak ikut flip
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y),
            Mathf.Abs(transform.localScale.z)
        );
    }
}