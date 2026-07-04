using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menampilkan indikator hati (❤) di HUD menggunakan unicode Text.
///
/// CARA SETUP:
/// 1. Pasang script ini ke GameObject "HeartsContainer" (child dari Canvas)
/// 2. Set posisi HeartsContainer langsung di Inspector RectTransform:
///    - Anchor: top-left (min 0,1 / max 0,1)
///    - Pivot: 0, 1
///    - Pos X: 30, Pos Y: -30
/// 3. Drag GameObject "player" ke slot Player Health
/// 4. Play — hati ❤ akan muncul otomatis
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Drag GameObject player ke sini")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Tampilan Hati")]
    [Tooltip("Ukuran font hati (pixel)")]
    [SerializeField] private int heartFontSize = 30;

    [Tooltip("Jarak antar hati")]
    [SerializeField] private float heartSpacing = 10f;

    [Header("Warna")]
    [SerializeField] private Color heartFullColor  = new Color(0.9f, 0.1f, 0.1f); // merah
    [SerializeField] private Color heartEmptyColor = new Color(0.3f, 0.3f, 0.3f); // abu-abu

    private Text[] heartTexts;
    private int lastHP = -1;

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("[HealthUI] PlayerHealth belum di-assign di Inspector!");
            return;
        }

        BuildHearts();
        RefreshHearts(playerHealth.CurrentHP);
        lastHP = playerHealth.CurrentHP;
    }

    private void Update()
    {
        if (playerHealth == null) return;

        if (playerHealth.CurrentHP != lastHP)
        {
            RefreshHearts(playerHealth.CurrentHP);
            lastHP = playerHealth.CurrentHP;
        }
    }

    private void BuildHearts()
    {
        // Hapus heart lama (mencegah duplikat saat play ulang)
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        int maxHP = playerHealth.GetMaxHP();
        heartTexts = new Text[maxHP];

        float cellSize = heartFontSize + heartSpacing;

        for (int i = 0; i < maxHP; i++)
        {
            // Buat GameObject per hati
            GameObject heartObj = new GameObject($"Heart_{i + 1}");
            heartObj.transform.SetParent(transform, false);

            // RectTransform
            RectTransform rt = heartObj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(cellSize, cellSize);
            rt.anchoredPosition = new Vector2(i * cellSize, 0f);

            // Text unicode ❤
            Text txt = heartObj.AddComponent<Text>();
            txt.text      = "\u2764";           // ❤
            txt.fontSize  = heartFontSize;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color     = heartFullColor;

            // Pakai font default bawaan Unity
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            heartTexts[i] = txt;
        }
    }

    private void RefreshHearts(int currentHP)
    {
        if (heartTexts == null) return;

        for (int i = 0; i < heartTexts.Length; i++)
        {
            heartTexts[i].color = (i < currentHP) ? heartFullColor : heartEmptyColor;
        }
    }
}