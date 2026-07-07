using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] private KeyCode blockKey = KeyCode.F;
    [SerializeField] private float parryDuration = 0.5f; // Sesuaikan dengan panjang animasi parry kamu di Unity

    public bool IsBlocking { get; private set; }

    private Animator animator;
    private PlayerHealth playerHealth;
    private bool isParrying = false; // Mencegah input block terbaca saat sedang animasi parry

    private void Start()
    {
        animator     = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (playerHealth != null && (playerHealth.IsKnockedBack || playerHealth.CurrentHP <= 0))
        {
            StopBlock();
            return;
        }

        // Jangan izinkan input block baru jika sedang di tengah-tengah animasi parry
        if (isParrying) return;

        // Mulai block HANYA saat tombol baru ditekan
        if (Input.GetKeyDown(blockKey))
            StartBlock();

        // Berhenti block HANYA saat tombol dilepas
        if (Input.GetKeyUp(blockKey))
            StopBlock();
    }

    private void StartBlock()
    {
        if (IsBlocking) return; // Mencegah terpanggil berulang kali
        
        IsBlocking = true;
        if (animator != null)
            animator.SetBool("isBlocking", true);
        Debug.Log("[Block] Block aktif");
    }

    private void StopBlock()
    {
        if (!IsBlocking) return;

        IsBlocking = false;
        if (animator != null)
            animator.SetBool("isBlocking", false);
    }

    public bool TryParry()
    {
        if (!IsBlocking) return false;

        // [LOGIKA KAMU DIMULAI DI SINI]
        
        // 1. Paksa berhenti block (kondisi jadi false)
        StopBlock();
        isParrying = true; // Kunci sistem input sementara

        // 2. Mainkan animasi Parry
        if (animator != null)
            animator.SetTrigger("Parry");

        Debug.Log("[Block] Serangan diblock dan Parry Muncul!");

        // 3. Cek apakah tombol F masih ditahan setelah animasi Parry selesai
        Invoke(nameof(CheckBlockHold), parryDuration);

        return true;
    }

    // Fungsi ini dipanggil otomatis setelah parryDuration habis
    private void CheckBlockHold()
    {
        isParrying = false; // Buka kunci input

        // Cek apakah jari pemain MASIH menahan tombol F (GetKey, bukan GetKeyDown)
        if (Input.GetKey(blockKey))
        {
            Debug.Log("[Block] Tombol masih ditahan, kembali ke posisi Block!");
            StartBlock(); // Otomatis true kembali
        }
    }
}