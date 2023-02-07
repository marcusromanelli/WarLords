using UnityEngine;

public class Token : MonoBehaviour
{
    [SerializeField] int coverMaterialIndex;
    [SerializeField] Renderer coverRenderer;

    Sprite lastUsed;

    public void Setup(Sprite sprite)
    {
        if (sprite == lastUsed)
            return;

        coverRenderer.materials[coverMaterialIndex].mainTexture = sprite.texture;
        lastUsed = sprite;
    }
}