using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Drag Text (TMP) child ke sini")]
    public TextMeshProUGUI buttonText;

    [Header("Warna outline saat hover")]
    public Color hoverOutlineColor = new Color(0.85f, 0.65f, 0.0f, 1f); // #D9A600 emas

    [Range(0f, 0.5f)]
    public float outlineThickness = 0.18f;

    private Material _mat;
    private bool _ready = false;

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            Debug.LogWarning("ButtonHoverEffect: Text (TMP) tidak ditemukan di "
                + gameObject.name);
            return;
        }

        _mat = new Material(buttonText.fontMaterial);
        buttonText.fontMaterial = _mat;
        _ready = true;

        SetNormal();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!_ready) return;
        SetHover();
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!_ready) return;
        SetNormal();
    }

    void SetNormal()
    {
        _mat.SetColor(ShaderUtilities.ID_OutlineColor, new Color(0, 0, 0, 0));
        _mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
    }

    void SetHover()
    {
        _mat.SetColor(ShaderUtilities.ID_OutlineColor, hoverOutlineColor);
        _mat.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineThickness);
    }

    void OnDestroy()
    {
        if (_mat != null) Destroy(_mat);
    }
}