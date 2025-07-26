// FadeIn.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class FadeIn : MonoBehaviour
{
    [Tooltip("Длительность плавного появления (сек)")]
    public float duration = 1f;

    private Material mat;
    private Color baseColor;

    void Start()
    {
        // Копируем материал, чтобы не менять sharedMaterial
        mat = GetComponent<Renderer>().material;
        baseColor = mat.color;

        // Переводим материал в режим прозрачности
        SetupMaterialWithTransparency(mat);

        // Сразу делаем объект полностью прозрачным
        mat.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        // Запускаем корутину плавного появления
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, baseColor.a, elapsed / duration);
            mat.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }
        // Гарантируем, что в конце альфа точно равна исходной
        mat.color = baseColor;
    }

    private void SetupMaterialWithTransparency(Material m)
    {
        // Для Standard Shader
        m.SetOverrideTag("RenderType", "Transparent");
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
