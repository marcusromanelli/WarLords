using UnityEngine;

public class PhysicalToken : MonoBehaviour
{
    [SerializeField] int coverMaterialIndex;
    [SerializeField] Renderer coverRenderer;
    [SerializeField] Animation cardPivot;

    public Transform CardPivot => cardPivot.transform;

    Sprite lastUsed;

    public void Setup(Sprite sprite)
    {
        if (sprite == lastUsed)
            return;

        coverRenderer.materials[coverMaterialIndex].mainTexture = sprite.texture;
        lastUsed = sprite;
    }
    public void SlideIn()
    {
        cardPivot.Play();
    }
}