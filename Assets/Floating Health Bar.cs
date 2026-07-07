using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target; // Masukkan Boss ke sini
    [SerializeField] private Vector3 offset; // Posisi relatif terhadap Boss

    private Vector3 initialScale;

    private void Start()
    {
        // Simpan ukuran asli HealthBar saat game baru dimulai
        initialScale = transform.localScale;
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Cek arah Boss saat ini (1 jika hadap kanan, -1 jika hadap kiri)
        float bossDirection = Mathf.Sign(target.localScale.x);

        // 1. KUNCI POSISI (Menggunakan localPosition agar tidak glitch/bergetar)
        // Posisi X dikalikan arah Boss agar bar tidak bergeser ke sisi yang salah saat Boss berbalik
        transform.localPosition = new Vector3(
            bossDirection * offset.x,
            offset.y,
            offset.z
        );

        // 2. KUNCI ROTASI (Hadap Kamera)
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }

        // 3. LAWAN EFEK FLIP DARI BOSS
        // Jika Boss berbalik (scale X menjadi -1), kita kalikan scale X HealthBar dengan -1 juga.
        // Hasilnya: -1 x -1 = 1 (HealthBar kembali positif / tidak terbalik secara visual)
        transform.localScale = new Vector3(
            bossDirection * Mathf.Abs(initialScale.x),
            Mathf.Abs(initialScale.y),
            Mathf.Abs(initialScale.z)
        );
    }
}