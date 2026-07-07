using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string LastCheckpointScene    { get; private set; }
    public Vector3 LastCheckpointPosition { get; private set; }

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 1.5f;
    [SerializeField] private int maxRespawns = 4; 
    private int currentRespawns = 0;              

    [Header("Starting Checkpoint (For Restart)")]
    [SerializeField] private Transform checkpoint1; 

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private GameObject theEndPanel; 
    [SerializeField] private GameObject pausePanel; 

    [Header("Font Settings (OnGUI)")]
    [SerializeField] private Font customFont; 

    private GameObject playerObject;
    private bool isPaused = false; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LastCheckpointScene    = SceneManager.GetActiveScene().name;
        LastCheckpointPosition = Vector3.zero;
    }

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        Debug.Log($"[GameManager] Player ditemukan: {playerObject}");

        if (checkpoint1 != null)
        {
            LastCheckpointPosition = checkpoint1.position;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (theEndPanel != null) theEndPanel.SetActive(false); 
        if (pausePanel != null) pausePanel.SetActive(false); 
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if (gameOverPanel != null && gameOverPanel.activeSelf) return;
        if (theEndPanel != null && theEndPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; 
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; 
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void SetCheckpoint(Vector3 position)
    {
        LastCheckpointScene    = SceneManager.GetActiveScene().name;
        LastCheckpointPosition = position;
    }

    public void RespawnPlayer()
    {
        if (currentRespawns < maxRespawns)
        {
            currentRespawns++;
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            ShowGameOver();
        }
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (playerObject != null)
        {
            // Panggil fungsi perbaikan urutan posisi & aktif
            ResetPlayerPhysicsAndPosition(LastCheckpointPosition);

            PlayerHealth ph = playerObject.GetComponent<PlayerHealth>();
            if (ph != null) ph.ResetHP();
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void ShowTheEnd()
    {
        if (theEndPanel != null)
        {
            theEndPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        currentRespawns = 0; 
        isPaused = false; 

        if (checkpoint1 != null)
        {
            LastCheckpointPosition = checkpoint1.position;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (theEndPanel != null) theEndPanel.SetActive(false); 
        if (pausePanel != null) pausePanel.SetActive(false); 

        if (playerObject != null)
        {
            ResetPlayerPhysicsAndPosition(LastCheckpointPosition);

            PlayerHealth ph = playerObject.GetComponent<PlayerHealth>();
            if (ph != null) ph.ResetHP();
        }
    }

    // ====================================================================
    // PERBAIKAN UTAMA: Mengubah urutan SetActive agar Fisika tidak bug/error
    // ====================================================================
    private void ResetPlayerPhysicsAndPosition(Vector3 targetPosition)
    {
        Vector3 amanPosisi = new Vector3(targetPosition.x, targetPosition.y, 0f);

        // 1. Pindahkan posisi transform dasarnya dulu
        playerObject.transform.position = amanPosisi;

        // 2. AKTIFKAN OBJEK TERLEBIH DAHULU agar komponen fisika bangun dari tidur
        playerObject.SetActive(true);

        // 3. Baru setelah aktif, kita bersihkan gaya gravitasi & kecepatan lamanya
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; 
            rb.angularVelocity = 0f;
            rb.position = amanPosisi; // Paksa posisi sinkron dengan physics map global
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        isPaused = false; 
        SceneManager.LoadScene("MainMenu"); 
    }

    private void OnGUI()
    {
        if (theEndPanel != null && theEndPanel.activeSelf) return;
        if (pausePanel != null && pausePanel.activeSelf) return; 

        float xPos = Screen.width - 180; 
        float yPos = 20; 
        float width = 160;
        float height = 40;

        GUIStyle gayaTeks = new GUIStyle();
        gayaTeks.fontSize = 24;                 
        gayaTeks.fontStyle = FontStyle.Bold;    
        gayaTeks.normal.textColor = Color.white; 

        if (customFont != null) gayaTeks.font = customFont;

        int sisaKesempatan = maxRespawns - currentRespawns;
        GUI.Label(new Rect(xPos, yPos, width, height), "Respawn: " + sisaKesempatan, gayaTeks);
    }
}