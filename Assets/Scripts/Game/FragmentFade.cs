// FragmentFade.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class FragmentFade : MonoBehaviour
{
    private Material matInstance;
    private Color  baseColor;
    private float  lifetime;

    void Start()
    {
        // Делаем уникальный экземпляр материала, чтобы не менять sharedMaterial
        matInstance = GetComponent<Renderer>().material;
        baseColor   = matInstance.color;

        // Переводим материал в режим прозрачности
        SetupMaterialWithTransparency();

        // Случайное время жизни 3–10 сек
        lifetime = Random.Range(3f, 10f);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / lifetime);
            matInstance.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void SetupMaterialWithTransparency()
    {
        // Для Standard Shader
        matInstance.SetOverrideTag("RenderType", "Transparent");
        matInstance.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        matInstance.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        matInstance.SetInt("_ZWrite", 0);
        matInstance.DisableKeyword("_ALPHATEST_ON");
        matInstance.EnableKeyword("_ALPHABLEND_ON");
        matInstance.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        matInstance.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
