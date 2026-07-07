using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

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

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (scene.name != "SampleScene") return;

        // CARI PANEL DAN HUBUNGKAN TOMBOLNYA SECARA OTOMATIS
        GameObject foundGameOver = CariUIPanel("GameOverPanel");
        if (foundGameOver != null) 
        {
            gameOverPanel = foundGameOver;
            HubungkanTombolOtomatis(gameOverPanel);
        }

        GameObject foundTheEnd = CariUIPanel("TheEndPanel");
        if (foundTheEnd != null) 
        {
            theEndPanel = foundTheEnd;
            HubungkanTombolOtomatis(theEndPanel);
        }

        GameObject foundPaused = CariUIPanel("PausedPanel");
        if (foundPaused != null) 
        {
            pausePanel = foundPaused;
            HubungkanTombolOtomatis(pausePanel);
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (theEndPanel != null) theEndPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (LastCheckpointScene != "SampleScene")
        {
            GameObject spawn = GameObject.Find("InitialSpawnPoint");
            if (spawn != null)
            {
                LastCheckpointScene    = "SampleScene";
                LastCheckpointPosition = spawn.transform.position;
            }
            else if (checkpoint1 != null)
            {
                LastCheckpointScene    = "SampleScene";
                LastCheckpointPosition = checkpoint1.position;
            }
        }
    }

    // =========================================================
    // FUNGSI PENGHUBUNG TOMBOL AGAR TIDAK ERROR (PUTUS)
    // =========================================================
    private void HubungkanTombolOtomatis(GameObject panel)
    {
        if (panel == null) return;

        Button[] semuaTombol = panel.GetComponentsInChildren<Button>(true);
        
        foreach (Button btn in semuaTombol)
        {
            string namaTombol = btn.gameObject.name.ToLower();

            // 1. Hapus sambungan OnClick lama dari Inspector
            btn.onClick.RemoveAllListeners();

            // 2. Hubungkan ulang kodenya berdasarkan nama tombolnya
            if (namaTombol.Contains("try") || namaTombol.Contains("restart") || namaTombol.Contains("again"))
            {
                btn.onClick.AddListener(RestartGame);
            }
            else if (namaTombol.Contains("main") || namaTombol.Contains("menu"))
            {
                btn.onClick.AddListener(LoadMainMenu);
            }
            else if (namaTombol.Contains("resume") || namaTombol.Contains("continue") || namaTombol.Contains("back"))
            {
                btn.onClick.AddListener(ResumeGame);
            }
        }
    }

    private void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if (gameOverPanel != null && gameOverPanel.activeSelf) return;
        if (theEndPanel != null && theEndPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
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

        // --- MODIFIKASI: KEMBALI KE CHECKPOINT AWAL ---
        // Cari posisi spawn awal ("InitialSpawnPoint") yang ada di scene saat ini.
        // Ini perlu karena GameManager persist dari MainMenu (DontDestroyOnLoad).
        Vector3 titikRestart = Vector3.zero;
        GameObject spawnAwal = GameObject.Find("InitialSpawnPoint");

        if (spawnAwal != null)
        {
            titikRestart = spawnAwal.transform.position;
        }
        else if (checkpoint1 != null)
        {
            // Fallback kalau InitialSpawnPoint tidak ada, tapi checkpoint1 ter-assign
            titikRestart = checkpoint1.position;
        }
        else
        {
            // Fallback terakhir kalau keduanya tidak ada (mencegah error)
            titikRestart = LastCheckpointPosition;
            Debug.LogWarning("[GameManager] InitialSpawnPoint / checkpoint1 tidak ditemukan saat Restart!");
        }

        // Reset LastCheckpointPosition ke titik awal, supaya kalau pemain mati lagi,
        // dia tetap respawn di awal, bukan di checkpoint terakhir sebelum game over.
        LastCheckpointPosition = titikRestart;
        // ---------------------------------------------

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (theEndPanel != null) theEndPanel.SetActive(false); 
        if (pausePanel != null) pausePanel.SetActive(false); 

        if (playerObject != null)
        {
            // Gunakan titikRestart yang sudah kita cari di atas
            ResetPlayerPhysicsAndPosition(titikRestart);

            PlayerHealth ph = playerObject.GetComponent<PlayerHealth>();
            if (ph != null) ph.ResetHP();
        }
    }
    private void ResetPlayerPhysicsAndPosition(Vector3 targetPosition)
    {
        Vector3 amanPosisi = new Vector3(targetPosition.x, targetPosition.y, 0f);
        playerObject.transform.position = amanPosisi;
        playerObject.SetActive(true);

        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.angularVelocity = 0f;
            rb.position = amanPosisi; 
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        isPaused = false; 
        currentRespawns = 0; 

        LastCheckpointScene    = "MainMenu";
        LastCheckpointPosition = Vector3.zero;

        SceneManager.LoadScene("MainMenu"); 
    }

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name != "SampleScene") return;
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

    private GameObject CariUIPanel(string namaPanel)
    {
        Scene sceneAktif = SceneManager.GetActiveScene();
        if (!sceneAktif.isLoaded) return null;

        GameObject[] rootObjects = sceneAktif.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            Transform[] semuaAnak = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform anak in semuaAnak)
            {
                if (anak.name == namaPanel) return anak.gameObject;
            }
        }
        return null;
    }
}