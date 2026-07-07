using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIGradient : BaseMeshEffect
{
    [Header("Gradient Kiri ke Kanan")]
    public Color colorLeft  = new Color(0, 0, 0, 0);   // transparan
    public Color colorRight = new Color(0, 0, 0, 0.85f); // hitam

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        var verts = new System.Collections.Generic.List<UIVertex>();
        vh.GetUIVertexStream(verts);

        for (int i = 0; i < verts.Count; i++)
        {
            var v = verts[i];
            // x lokal: -0.5 (kiri) sampai +0.5 (kanan)
            float t = v.position.x / GetComponent<RectTransform>().rect.width + 0.5f;
            t = Mathf.Clamp01(t);
            v.color = Color.Lerp(colorLeft, colorRight, t);
            verts[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }
}