using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("References")]
    public Image cursorImage;

    [Header("Sprites")]
    public Sprite idleSprite;           // UI_TravelBook_MouseCursorClick01b_2
    public Sprite[] clickAnimFrames;    // urutan: 01b_1, 01a_1, 01a_2, 01a_3, 01a_4

    [Header("Settings")]
    public float clickFrameDuration = 0.05f;
    public Vector2 hotspotPivot = new Vector2(0f, 1f);

    private Coroutine clickAnimCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        if (cursorImage != null)
        {
            cursorImage.rectTransform.pivot = hotspotPivot;
            cursorImage.sprite = idleSprite;
            cursorImage.raycastTarget = false;
        }
    }

    void Update()
    {
        if (cursorImage == null) return;

        cursorImage.rectTransform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            PlayClickAnimation();
        }
    }

    private void PlayClickAnimation()
    {
        if (clickAnimCoroutine != null)
            StopCoroutine(clickAnimCoroutine);

        clickAnimCoroutine = StartCoroutine(ClickAnimationRoutine());
    }

    private IEnumerator ClickAnimationRoutine()
    {
        for (int i = 0; i < clickAnimFrames.Length; i++)
        {
            cursorImage.sprite = clickAnimFrames[i];
            yield return new WaitForSeconds(clickFrameDuration);
        }

        cursorImage.sprite = idleSprite;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Cursor.visible = true;
    }
}