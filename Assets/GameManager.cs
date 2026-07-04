using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string LastCheckpointScene    { get; private set; }
    public Vector3 LastCheckpointPosition { get; private set; }

    [SerializeField] private float respawnDelay = 1.5f;

    private GameObject playerObject;

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
    }

    public void SetCheckpoint(Vector3 position)
    {
        LastCheckpointScene    = SceneManager.GetActiveScene().name;
        LastCheckpointPosition = position;
        Debug.Log($"[GameManager] Checkpoint disimpan: {position} di scene {LastCheckpointScene}");
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (playerObject != null)
        {
            playerObject.transform.position = LastCheckpointPosition;
            playerObject.SetActive(true);

            PlayerHealth ph = playerObject.GetComponent<PlayerHealth>();
            if (ph != null) ph.ResetHP();

            Debug.Log($"[GameManager] Player respawn di {LastCheckpointPosition}");
        }
        else
        {
            Debug.LogError("[GameManager] playerObject null!");
        }
    }
}