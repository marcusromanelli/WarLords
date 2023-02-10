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
    public void SetCardObject(GameObject physicalCardObject)
    {
        var pivot = cardPivot.transform;


        physicalCardObject.transform.position = cardPivot.transform.position;

        physicalCardObject.transform.SetParent(pivot, true);

        physicalCardObject.transform.localPosition = Vector3.zero;

        physicalCardObject.transform.localRotation = pivot.rotation;

        SlideIn();
    }
    public void SlideIn()
    {
        cardPivot.Play();
    }
}