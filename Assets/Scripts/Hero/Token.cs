using UnityEngine;

public class Token : MonoBehaviour
{
    [SerializeField] int coverMaterialIndex;
    [SerializeField] Renderer coverRenderer;
    [SerializeField] Animation cardPivot;
    [SerializeField] CardObject cardObject;

    Sprite lastUsed;

    public void Setup(Sprite sprite)
    {
        if (sprite == lastUsed)
            return;

        coverRenderer.materials[coverMaterialIndex].mainTexture = sprite.texture;
        lastUsed = sprite;
    }
    public void SetCardObject(CardObject _cardObject)
    {
        cardObject = _cardObject;

        var pivot = cardPivot.transform;

        cardObject.transform.SetParent(pivot);

        cardObject.HideInfo(true);
        cardObject.Lock();

        cardObject.SetPositionAndRotation(CardPositionData.Create(pivot.position, pivot.rotation), () => {
            SlideIn();
        });
    }
    public void SlideIn()
    {
        cardPivot.Play();
    }
    public void Destroy()
    {
        if(cardObject != null)
            CardFactory.AddToPool(cardObject);
    }
}