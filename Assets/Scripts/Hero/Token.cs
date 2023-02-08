using UnityEngine;

public class Token : MonoBehaviour
{
    [SerializeField] int coverMaterialIndex;
    [SerializeField] Renderer coverRenderer;
    [SerializeField] Animation cardPivot;

    Sprite lastUsed;

    public void Setup(Sprite sprite)
    {
        if (sprite == lastUsed)
            return;

        coverRenderer.materials[coverMaterialIndex].mainTexture = sprite.texture;
        lastUsed = sprite;
    }

    public Transform GetCardPivot()
    {
        return cardPivot.transform;
    }

    public void SlideIn(CardObject cardObject)
    {
        cardObject.transform.SetParent(cardPivot.transform);

        cardPivot.Play();
    }
}